using NAudio.Lame;
using NAudio.MediaFoundation;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AudioAgent
{
    public class OutputEncodingProcessor
    {
        /// <summary>
        /// Convert WAV to MP3 using libmp3lame library
        /// </summary>
        /// <param name="waveFileName"></param>
        /// <param name="mp3FileName"></param>
        /// <param name="bitRate"></param>
        public void WaveToMP3(string waveFileName, string mp3FileName, int bitRate = 128)
        {
            using (var reader = new WaveFileReader(waveFileName))
            using (var writer = new LameMP3FileWriter(mp3FileName, reader.WaveFormat, bitRate))
                reader.CopyTo(writer);
        }

        /// <summary>
        /// Convert WAV to WMA 
        /// </summary>
        /// <param name="waveFileName"></param>
        /// <param name="mp3FileName"></param>
        /// <param name="bitRate"></param>
        public void WaveToWMA(string waveFileName, string wmaFileName, int bitRate = 44100)
        {
            using (MediaFoundationReader reader = new MediaFoundationReader(waveFileName))
            {
                MediaFoundationEncoder.EncodeToWma(reader, wmaFileName, bitRate);
            }
        }

        /// <summary>
        /// Convert WAV to AAC
        /// </summary>
        /// <param name="waveFileName"></param>
        /// <param name="mp3FileName"></param>
        /// <param name="bitRate"></param>
        public void WaveToAAC(string waveFileName, string aacFileName, int bitRate = 44100)
        {
            using (MediaFoundationReader reader = new MediaFoundationReader(waveFileName))
            {
                NAudio.MediaFoundation.MediaType mt = new NAudio.MediaFoundation.MediaType();
                mt.MajorType = NAudio.MediaFoundation.MediaTypes.MFMediaType_Audio;
                mt.SubType = NAudio.MediaFoundation.AudioSubtypes.MFAudioFormat_AAC;
                mt.BitsPerSample = 16;
                mt.SampleRate = bitRate;
                mt.ChannelCount = 2;
                using (MediaFoundationEncoder mfe = new MediaFoundationEncoder(mt))
                {
                    mfe.Encode(aacFileName, reader);
                }
                //MediaFoundationEncoder.EncodeToAac(reader, aacFileName, bitRate);
            }
        }
        /// <summary>
        /// Check if windows is vista or higher to run the WAV to AAC conversion
        /// </summary>
        /// <returns></returns>
        public bool IsWinVistaOrHigher()
        {
            OperatingSystem OS = Environment.OSVersion;
            return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
        }
    }
}
