   M S B A M L   `   `   `  ����  WeatherClock%    WeatherClockWidget.LocationItem   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      $��20��5        J� Opacity$  0Q�6   z�5      T� 5   
   -   5        	Transform��5       � Triggers 5      :� 5	   
    :� RoutedEvent$ UserControl.Loaded��6   .[�5
      �� .��5      �� .��5      W� -   5         W�Name$ TransformAnim��5   &    �� 
TargetName$ 	Transform��6=    �� TargetProperty$ X�6_    W� From$	 -35X�6}   	 W� To$ 0X�6�    g� Duration$ 0:0:0.3D�6�   W� 5      -   5        $ OpacityAnim��5   &   $ Opacity�6;   $ 0X�6_   $ 1X�6h   $ 0:0:0.3D�6o   5      5      5      :� 5   
   $ UserControl.MouseEnter��6   .[�5      �� .��5      �� .��5      W� $	 Bg2��6&   $ Opacity�6B   $ 1X�6f   $ 0:0:0.12D�6m   5      5      5      :� 5   
   $ UserControl.MouseLeave��6   .[�5      �� .��5      �� .��5      W� $	 Bg2��6&   $ Opacity�6B   $ 0X�6f   $ 0:0:0.12D�6m   5      5      5      .��5       � .+�5!   
   �� -   5        $	��Bg2��5!      ���#B8D6FB	 �� RadiusX$	 4��60   
 �� RadiusY$
 4��6<   $  0Q�6H   ��5"      �� 5#       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5$      �  � Color$ 	#11FAFBFD��6#    � Offset$ 0Q�65   � 5%      $ 	#B2EBF3FD��6#   $ 1Q�65   5&      5(      �� 5)   
   -   5        $	��Bg1��5)      ���#B8D6FB$	 4��60   $
 4��6<   $  0Q�6H   ��5*      �� 5+      $ 0.5,1&�6&   $ 0.5,0&�67   .��5,      � $ 	#B2DCEBFC��6#   $ 0Q�65   � 5-      $ 	#7FC1DBFC��6#   $ 1Q�65   5.      50      �� 51   
   -   5        $��ItemTitleTextBlock��51      $��5,0,5,0q�6.   $��Header��6?    �� TextAlignment$
 Left=�6M   $��CharacterEllipsis=�6b   $��Center=�6�   52      53      