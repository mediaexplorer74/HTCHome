   M S B A M L   `   `   `  ����  WeatherClock/    )WeatherClockWidget.WeatherAnimation.Cloud   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$
  this��5       � 	Resources 5         MoveAnim �� 
TargetName �� TargetProperty W� From	 W� To g� Duration 
 FadeIn  FadeOut��%�  &	        &	 �     &	 (    �� 5   
   .��5      W� -   5        $
 this��5      $ (Canvas.Left)�6;   $	 -50X�6e   $	 300X�6p   $ 0:0:3.5D�6y   6�   5	      �� 5
   
   .��5      W� -   5        $
 this��5      $ Opacity�6;   $ 0X�6_   $ 1X�6h   $ 0:0:0.3D�6o   6�   5      �� 5   
   .��5      W� $
 this��6   $ Opacity�6;   $ 0X�6_   $ 0:0:0.3D�6f   5      5      .��5      � .+�5   
   �� -   5        $��Image��5       J� RenderTransformOrigin$ 0.5,0.5&�6   z�5      V� 5      .~�5      �� -   5        	Scale�� �� ScaleX$	 0.7Q�5   4   	 �� ScaleY	 6A   �� 
 �� ElementName$
 Scale�� �� Path$ ScaleX�5      5      5      5      