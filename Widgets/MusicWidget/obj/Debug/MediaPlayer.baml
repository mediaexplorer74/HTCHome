   M S B A M L   `   `   `  ����	  Music#    MediaPlayerWidget.MediaPlayer   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5           WidgetStyle#���   5   *     � 	Resources  5      �� 5   
   -   5        Skin��%    5      .��5      � -   5        $
��Root��5        BasePanelStyle#���  6     5	   
     VolumeFadeIn �� 
TargetName �� TargetProperty	 W� To g� Duration  VolumeFadeOut g� 	BeginTime��%  &	       &	 �     �� 5
      .��5      W� -   5        $ 	VolumeBox��5   "   $ Opacity�6D   $ 1X�6h   $ 0:0:0.5D�6o   6�   5      �� 5      $ 0:0:1.5X�6/   .��5      W� $ 	VolumeBox��6"   $ Opacity�6D   $ 0X�6h   $ 0:0:0.5D�6o   5      5      z�5   
   �� 5      -   5        	Scale�� �� ScaleX$ 1Q�5   ,    �� ScaleY 67   ��  �� ElementName$ Scale��	 �� Path$	 ScaleX�.+�5   
   >�   BackgroundStyle#���  6   � 5&   
   -   5        $��AlbumArtGrid��5&        AlbumArtGridStyle#���  6#   .+�5'      �� -   5        $��AlbumArtCover��5'        AlbumArtCoverStyle#���  6)   �� 5(      -   5        $��AlbumArt��5(        AlbumArtStyle#���  6$   5)      � 5+   
     SongTitleGridStyle#���  6   .+�5,      �� -	   5        $��SongTitleShadow��5,       	 SongTitleShadowStyle#��� 	 6a   ��6/   �� $ 	SongTitle��$
	 Text�6a   �� 5-      -
   5        $��	SongTitle��5-      $��No media��6)    
 SongTitleStyle#��� 
 69   5.      � 50   
     SongAlbumGridStyle#���  6   .+�51      �� -   5        $��SongAlbumShadow��51        SongAlbumShadowStyle#���  6a   ��6/   �� $ 	SongAlbum��$
	 Text�6a   �� 52      -   5        $��	SongAlbum��52      $��No media��6)     SongAlbumStyle#���  69   53      � 54   
     SongTimeGridStyle#���  6   .+�55      �� -   5        $��SongTimeShadow��55        SongTimeShadowStyle#���  6_   ��6.   �� $ SongTime��$
	 Text�6_   �� 56      -   5        $��SongTime��56      $
��0:00��6(     SongTimeStyle#���  64   57      � 58   
     SongTimeLeftGridStyle#���  6   .+�59      �� -   5        $��SongTimeLeftShadow��59        SongTimeLeftShadowStyle#���  6g   ��62   �� $ SongTimeLeft��$
	 Text�6g   �� 5:      -   5        $��SongTimeLeft��5:      $
��0:00��6,     SongTimeLeftStyle#���  68   5;      � 5<   
     SongArtistGridStyle#���  6   .+�5=      �� -   5        $��SongArtistShadow��5=        SongArtistShadowStyle#���  6h   ��60   �� $ SongArtistTitle��$
	 Text�6h   �� 5>      -   5        $��SongArtistTitle��5>      $�� ��6/     SongArtistStyle#���  67   5?      � 5@   
   -   5        $��Progress��5@   B     SongProgressStyle#���  6   
 � Cursor$

 Hand��6R   6`   �� 5B   
     ControlButtonsPanelStyle#���  6   .+�5C      �� -   5        $��
PrevButton��5C        PrevButtonStyle#���  6'   6Q   d� 5D      -   5        $��
PlayButton��5D        PlayButtonStyle#���  6-   6W   �� 5E      -   5        $��
NextButton��5E        NextButtonStyle#���  6'   6Q   5F      �� 5G   
   -   5        $��	VolumeBox��5G        VolumeBoxStyle#���  6%   5H      5I      