using FFmpeg.AutoGen;
using FFmpeg.AutoGen.Example;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JDP
{
    public unsafe class Remux
    {
        static unsafe void log_packet(AVFormatContext* fmt_ctx, AVPacket* pkt, string tag, int stream_index)
        {
            AVRational* time_base = &fmt_ctx->streams[pkt->stream_index]->time_base;

            Console.WriteLine($"{tag}: pts:{FFmpegHelper.av_ts2str(pkt->pts)} pts_time:{FFmpegHelper.av_ts2timestr(pkt->pts, time_base)}"
                + $" dts:{FFmpegHelper.av_ts2str(pkt->dts)} dts_time:{FFmpegHelper.av_ts2timestr(pkt->dts, time_base)}"
                + $" duration:{FFmpegHelper.av_ts2str(pkt->duration)} duration_time:{FFmpegHelper.av_ts2timestr(pkt->duration, time_base)}"
                + $" stream_index: {pkt->stream_index}/{stream_index}");
        }


        private static unsafe int write_stream(AVFormatContext* iFmtCtx,
                                                int stream_index,
                                                AVFormatContext* oFmtCtx,
                                                ref AVPacket* pkt,
                                                long dts,
                                                long pts)
        {
            int ret = 0;

            do
            {
                AVStream* in_stream;
                AVStream* out_stream;

                ret = ffmpeg.av_read_frame(iFmtCtx, pkt);
                if (ret < 0)
                {
                    break;
                }

                in_stream = iFmtCtx->streams[pkt->stream_index];
                pkt->pts = pts;
                pkt->dts = dts;
                AVRational rational = new AVRational();
                rational.den = 1000;
                rational.num = 1;
                pkt->duration = ffmpeg.av_rescale_q(1, rational, in_stream->time_base); 
                //log_packet(iFmtCtx, pkt, "in", stream_index);

                pkt->stream_index = stream_index;
                out_stream = oFmtCtx->streams[pkt->stream_index];

                ffmpeg.av_packet_rescale_ts(pkt, in_stream->time_base, out_stream->time_base);
                pkt->pos = -1;
                //log_packet(oFmtCtx, pkt, "out", stream_index);

                ret = ffmpeg.av_interleaved_write_frame(oFmtCtx, pkt);
                if (ret < 0)
                {
                    Console.WriteLine("Error muxing packet");
                }

                break;
            } while (false);

            return ret;
        }

        private static unsafe int fill_stream(AVFormatContext* iFmtCtx, AVFormatContext* oFmtCtx)
        {
            int ret = 0;
            AVStream* out_stream;
            AVStream* in_stream = iFmtCtx->streams[0];
            AVCodecParameters* in_codecpar = in_stream->codecpar;

            do
            {
                out_stream = ffmpeg.avformat_new_stream(oFmtCtx, null);
                if (out_stream == null)
                {
                    Console.WriteLine("Failed allocating output stream");
                    ret = ffmpeg.AVERROR_UNKNOWN;
                    break;
                }

                ret = ffmpeg.avcodec_parameters_copy(out_stream->codecpar, in_codecpar);
                if (ret < 0)
                {
                    Console.WriteLine("Failed to copy codec parameters");
                    break;
                }
            } while (false);

            out_stream->codecpar->codec_tag = 0;
            return ret;
        }

        private static unsafe int get_stream_info(string filename, ref AVFormatContext* iFmtCtx)
        {
            AVFormatContext* ic = null;
            int ret = -1;

            do
            {
                if (filename == null)
                {
                    break;
                }

                if ((ret = ffmpeg.avformat_open_input(&ic, filename, null, null)) < 0)
                {
                    Console.WriteLine($"Could not open input fiel '{filename}'");
                    break;
                }
                ic->flags |= ffmpeg.AVFMT_FLAG_GENPTS;

                if ((ret = ffmpeg.avformat_find_stream_info(ic, null)) < 0)
                {
                    Console.WriteLine("Failed to retrieve input stream information");
                    break;
                }

                ffmpeg.av_dump_format(ic, 0, filename, 0);

                var par = ic->streams[0]->codecpar;
                ic->streams[0]->time_base.den = 1000;
                ic->streams[0]->time_base.num = 1;

                if (par->codec_type != AVMediaType.AVMEDIA_TYPE_AUDIO &&
                        par->codec_type != AVMediaType.AVMEDIA_TYPE_VIDEO &&
                        par->codec_type != AVMediaType.AVMEDIA_TYPE_SUBTITLE)
                {
                    break;
                }

                ret = 0;
            } while (false);

            if (ret != 0)
            {
                ffmpeg.avformat_close_input(&ic);
            }
            else
            {
                iFmtCtx = ic;
            }

            return ret;
        }

        public static unsafe int Start(string audioFileName,
                                        string videoFileName,
                                        string outFileName,
                                        List<uint> aptss,
                                        List<uint> vdtss,
                                        List<uint> vptss)
        {
            FFmpegBinariesHelper.RegisterFFmpegBinaries();

            try
            {
                ffmpeg.av_version_info();
            }
            catch (Exception)
            {
                throw new DllNotFoundException($"FFmpeg Depends Not Found, {outFileName} Not Created.");
            }

#if DEBUG
            Console.WriteLine("Current directory: " + Environment.CurrentDirectory);
            Console.WriteLine("Running in {0}-bit mode.", Environment.Is64BitProcess ? "64" : "32");
            Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");

            Console.WriteLine();
#endif

            AVOutputFormat* ofmt = null;
            AVFormatContext* ifmt_audio_ctx = null;
            AVFormatContext* ifmt_video_ctx = null;
            AVFormatContext* ofmt_ctx = null;
            AVPacket* pkt = null;
            string in_audio_filename, in_video_filename, out_filename;
            int ret;
            int stream_index = 0;
            int[] stream_mapping = { 0, 0, 0, 0 };

            bool has_audio = false;
            bool has_video = false;

            in_audio_filename = audioFileName;
            in_video_filename = videoFileName;
            out_filename = outFileName;

            pkt = ffmpeg.av_packet_alloc();
            if (pkt == null)
            {
                Console.WriteLine("Could not allocate AVPacket");
                return 1;
            }

            ffmpeg.avformat_alloc_output_context2(&ofmt_ctx, null, null, out_filename);
            if (ofmt_ctx == null)
            {
                Console.WriteLine("Could not create output context");
                ret = ffmpeg.AVERROR_UNKNOWN;
                goto end;
            }

            ofmt = ofmt_ctx->oformat;
            if ((ofmt->flags & ffmpeg.AVFMT_NOFILE) == 0)
            {
                ret = ffmpeg.avio_open(&ofmt_ctx->pb, out_filename, ffmpeg.AVIO_FLAG_WRITE);
                if (ret < 0)
                {
                    Console.WriteLine($"Could not open output file '{out_filename}");
                    goto end;
                }
            }

            if (get_stream_info(in_audio_filename, ref ifmt_audio_ctx) >= 0)
            {
                has_audio = true;
                fill_stream(ifmt_audio_ctx, ofmt_ctx);
                stream_mapping[0] = stream_index++;
            }
            if (get_stream_info(in_video_filename, ref ifmt_video_ctx) >= 0)
            {
                has_video = true;
                fill_stream(ifmt_video_ctx, ofmt_ctx);
                stream_mapping[1] = stream_index++;
            }

            ret = ffmpeg.avformat_write_header(ofmt_ctx, null);
            if (ret < 0)
            {
                Console.WriteLine("Error occurred when opening output file");
                goto end;
            }

            ret = 0;
            int audio_frame_index = 0;
            int video_frame_index = 0;
            while (true)
            {
                if (has_audio && has_video)
                {
                    if (vdtss[video_frame_index] < aptss[audio_frame_index])
                    {
                        long pts = vptss[video_frame_index];
                        long dts = vdtss[video_frame_index];

                        //Console.WriteLine($"->V {dts},{pts}@{video_frame_index}");
                        ret = write_stream(ifmt_video_ctx, stream_mapping[1], ofmt_ctx, ref pkt, dts, pts);
                        if (ret < 0)
                            break;

                        video_frame_index++;
                    }
                    else
                    {
                        var pts = aptss[audio_frame_index];

                        //Console.WriteLine($"->A {pts}@{audio_frame_index}");
                        ret = write_stream(ifmt_audio_ctx, stream_mapping[0], ofmt_ctx, ref pkt, pts, pts);
                        if (ret < 0)
                            break;

                        audio_frame_index++;
                    }
                }
                else if (has_video)
                {
                    long pts = vptss[video_frame_index];
                    long dts = vdtss[video_frame_index];

                    ret = write_stream(ifmt_video_ctx, stream_mapping[1], ofmt_ctx, ref pkt, dts, pts);
                    if (ret < 0)
                        break;

                    video_frame_index++;
                }
                else if (has_audio)
                {
                    var pts = aptss[audio_frame_index];

                    ret = write_stream(ifmt_audio_ctx, stream_mapping[0], ofmt_ctx, ref pkt, pts, pts);
                    if (ret < 0)
                        break;

                    audio_frame_index++;
                }
                else
                {
                    break;
                }
                ffmpeg.av_packet_unref(pkt);
            }

            if (has_audio)
                Console.WriteLine($"audio -> {audio_frame_index}@{aptss[aptss.Count-1] - aptss[0]}");
            if(has_video)
                Console.WriteLine($"video -> {video_frame_index}@{vdtss[vdtss.Count-1] - vdtss[0]}@{vptss[vdtss.Count-1] - vptss[0]}");

            ffmpeg.av_write_trailer(ofmt_ctx);
end:
            ffmpeg.av_packet_free(&pkt);

            if (has_audio)
                ffmpeg.avformat_close_input(&ifmt_audio_ctx);
            if (has_video)
                ffmpeg.avformat_close_input(&ifmt_video_ctx);

            if (ofmt_ctx != null && (ofmt->flags & ffmpeg.AVFMT_NOFILE) == 0)
            {
                ffmpeg.avio_closep(&ofmt_ctx->pb);
            }

            ffmpeg.avformat_free_context(ofmt_ctx);

            if (ret < 0 && ret != ffmpeg.AVERROR_EOF)
            {
                Console.WriteLine($"Error occurred: {FFmpegHelper.av_strerror(ret)}");
                return 1;
            }

            return 0;
        }
    }
}
