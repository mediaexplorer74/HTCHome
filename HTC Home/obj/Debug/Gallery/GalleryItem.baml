   M S B A M L   `   `   `  ����  HTCHome!    HTCHome.Gallery.GalleryItem   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$
  this��5      $	��200��5       J� RenderTransformOrigin$ 0.5,0.5&�6   z�5      �� 5   
   -   5        	Scale��5       � 	Resources 5
         LoadAnim �� 
TargetName �� TargetProperty g� DecelerationRatio9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value W� From		 W� To
 g� Duration  
UnloadAnim g� AccelerationRatio��%�  &	        &	 (    �� 5   
   .��5      U� $ Scale��6,   $ ScaleX�6J   $	 0.3Q�6m   .\�5        $ 0:0:0.0��6'   $ 2Q�69     5      $ 0:0:0.3��6'   $	 0.8Q�69     5      $ 0:0:0.4��6'   $ 1Q�69   5      U� 5      $ Scale��6,   $ ScaleY�6J   $	 0.3Q�6m   .\�5        $ 0:0:0.0��6'   $ 2Q�69     5      $ 0:0:0.3��6'   $	 0.8Q�69     5      $ 0:0:0.4��6'   $ 1Q�69   5      W� 5      $
 this��6   $ Opacity�6;   $ 0X�6_   $	 1X�6h   $
 0:0:0.3D�6o   5      �� 5   
   -   5        .��5      W� $ Scale��6   $ ScaleX�6<   $ 1X�6_   $	 2X�6h   $
 0:0:0.3D�6o   $	 0.3Q�6�   W� 5      $ Scale��6   $ ScaleY�6<   $ 1X�6_   $	 2X�6h   $
 0:0:0.3D�6o   $	 0.3Q�6�   W� 5      $
 this��6   $ Opacity�6;   $ 1X�6_   $	 0X�6h   $
 0:0:0.3D�6o   5      5      .��5       ]� .+�5!   
   �� -   5        $��CloseButton��5!      $��Bottom=�6#   $��30��6;   $8��2/HTCHome;component/Resources/gallery_itemclose.png��6F   $��0,0,0,0q�6�   6�   �� 5"   
   $	��Top=�6   ���	#77000000
�������$��1q�6Q    �� CornerRadius$ 7��6e   $��5q�6v   $��Center=�6�   ._�5#      �� -   5        $��
WidgetName��5#      
�������$��Widget Name��6=   $��5q�6P    �� TextAlignment$ Center=�6[   5$      �� 5%   
   -   5        $��VisualBorder��5%      ��5&      !� 5'      -   5        VisualBrush�� j� Stretch$ Uniform=�5'   3   5)      5*      5+      