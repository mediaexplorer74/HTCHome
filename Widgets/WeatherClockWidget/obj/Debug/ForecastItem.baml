   M S B A M L   `   `   `  ����  WeatherClock%    WeatherClockWidget.ForecastItem   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5        .��5      �    ForecastItemBaseStyle#���   6   .+�5   
   �� -   5        $
��Icon��5      $'��!Skins\Sense\Weather\weather_1.png��6     ForecastItemIconStyle#���  6G   z�5      T� 5      -   5        	Transform��5   %     � 	Resources  5
        	FlipAnim1 W�Name �� 
TargetName �� TargetProperty W� From	 W� To g� Duration  	FlipAnim2��%"  &	       &	     �� 5      .��5      W� $ TransformAnim1��6&   -   5        $ 	Transform��5   >   $ Y�6`   $ 0X�6~   $ 10X�6�   $ 0:0:0.3D�6�   6�   W� 5      $ OpacityAnim1��6&   $
 Icon��6<   $ Opacity�6Y   $ 0X�6}   $ 0:0:0.3D�6�   5      �� 5      .��5      W� $ TransformAnim2��6&   $ 	Transform��6>   $ Y�6`   $	 -10X�6~   $ 0X�6�   $ 0:0:0.3D�6�   W� 5      $ OpacityAnim2��6&   $
 Icon��6<   $ Opacity�6Y   $ 1X�6}   $ 0:0:0.3D�6�   5      5      5      � 5   
     ForecastItemDayGridStyle#���  6   .+�5      �� -   5        $��	DayShadow��5        ForecastItemDayShadowStyle#���  6)   ��6^   ��  �� ElementName$	 Day�� �� Path$
 Text��� 5      -   5        $	��Day��5        ForecastItemDayStyle#���  6#   $	��Mon��6R   5      �� 5   
    % !ForecastItemTemperaturePanelStyle#���  6   .+�5      �  % !ForecastItemTemperatureHGridStyle#���  6   .+�5      �� -   5        $��TemperatureHShadow��5       '	 #ForecastItemTemperatureHShadowStyle#��� 	 66   ��6t   �� $ TemperatureH��$
 Text��� 5      -   5        $��TemperatureH��5       !
 ForecastItemTemperatureHStyle#��� 
 60   $
��0⁰��6h   5      � 5       % !ForecastItemTemperatureLGridStyle#���  6   .+�5      �� -	   5        $��TemperatureLShadow��5       ' #ForecastItemTemperatureLShadowStyle#���  66   ��6t   �� $ TemperatureL��$
 Text��� 5       -
   5        $��TemperatureL��5        ! ForecastItemTemperatureLStyle#���  60   $
��0⁰��6h   5!      5"      5#      5$      