   M S B A M L   `   `   `  ����  WeatherClock3    -WeatherClockWidget.WeatherAnimation.RainWiper   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          J� Opacity$  0Q�5   *    � 	Resources 5         MoveAnim �� 
TargetName �� TargetProperty W� From	 W� To g� Duration g� AutoReverse g� 	BeginTime  StreaksAnim	 g� FillBehavior��%]  &	        &	 �     �� 5   
   .��5      W� -   5        $ Rotation��5      $ Angle�6?   $	 170X�6a   $
 -170X�6l   $ 0:0:0.8D�6v    . $ 0:0:8X�6�   6�   5      �� 5	   
   .��5
      W� $ Grad1��6   $ Offset�6<   $ 0X�6_   $ 1X�6h   $ 0:0:0.35D�6o   $ 0:0:8.2X�6�   W� 5      $ Grad2��6   $ Offset�6<   $ 0X�6_   $ 1X�6h   $ 0:0:0.35D�6o   $ 0:0:8.1X�6�   W� 5      -   5        $ Streaks��5      $ Opacity�6>   $	 0.5X�6b   $ 0X�6m   $ 0:0:0.3D�6t   $ 0:0:9.2X�6�   $
	 Stop=�6�   6�   5      5      .��5      � .+�5   
   �� -   5        $��Wiper��5      $��40��6   $	��270��6(   $
��Fill=�65   
 J� RenderTransformOrigin$
 0.5,0&�6D   $	��Top=�6b   z�5      V� 5      .~�5      �� -   5        Rotation�� �� Angle$ 0Q�5   8   T� 5       T� Y$	 -20Q�6)   5      5      � 5   
   $��5,40,0,0q�6    J� OpacityMask 5      �� 5       �� 
StartPoint$	 0,0&�6&    �� EndPoint$	 1,0&�67   .��5      � -   5        	Grad1�� � Color$ 	#FF000000��5   2    � Offset$ 0Q�6D   � 5      -   5        	Grad2��$ 	#00000000��5   2   $ 0Q�6D   5      .+�5      �� -   5        $��Streaks��5      $	��295��6#   $	��210��6/   $
��Fill=�6<   $	  0.5Q�6K   5       5!      5"      