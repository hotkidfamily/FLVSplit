// --------------------------------------------------------------------------------
// Copyright (c) 2006 J.D. Purcell
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// --------------------------------------------------------------------------------

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace JDP
{
    internal interface IAudioWriter {
        void WriteChunk(byte[] chunk, uint timeStamp);
        void Finish();
        string Path { get; }
    }

    internal interface IVideoWriter {
        void WriteChunk(byte[] chunk, uint timeStamp, int frameType, bool hevc_in_annexb=false);
        void Finish(FractionUInt32 averageFrameRate);
        string Path { get; }
    }

    public delegate bool OverwriteDelegate(string destPath);

    public class OutlierDetection
    {
        public static List<uint> RemoveOutliers(List<uint> data)
        {
            var sortedData = data.OrderBy(x => x).ToList();
            int count = sortedData.Count;

            double Q1 = sortedData[(int)(count * 0.25)];
            double Q3 = sortedData[(int)(count * 0.75)];
            double IQR = Q3 - Q1;

            double lowerBound = Q1 - 1.5 * IQR;
            double upperBound = Q3 + 1.5 * IQR;

            return data.Where(x => x >= lowerBound && x <= upperBound).ToList();
        }
    }

    public class LiveTime
    {
        private const uint MaxiumGapDuration = 3000;
        private const uint MaxiumCorrectGapFrames = 10;
        private const uint MaxiumCorrectFirstLimitedTimes = 16; /* as much as 16 b frames */
        private const uint MaxiumFramesAhead = 16;

        private static List<uint> CorrectStartFromZero(ref List<uint> times)
        {
            List<uint> nValues = times.ToList();
            uint baseValue = times[0];
            for (int i = 0; i < times.Count; i++)
            {
                nValues[i] -= baseValue;
            }
            return nValues;
        }

        private static List<uint> CorrectToZeroWithFirstLimitedTimes(ref List<uint> times, uint Limited)
        {
            uint sampleCount = Math.Min((uint)times.Count, Limited);
            uint minValue = uint.MaxValue;
            for (int i = 0; i < sampleCount; i++)
            {
                minValue = Math.Min(times[i], minValue);
            }

            List<uint> nValues = times.ToList();
            for (int i = 0; i < times.Count; i++)
            {
                nValues[i] -= minValue;
            }
            return nValues;
        }

        /* prevent equal value */
        private static List<uint> CorrectGrowingTimestamp(ref List<uint> times)
        {
            List<uint> nValues = times.ToList();
            for (int i = 0; i < times.Count - 1; i++)
            {
                if (times[i] >= times[i+1])
                {
                    Console.WriteLine($"-> find equal timestamp {times[i]} @ {i}");
                    nValues[i+1] = times[i]+1;
                }
            }
            return nValues;
        }

        private static void CorrectLogGapSignalChannel(ref List<uint> data, uint maxium, uint fps)
        {
            uint delta = 1000 / fps;

            int dataSize = Math.Min(data.Count, (int)maxium);

            for (int i = 1; i < dataSize; i++)
            {
                long vdiff = (long)data[i] - (long)data[i - 1];

                if (vdiff > MaxiumGapDuration)
                {
                    uint correctValue;
                    if (data[i-1] == 0)
                    {
                        correctValue = Math.Max(0, data[i] - delta);
                    }
                    else
                    {
                        correctValue = data[i] - delta * MaxiumFramesAhead;
                    }

                    Console.WriteLine($"-> Long Gap {vdiff}@{i} Fix {data[i-1]} -> {correctValue}");

                    data[i-1] = correctValue;
                }
            }
        }

        /* limited frame duration in 10s */
        private static void CorrectLongGap(ref List<uint> audio, ref List<uint> video, uint maxium, uint fps)
        {
            uint delta = 1000 / fps;

            int dataSize = Math.Min(Math.Min(audio.Count, (int)maxium), video.Count);

            for (int i = 1; i < dataSize; i++)
            {
                uint adiff = audio[i] - audio[i - 1];
                uint vdiff = video[i] - video[i - 1];

                if (vdiff > MaxiumGapDuration)
                {
                    uint correctValue;
                    if (adiff < vdiff)
                    {
                        correctValue = audio[i];
                    }
                    else
                    {
                        correctValue = Math.Max(0, video[i] - delta);
                    }

                    Console.WriteLine($"-> Video Long Gap {vdiff}@{i} Fix {video[i-1]} -> {correctValue}");

                    video[i-1] = correctValue;
                }

                if (adiff > MaxiumGapDuration)
                {
                    uint correctValue;

                    if (vdiff < adiff)
                    {
                        correctValue = video[i];
                    }
                    else
                    {
                        correctValue = Math.Max(0, audio[i] - delta);
                    }

                    Console.WriteLine($"-> Audio Long Gap {vdiff}@{i} Fix {audio[i-1]} -> {correctValue}");

                    audio[i-1] = correctValue;
                }
            }
        }

        public static (List<uint>, List<uint>) CorrectRelativeLinearData(ref List<uint> audio, ref List<uint> video, uint fps)
        {
            var na = CorrectGrowingTimestamp(ref audio);
            var nv = CorrectGrowingTimestamp(ref video);

            CorrectLongGap(ref na, ref nv, MaxiumCorrectGapFrames, fps);

            var na2 = CorrectStartFromZero(ref na);
            var nv2 = CorrectStartFromZero(ref nv);
            return (na2, nv2);
        }

        public static List<uint> CorrectLinearData(ref List<uint> data, uint fps)
        {
            var na = CorrectGrowingTimestamp(ref data);
            CorrectLogGapSignalChannel(ref na, MaxiumCorrectGapFrames, fps);
            var na2 = CorrectStartFromZero(ref na);
            return na2;
        }

        public static List<uint> CorrectJumpyData(ref List<uint> data, uint fps)
        {
            var ov = data.ToList();
            CorrectLogGapSignalChannel(ref ov, MaxiumCorrectGapFrames, fps);
            var na2 = CorrectToZeroWithFirstLimitedTimes(ref ov, MaxiumCorrectFirstLimitedTimes);
            return na2;
        }

        public static void CorrectPTSBiggerThanDTS(ref List<uint> dts, ref List<uint> pts)
        {
            long maxDiff = long.MinValue;
            int dataSize = dts.Count;

            for (int i = 0; i<dataSize; i++)
            {
                long ptsDiff = (long)dts[i] - (long)pts[i];
                maxDiff = Math.Max(maxDiff, ptsDiff);
            }

            maxDiff = Math.Abs(maxDiff);
            Console.WriteLine($"-> pts offset {maxDiff}");

            for (int i = 0; i<dataSize; i++)
            {
                pts[i] += (uint)maxDiff;
            }
        }
    }

    public class FLVFile : IDisposable {
        private static readonly string[] _outputExtensions = new string[] { ".avi", ".mp3", ".264", ".aac", ".spx", ".txt" };

        private string _inputPath;
        private string _outputDirectory;
        private string _outputPathBase;
        private OverwriteDelegate _overwrite;
        private FileStream _fs;
        private long _fileOffset, _fileLength;
        private IAudioWriter _audioWriter;
        private IVideoWriter _videoWriter;
        private TimeCodeWriter _timeCodeWriter;
        private List<uint> _videoDTS;
        private List<uint> _videoPTS;
        private List<uint> _audioPTS;
        private bool _extractAudio;
        private bool _extractVideo;
        private bool _extractTimeCodes;
        private bool _transCode;
        private bool _FFmpegInternal = true;
        private bool _FFmpegCommand = true;
        private bool _extractedAudio;
        private bool _extractedVideo;
        private bool _extractedTimeCodes;
        private FractionUInt32? _averageFrameRate;
        private FractionUInt32? _trueFrameRate;
        private FractionUInt32? _guessFrameRate;
        private List<string> _warnings;
        private bool _hevc_in_annexb = false;

        public FLVFile(string path) {
            _inputPath = path;
            _outputDirectory = Path.GetDirectoryName(path);
            _warnings = new List<string>();
            _fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536);
            _fileOffset = 0;
            _fileLength = _fs.Length;
        }

        public void Dispose() {
            if (_fs != null) {
                _fs.Close();
                _fs = null;
            }
            CloseOutput(null, true);
        }

        public void Close() {
            Dispose();
        }

        public string OutputDirectory {
            get { return _outputDirectory; }
            set { _outputDirectory = value; }
        }

        public FractionUInt32? AverageFrameRate {
            get { return _averageFrameRate; }
        }

        public FractionUInt32? TrueFrameRate {
            get { return _trueFrameRate; }
        }

        public FractionUInt32? GuessFrameRate
        {
            get { return _guessFrameRate; }
        }

        public string[] Warnings {
            get { return _warnings.ToArray(); }
        }

        public bool ExtractedAudio {
            get { return _extractedAudio; }
        }

        public bool ExtractedVideo {
            get { return _extractedVideo; }
        }

        public bool ExtractedTimeCodes {
            get { return _extractedTimeCodes; }
        }
        public bool TransCode
        {
            get { return _transCode; }
        }

        private void run_remuxer(string videoInput, string audioInput, uint fps)
        {
            string commander = default;
            string output = _inputPath + ".mp4";
            string args = default;
            bool v = videoInput != null && File.Exists(videoInput);
            bool a = audioInput != null && File.Exists(audioInput);

            if (!v && !a) {
                return ;
            }

            if(!_FFmpegCommand)
            {
                /* mp4box is not compatible with video files that contain mutiple resolutions. */
                commander = "mp4box";
                if(v && a)
                {
                    args = $"-add \"{videoInput}\" -add \"{audioInput}\" -new \"{output}\"";
                }
                else if (v)
                {
                    args = $"-add \"{videoInput}\" -new \"{output}\"";
                }
                else
                {
                    output = _inputPath + ".m4a";
                    args = $"-add \"{audioInput}\" -new \"{output}\"";
                }
            }
            else
            {
                commander = "ffmpeg";
                if (v && a)
                {
                    args = $"-fflags +genpts -r {fps} -i \"{videoInput}\" -i \"{audioInput}\" -c copy -y \"{output}\"";
                }
                else if (v)
                {
                    args = $"-fflags +genpts -r {fps} -i \"{videoInput}\" -c copy -y \"{output}\"";
                }
                else
                {
                    output = _inputPath + ".m4a";
                    args = $"-i \"{audioInput}\" -c copy -y \"{output}\"";
                }
            }

            string dir = Path.GetDirectoryName(_inputPath);

            var proc = new Process
            {
                    
                StartInfo = new ProcessStartInfo
                {
                    FileName = commander,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = dir
                }
            };
            proc.Start();
            proc.WaitForExit();
        }

        public void ExtractStreams(bool extractAudio, bool extractVideo, bool extractTimeCodes, bool transCode, OverwriteDelegate overwrite) {
            uint dataOffset, flags, prevTagSize;

            _outputPathBase = Path.Combine(_outputDirectory, Path.GetFileNameWithoutExtension(_inputPath));
            _overwrite = overwrite;
            _extractAudio = extractAudio;
            _extractVideo = extractVideo;
            _extractTimeCodes = extractTimeCodes;
            _transCode = transCode;
            _videoDTS = new List<uint>();
            _videoPTS = new List<uint>();
            _audioPTS = new List<uint>();

            Seek(0);
            if (_fileLength < 4 || ReadUInt32() != 0x464C5601) {
                if (_fileLength >= 8 && ReadUInt32() == 0x66747970) {
                    throw new Exception("This is a MP4 file. YAMB or MP4Box can be used to extract streams.");
                }
                else {
                    throw new Exception("This isn't a FLV file.");
                }
            }

            if (Array.IndexOf(_outputExtensions, Path.GetExtension(_inputPath).ToLowerInvariant()) != -1) {
                // Can't have the same extension as files we output
                throw new Exception("Please change the extension of this FLV file.");
            }

            if (!Directory.Exists(_outputDirectory)) {
                throw new Exception("Output directory doesn't exist.");
            }

            flags = ReadUInt8();
            dataOffset = ReadUInt32();

            Seek(dataOffset);

            prevTagSize = ReadUInt32();
            long idx = 0;

            {
                _ = ParseHEVCMuxerType(ref _hevc_in_annexb);
                // back to offset
                Seek(dataOffset);
                prevTagSize = ReadUInt32();
            }

            while (_fileOffset < _fileLength) {
                if (!ReadTag(idx++)) break;
                if ((_fileLength - _fileOffset) < 4) break;
                prevTagSize = ReadUInt32();
            }

            _averageFrameRate = CalculateAverageFrameRate();
            _trueFrameRate = CalculateTrueFrameRate();
            _guessFrameRate = CalculateGuessFrameRate();

            string videoInput = _videoWriter?.Path;
            string audioInput = _audioWriter?.Path;

            CloseOutput(_averageFrameRate, false);

            if (_transCode)
            {
                uint fps;
                {
                    double afps = _averageFrameRate?.ToDouble() ?? 0;
                    double tfps = _trueFrameRate?.ToDouble() ?? 0;
                    double gfps = _guessFrameRate?.ToDouble() ?? 0;

                    if (tfps > afps)
                    {
                        fps = (uint)tfps;
                    }
                    else if (gfps > afps)
                    {
                        fps = (uint)gfps;
                    }
                    else if (afps > 2)
                    {
                        fps = (uint)afps;
                    }
                    else
                    {
                        fps = 24; /* default */
                    }
                }

                if (_FFmpegInternal)
                {
                    bool v = videoInput != null && File.Exists(videoInput);
                    bool a = audioInput != null && File.Exists(audioInput);
                    string output = _inputPath + ".mp4";

                    List<uint> apts = new List<uint>();
                    List<uint> vdts = new List<uint>();
                    List<uint> vpts = new List<uint>();

                    if(a && v)
                    {
                        (apts, vdts) = LiveTime.CorrectRelativeLinearData(ref _audioPTS, ref _videoDTS, fps);
                        vpts = LiveTime.CorrectJumpyData(ref _videoPTS, fps);
                        LiveTime.CorrectPTSBiggerThanDTS(ref vdts, ref vpts);
                    }
                    else if (v)
                    {
                        vdts = LiveTime.CorrectLinearData(ref _videoDTS, fps);
                        vpts = LiveTime.CorrectJumpyData(ref _videoPTS, fps);
                        LiveTime.CorrectPTSBiggerThanDTS(ref vdts, ref vpts);
                    }
                    else
                    {
                        apts = LiveTime.CorrectLinearData(ref _audioPTS, fps);
                    }
                    Remux.Start(audioInput, videoInput, output, apts, vdts, vpts);   
                }
                else
                {
                    run_remuxer(videoInput, audioInput, fps);
                }

            }
        }

        private void CloseOutput(FractionUInt32? averageFrameRate, bool disposing) {
            if (_videoWriter != null) {
                _videoWriter.Finish(averageFrameRate ?? new FractionUInt32(25, 1));
                if (disposing && (_videoWriter.Path != null)) {
                    try { File.Delete(_videoWriter.Path); }
                    catch { }
                }
                _videoWriter = null;
            }
            if (_audioWriter != null) {
                _audioWriter.Finish();
                if (disposing && (_audioWriter.Path != null)) {
                    try { File.Delete(_audioWriter.Path); }
                    catch { }
                }
                _audioWriter = null;
            }
            if (_timeCodeWriter != null) {
                _timeCodeWriter.Finish();
                if (disposing && (_timeCodeWriter.Path != null)) {
                    try { File.Delete(_timeCodeWriter.Path); }
                    catch { }
                }
                _timeCodeWriter = null;
            }
        }

        /*
         * Parse Video Tag to guess either annexb or standard mp4 format
         * return: True -> parse end
         *         False -> next Tag
         */ 
        private bool PreParseTag (ref int VideoTags, ref bool bAnnexb)
        {
            uint tagType, dataSize, timeStamp, streamID, mediaInfo, pkgType = 0, codecId = 0;
            byte[] data;
            long curTagpos = 0;
            uint tagSize = 0;

            if ((_fileLength - _fileOffset) < 11)
            {
                return false;
            }
            curTagpos = CurReadPosition();
            // Read tag header
            tagType = ReadUInt8();
            dataSize = ReadUInt24();
            timeStamp = ReadUInt24();
            timeStamp |= ReadUInt8() << 24;
            streamID = ReadUInt24();

            tagSize = dataSize + 11;

            // Read tag data
            if (dataSize == 0)
            {
                return true;
            }
            if ((_fileLength - _fileOffset) < dataSize)
            {
                return false;
            }

            mediaInfo = ReadUInt8();
            UInt32 composition = GetUInt32();
            dataSize -= 1;
            data = ReadBytes((int)dataSize);

            if ((tagType == 0x9) && ((mediaInfo >> 4) != 5))
            {
                VideoTags++;
                pkgType = (mediaInfo >> 4) & 0x0f;
                codecId = mediaInfo & 0x0f;

                if (codecId == 12)
                {
                    /* 
                     * first 4 byte = AVCPacketType + CompositionTime
                     * var AVCPacketType = data[0]; 
                     */
                    bool annexb = data[4] == 0x00 && data[5] == 0x00 && ((data[6] == 0x00 && data[7] == 0x01) || (data[6] == 0x01));

                    bAnnexb = annexb;
                }
                else
                {
                    bAnnexb = false;
                }

                return true;
            }
            return false;
        }

        private bool ParseHEVCMuxerType(ref bool bAnnexb)
        {
            int trueCnt = 0;
            uint prevTagSize;
            bool bAnnexb1 = false;
            int videoTagParseCnt = 0;
            int maxVideoTagParseCnt = 100; /* max parse tag is 100 video frames */
            while (_fileOffset < _fileLength)
            {
                if (PreParseTag(ref videoTagParseCnt, ref bAnnexb1)) {
                    if(bAnnexb1) trueCnt++;
                };

                if (videoTagParseCnt > maxVideoTagParseCnt)
                    break;

                if ((_fileLength - _fileOffset) < 4) 
                    break;

                prevTagSize = ReadUInt32();
            }

            if (videoTagParseCnt == trueCnt)
            {
                bAnnexb = true;
            }

            return true;
        }

        private bool ReadTag(long frameIdx) {
            uint tagType, dataSize, timeStamp, streamID, mediaInfo, pkgType = 0, codecId = 0;
            byte[] data;
            long curTagpos = 0;
            uint tagSize = 0;

            if ((_fileLength - _fileOffset) < 11) {
                return false;
            }
            curTagpos = CurReadPosition();
            // Read tag header
            tagType = ReadUInt8();
            dataSize = ReadUInt24();
            timeStamp = ReadUInt24();
            timeStamp |= ReadUInt8() << 24;
            streamID = ReadUInt24();

            tagSize = dataSize + 11;

            // Read tag data
            if (dataSize == 0) {
                return true;
            }
            if ((_fileLength - _fileOffset) < dataSize) {
                return false;
            }

            mediaInfo = ReadUInt8();
            UInt32 composition = GetUInt32();
            dataSize -= 1;
            data = ReadBytes((int)dataSize);

            if (_timeCodeWriter == null)
            {
                string path = _outputPathBase + ".txt";
                _timeCodeWriter = new TimeCodeWriter((_extractTimeCodes && CanWriteTo(path)) ? path : null);
                _extractedTimeCodes = _extractTimeCodes;
            }

            if (tagType == 0x8) {  // Audio
                if (_audioWriter == null) {
                    _audioWriter = _extractAudio ? GetAudioWriter(mediaInfo) : new DummyAudioWriter();
                    _extractedAudio = !(_audioWriter is DummyAudioWriter);
                }
                _audioWriter.WriteChunk(data, timeStamp);

                uint diff = 0;
                if (_audioPTS.Count > 0)
                {
                    uint lastTimeStamp = 0;
                    lastTimeStamp = _audioPTS[_audioPTS.Count - 1];
                    diff = timeStamp - lastTimeStamp;
                }
                _audioPTS.Add(timeStamp);
                codecId = (mediaInfo >> 4) & 0x0f;
                _timeCodeWriter.Write(frameIdx, curTagpos, tagType, tagSize, pkgType, codecId, timeStamp, diff, 0, 0);
            }
            else if ((tagType == 0x9) && ((mediaInfo >> 4) != 5)) { // Video
                uint tempv = composition & 0x00ffffff;
                int compositionTime = (int)((tempv & 0x00800000) << 8 | (tempv & 0x007fffff));
                uint pts = (uint)((int)timeStamp + compositionTime);

                if (_videoWriter == null) {
                    _videoWriter = _extractVideo ? GetVideoWriter(mediaInfo) : new DummyVideoWriter();
                    _extractedVideo = !(_videoWriter is DummyVideoWriter);
                }
                uint diff = 0;
                if (_videoDTS.Count > 0)
                {
                    uint lastTimeStamp = 0;
                    lastTimeStamp = _videoDTS[_videoDTS.Count - 1];
                    diff = timeStamp - lastTimeStamp;
                }
                _videoDTS.Add(timeStamp);
                _videoPTS.Add(pts);
                pkgType = (mediaInfo >> 4) & 0x0f;
                codecId = mediaInfo & 0x0f;
                _videoWriter.WriteChunk(data, timeStamp, (int)pkgType, _hevc_in_annexb);
                _timeCodeWriter.Write(frameIdx, curTagpos, tagType, tagSize, pkgType, codecId, timeStamp, diff, (int)pts, compositionTime);
            }

            return true;
        }

        private IAudioWriter GetAudioWriter(uint mediaInfo) {
            uint format = mediaInfo >> 4;
            uint rate = (mediaInfo >> 2) & 0x3;
            uint bits = (mediaInfo >> 1) & 0x1;
            uint chans = mediaInfo & 0x1;
            string path;

            if ((format == 2) || (format == 14)) { // MP3
                path = _outputPathBase + ".mp3";
                if (!CanWriteTo(path)) return new DummyAudioWriter();
                return new MP3Writer(path, _warnings);
            }
            else if ((format == 0) || (format == 3)) { // PCM
                int sampleRate = 0;
                switch (rate) {
                    case 0: sampleRate =  5512; break;
                    case 1: sampleRate = 11025; break;
                    case 2: sampleRate = 22050; break;
                    case 3: sampleRate = 44100; break;
                }
                path = _outputPathBase + ".wav";
                if (!CanWriteTo(path)) return new DummyAudioWriter();
                if (format == 0) {
                    _warnings.Add("PCM byte order unspecified, assuming little endian.");
                }
                return new WAVWriter(path, (bits == 1) ? 16 : 8,
                    (chans == 1) ? 2 : 1, sampleRate);
            }
            else if (format == 10) { // AAC
                path = _outputPathBase + ".aac";
                if (!CanWriteTo(path)) return new DummyAudioWriter();
                return new AACWriter(path);
            }
            else if (format == 11) { // Speex
                path = _outputPathBase + ".spx";
                if (!CanWriteTo(path)) return new DummyAudioWriter();
                return new SpeexWriter(path, (int)(_fileLength & 0xFFFFFFFF));
            }
            else {
                string typeStr;

                if (format == 1)
                    typeStr = "ADPCM";
                else if ((format == 4) || (format == 5) || (format == 6))
                    typeStr = "Nellymoser";
                else
                    typeStr = "format=" + format.ToString();

                _warnings.Add("Unable to extract audio (" + typeStr + " is unsupported).");

                return new DummyAudioWriter();
            }
        }

        private IVideoWriter GetVideoWriter(uint mediaInfo) {
            uint codecID = mediaInfo & 0x0F;
            string path;

            if ((codecID == 2) || (codecID == 4) || (codecID == 5)) {
                path = _outputPathBase + ".avi";
                if (!CanWriteTo(path)) return new DummyVideoWriter();
                return new AVIWriter(path, (int)codecID, _warnings);
            }
            else if (codecID == 7) {
                path = _outputPathBase + ".h264";
                if (!CanWriteTo(path)) return new DummyVideoWriter();
                return new RawH264Writer(path);
            }
            else if (codecID == 12)
            {
                path = _outputPathBase + ".h265";
                if (!CanWriteTo(path)) return new DummyVideoWriter();
                return new RawH265Writer(path);
            }
            else {
                string typeStr;

                if (codecID == 3)
                    typeStr = "Screen";
                else if (codecID == 6)
                    typeStr = "Screen2";
                else
                    typeStr = "codecID=" + codecID.ToString();

                _warnings.Add("Unable to extract video (" + typeStr + " is unsupported).");

                return new DummyVideoWriter();
            }
        }

        private bool CanWriteTo(string path) {
            if (File.Exists(path) && (_overwrite != null)) {
                return _overwrite(path);
            }
            return true;
        }

        private FractionUInt32? CalculateAverageFrameRate() {
            FractionUInt32 frameRate;
            int frameCount = _videoDTS.Count;

            if (frameCount > 1) {
                frameRate.N = (uint)(frameCount - 1) * 1000;
                frameRate.D = (uint)(_videoDTS[frameCount - 1] - _videoDTS[0]);
                frameRate.Reduce();
                return frameRate;
            }
            else {
                return null;
            }
        }

        private FractionUInt32? CalculateTrueFrameRate() {
            FractionUInt32 frameRate;
            Dictionary<uint, uint> deltaCount = new Dictionary<uint, uint>();
            int i, threshold;
            uint delta, count, minDelta;

            // Calculate the distance between the timestamps, count how many times each delta appears
            for (i = 1; i < _videoDTS.Count; i++) {
                int deltaS = (int)((long)_videoDTS[i] - (long)_videoDTS[i - 1]);

                if (deltaS <= 0) continue;
                delta = (uint)deltaS;

                if (deltaCount.ContainsKey(delta)) {
                    deltaCount[delta] += 1;
                }
                else {
                    deltaCount.Add(delta, 1);
                }
            }

            threshold = _videoDTS.Count / 10;
            minDelta = UInt32.MaxValue;

            // Find the smallest delta that made up at least 10% of the frames (grouping in delta+1
            // because of rounding, e.g. a NTSC video will have deltas of 33 and 34 ms)
            foreach (KeyValuePair<uint, uint> deltaItem in deltaCount) {
                delta = deltaItem.Key;
                count = deltaItem.Value;

                if (deltaCount.ContainsKey(delta + 1)) {
                    count += deltaCount[delta + 1];
                }

                if ((count >= threshold) && (delta < minDelta)) {
                    minDelta = delta;
                }
            }

            // Calculate the frame rate based on the smallest delta, and delta+1 if present
            if (minDelta != UInt32.MaxValue) {
                uint totalTime, totalFrames;

                count = deltaCount[minDelta];
                totalTime = minDelta * count;
                totalFrames = count;

                if (deltaCount.ContainsKey(minDelta + 1)) {
                    count = deltaCount[minDelta + 1];
                    totalTime += (minDelta + 1) * count;
                    totalFrames += count;
                }

                if (totalTime != 0) {
                    frameRate.N = totalFrames * 1000;
                    frameRate.D = totalTime;
                    frameRate.Reduce();
                    return frameRate;
                }
            }

            // Unable to calculate frame rate
            return null;
        }

        private FractionUInt32? CalculateGuessFrameRate()
        {
            FractionUInt32 frameRate;
            
            uint totalTime = 0, totalFrames = 0;

            var frames = OutlierDetection.RemoveOutliers(_videoDTS);
            if (frames != null)
            {
                totalTime = (uint)(frames[frames.Count - 1] - frames[0]);
                totalFrames = (uint)frames.Count;
            }

            if (totalTime != 0)
            {
                frameRate.N = totalFrames * 1000;
                frameRate.D = totalTime;
                frameRate.Reduce();
                return frameRate;
            }

            // Unable to calculate frame rate
            return null;
        }

        private void Seek(long offset) {
            _fs.Seek(offset, SeekOrigin.Begin);
            _fileOffset = offset;
        }

        private uint ReadUInt8() {
            _fileOffset += 1;
            return (uint)_fs.ReadByte();
        }

        private long CurReadPosition()
        {
            return _fs.Position;
        }

        private uint ReadUInt24() {
            byte[] x = new byte[4];
            _fs.Read(x, 1, 3);
            _fileOffset += 3;
            return BitConverterBE.ToUInt32(x, 0);
        }

        private uint ReadUInt32() {
            byte[] x = new byte[4];
            _fs.Read(x, 0, 4);
            _fileOffset += 4;
            return BitConverterBE.ToUInt32(x, 0);
        }

        private byte[] ReadBytes(int length) {
            byte[] buff = new byte[length];
            _fs.Read(buff, 0, length);
            _fileOffset += length;
            return buff;
        }

        private UInt32 GetUInt32()
        {
            byte[] x = new byte[4];
            _fs.Read(x, 0, 4);
            _fs.Seek(-4, SeekOrigin.Current);
            return BitConverterBE.ToUInt32(x, 0);
        }
    }

    internal class DummyAudioWriter : IAudioWriter {
        public DummyAudioWriter() {
        }

        public void WriteChunk(byte[] chunk, uint timeStamp) {
        }

        public void Finish() {
        }

        public string Path {
            get {
                return null;
            }
        }
    }

    internal class DummyVideoWriter : IVideoWriter {
        public DummyVideoWriter() {
        }

        public void WriteChunk(byte[] chunk, uint timeStamp, int frameType, bool hevc_in_annexb = false) {
        }

        public void Finish(FractionUInt32 averageFrameRate) {
        }

        public string Path {
            get {
                return null;
            }
        }
    }

    internal class MP3Writer : IAudioWriter {
        private string _path;
        private FileStream _fs;
        private List<string> _warnings;
        private List<byte[]> _chunkBuffer;
        private List<uint> _frameOffsets;
        private uint _totalFrameLength;
        private bool _isVBR;
        private bool _delayWrite;
        private bool _hasVBRHeader;
        private bool _writeVBRHeader;
        private int _firstBitRate;
        private int _mpegVersion;
        private int _sampleRate;
        private int _channelMode;
        private uint _firstFrameHeader;

        public MP3Writer(string path, List<string> warnings) {
            _path = path;
            _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 65536);
            _warnings = warnings;
            _chunkBuffer = new List<byte[]>();
            _frameOffsets = new List<uint>();
            _delayWrite = true;
        }

        public void WriteChunk(byte[] chunk, uint timeStamp) {
            _chunkBuffer.Add(chunk);
            ParseMP3Frames(chunk);
            if (_delayWrite && _totalFrameLength >= 65536) {
                _delayWrite = false;
            }
            if (!_delayWrite) {
                Flush();
            }
        }

        public void Finish() {
            Flush();
            if (_writeVBRHeader) {
                _fs.Seek(0, SeekOrigin.Begin);
                WriteVBRHeader(false);
            }
            _fs.Close();
        }

        public string Path {
            get {
                return _path;
            }
        }

        private void Flush() {
            foreach (byte[] chunk in _chunkBuffer) {
                _fs.Write(chunk, 0, chunk.Length);
            }
            _chunkBuffer.Clear();
        }

        private void ParseMP3Frames(byte[] buff) {
            int[] MPEG1BitRate = new int[] { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0 };
            int[] MPEG2XBitRate = new int[] { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0 };
            int[] MPEG1SampleRate = new int[] { 44100, 48000, 32000, 0 };
            int[] MPEG20SampleRate = new int[] { 22050, 24000, 16000, 0 };
            int[] MPEG25SampleRate = new int[] { 11025, 12000, 8000, 0 };

            int offset = 0;
            int length = buff.Length;

            while (length >= 4) {
                ulong header;
                int mpegVersion, layer, bitRate, sampleRate, padding, channelMode;
                int frameLen;

                header = (ulong)BitConverterBE.ToUInt32(buff, offset) << 32;
                if (BitHelper.Read(ref header, 11) != 0x7FF) {
                    break;
                }
                mpegVersion = BitHelper.Read(ref header, 2);
                layer = BitHelper.Read(ref header, 2);
                BitHelper.Read(ref header, 1);
                bitRate = BitHelper.Read(ref header, 4);
                sampleRate = BitHelper.Read(ref header, 2);
                padding = BitHelper.Read(ref header, 1);
                BitHelper.Read(ref header, 1);
                channelMode = BitHelper.Read(ref header, 2);

                if ((mpegVersion == 1) || (layer != 1) || (bitRate == 0) || (bitRate == 15) || (sampleRate == 3)) {
                    break;
                }

                bitRate = ((mpegVersion == 3) ? MPEG1BitRate[bitRate] : MPEG2XBitRate[bitRate]) * 1000;

                if (mpegVersion == 3)
                    sampleRate = MPEG1SampleRate[sampleRate];
                else if (mpegVersion == 2)
                    sampleRate = MPEG20SampleRate[sampleRate];
                else
                    sampleRate = MPEG25SampleRate[sampleRate];

                frameLen = GetFrameLength(mpegVersion, bitRate, sampleRate, padding);
                if (frameLen > length) {
                    break;
                }

                bool isVBRHeaderFrame = false;
                if (_frameOffsets.Count == 0) {
                    // Check for an existing VBR header just to be safe (I haven't seen any in FLVs)
                    int o = offset + GetFrameDataOffset(mpegVersion, channelMode);
                    if (BitConverterBE.ToUInt32(buff, o) == 0x58696E67) { // "Xing"
                        isVBRHeaderFrame = true;
                        _delayWrite = false;
                        _hasVBRHeader = true;
                    }
                }

                if (isVBRHeaderFrame) { }
                else if (_firstBitRate == 0) {
                    _firstBitRate = bitRate;
                    _mpegVersion = mpegVersion;
                    _sampleRate = sampleRate;
                    _channelMode = channelMode;
                    _firstFrameHeader = BitConverterBE.ToUInt32(buff, offset);
                }
                else if (!_isVBR && (bitRate != _firstBitRate)) {
                    _isVBR = true;
                    if (_hasVBRHeader) { }
                    else if (_delayWrite) {
                        WriteVBRHeader(true);
                        _writeVBRHeader = true;
                        _delayWrite = false;
                    }
                    else {
                        _warnings.Add("Detected VBR too late, cannot add VBR header.");
                    }
                }

                _frameOffsets.Add(_totalFrameLength + (uint)offset);

                offset += frameLen;
                length -= frameLen;
            }

            _totalFrameLength += (uint)buff.Length;
        }

        private void WriteVBRHeader(bool isPlaceholder) {
            byte[] buff = new byte[GetFrameLength(_mpegVersion, 64000, _sampleRate, 0)];
            if (!isPlaceholder) {
                uint header = _firstFrameHeader;
                int dataOffset = GetFrameDataOffset(_mpegVersion, _channelMode);
                header &= 0xFFFF0DFF; // Clear bitrate and padding fields
                header |= 0x00010000; // Set protection bit (indicates that CRC is NOT present)
                header |= (uint)((_mpegVersion == 3) ? 5 : 8) << 12; // 64 kbit/sec
                General.CopyBytes(buff, 0, BitConverterBE.GetBytes(header));
                General.CopyBytes(buff, dataOffset, BitConverterBE.GetBytes((uint)0x58696E67)); // "Xing"
                General.CopyBytes(buff, dataOffset + 4, BitConverterBE.GetBytes((uint)0x7)); // Flags
                General.CopyBytes(buff, dataOffset + 8, BitConverterBE.GetBytes((uint)_frameOffsets.Count)); // Frame count
                General.CopyBytes(buff, dataOffset + 12, BitConverterBE.GetBytes((uint)_totalFrameLength)); // File length
                for (int i = 0; i < 100; i++) {
                    int frameIndex = (int)((i / 100.0) * _frameOffsets.Count);
                    buff[dataOffset + 16 + i] = (byte)((_frameOffsets[frameIndex] / (double)_totalFrameLength) * 256.0);
                }
            }
            _fs.Write(buff, 0, buff.Length);
        }

        private int GetFrameLength(int mpegVersion, int bitRate, int sampleRate, int padding) {
            return ((mpegVersion == 3) ? 144 : 72) * bitRate / sampleRate + padding;
        }

        private int GetFrameDataOffset(int mpegVersion, int channelMode) {
            return 4 + ((mpegVersion == 3) ?
                ((channelMode == 3) ? 17 : 32) :
                ((channelMode == 3) ?  9 : 17));
        }
    }

    internal class SpeexWriter : IAudioWriter {
        private const string _vendorString = "FLV Extract";
        private const uint _sampleRate = 16000;
        private const uint _msPerFrame = 20;
        private const uint _samplesPerFrame = _sampleRate / (1000 / _msPerFrame);
        private const int _targetPageDataSize = 4096;

        private string _path;
        private FileStream _fs;
        private int _serialNumber;
        private List<OggPacket> _packetList;
        private int _packetListDataSize;
        private byte[] _pageBuff;
        private int _pageBuffOffset;
        private uint _pageSequenceNumber;
        private ulong _granulePosition;

        public SpeexWriter(string path, int serialNumber) {
            _path = path;
            _serialNumber = serialNumber;
            _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 65536);
            _fs.Seek((28 + 80) + (28 + 8 + _vendorString.Length), SeekOrigin.Begin); // Speex header + Vorbis comment
            _packetList = new List<OggPacket>();
            _packetListDataSize = 0;
            _pageBuff = new byte[27 + 255 + _targetPageDataSize + 254]; // Header + max segment table + target data size + extra segment
            _pageBuffOffset = 0;
            _pageSequenceNumber = 2; // First audio packet
            _granulePosition = 0;
        }

        public void WriteChunk(byte[] chunk, uint timeStamp) {
            int[] subModeSizes = new int[] { 0, 43, 119, 160, 220, 300, 364, 492, 79 };
            int[] wideBandSizes = new int[] { 0, 36, 112, 192, 352 };
            int[] inBandSignalSizes = new int[] { 1, 1, 4, 4, 4, 4, 4, 4, 8, 8, 16, 16, 32, 32, 64, 64 };
            int frameStart = -1;
            int frameEnd = 0;
            int offset = 0;
            int length = chunk.Length * 8;
            int x;

            while (length - offset >= 5) {
                x = BitHelper.Read(chunk, ref offset, 1);
                if (x != 0) {
                    // wideband frame
                    x = BitHelper.Read(chunk, ref offset, 3);
                    if (x < 1 || x > 4) goto Error;
                    offset += wideBandSizes[x] - 4;
                }
                else {
                    x = BitHelper.Read(chunk, ref offset, 4);
                    if (x >= 1 && x <= 8) {
                        // narrowband frame
                        if (frameStart != -1) {
                            WriteFramePacket(chunk, frameStart, frameEnd);
                        }
                        frameStart = frameEnd;
                        offset += subModeSizes[x] - 5;
                    }
                    else if (x == 15) {
                        // terminator
                        break;
                    }
                    else if (x == 14) {
                        // in-band signal
                        if (length - offset < 4) goto Error;
                        x = BitHelper.Read(chunk, ref offset, 4);
                        offset += inBandSignalSizes[x];
                    }
                    else if (x == 13) {
                        // custom in-band signal
                        if (length - offset < 5) goto Error;
                        x = BitHelper.Read(chunk, ref offset, 5);
                        offset += x * 8;
                    }
                    else goto Error;
                }
                frameEnd = offset;
            }
            if (offset > length) goto Error;

            if (frameStart != -1) {
                WriteFramePacket(chunk, frameStart, frameEnd);
            }

            return;

        Error:
            throw new Exception("Invalid Speex data.");
        }

        public void Finish() {
            WritePage();
            FlushPage(true);
            _fs.Seek(0, SeekOrigin.Begin);
            _pageSequenceNumber = 0;
            _granulePosition = 0;
            WriteSpeexHeaderPacket();
            WriteVorbisCommentPacket();
            FlushPage(false);
            _fs.Close();
        }

        public string Path {
            get {
                return _path;
            }
        }

        private void WriteFramePacket(byte[] data, int startBit, int endBit) {
            int lengthBits = endBit - startBit;
            byte[] frame = BitHelper.CopyBlock(data, startBit, lengthBits);
            if (lengthBits % 8 != 0) {
                frame[frame.Length - 1] |= (byte)(0xFF >> ((lengthBits % 8) + 1)); // padding
            }
            AddPacket(frame, _samplesPerFrame, true);
        }

        private void WriteSpeexHeaderPacket() {
            byte[] data = new byte[80];
            General.CopyBytes(data, 0, Encoding.ASCII.GetBytes("Speex   ")); // speex_string
            General.CopyBytes(data, 8, Encoding.ASCII.GetBytes("unknown")); // speex_version
            data[28] = 1; // speex_version_id
            data[32] = 80; // header_size
            General.CopyBytes(data, 36, BitConverterLE.GetBytes((uint)_sampleRate)); // rate
            data[40] = 1; // mode (e.g. narrowband, wideband)
            data[44] = 4; // mode_bitstream_version
            data[48] = 1; // nb_channels
            General.CopyBytes(data, 52, BitConverterLE.GetBytes(unchecked((uint)-1))); // bitrate
            General.CopyBytes(data, 56, BitConverterLE.GetBytes((uint)_samplesPerFrame)); // frame_size
            data[60] = 0; // vbr
            data[64] = 1; // frames_per_packet
            AddPacket(data, 0, false);
        }

        private void WriteVorbisCommentPacket() {
            byte[] vendorStringBytes = Encoding.ASCII.GetBytes(_vendorString);
            byte[] data = new byte[8 + vendorStringBytes.Length];
            data[0] = (byte)vendorStringBytes.Length;
            General.CopyBytes(data, 4, vendorStringBytes);
            AddPacket(data, 0, false);
        }

        private void AddPacket(byte[] data, uint sampleLength, bool delayWrite) {
            OggPacket packet = new OggPacket();
            if (data.Length >= 255) {
                throw new Exception("Packet exceeds maximum size.");
            }
            _granulePosition += sampleLength;
            packet.Data = data;
            packet.GranulePosition = _granulePosition;
            _packetList.Add(packet);
            _packetListDataSize += data.Length;
            if (!delayWrite || (_packetListDataSize >= _targetPageDataSize) || (_packetList.Count == 255)) {
                WritePage();
            }
        }

        private void WritePage() {
            if (_packetList.Count == 0) return;
            FlushPage(false);
            WriteToPage(BitConverterBE.GetBytes(0x4F676753U), 0, 4); // "OggS"
            WriteToPage((byte)0); // Stream structure version
            WriteToPage((byte)((_pageSequenceNumber == 0) ? 0x02 : 0)); // Page flags
            WriteToPage((ulong)_packetList[_packetList.Count - 1].GranulePosition); // Position in samples
            WriteToPage((uint)_serialNumber); // Stream serial number
            WriteToPage((uint)_pageSequenceNumber); // Page sequence number
            WriteToPage((uint)0); // Checksum
            WriteToPage((byte)_packetList.Count); // Page segment count
            foreach (OggPacket packet in _packetList) {
                WriteToPage((byte)packet.Data.Length);
            }
            foreach (OggPacket packet in _packetList) {
                WriteToPage(packet.Data, 0, packet.Data.Length);
            }
            _packetList.Clear();
            _packetListDataSize = 0;
            _pageSequenceNumber++;
        }

        private void FlushPage(bool isLastPage) {
            if (_pageBuffOffset == 0) return;
            if (isLastPage) _pageBuff[5] |= 0x04;
            uint crc = OggCRC.Calculate(_pageBuff, 0, _pageBuffOffset);
            General.CopyBytes(_pageBuff, 22, BitConverterLE.GetBytes(crc));
            _fs.Write(_pageBuff, 0, _pageBuffOffset);
            _pageBuffOffset = 0;
        }

        private void WriteToPage(byte[] data, int offset, int length) {
            Buffer.BlockCopy(data, offset, _pageBuff, _pageBuffOffset, length);
            _pageBuffOffset += length;
        }

        private void WriteToPage(byte data) {
            WriteToPage(new byte[] { data }, 0, 1);
        }

        private void WriteToPage(uint data) {
            WriteToPage(BitConverterLE.GetBytes(data), 0, 4);
        }

        private void WriteToPage(ulong data) {
            WriteToPage(BitConverterLE.GetBytes(data), 0, 8);
        }

        private class OggPacket {
            public ulong GranulePosition;
            public byte[] Data;
        }
    }

    internal class AACWriter : IAudioWriter {
        private string _path;
        private FileStream _fs;
        private int _aacProfile;
        private int _sampleRateIndex;
        private int _channelConfig;

        public AACWriter(string path) {
            _path = path;
            _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 65536);
        }

        public void WriteChunk(byte[] chunk, uint timeStamp) {
            if (chunk.Length < 1) return;

            if (chunk[0] == 0) { // Header
                if (chunk.Length < 3) return;

                ulong bits = (ulong)BitConverterBE.ToUInt16(chunk, 1) << 48;

                _aacProfile = BitHelper.Read(ref bits, 5) - 1;
                _sampleRateIndex = BitHelper.Read(ref bits, 4);
                _channelConfig = BitHelper.Read(ref bits, 4);

                if (_aacProfile == 4) // HE-AAC
                    _aacProfile = 1; // Uses LC profile + SBR

                if ((_aacProfile < 0) || (_aacProfile > 3))
                    throw new Exception("Unsupported AAC profile.");
                if (_sampleRateIndex > 12)
                    throw new Exception("Invalid AAC sample rate index.");
                if (_channelConfig > 6)
                    throw new Exception("Invalid AAC channel configuration.");
            }
            else { // Audio data
                int dataSize = chunk.Length - 1;
                ulong bits = 0;

                // Reference: WriteADTSHeader from FAAC's bitstream.c

                BitHelper.Write(ref bits, 12, 0xFFF);
                BitHelper.Write(ref bits,  1, 0);
                BitHelper.Write(ref bits,  2, 0);
                BitHelper.Write(ref bits,  1, 1);
                BitHelper.Write(ref bits,  2, _aacProfile);
                BitHelper.Write(ref bits,  4, _sampleRateIndex);
                BitHelper.Write(ref bits,  1, 0);
                BitHelper.Write(ref bits,  3, _channelConfig);
                BitHelper.Write(ref bits,  1, 0);
                BitHelper.Write(ref bits,  1, 0);
                BitHelper.Write(ref bits,  1, 0);
                BitHelper.Write(ref bits,  1, 0);
                BitHelper.Write(ref bits, 13, 7 + dataSize);
                BitHelper.Write(ref bits, 11, 0x7FF);
                BitHelper.Write(ref bits,  2, 0);

                _fs.Write(BitConverterBE.GetBytes(bits), 1, 7);
                _fs.Write(chunk, 1, dataSize);
            }
        }

        public void Finish() {
            _fs.Close();
        }

        public string Path {
            get {
                return _path;
            }
        }
    }

    static class ByteArrayRocks {

        static readonly int [] Empty = new int [0];

        public static int Locate (byte [] self, byte [] candidate, int startPos)
        {
            int ret = 0;
            bool found = false;
            if (IsEmptyLocate (self, candidate))
                return -1;

            for (int i = startPos; i < self.Length; i++) {
                if (!IsMatch (self, i, candidate))
                {
                    continue;
                }
                else
                {
                    found = true;
                    ret = i;
                    break;
                }
            }

            if (found)
                return ret;
            else
                return -1;
        }

        static bool IsMatch (byte [] array, int position, byte [] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array [position + i] != candidate [i])
                    return false;

            return true;
        }

        static bool IsEmptyLocate (byte [] array, byte [] candidate)
        {
            return array == null
                || candidate == null
                || array.Length == 0
                || candidate.Length == 0
                || candidate.Length > array.Length;
        }
    }

    internal class RawH265Writer : IVideoWriter
    {
        private static readonly byte[] _startCode = new byte[] { 0, 0, 0, 1 };

        private string _path;
        private FileStream _fs;
        private int _nalLengthSize = 4;

        enum HEVCNalType : int
        {
            TRAIL_N = 0,
            TRAIL_R = 1,
            TSA_N = 2,
            TSA_R = 3,
            STSA_N = 4,
            STSA_R = 5,
            RADL_N = 6,
            RADL_R = 7,
            RASL_N = 8,
            RASL_R = 9,
            VCL_N10 = 10,
            VCL_R11 = 11,
            VCL_N12 = 12,
            VCL_R13 = 13,
            VCL_N14 = 14,
            VCL_R15 = 15,
            BLA_W_LP = 16,
            BLA_W_RADL = 17,
            BLA_N_LP = 18,
            IDR_W_RADL = 19,
            IDR_N_LP = 20,
            CRA_NUT = 21,
            RSV_IRAP_VCL22 = 22,
            RSV_IRAP_VCL23 = 23,
            RSV_VCL24 = 24,
            RSV_VCL25 = 25,
            RSV_VCL26 = 26,
            RSV_VCL27 = 27,
            RSV_VCL28 = 28,
            RSV_VCL29 = 29,
            RSV_VCL30 = 30,
            RSV_VCL31 = 31,
            VPS = 32,
            SPS = 33,
            PPS = 34,
            AUD = 35,
            EOS_NUT = 36,
            EOB_NUT = 37,
            FD_NUT = 38,
            SEI_PREFIX = 39,
            SEI_SUFFIX = 40,
            RSV_NVCL41 = 41,
            RSV_NVCL42 = 42,
            RSV_NVCL43 = 43,
            RSV_NVCL44 = 44,
            RSV_NVCL45 = 45,
            RSV_NVCL46 = 46,
            RSV_NVCL47 = 47,
            UNSPEC48 = 48,
            UNSPEC49 = 49,
            UNSPEC50 = 50,
            UNSPEC51 = 51,
            UNSPEC52 = 52,
            UNSPEC53 = 53,
            UNSPEC54 = 54,
            UNSPEC55 = 55,
            UNSPEC56 = 56,
            UNSPEC57 = 57,
            UNSPEC58 = 58,
            UNSPEC59 = 59,
            UNSPEC60 = 60,
            UNSPEC61 = 61,
            UNSPEC62 = 62,
            UNSPEC63 = 63,
        };
        enum HVCCPayloadOffset : int
        {
            lengthSizeMinusOne = 21,
            numOfArrays = 22,
        };

        public RawH265Writer(string path)
        {
            _path = path;
            _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 1*1024*1024);
        }

        public void WriteChunk(byte[] chunk, uint timeStamp, int frameType, bool hevc_in_annexb)
        {
            if (chunk.Length < 8) return;

            /* first 4 byte = AVCPacketType + CompositionTime */
            var AVCPacketType = chunk[0];

            if (hevc_in_annexb) /* annexb format */
            {
                int offset = 4;
                int len = chunk.Length - offset;
                _fs.Write(chunk, offset, len);
            }
            else /* standard mp4 format */
            {
                if (AVCPacketType == 0) /* single HVCCDecoderConfigurationRecord */
                {
                    if (chunk.Length < 10) return;

                    int offset, vpsCount = 0, spsCount = 0, ppsCount = 0, nalArrays;

                    offset = (int)HVCCPayloadOffset.lengthSizeMinusOne + 4;
                    _nalLengthSize = (chunk[offset++] & 0x03) + 1;

                    offset = (int)HVCCPayloadOffset.numOfArrays + 4;
                    nalArrays = chunk[offset++];

                    for (int i = 0; i < nalArrays; i++)
                    {
                        int nalType = chunk[offset++] & 0x3f;
                        int numNalus = (int)BitConverterBE.ToUInt16(chunk, offset);
                        offset += 2;

                        if (nalType == (int)HEVCNalType.VPS)
                            vpsCount++;
                        if (nalType == (int)HEVCNalType.SPS)
                            spsCount++;
                        if (nalType == (int)HEVCNalType.PPS)
                            ppsCount++;

                        for (int j = 0; j < numNalus; j++)
                        {
                            int len = (int)BitConverterBE.ToUInt16(chunk, offset);
                            offset += 2;
                            _fs.Write(_startCode, 0, _startCode.Length);
                            _fs.Write(chunk, offset, len);
                            offset += len;
                        }
                    }
                }
                else
                {
                    int offset = 4;

                    while (offset <= chunk.Length - _nalLengthSize)
                    {
                        int len = (_nalLengthSize == 2) ?
                            (int)BitConverterBE.ToUInt16(chunk, offset) :
                            (int)BitConverterBE.ToUInt32(chunk, offset);
                        offset += _nalLengthSize;
                        if (offset + len > chunk.Length) break;
                        _fs.Write(_startCode, 0, _startCode.Length);
                        _fs.Write(chunk, offset, len);
                        offset += len;
                    }
                }
            }
        }

        public void Finish(FractionUInt32 averageFrameRate)
        {
            _fs.Close();
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }
    }

    internal class RawH264Writer : IVideoWriter {
        private static readonly byte[] _startCode = new byte[] { 0, 0, 0, 1 };

        private string _path;
        private FileStream _fs;
        private int _nalLengthSize;

        public RawH264Writer(string path) {
            _path = path;
            _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 65536);
        }

        public void WriteChunk(byte[] chunk, uint timeStamp, int frameType, bool hevc_in_annexb = false) {
            if (chunk.Length < 4) return;

            // Reference: decode_frame from libavcodec's h264.c

            if (chunk[0] == 0)
            { // Headers AVCDecoderConfigurationRecord
                if (chunk.Length < 10) return;

                int offset, spsCount, ppsCount;

                offset = 8;
                _nalLengthSize = (chunk[offset++] & 0x03) + 1;
                spsCount = chunk[offset++] & 0x1F;
                ppsCount = -1;

                while (offset <= chunk.Length - 2) {
                    if ((spsCount == 0) && (ppsCount == -1)) {
                        ppsCount = chunk[offset++];
                        continue;
                    }

                    if (spsCount > 0) spsCount--;
                    else if (ppsCount > 0) ppsCount--;
                    else break;

                    int len = (int)BitConverterBE.ToUInt16(chunk, offset);
                    offset += 2;
                    if (offset + len > chunk.Length) break;
                    _fs.Write(_startCode, 0, _startCode.Length);
                    _fs.Write(chunk, offset, len);
                    offset += len;
                }
            }
            else { // Video data
                int offset = 4;

                if (_nalLengthSize != 2) {
                    _nalLengthSize = 4;
                }

                while (offset <= chunk.Length - _nalLengthSize) {
                    int len = (_nalLengthSize == 2) ?
                        (int)BitConverterBE.ToUInt16(chunk, offset) :
                        (int)BitConverterBE.ToUInt32(chunk, offset);
                    offset += _nalLengthSize;
                    if (offset + len > chunk.Length) break;
                    _fs.Write(_startCode, 0, _startCode.Length);
                    _fs.Write(chunk, offset, len);
                    offset += len;
                }
            }
        }

        public void Finish(FractionUInt32 averageFrameRate) {
            _fs.Close();
        }

        public string Path {
            get {
                return _path;
            }
        }
    }

    internal class WAVWriter : IAudioWriter {
        private string _path;
        private WAVTools.WAVWriter _wr;
        private int blockAlign;

        public WAVWriter(string path, int bitsPerSample, int channelCount, int sampleRate) {
            _path = path;
            _wr = new WAVTools.WAVWriter(path, bitsPerSample, channelCount, sampleRate);
            blockAlign = (bitsPerSample / 8) * channelCount;
        }

        public void WriteChunk(byte[] chunk, uint timeStamp) {
            _wr.Write(chunk, chunk.Length / blockAlign);
        }

        public void Finish() {
            _wr.Close();
        }

        public string Path {
            get {
                return _path;
            }
        }
    }

    internal class AVIWriter : IVideoWriter {
        private string _path;
        private BinaryWriter _bw;
        private int _codecID;
        private int _width;
        private int _height;
        private int _frameCount;
        private uint _moviDataSize;
        private uint _indexChunkSize;
        private List<uint> _index;
        private bool _isAlphaWriter;
        private AVIWriter _alphaWriter;
        private List<string> _warnings;

        // Chunk:          Off:  Len:
        //
        // RIFF AVI          0    12
        //   LIST hdrl      12    12
        //     avih         24    64
        //     LIST strl    88    12
        //       strh      100    64
        //       strf      164    48
        //   LIST movi     212    12
        //     (frames)    224   ???
        //   idx1          ???   ???

        public AVIWriter(string path, int codecID, List<string> warnings) :
            this(path, codecID, warnings, false) { }

        private AVIWriter(string path, int codecID, List<string> warnings, bool isAlphaWriter) {
            if ((codecID != 2) && (codecID != 4) && (codecID != 5)) {
                throw new Exception("Unsupported video codec.");
            }

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);

            _path = path;
            _bw = new BinaryWriter(fs);
            _codecID = codecID;
            _warnings = warnings;
            _isAlphaWriter = isAlphaWriter;

            if ((codecID == 5) && !_isAlphaWriter) {
                _alphaWriter = new AVIWriter(path.Substring(0, path.Length - 4) + ".alpha.avi", codecID, warnings, true);
            }

            WriteFourCC("RIFF");
            _bw.Write((uint)0); // chunk size
            WriteFourCC("AVI ");

            WriteFourCC("LIST");
            _bw.Write((uint)192);
            WriteFourCC("hdrl");

            WriteFourCC("avih");
            _bw.Write((uint)56);
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0x10);
            _bw.Write((uint)0); // frame count
            _bw.Write((uint)0);
            _bw.Write((uint)1);
            _bw.Write((uint)0);
            _bw.Write((uint)0); // width
            _bw.Write((uint)0); // height
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0);

            WriteFourCC("LIST");
            _bw.Write((uint)116);
            WriteFourCC("strl");

            WriteFourCC("strh");
            _bw.Write((uint)56);
            WriteFourCC("vids");
            WriteFourCC(CodecFourCC);
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0); // frame rate denominator
            _bw.Write((uint)0); // frame rate numerator
            _bw.Write((uint)0);
            _bw.Write((uint)0); // frame count
            _bw.Write((uint)0);
            _bw.Write((int)-1);
            _bw.Write((uint)0);
            _bw.Write((ushort)0);
            _bw.Write((ushort)0);
            _bw.Write((ushort)0); // width
            _bw.Write((ushort)0); // height

            WriteFourCC("strf");
            _bw.Write((uint)40);
            _bw.Write((uint)40);
            _bw.Write((uint)0); // width
            _bw.Write((uint)0); // height
            _bw.Write((ushort)1);
            _bw.Write((ushort)24);
            WriteFourCC(CodecFourCC);
            _bw.Write((uint)0); // biSizeImage
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0);
            _bw.Write((uint)0);

            WriteFourCC("LIST");
            _bw.Write((uint)0); // chunk size
            WriteFourCC("movi");

            _index = new List<uint>();
        }

        public void WriteChunk(byte[] chunk, uint timeStamp, int frameType, bool hevc_in_annexb = false) {
            int offset, len;

            offset = 0;
            len = chunk.Length;
            if (_codecID == 4) {
                offset = 1;
                len -= 1;
            }
            if (_codecID == 5) {
                offset = 4;
                if (len >= 4) {
                    int alphaOffset = (int)BitConverterBE.ToUInt32(chunk, 0) & 0xFFFFFF;
                    if (!_isAlphaWriter) {
                        len = alphaOffset;
                    }
                    else {
                        offset += alphaOffset;
                        len -= offset;
                    }
                }
                else {
                    len = 0;
                }
            }
            len = Math.Max(len, 0);
            len = Math.Min(len, chunk.Length - offset);

            _index.Add((frameType == 1) ? (uint)0x10 : (uint)0);
            _index.Add(_moviDataSize + 4);
            _index.Add((uint)len);

            if ((_width == 0) && (_height == 0)) {
                GetFrameSize(chunk);
            }

            WriteFourCC("00dc");
            _bw.Write(len);
            _bw.Write(chunk, offset, len);

            if ((len % 2) != 0) {
                _bw.Write((byte)0);
                len++;
            }
            _moviDataSize += (uint)len + 8;
            _frameCount++;

            if (_alphaWriter != null) {
                _alphaWriter.WriteChunk(chunk, timeStamp, frameType);
            }
        }

        private void GetFrameSize(byte[] chunk) {
            if (_codecID == 2) {
                // Reference: flv_h263_decode_picture_header from libavcodec's h263.c

                if (chunk.Length < 10) return;

                if ((chunk[0] != 0) || (chunk[1] != 0)) {
                    return;
                }

                ulong x = BitConverterBE.ToUInt64(chunk, 2);
                int format;

                if (BitHelper.Read(ref x, 1) != 1) {
                    return;
                }
                BitHelper.Read(ref x, 5);
                BitHelper.Read(ref x, 8);

                format = BitHelper.Read(ref x, 3);
                switch (format) {
                    case 0:
                        _width = BitHelper.Read(ref x, 8);
                        _height = BitHelper.Read(ref x, 8);
                        break;
                    case 1:
                        _width = BitHelper.Read(ref x, 16);
                        _height = BitHelper.Read(ref x, 16);
                        break;
                    case 2:
                        _width = 352;
                        _height = 288;
                        break;
                    case 3:
                        _width = 176;
                        _height = 144;
                        break;
                    case 4:
                        _width = 128;
                        _height = 96;
                        break;
                    case 5:
                        _width = 320;
                        _height = 240;
                        break;
                    case 6:
                        _width = 160;
                        _height = 120;
                        break;
                    default:
                        return;
                }
            }
            else if ((_codecID == 4) || (_codecID == 5)) {
                // Reference: vp6_parse_header from libavcodec's vp6.c

                int skip = (_codecID == 4) ? 1 : 4;
                if (chunk.Length < (skip + 8)) return;
                ulong x = BitConverterBE.ToUInt64(chunk, skip);

                int deltaFrameFlag = BitHelper.Read(ref x, 1);
                int quant = BitHelper.Read(ref x, 6);
                int separatedCoeffFlag = BitHelper.Read(ref x, 1);
                int subVersion = BitHelper.Read(ref x, 5);
                int filterHeader = BitHelper.Read(ref x, 2);
                int interlacedFlag = BitHelper.Read(ref x, 1);

                if (deltaFrameFlag != 0) {
                    return;
                }
                if ((separatedCoeffFlag != 0) || (filterHeader == 0)) {
                    BitHelper.Read(ref x, 16);
                }

                _height = BitHelper.Read(ref x, 8) * 16;
                _width = BitHelper.Read(ref x, 8) * 16;

                // chunk[0] contains the width and height (4 bits each, respectively) that should
                // be cropped off during playback, which will be non-zero if the encoder padded
                // the frames to a macroblock boundary.  But if you use this adjusted size in the
                // AVI header, DirectShow seems to ignore it, and it can cause stride or chroma
                // alignment problems with VFW if the width/height aren't multiples of 4.
                if (!_isAlphaWriter) {
                    int cropX = chunk[0] >> 4;
                    int cropY = chunk[0] & 0x0F;
                    if (((cropX != 0) || (cropY != 0)) && !_isAlphaWriter) {
                        _warnings.Add(String.Format("Suggested cropping: {0} pixels from right, {1} pixels from bottom.", cropX, cropY));
                    }
                }
            }
        }

        private string CodecFourCC {
            get {
                if (_codecID == 2) {
                    return "FLV1";
                }
                if ((_codecID == 4) || (_codecID == 5)) {
                    return "VP6F";
                }
                return "NULL";
            }
        }

        private void WriteIndexChunk() {
            uint indexDataSize = (uint)_frameCount * 16;

            WriteFourCC("idx1");
            _bw.Write(indexDataSize);

            for (int i = 0; i < _frameCount; i++) {
                WriteFourCC("00dc");
                _bw.Write(_index[(i * 3) + 0]);
                _bw.Write(_index[(i * 3) + 1]);
                _bw.Write(_index[(i * 3) + 2]);
            }

            _indexChunkSize = indexDataSize + 8;
        }

        public void Finish(FractionUInt32 averageFrameRate) {
            WriteIndexChunk();

            _bw.BaseStream.Seek(4, SeekOrigin.Begin);
            _bw.Write((uint)(224 + _moviDataSize + _indexChunkSize - 8));

            _bw.BaseStream.Seek(24 + 8, SeekOrigin.Begin);
            _bw.Write((uint)0);
            _bw.BaseStream.Seek(12, SeekOrigin.Current);
            _bw.Write((uint)_frameCount);
            _bw.BaseStream.Seek(12, SeekOrigin.Current);
            _bw.Write((uint)_width);
            _bw.Write((uint)_height);

            _bw.BaseStream.Seek(100 + 28, SeekOrigin.Begin);
            _bw.Write((uint)averageFrameRate.D);
            _bw.Write((uint)averageFrameRate.N);
            _bw.BaseStream.Seek(4, SeekOrigin.Current);
            _bw.Write((uint)_frameCount);
            _bw.BaseStream.Seek(16, SeekOrigin.Current);
            _bw.Write((ushort)_width);
            _bw.Write((ushort)_height);

            _bw.BaseStream.Seek(164 + 12, SeekOrigin.Begin);
            _bw.Write((uint)_width);
            _bw.Write((uint)_height);
            _bw.BaseStream.Seek(8, SeekOrigin.Current);
            _bw.Write((uint)(_width * _height * 6));

            _bw.BaseStream.Seek(212 + 4, SeekOrigin.Begin);
            _bw.Write((uint)(_moviDataSize + 4));

            _bw.Close();

            if (_alphaWriter != null) {
                _alphaWriter.Finish(averageFrameRate);
            }
        }

        private void WriteFourCC(string fourCC) {
            byte[] bytes = Encoding.ASCII.GetBytes(fourCC);
            if (bytes.Length != 4) {
                throw new Exception("Invalid FourCC length.");
            }
            _bw.Write(bytes);
        }

        public string Path {
            get {
                return _path;
            }
        }
    }

    internal class TimeCodeWriter {
        private string _path;
        private StreamWriter _sw;

        public TimeCodeWriter(string path) {
            _path = path;
            if (path != null) {
                _sw = new StreamWriter(path, false, Encoding.ASCII);
                _sw.WriteLine("# timecode format v2");
                _sw.WriteLine("frames,offset,tagType,tagSize,pkgType,codecType,dts,dts-step,pts,pts-dts");
            }
        }

        public void Write(long indx, long pos, 
                            uint tagType, uint tagSize, 
                            uint pkgType, uint codecType, 
                            uint timeStamp, uint diff = 0, 
                            int composite = 0, int delta = 0)
        {
            if (_sw != null)
            {
                _sw.WriteLine("{0:D},{1:D},{2:D},{3:D},{4:D},{5:D},{6:D},{7:D},{8:D},{9:D}", indx, pos, tagType, tagSize, pkgType, codecType, timeStamp, diff, composite, delta);
            }
        }

        public void Finish() {
            if (_sw != null) {
                _sw.Close();
                _sw = null;
            }
        }

        public string Path {
            get {
                return _path;
            }
        }
    }

    public struct FractionUInt32 {
        public uint N;
        public uint D;

        public FractionUInt32(uint n, uint d) {
            N = n;
            D = d;
        }

        public double ToDouble() {
            return (double)N / (double)D;
        }

        public void Reduce() {
            uint gcd = GCD(N, D);
            N /= gcd;
            D /= gcd;
        }

        private uint GCD(uint a, uint b) {
            uint r;

            while (b != 0) {
                r = a % b;
                a = b;
                b = r;
            }

            return a;
        }

        public override string ToString() {
            return ToString(true);
        }

        public string ToString(bool full) {
            if (full) {
                return ToDouble().ToString() + " (" + N.ToString() + "/" + D.ToString() + ")";
            }
            else {
                return ToDouble().ToString("0.####");
            }
        }
    }
}
