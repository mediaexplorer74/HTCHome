   M S B A M L   `   `   `  ����  HTCHome    HTCHome.Widget   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5        
������   � AllowsTransparency  .  � WindowStyle$
 None=�5   <    � 
ResizeMode$ NoResize=�6O    � ShowInTaskbar .   J� 	AllowDrop .  J� Opacity$ 0Q�5   9    � 	Resources 5         LoadAnim �� TargetProperty W� From		 W� To
 g� Duration  
MouseEnter g� FillBehavior  
MouseLeave��%�  &	        &	 v     &	 �     �� 5   
   -   5        .��5
      W� $ Opacity�6&   $ 0X�6J   $	 1X�6S   $
 0:0:0.5D�6Z   5      �� 5   
   .��5      W� -   5        $ Opacity�5      $	 1X�6B   $
 0:0:0.3D�6I   $
 Stop=�6\   6p   5      �� 5   
   .��5      W� -   5        $ Opacity�5      $
 0:0:0.3D�6B   $
 Stop=�6U   6i   5      5       � ContextMenu 5      �� 5   
   -   5         �� HasDropShadow . .@�5      m� -   5        $��TopMostItem��5      $��Top most��6*    m� IsCheckable . 6|   m� 5      -   5        $��PinItem��5      $	��Pin��6&    . 6`   m� 5      -   5        $��SizeItem��5      $
��Size��6'   {�.  
���   �6G   �� 5      -	   5        $��
SizeSlider��5      $��	0,0,-30,0q�6'    �� Orientation$ 
Horizontal=�6:    � Minimum$ 30Q�6S    � Maximum$	 100Q�6`    � SmallChange$
 0.05Q�6n    � Value$	 100Q�6�    �� TickPlacement$ TopLeft=�6�    �� TickFrequency$ 10Q�6�    �� AutoToolTipPlacement$ TopLeft=�6�   m� 5      -
   5        $��OpacityItem��5      $��Opacity��6*   {�.  
���   �6M   �� 5      -   5        $��OpacitySlider��5      $��	0,0,-30,0q�6*   $ 
Horizontal=�6=   $ 20Q�6V   $	 100Q�6c   $
 0.05Q�6q   $	 100Q�6�   $ TopLeft=�6�   $ 10Q�6�   $ TopLeft=�6�   m� 5      -   5        $��	CloseItem��5      $��Close widget��6(   6>   �� 5      �� 5      -   5        $��AddWidgetPanel��5       �� Orientation$ 
Horizontal=�6/   $��Center=�6H   �� 5      m� 5      -   5        $��GalleryItem��5      $��Widgets��6*    m� Icon 5       �� 5!      $��Resources/icon.png��6   $��16��68   $��16��6C   5#      m� 5$      -   5        $��HomeOptionsItem��5$      $��HTC Home settings��6.    5%      �� 5&      $��Resources/icon.png��6   $��16��68   $��16��6C   5(      m� 5)      -   5        $��CloseHomeItem��5)      $��Close HTC Home��6,    5*      �� 5+      $��Resources/icon.png��6   $��16��68   $��16��6C   5-      5.      .��50      � -   5        $��MainGrid��50      52      53      