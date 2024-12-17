# FLVExtract - demuxer for flv files
## English
### Description
  * Base On [FlvExtract](https://github.com/flagbug/FlvExtract), Thanks to
  * Support HEVC for codec id 0x09 or 0x0D (always using in **CDN**)
  * support export video stream to .h264 or .h265 and audio stream to .aac etc.
  * support export pts, dts and diff of them 
  * support **remuxer** to mp4 (using ffmpeg as backend, which should be in your %PATH%)


## USAGE

- start

![start](docs/start.png)

- result

![result](docs/result.png)

- hexview

![hexview](docs/hexview.png)


## License
  * [GNU General Public License version 2 (GPLv2)](https://opensource.org/licenses/gpl-2.0)

## References
* A portable (PCL) .NET library that extracts the audio and/or video tracks from FLV files
This library is a PCL port of the FlvExtract library from J.D. Purcell (http://moitah.net/)


## changelog
- 2.3.0(2024/12/10)
  - feat: change mp4 muxer(FFmpeg should be in your path)
  - feat: add hevc muxer dector
- 2.2.0(2023/9/1)
  - feat: add mp4 muxer(by using mp4box, mp4box should be in your path)
- 2.1.0(2023/4/24)
  - feat: add time stamp information 
  - feat: add hex view for per Flv Tag
  - feat: key frame jump for video Tag
- 1.7.0 (2023/4/14)
  - feat: add HVCCDecoderConfigurationRecord support
  - UI: big fonts
- Ancient
  - HEVC support
  - dts、pts output 
