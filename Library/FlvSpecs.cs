using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDP.Library
{
    struct video
    {
        public string frametype;
        public string codecID;
        public string avcPacketType;
        public int compositionTime;
    };
    struct audio
    {
        public byte soundFormat;
        public byte soundRate;
        public byte soundSize;
        public byte soundType;
        public byte aacPacketType;
    };
    struct FlvTag
    {
        public uint tagType;
        public uint dataSize;
        public uint timestamp;
        public uint streamID;
        public video v;
        public audio a;
        public byte[] data;
        public uint previousTagSize;
    }
    internal class FlvSpecs
    {
        private FileStream _fs;
        long _fileOffset = 0;
        long _fileLength = 0;
        public FlvSpecs(string path) 
        {
            _fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536);
            _fileLength = _fs.Length;
        }

        public bool parseTag(long offset, ref FlvTag detail)
        {
            
            _fs.Seek(offset, SeekOrigin.Begin);
            
            uint tagType, dataSize, timeStamp, streamID, mediaInfo, avcPacketType;
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
            avcPacketType = (composition >> 24) & 0xff;
            UInt32 tempv = composition & 0x00ffffff;
            Int32 compositionTime = (Int32)((tempv & 0x00800000) << 8 | (tempv & 0x007fffff));
            Int32 pts = (Int32)timeStamp + compositionTime;
            Seek(curTagpos);
            data = ReadBytes((int)dataSize + 11);
            uint previousTagSize = ReadUInt32();

            detail.tagType = tagType;
            detail.dataSize = dataSize;
            detail.timestamp = timeStamp;
            detail.streamID = streamID;
            detail.data = data;
            detail.previousTagSize = previousTagSize;
            if(tagType == 9)
            {
                detail.v.frametype = videoTagFrameType((mediaInfo>>4)&0x0f);
                detail.v.codecID = videoCodecID(mediaInfo & 0x0f);
                detail.v.avcPacketType = videoAVCPacketType(avcPacketType);
                detail.v.compositionTime = compositionTime;
            }
            else if(tagType == 8)
            {

            }
            return true;
        }

        private string videoTagFrameType(uint v)
        {
            string[] types = new string[]
            {
                "key frame (seekable)",
                "inter frame (non-seekable)",
                "disposable inter frame (H.263 only)",
                "generated key frame (reserved for server use only)",
                "video info/command frame"
            };
            if(v < 5 && v > 0)
            {
                return types[v-1] + "[" + v + "]";
            }
            else
            {
                return "(unknown)" + "[" + v + "]";
            }
        }

        private string videoCodecID(uint v)
        {
            string[] types = new string[]
            {
                "Sorenson H.263",
                "Screen video",
                "On2 VP6",
                "On2 VP6 with alpha channel",
                "Screen video version 2",
                "AVC",
                "HEVC"
            };
            if (v < 9 && v > 1)
            {
                return types[v-2] + "[" + v + "]";
            }
            else if(v == 12)
            {
                return "HEVC" + "[" + v + "]";
            }
            else
            {
                return "(unknown)" + "[" + v + "]";
            }
        }

        private string videoAVCPacketType(uint v)
        {
            string[] types = new string[]
            {
                "sequence header",
                "NALU",
                "end of sequence"
            };
            if (v < 2)
            {
                return types[v] + "[" + v + "]";
            }
            else
            {
                return "(unknown)" + "[" + v + "]";
            }
        }

        private void Seek(long offset)
        {
            _fs.Seek(offset, SeekOrigin.Begin);
            _fileOffset = offset;
        }

        private uint ReadUInt8()
        {
            _fileOffset += 1;
            return (uint)_fs.ReadByte();
        }

        private long CurReadPosition()
        {
            return _fs.Position;
        }

        private uint ReadUInt24()
        {
            byte[] x = new byte[4];
            _fs.Read(x, 1, 3);
            _fileOffset += 3;
            return BitConverterBE.ToUInt32(x, 0);
        }

        private uint ReadUInt32()
        {
            byte[] x = new byte[4];
            _fs.Read(x, 0, 4);
            _fileOffset += 4;
            return BitConverterBE.ToUInt32(x, 0);
        }

        private byte[] ReadBytes(int length)
        {
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
}
