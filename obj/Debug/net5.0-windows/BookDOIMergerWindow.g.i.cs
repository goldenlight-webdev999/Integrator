﻿#pragma checksum "..\..\..\BookDOIMergerWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CBED9B8A3F6C851FFFFF4E8CE321A19A8440DA8D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using BookScrapperDOI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Converters;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Mag.Converters;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace BookScrapperDOI {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 45 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnMinimizeWindow;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnCloseWindow;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSelectExcelFile;
        
        #line default
        #line hidden
        
        
        #line 143 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TbWorkFolderPath;
        
        #line default
        #line hidden
        
        
        #line 157 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnStartWork;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LbProgressStatusTotal;
        
        #line default
        #line hidden
        
        
        #line 202 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LbFailedStatusTotal;
        
        #line default
        #line hidden
        
        
        #line 212 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LbTimeEstimationTotal;
        
        #line default
        #line hidden
        
        
        #line 226 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LbProgressStatus;
        
        #line default
        #line hidden
        
        
        #line 236 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LbFailedStatus;
        
        #line default
        #line hidden
        
        
        #line 246 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LbTimeEstimation;
        
        #line default
        #line hidden
        
        
        #line 258 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar PbStatus;
        
        #line default
        #line hidden
        
        
        #line 265 "..\..\..\BookDOIMergerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TbLog;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.14.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BookDOIMerger;V1.0.0.0;component/bookdoimergerwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\BookDOIMergerWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.14.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 12 "..\..\..\BookDOIMergerWindow.xaml"
            ((BookScrapperDOI.MainWindow)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseDown);
            
            #line default
            #line hidden
            
            #line 12 "..\..\..\BookDOIMergerWindow.xaml"
            ((BookScrapperDOI.MainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            
            #line 12 "..\..\..\BookDOIMergerWindow.xaml"
            ((BookScrapperDOI.MainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.BtnMinimizeWindow = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\..\BookDOIMergerWindow.xaml"
            this.BtnMinimizeWindow.Click += new System.Windows.RoutedEventHandler(this.BtnMinimizeWindow_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.BtnCloseWindow = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\..\BookDOIMergerWindow.xaml"
            this.BtnCloseWindow.Click += new System.Windows.RoutedEventHandler(this.BtnCloseWindow_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.BtnSelectExcelFile = ((System.Windows.Controls.Button)(target));
            
            #line 115 "..\..\..\BookDOIMergerWindow.xaml"
            this.BtnSelectExcelFile.Click += new System.Windows.RoutedEventHandler(this.BtnSelectWorkFolder_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.TbWorkFolderPath = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.BtnStartWork = ((System.Windows.Controls.Button)(target));
            
            #line 161 "..\..\..\BookDOIMergerWindow.xaml"
            this.BtnStartWork.Click += new System.Windows.RoutedEventHandler(this.BtnStartWork_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.LbProgressStatusTotal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.LbFailedStatusTotal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.LbTimeEstimationTotal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.LbProgressStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.LbFailedStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.LbTimeEstimation = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.PbStatus = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 14:
            this.TbLog = ((System.Windows.Controls.TextBox)(target));
            
            #line 278 "..\..\..\BookDOIMergerWindow.xaml"
            this.TbLog.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TbLog_TextChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

