﻿#pragma checksum "..\..\..\Wizard\Page2.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "016720BFA3F84609EEF811F853DBFB6D2080F80625372D363EF22603DB6B14E1"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace WeatherClockWidget.Wizard {
    
    
    /// <summary>
    /// Page2
    /// </summary>
    public partial class Page2 : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Title;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock String1;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ProviderBox;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock String2;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox LocationBox;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel SearchResults;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar SearchProgress;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Wizard\Page2.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NextButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WeatherClock;component/wizard/page2.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Wizard\Page2.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 4 "..\..\..\Wizard\Page2.xaml"
            ((WeatherClockWidget.Wizard.Page2)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Title = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.String1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.ProviderBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 13 "..\..\..\Wizard\Page2.xaml"
            this.ProviderBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ProviderBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.String2 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.LocationBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 16 "..\..\..\Wizard\Page2.xaml"
            this.LocationBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.LocationBox_KeyDown);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 17 "..\..\..\Wizard\Page2.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.SearchResults = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            this.SearchProgress = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 10:
            this.NextButton = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\..\Wizard\Page2.xaml"
            this.NextButton.Click += new System.Windows.RoutedEventHandler(this.NextButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

