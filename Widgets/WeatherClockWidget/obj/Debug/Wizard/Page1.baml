   M S B A M L   `   `   `  ����  WeatherClock%    WeatherClockWidget.Wizard.Page1   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        � 	Resources  5       �� Source�� 5   
   $ ./Block.xaml?�6   %     � Triggers 5      :� 5   
    :� RoutedEvent$ Loaded��6   .[�5	      �� .��5
      ��  g� 	BeginTime$ 0:0:0.5X�6   .��5      W�  �� 
TargetName$ SunRotation��6&    �� TargetProperty$ Angle�6J    W� From$	 -35X�6l   	 W� To$ 0X�6w   	 g� Duration$	 0:0:0.7D�6~   W� 5      $ SunScale��6&   $ ScaleX�6G   $	 0.7X�6j   $ 1X�6u   $	 0:0:0.7D�6|   W� 5      $ SunScale��6&   $ ScaleY�6G   $	 0.7X�6j   $ 1X�6u   $	 0:0:0.7D�6|   W� 5      $	 Sun��6&   $ Opacity�6B   $ 0X�6f   $ 1X�6o   $	 0:0:0.7D�6v   W� 5      $ Cloud1Translation��6&   $ X�6P   $ 80X�6n   $	 -20X�6x   $	 0:0:0.7D�6�   W� 5      $ Cloud2Translation��6&   $ X�6P   $	 -80X�6n   $ 30X�6y   $	 0:0:0.7D�6�   W� 5      $ Cloud1��6&   $ Opacity�6E   $ 0X�6i   $ 1X�6r   $	 0:0:0.7D�6y   W� 5      $ Cloud2��6&   $ Opacity�6E   $ 0X�6i   $ 1X�6r   $	 0:0:0.7D�6y   5      5      5      .��5      � .+�5   
   >�  	  Block#��[  6   $��0,0,0,40q�65   �� 5   
   -   5        $	��Sun��5      $"��../Resources/Weather/sun.png��6   $	��150��6A   $	��150��6M   $��Center=�6Z   
 J� RenderTransformOrigin$
 0.5,0.5&�6u    J� Opacity$ 0Q�6�   z�5      V� 5      .~�5      �� -   5        SunRotation��5   &   �� 5      -   5        SunScale��5   %   5      5       �� 5!   
   -   5        $��Cloud1��5!      $&�� ../Resources/Weather/cloud_3.png��6   $	��150��6I   $	��150��6U   $��Center=�6b   $��0,45,0,0q�6}   $ 0Q�6�   z�5"      T� 5#      -   5        Cloud1Translation��5#   %   5%      �� 5&   
   -   5        $��Cloud2��5&      $&�� ../Resources/Weather/cloud_9.png��6   $	��150��6I   $	��150��6U   $��Center=�6b   $��0,70,0,0q�6}   $ 0Q�6�   z�5'      T� 5(      -   5        Cloud2Translation��5(   %   5*      �� 5+   
   .+�5,      �� -   5        $��Title��5,      $&�� Welcome to Weather/Clock widget!��6%   $��Center=�6M   $��0,15,0,0q�6j   $��22(�6|   
�������6�   �� 5-      -	   5        $��String1��5-      $W��QThis wizard will help you to setup widget for your needs. Click next to continue.��6'   $��
10,10,10,0q�6�   $��14(�6�   $
��Wrap=�6�   �� 5.      -
   5        $��String2��5.      $b��\You can also skip this wizard by clicking Skip button. Then default options will be applied.��6'   $��
10,10,10,0q�6�   $��14(�6�   $
��Wrap=�6�   5/      �� 50   
    �� Orientation$ 
Horizontal=�6   $��Bottom=�6.   $��Right=�6I   $��	0,0,10,10q�6e   .+�51      �� -   5        $��
SkipButton��51      $
��Skip��6'   $��75��66   $��23��6A   $��0,0,15,0q�6M   6_   �� 52      -   5        $��
NextButton��52      $
��Next��6'   $��75��66   $��23��6A   6M   53      54      55      