using System;
using System.IO;

namespace JDP.Library
{
    struct nalu
    {
        public int type;
        public byte[] data;
    }
    struct video
    {
        public string frametype;
        public string codecID;
        public string avcPacketType;
        public int compositionTime;

        public int NALUs;
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
        private static readonly byte[] _startCode = new byte[] { 0, 0, 0, 1 };

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

            if ((_fileLength - _fileOffset) < 11)
            {
                return false;
            }

            // Read tag header
            tagType = ReadUInt8();
            dataSize = ReadUInt24();
            timeStamp = ReadUInt24();
            timeStamp |= ReadUInt8() << 24;
            streamID = ReadUInt24();

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
            Seek(offset);
            data = ReadBytes((int)dataSize + 11);
            uint previousTagSize = ReadUInt32();

            parserNalu(ref data, ref detail);

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

        private bool parserNalu(ref byte[] data, ref FlvTag detail)
        {
            if(data == null)
                return false;

            int nalus = 0;

            int dataOffset = 16; // 11 bytes tag size + 5 bytes video tag 

            int v1 = BitConverter.ToInt32(data, dataOffset);
            int v2 = BitConverter.ToInt32(_startCode, 0);

            if (v1 == v2) // annexb format
            {
                if (data[dataOffset] == 0)
                { // Headers vps sps pps 
                    if (data.Length < 10 + dataOffset) return false;
                    int offset = 4 + dataOffset;
                    int len = data.Length - offset;
                }
                else
                { // Video data
                    int offset = 4 + dataOffset;
                    int len = data.Length - offset;
                }
            }
            else if (data[dataOffset - 4] == 0)
            { // Headers HVCCDecoderConfigurationRecord
                if (data.Length < 10) return false;

                int offset, vpsCount = 0, spsCount = 0, ppsCount = 0, nalArrays;

                offset = (int)HVCCPayloadOffset.lengthSizeMinusOne + dataOffset;
                _nalLengthSize = (data[offset++] & 0x03) + 1;

                offset = (int)HVCCPayloadOffset.numOfArrays + dataOffset;
                nalArrays = data[offset++];

                for (int i = 0; i < nalArrays; i++)
                {
                    int nalType = data[offset++] & 0x3f;
                    int numNalus = (int)BitConverterBE.ToUInt16(data, offset);
                    offset += 2;

                    if (nalType == (int)HEVCNalType.VPS)
                        vpsCount++;
                    if (nalType == (int)HEVCNalType.SPS)
                        spsCount++;
                    if (nalType == (int)HEVCNalType.PPS)
                        ppsCount++;

                    for (int j = 0; j < numNalus; j++)
                    {
                        int len = (int)BitConverterBE.ToUInt16(data, offset);
                        offset += 2;
                        offset += len;
                        nalus++;
                    }
                }
            }
            else
            { // Video data
                int offset = dataOffset;

                while (offset <= data.Length - _nalLengthSize)
                {
                    int len = (_nalLengthSize == 2) ?
                        (int)BitConverterBE.ToUInt16(data, offset) :
                        (int)BitConverterBE.ToUInt32(data, offset);
                    offset += _nalLengthSize;
                    if (offset + len > data.Length) break;
                    offset += len;
                    nalus++;
                }
            }

            detail.v.NALUs = nalus;

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
