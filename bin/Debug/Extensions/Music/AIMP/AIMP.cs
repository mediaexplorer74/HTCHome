using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AIMP
{
    public class AIMP
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenFileMapping(int dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("kernel32.dll")]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfbytesToMap);
        [DllImport("kernel32.dll")]
        public static extern int UnmapViewOfFile(IntPtr lpBaseAddress);
        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);
 


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        public struct MediaInfo
        {
            public int cbSizeOf;
            public bool nActive;
            public int nBitRate;
            public int nChannels;
            public int nDuration;
            public long nFileSize;
            public int nRating;
            public int nSampleRate;
            public int nTrackID;
            public int nAlbumLen;
            public int nArtistLen;
            public int nDateLen;
            public int nFileNameLen;
            public int nGenreLen;
            public int nTitleLen;
            public string sAlbum;
            public string sArtist;
            public string sDate;
            public string sFileName;
            public string sGenre;
            public string sTitle;
        }

 


        private enum AimpCommand
        {
            PlayPause = 0x10,
            Stop = 0x11,
            Next = 0x12,
            Prev = 0x13
        }

        public enum PlayingState
        {
            Stopped,
            Paused,
            Playing
        }

        private IntPtr hwnd;
        public bool IsRunning;
        public PlayingState playingState;
        public MediaInfo CurrentMedia;

        public AIMP()
        {
            Init();
        }

        private void Init()
        {
            hwnd = FindWindow("AIMP2_RemoteInfo", "AIMP2_RemoteInfo");
            if (hwnd != IntPtr.Zero)
            {
                IsRunning = true;

                UpdateState();
            }
        }

        public void UpdateState()
        {
            if (IsRunning)
            {
                switch (SendMessage(hwnd, 0x475, 1, 4))
                {
                    case 0:
                        playingState = AIMP.PlayingState.Stopped;
                        break;

                    case 1:
                        playingState = AIMP.PlayingState.Playing;
                        UpdateFileInfo();
                        break;

                    case 2:
                        playingState = AIMP.PlayingState.Paused;
                        UpdateFileInfo();
                        break;
                }
            }
        }

        private void SendCommand(AimpCommand c)
        {
            SendMessage(hwnd, 0x475, 3, (int)c);
        }

        public void PlayPause()
        {
            if (IsRunning)
                SendCommand(AimpCommand.PlayPause);
            else
                Init();
        }

        public void Prev()
        {
            if (IsRunning)
                SendCommand(AimpCommand.Prev);
            else
                Init();
        }
        public void Next()
        {
            if (IsRunning)
                SendCommand(AimpCommand.Next);
            else
                Init();
        }

        public int GetPosition()
        {
            return SendMessage(hwnd, 0x475, 1, 0x1f);
        }

        public void UpdateFileInfo()
        {
            CurrentMedia = new MediaInfo();
            string newValue = "";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            string str6 = "";
            IntPtr hFileMappingObject = OpenFileMapping(4, true, "AIMP2_RemoteInfo");
            IntPtr ptr = MapViewOfFile(hFileMappingObject, 4, 0, 0, 0x800);
            CurrentMedia.cbSizeOf = Marshal.ReadInt32(ptr);
            byte[] buffer = new byte[] { Marshal.ReadByte(ptr, 4), Marshal.ReadByte(ptr, 5), Marshal.ReadByte(ptr, 6), Marshal.ReadByte(ptr, 7) };
            CurrentMedia.nActive = BitConverter.ToBoolean(buffer, 0);
            CurrentMedia.nBitRate = Marshal.ReadInt32(ptr, 8);
            CurrentMedia.nChannels = Marshal.ReadInt32(ptr, 12);
            CurrentMedia.nDuration = Marshal.ReadInt32(ptr, 0x10);
            CurrentMedia.nFileSize = Marshal.ReadInt64(ptr, 20);
            CurrentMedia.nRating = Marshal.ReadInt32(ptr, 0x1c);
            CurrentMedia.nSampleRate = Marshal.ReadInt32(ptr, 0x20);
            CurrentMedia.nTrackID = Marshal.ReadInt32(ptr, 0x24);
            CurrentMedia.nAlbumLen = Marshal.ReadInt32(ptr, 40);
            CurrentMedia.nArtistLen = Marshal.ReadInt32(ptr, 0x2c);
            CurrentMedia.nDateLen = Marshal.ReadInt32(ptr, 0x30);
            CurrentMedia.nFileNameLen = Marshal.ReadInt32(ptr, 0x34);
            CurrentMedia.nGenreLen = Marshal.ReadInt32(ptr, 0x38);
            CurrentMedia.nTitleLen = Marshal.ReadInt32(ptr, 60);
            try
            {
                if (ptr != IntPtr.Zero)
                {
                    //sInfo[0] = info.nTrackID.ToString();
                    string str = Marshal.PtrToStringUni(new IntPtr(ptr.ToInt32() + CurrentMedia.cbSizeOf));
                    CurrentMedia.sAlbum = str.Substring(0, CurrentMedia.nAlbumLen);
                    str = str.Substring(CurrentMedia.nAlbumLen);
                    newValue = str.Substring(0, CurrentMedia.nArtistLen);
                    CurrentMedia.sArtist = newValue;
                    str = str.Substring(CurrentMedia.nArtistLen);
                    CurrentMedia.sFileName = str.Substring(CurrentMedia.nDateLen, CurrentMedia.nFileNameLen);
                    str = str.Substring(CurrentMedia.nDateLen + CurrentMedia.nFileNameLen);
                    str6 = str.Substring(0, CurrentMedia.nGenreLen);
                    //sInfo[4] = str6;
                    CurrentMedia.sTitle = str.Substring(CurrentMedia.nGenreLen).Substring(0, CurrentMedia.nTitleLen);
                    //sInfo[2] = str2;
                    //str4 = SampleBarsUnit.ConvertTime(info.nDuration / 0x3e8);
                    //sInfo[5] = string.Format("Chanell: {0}, Bitrate: {1}kbps, Sample: {2}Hz", info.nChannels.ToString(), info.nBitRate.ToString(), info.nSampleRate.ToString());
                }
            }
            finally
            {
                UnmapViewOfFile(ptr);
                CloseHandle(hFileMappingObject);
            }
        }

 

    }
}
