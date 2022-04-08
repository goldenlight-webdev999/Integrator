﻿using BookScrapperDOI.Models;
using HtmlAgilityPack;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using BookDOIMerger.Properties;

namespace BookScrapperDOI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsDoingWork = false;
        private List<Article> Articles = new List<Article>();

        public static string CurrentPath = Directory.GetParent(Process.GetCurrentProcess().MainModule.FileName).FullName;

        string ConnectionString = "";
        string WorkFolderPath = "";
        string LogFilePath = CurrentPath + "\\log.txt";

        private string ArticleFileCSVHeader = "No,Publisher,Journal,Year,Volume,Issue,\"Pub YM\",Section,\"Article Title\",Author(s),\"Literature source\",DOI,PdfURL,Download";
        private string VolumesFileCSVHeader = "No,Publisher,Journal,Year,Volume,Issue,FrontMatter,BackMatter,TOC,FM_Down,BM_Down,TOC_Down,TotalCount,DownCount";

        private static string[] SiteDomains = new string[]
        {
            "https://sci-hub.ee/",
            //"https://sci-hub.st/",
            //"https://sci-hub.se/",
            //"https://sci-hub.ren/",
            //"https://www.sci-hub.ren/"
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitTables()
        {
            try
            {
                Directory.CreateDirectory(WorkFolderPath + "\\DB");

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string sql = "create table if not exists Articles (DOI varchar(500) primary key, Publisher varchar(200), Journal varchar(200), Year varchar(20), Volume varchar(200), Issue varchar(500), PubYM varchar(50), Section varchar(300), Title varchar(2000), Authors varchar(500), Literature varchar(500), PdfUrl varchar(5000), DownloadDate datetime, FilePath varchar(3000), Status varchar(100), FailedCnt int, Other varchar(1000))";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log("InitTables error : " + ex.Message);
            }
        }

        private async Task<Article> GetArticleFromDB(string doi)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string sql = "select * from Articles where DOI = @DOI";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@DOI", doi));
                        using (SQLiteDataReader r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return null;

                            DownloadStatus status;
                            Enum.TryParse(Convert.ToString(r["Status"]), out status);
                            return new Article
                            {
                                DOI = Convert.ToString(r["DOI"]),
                                Publisher = Convert.ToString(r["Publisher"]),
                                Journal = Convert.ToString(r["Journal"]),
                                Year = Convert.ToString(r["Year"]),
                                Volume = Convert.ToString(r["Volume"]),
                                Issue = Convert.ToString(r["Issue"]),
                                PubYM = Convert.ToString(r["PubYM"]),
                                Section = Convert.ToString(r["Section"]),
                                Title = Convert.ToString(r["Title"]),
                                Authors = Convert.ToString(r["Authors"]),
                                Literature = Convert.ToString(r["Literature"]),
                                PdfUrl = Convert.ToString(r["PdfUrl"]),
                                DownloadDate = Convert.ToDateTime(r["DownloadDate"]),
                                FilePath = Convert.ToString(r["FilePath"]),
                                Status = status,
                                FailedCnt = Convert.ToInt32(r["FailedCnt"]),
                                Other = Convert.ToString(r["Other"]),
                            };

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Log($"GetArticleFromDB {doi} Error: {ex.Message}");
            }
            return null;
        }

        private async Task<bool> InsertArticleIntoDB(Article article)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string sql = "insert or replace into Articles (DOI,Publisher,Journal,Year,Volume,Issue, PubYM, Section, Title, Authors,Literature, PdfUrl, DownloadDate,FilePath,Status,FailedCnt, Other) " +
                                                          "values (@DOI,@Publisher,@Journal,@Year,@Volume,@Issue, @PubYM, @Section,@Title,@Authors,@Literature, @PdfUrl, @DownloadDate,@FilePath,@Status, @FailedCnt, @Other) ";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@DOI", article.DOI));
                        cmd.Parameters.Add(new SQLiteParameter("@Publisher", article.Publisher));
                        cmd.Parameters.Add(new SQLiteParameter("@Journal", article.Journal));
                        cmd.Parameters.Add(new SQLiteParameter("@Year", article.Year));
                        cmd.Parameters.Add(new SQLiteParameter("@Volume", article.Volume));
                        cmd.Parameters.Add(new SQLiteParameter("@Issue", article.Issue));
                        cmd.Parameters.Add(new SQLiteParameter("@PubYM", article.PubYM));
                        cmd.Parameters.Add(new SQLiteParameter("@Section", article.Section));
                        cmd.Parameters.Add(new SQLiteParameter("@Title", article.Title));
                        cmd.Parameters.Add(new SQLiteParameter("@Authors", article.Authors));
                        cmd.Parameters.Add(new SQLiteParameter("@Literature", article.Literature));
                        cmd.Parameters.Add(new SQLiteParameter("@PdfUrl", article.PdfUrl));
                        cmd.Parameters.Add(new SQLiteParameter("@DownloadDate", article.DownloadDate));
                        cmd.Parameters.Add(new SQLiteParameter("@FilePath", article.FilePath));
                        cmd.Parameters.Add(new SQLiteParameter("@Status", article.Status));
                        cmd.Parameters.Add(new SQLiteParameter("@Other", article.Other));
                        cmd.Parameters.Add(new SQLiteParameter("@FailedCnt", article.FailedCnt));

                        cmd.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                await Log($"InsertArticleIntoDB {article.DOI} Error: {ex.Message}");
            }
            return false;
        }

        private void EnableControls(bool enable)
        {
            BtnStartWork.IsEnabled = enable;
            BtnSelectExcelFile.IsEnabled = enable;
        }

        public async Task<int> LoadCSVFile()
        {
            await Log("Loading csv files...");
            EnableControls(false);

            Articles.Clear();

            var filesInFolder = Directory.EnumerateFiles(WorkFolderPath, "*.csv", System.IO.SearchOption.AllDirectories).ToList();
            PbStatus.Minimum = 0;
            PbStatus.Maximum = filesInFolder.Count;

            int fileCnt = 0;
            foreach (var file in filesInFolder)
            {
                try
                {
                    PbStatus.Value = fileCnt++;

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string ext = Path.GetExtension(file);
                    if (fileName != "articles" || ext != ".csv") continue;

                    int lineNumber = 0;
                    using (TextFieldParser csvParser = new TextFieldParser(file))
                    {
                        csvParser.CommentTokens = new string[] { "#" };
                        csvParser.SetDelimiters(new string[] { "," });
                        csvParser.HasFieldsEnclosedInQuotes = true;

                        // Skip the row with the column names
                        csvParser.ReadLine();

                        while (!csvParser.EndOfData)
                        {
                            // Read current line fields, pointer moves to the next line.
                            try
                            {
                                string[] fields = csvParser.ReadFields();
                                lineNumber++;
                                if (string.IsNullOrEmpty(fields[8])) break;

                                Articles.Add(new Article
                                {
                                    Publisher = fields[1],
                                    Journal = fields[2],
                                    Year = fields[3],
                                    Volume = fields[4],
                                    Issue = fields[5],
                                    PubYM = fields[6],
                                    Section = fields[7],
                                    Title = fields[8],
                                    Authors = fields[9],
                                    Literature = fields[10],
                                    DOI = fields[11].Replace("https://doi.org/", ""),
                                    PdfUrl = fields[12]
                                });
                            }
                            catch (Exception ex)
                            {
                                await Log($"Exception occurs while reading csv : {file}, at line {lineNumber} error : {ex.Message}. continue working...");
                            }
                        }

                        await Log($"{file} is loaded");
                        await Task.Delay(1);
                    }
                }
                catch (Exception ex)
                {
                    await Log($"Exception occurs while reading csv : {file}, error : {ex.Message}");
                }
            }

            if (Articles.Count <= 0)
            {
                await Log("No articles are loaded. Please check the csv content.");
            }

            await Log($"{Articles.Count} Articles are loaded.");
            EnableControls(true);

            return Articles.Count;
        }

        private async void BtnStartWork_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsDoingWork)
                {
                    if (string.IsNullOrEmpty(WorkFolderPath))
                    {
                        await Log("Please select work folder.");
                        return;
                    }

                    ConnectionString = "Data Source=" + WorkFolderPath + "\\DB\\DOI.db;Version=3;";
                    InitTables();

                    //asdfasdf
                    IsDoingWork = true;
                    BtnStartWork.Content = "Stop";
                    await LoadCSVFile();
                    int totalFilesCnt = Articles.Count;
                    int totalSucces = 0;
                    int totalFailed = 0;

                    EnableControls(false);
                    BtnStartWork.IsEnabled = true;
                    var filesInFolder = Directory.EnumerateFiles(WorkFolderPath, "articles.csv", System.IO.SearchOption.AllDirectories).ToList();
                    PbStatus.Minimum = 0;
                    PbStatus.Maximum = filesInFolder.Count;

                    DateTime workStartTime = DateTime.Now;

                    foreach (var file in filesInFolder)
                    {
                        int successCnt = 0;
                        int failedCnt = 0;

                        if (!IsDoingWork) break;

                        await Log("======================");
                        await Log($"Processing {file} ...");
                        LbTimeEstimation.Text = $"Processing {file}...";

                        string outArticlePath = Path.GetDirectoryName(file) + $"\\articles_" + "result_m.csv";
                        string inVolumePath = Path.GetDirectoryName(file) + $"\\volumes.csv";
                        string outVolumePath = Path.GetDirectoryName(file) + $"\\volumes_" + "result_m.csv";

                        // delete temp csv files
                        var oldFilesInFolder = Directory.EnumerateFiles(Path.GetDirectoryName(file), "*.csv", System.IO.SearchOption.AllDirectories).ToList();
                        foreach (var oldFile in oldFilesInFolder)
                        {
                            string oldFileName = Path.GetFileName(oldFile);
                            if (oldFileName == "articles.csv" || oldFileName == "volumes.csv") continue;
                            try
                            {
                                File.Delete(oldFile);
                            }
                            catch (Exception ex)
                            {
                                await Log($"error in while deleting {oldFile} : {ex.Message}");
                            }
                        }

                        var articles = new List<Article>();
                        int lineNumber = 0;
                        using (TextFieldParser csvParser = new TextFieldParser(file))
                        {
                            csvParser.CommentTokens = new string[] { "#" };
                            csvParser.SetDelimiters(new string[] { "," });
                            csvParser.HasFieldsEnclosedInQuotes = true;

                            // Skip the row with the column names
                            csvParser.ReadLine();

                            while (!csvParser.EndOfData)
                            {
                                // Read current line fields, pointer moves to the next line.
                                string[] fields = csvParser.ReadFields();
                                lineNumber++;

                                try
                                {
                                    if (string.IsNullOrEmpty(fields[8])) break;

                                    articles.Add(new Article
                                    {
                                        Publisher = fields[1],
                                        Journal = fields[2],
                                        Year = fields[3],
                                        Volume = fields[4],
                                        Issue = fields[5],
                                        PubYM = fields[6],
                                        Section = fields[7],
                                        Title = fields[8],
                                        Authors = fields[9],
                                        Literature = fields[10],
                                        DOI = fields[11].Replace("https://doi.org/", ""),
                                        PdfUrl = fields[12]
                                    });

                                }
                                catch (Exception ex)
                                {
                                    await Log($"Exception occurs while reading csv : {file}, at line {lineNumber} error : {ex.Message} continue working...");
                                }
                            }
                        }

                        PbStatus.Minimum = 0;
                        PbStatus.Maximum = articles.Count - 1;

                        DateTime elapsedTimeCheckTime = DateTime.Now;
                        for (int i = 0; i < articles.Count; i++)
                        {
                            try
                            {
                                PbStatus.Value = i;

                                if (!IsDoingWork) break;

                                string outFolderPath = "";

                                if (!string.IsNullOrEmpty(articles[i].Publisher))
                                    outFolderPath += $"\\{articles[i].Publisher}";
                                if (!string.IsNullOrEmpty(articles[i].Journal))
                                    outFolderPath += $"\\{articles[i].Journal}";
                                if (!string.IsNullOrEmpty(articles[i].Volume))
                                    outFolderPath += $"\\{articles[i].Volume}";
                                if (!string.IsNullOrEmpty(articles[i].Issue))
                                    outFolderPath += $"\\{articles[i].Issue}";

                                string outFilePathRelativePath = outFolderPath + "\\" + articles[i].DOI.Replace("/", "_") + ".pdf";

                                if (!string.IsNullOrEmpty(articles[i].PdfUrl))
                                {
                                    articles[i].Status = DownloadStatus.Success;
                                }
                                else if (File.Exists(WorkFolderPath + outFilePathRelativePath))
                                {

                                    FileInfo fi = new FileInfo(WorkFolderPath + outFilePathRelativePath);

                                    if (fi.Length > 5000)
                                    {
                                        articles[i].FilePath = outFilePathRelativePath;
                                        articles[i].Status = DownloadStatus.Success;
                                    }
                                    else
                                    {
                                        articles[i].Status = DownloadStatus.UnknowError; // For DB update
                                        File.Delete(WorkFolderPath + outFilePathRelativePath);
                                    }
                                }
                                else
                                {
                                    articles[i].Status = DownloadStatus.UnknowError;
                                }

                                articles[i].Other = DateTime.Now.ToString();

                                await Task.Delay(10);
                                await InsertArticleIntoDB(articles[i]);

                                if (articles[i].Status == DownloadStatus.Success)
                                {
                                    successCnt++;
                                    totalSucces++;
                                }
                                else
                                {
                                    failedCnt++;
                                    totalFailed++;
                                }

                                LbProgressStatus.Text = $"{successCnt}/{articles.Count} success.";
                                LbFailedStatus.Text = $"{failedCnt} failed.  ({successCnt + failedCnt} processed)";
                                LbProgressStatusTotal.Text = $"{totalSucces}/{totalFilesCnt} success.";
                                LbFailedStatusTotal.Text = $"{totalFailed} failed.  ({totalSucces + totalFailed} processed totally)";

                                if (i % 10 == 0)
                                {
                                    int remainedTimeTotal = (int)((totalFilesCnt - totalSucces - totalFailed) * (DateTime.Now - elapsedTimeCheckTime).TotalMilliseconds) / 10000;
                                    int remainedTimeForFile = (int)((articles.Count - successCnt - failedCnt) * (DateTime.Now - elapsedTimeCheckTime).TotalMilliseconds) / 10000;
                                    LbTimeEstimationTotal.Text = $"{remainedTimeTotal / 3600} hrs: {(remainedTimeTotal % 3600) / 60} mins ({remainedTimeForFile / 60} mins: {remainedTimeForFile % 60} secs for current file)  remained";
                                    elapsedTimeCheckTime = DateTime.Now; 
                                }
                            }
                            catch (Exception ex)
                            {
                                await Log($"Error in handling article {articles[i].DOI}, {ex.Message}");
                            }
                        }

                        PbStatus.Value = PbStatus.Maximum;

                        // now update the excel file

                        // update articles.csv
                        using (StreamWriter writer = new StreamWriter(outArticlePath, false))
                        {
                            writer.WriteLine(ArticleFileCSVHeader);
                            int number = 1;
                            foreach (var article in articles)
                            {
                                string line = $"{number++},\"{article.Publisher}\",\"{article.Journal}\",{article.Year},\"{article.Volume}\",\"{article.Issue}\",\"{article.PubYM}\",\"{article.Section}\",\"{article.Title}\",\"{article.Authors}\",\"{article.Literature}\",{article.DOI},{article.PdfUrl},";
                                if (article == null || article.Status == DownloadStatus.NotStarted)
                                    line += "";
                                if (article.Status == DownloadStatus.Success)
                                    line += "Success";
                                else
                                    line += "Failed";

                                writer.WriteLine(line);
                            }
                        }

                        // update volumes.csv

                        int lineNumberV = 0;
                        List<VolumeInfo> volumes = new List<VolumeInfo>();
                        using (TextFieldParser csvParser = new TextFieldParser(inVolumePath))
                        {
                            csvParser.CommentTokens = new string[] { "#" };
                            csvParser.SetDelimiters(new string[] { "," });
                            csvParser.HasFieldsEnclosedInQuotes = true;

                            // Skip the row with the column names
                            csvParser.ReadLine();

                            while (!csvParser.EndOfData)
                            {
                                try
                                {
                                    // Read current line fields, pointer moves to the next line.
                                    string[] fields = csvParser.ReadFields();
                                    if (string.IsNullOrEmpty(fields[1])) break;
                                    lineNumberV++;

                                    volumes.Add(new VolumeInfo
                                    {
                                        Publisher = fields[1],
                                        Journal = fields[2],
                                        Year = fields[3],
                                        Volume = fields[4],
                                        Issue = fields[5],
                                        FrontMatter = fields[6],
                                        BackMatter = fields[7],
                                        TOC = fields[8],
                                        FM_Down = fields[9],
                                        BM_Down = fields[10],
                                        TOC_Down = fields[11],
                                        TotalCount = string.IsNullOrEmpty(fields[12]) ? 0 : Convert.ToInt32(fields[12]),
                                        DownCount = string.IsNullOrEmpty(fields[13]) ? 0 : Convert.ToInt32(fields[13]),
                                    });
                                }
                                catch (Exception ex)
                                {
                                    await Log($"Exception occurs while reading csv : {inVolumePath}, at line {lineNumberV} error : {ex.Message} continue working...");
                                }
                            }
                        }

                        using (StreamWriter writer = new StreamWriter(outVolumePath, false))
                        {
                            writer.WriteLine(VolumesFileCSVHeader);
                            int number = 1;
                            foreach (var volume in volumes)
                            {
                                int totalCntInFolder = articles.Where(a => a.Publisher == volume.Publisher && a.Journal == volume.Journal && a.Year == volume.Year && a.Volume == volume.Volume && a.Issue == volume.Issue).Count();
                                int downCntInFolder = articles.Where(a => a.Publisher == volume.Publisher && a.Journal == volume.Journal && a.Year == volume.Year && a.Volume == volume.Volume && a.Issue == volume.Issue && a.Status == DownloadStatus.Success).Count();

                                string line = $"{number++},{volume.Publisher},{volume.Journal},{volume.Year},{volume.Volume},{volume.Issue},{volume.FrontMatter},{volume.BackMatter},{volume.TOC},{volume.FM_Down},{volume.BM_Down},{volume.TOC_Down},{totalCntInFolder},{downCntInFolder}";
                                writer.WriteLine(line);
                            }
                        }
                        await Log($" {file} is processed");
                    }

                    EnableControls(true);
                    await Log("Finished work");
                    IsDoingWork = false;
                    BtnStartWork.Content = "Start";
                    EnableControls(true);
                }
                else
                {
                    await Log("Stoped work.");
                    IsDoingWork = false;
                    BtnStartWork.Content = "Start";
                    EnableControls(true);
                }
            }
            catch (Exception ex)
            {
                await Log("Error occured while starting work. " + ex.Message);
            }
        }

        private async Task<bool> Log(string str)
        {
            string logStr = DateTime.Now.ToString("hh:mm:ss") + "    " + str + Environment.NewLine;
            TbLog.Text += logStr;
            using (var sw = File.AppendText(LogFilePath))
                sw.Write(logStr);

            await Task.Delay(10);
            return true;
        }

        private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch (Exception)
            {
            }
        }

        private void BtnSelectWorkFolder_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            dialog.InitialDirectory = !string.IsNullOrEmpty(WorkFolderPath) ? WorkFolderPath : "C:\\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

            WorkFolderPath = dialog.FileName;
            TbWorkFolderPath.Text = WorkFolderPath;

            BookDOIMerger.Properties.Settings.Default.WorkFolderPath = WorkFolderPath;
            BookDOIMerger.Properties.Settings.Default.Save();
        }

        private void TbLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            TbLog.SelectionStart = TbLog.Text.Length;
            TbLog.ScrollToEnd();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.Default.WorkFolderPath))
                TbWorkFolderPath.Text = Settings.Default.WorkFolderPath;
            WorkFolderPath = TbWorkFolderPath.Text;
        }

        public async static Task<string> GetString(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage message = await client.GetAsync(url).ConfigureAwait(false);
                    return await message.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void IudRetryMaxCnt_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }

        private void BtnMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowState = WindowState.Minimized;
            }
            catch (Exception)
            {
            }
        }
    }
}
