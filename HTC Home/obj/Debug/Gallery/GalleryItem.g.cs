﻿#pragma checksum "..\..\..\Gallery\GalleryItem.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "22ADC5474DE504B45933A70967B6BC970EFDFDDABEFF447102A82B656461C507"
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


namespace HTCHome.Gallery {
    
    
    /// <summary>
    /// GalleryItem
    /// </summary>
    public partial class GalleryItem : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 4 "..\..\..\Gallery\GalleryItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal HTCHome.Gallery.GalleryItem @this;
        
        #line default
        #line hidden
        
        
        #line 8 "..\..\..\Gallery\GalleryItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.ScaleTransform Scale;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Gallery\GalleryItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image CloseButton;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Gallery\GalleryItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock WidgetName;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Gallery\GalleryItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border VisualBorder;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Gallery\GalleryItem.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.VisualBrush VisualBrush;
        
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
            System.Uri resourceLocater = new System.Uri("/HTCHome;component/gallery/galleryitem.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Gallery\GalleryItem.xaml"
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
            this.@this = ((HTCHome.Gallery.GalleryItem)(target));
            
            #line 6 "..\..\..\Gallery\GalleryItem.xaml"
            this.@this.Loaded += new System.Windows.RoutedEventHandler(this.ThisLoaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Scale = ((System.Windows.Media.ScaleTransform)(target));
            return;
            case 3:
            
            #line 26 "..\..\..\Gallery\GalleryItem.xaml"
            ((System.Windows.Media.Animation.Storyboard)(target)).Completed += new System.EventHandler(this.UnloadAnimCompleted);
            
            #line default
            #line hidden
            return;
            case 4:
            this.CloseButton = ((System.Windows.Controls.Image)(target));
            
            #line 33 "..\..\..\Gallery\GalleryItem.xaml"
            this.CloseButton.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.CloseButtonMouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.WidgetName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.VisualBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 7:
            this.VisualBrush = ((System.Windows.Media.VisualBrush)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

