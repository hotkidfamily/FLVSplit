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
using System.IO;

namespace WAVTools {
    public class WAVWriter {
        private BinaryWriter _bw;
        private bool _canSeek;
        private bool _wroteHeaders;
        private int _bitsPerSample;
        private int _channelCount;
        private int _sampleRate;
        private int _blockAlign;
        private long _finalSampleLen;
        private long _sampleLen;

        public WAVWriter(string path, int bitsPerSample, int channelCount, int sampleRate) :
            this(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read),
                bitsPerSample, channelCount, sampleRate)
        {
        }

        public WAVWriter(Stream stream, int bitsPerSample, int channelCount, int sampleRate) {
            _bitsPerSample = bitsPerSample;
            _channelCount = channelCount;
            _sampleRate = sampleRate;
            _blockAlign = _channelCount * ((_bitsPerSample + 7) / 8);

            _bw = new BinaryWriter(stream);
            _canSeek = stream.CanSeek;
        }

        private void WriteHeaders() {
            const uint fccRIFF = 0x46464952;
            const uint fccWAVE = 0x45564157;
            const uint fccFormat = 0x20746D66;
            const uint fccData = 0x61746164;

            uint dataChunkSize = GetDataChunkSize(_finalSampleLen);

            _bw.Write(fccRIFF);
            _bw.Write((uint)(dataChunkSize + (dataChunkSize & 1) + 36));
            _bw.Write(fccWAVE);

            _bw.Write(fccFormat);
            _bw.Write((uint)16);
            _bw.Write((ushort)1);
            _bw.Write((ushort)_channelCount);
            _bw.Write((uint)_sampleRate);
            _bw.Write((uint)(_sampleRate * _blockAlign));
            _bw.Write((ushort)_blockAlign);
            _bw.Write((ushort)_bitsPerSample);

            _bw.Write(fccData);
            _bw.Write((uint)dataChunkSize);
        }

        private uint GetDataChunkSize(long sampleCount) {
            const long maxFileSize = 0x7FFFFFFEL;
            long dataSize = sampleCount * _blockAlign;
            if ((dataSize + 44) > maxFileSize) {
                dataSize = ((maxFileSize - 44) / _blockAlign) * _blockAlign;
            }
            return (uint)dataSize;
        }

        public void Close() {
            if (((_sampleLen * _blockAlign) & 1) == 1) {
                _bw.Write((byte)0);
            }

            try {
                if (_sampleLen != _finalSampleLen) {
                    if (_canSeek) {
                        uint dataChunkSize = GetDataChunkSize(_sampleLen);
                        _bw.Seek(4, SeekOrigin.Begin);
                        _bw.Write((uint)(dataChunkSize + (dataChunkSize & 1) + 36));
                        _bw.Seek(40, SeekOrigin.Begin);
                        _bw.Write((uint)dataChunkSize);
                    }
                    else {
                        throw new Exception("Samples written differs from the expected sample count.");
                    }
                }
            }
            finally {
                _bw.Close();
                _bw = null;
            }
        }

        public long Position {
            get {
                return _sampleLen;
            }
        }

        public long FinalSampleCount {
            set {
                _finalSampleLen = value;
            }
        }

        public void Write(byte[] buff, int sampleCount) {
            if (sampleCount <= 0) return;

            if (!_wroteHeaders) {
                WriteHeaders();
                _wroteHeaders = true;
            }

            _bw.Write(buff, 0, sampleCount * _blockAlign);
            _sampleLen += sampleCount;
        }
    }
}
