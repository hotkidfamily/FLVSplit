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

using FFmpeg.AutoGen.Example;
using System;
using System.IO;

namespace JDP
{
    internal class Program
    {
        private static bool _autoOverwrite;

        private static int Main(string[] args)
        {
            int argCount = args.Length;
            int argIndex = 0;
            bool extractVideo = false;
            bool extractAudio = false;
            bool extractTimeCodes = false;
            bool transcode = false;
            string outputDirectory = null;
            string inputPath;

            Console.WriteLine("FLV Extract CL v" + VersionInfo.DisplayVersion);
            Console.WriteLine($"Copyright {VersionInfo.CopyrightYears} {Environment.NewLine}Authors {VersionInfo.Authors}");
            Console.WriteLine(VersionInfo.Website);
            Console.WriteLine();

            try
            {
                while (argIndex < argCount)
                {
                    switch (args[argIndex])
                    {
                    case "-v":
                        extractVideo = true;
                        break;
                    case "-a":
                        extractAudio = true;
                        break;
                    case "-t":
                        extractTimeCodes = true;
                        break;
                    case "-o":
                        _autoOverwrite = true;
                        break;
                    case "-r":
                    {
                        extractAudio = true;
                        extractVideo = true;
                        transcode = true;
                        break;
                    }
                    case "-d":
                        outputDirectory = args[++argIndex];
                        break;
                    default:
                        goto BreakArgLoop;
                    }
                    argIndex++;
                }
BreakArgLoop:

                if (argIndex != (argCount - 1))
                {
                    throw new Exception();
                }
                inputPath = args[argIndex];
            }
            catch
            {
                Console.WriteLine("Arguments: [switches] source_path");
                Console.WriteLine();
                Console.WriteLine("Switches:");
                Console.WriteLine("  -v         Extract video.");
                Console.WriteLine("  -a         Extract audio.");
                Console.WriteLine("  -t         Extract timecodes.");
                Console.WriteLine("  -r         Remuxer to mp4 (FFmpeg libraries should be in FFmpeg/bin/x64, -v or -a or both should be selected).");
                Console.WriteLine("  -o         Overwrite output files without prompting.");
                Console.WriteLine("  -d <dir>   Output directory.  If not specified, output files will be written");
                Console.WriteLine("             in the same directory as the source file.");
                return 1;
            }

            try
            {
                using (FLVFile flvFile = new FLVFile(Path.GetFullPath(inputPath)))
                {
                    if (outputDirectory != null)
                    {
                        flvFile.OutputDirectory = Path.GetFullPath(outputDirectory);
                    }
                    flvFile.ExtractStreams(extractAudio, extractVideo, extractTimeCodes, transcode, PromptOverwrite);
                    if ((flvFile.TrueFrameRate != null) || (flvFile.AverageFrameRate != null) || flvFile.GuessFrameRate != null)
                    {
                        if (flvFile.TrueFrameRate != null)
                        {
                            Console.WriteLine("True Frame Rate: " + flvFile.TrueFrameRate.ToString());
                        }
                        if (flvFile.AverageFrameRate != null)
                        {
                            Console.WriteLine("Average Frame Rate: " + flvFile.AverageFrameRate.ToString());
                        }
                        if (flvFile.GuessFrameRate != null)
                        {
                            Console.WriteLine("Guess Frame Rate: " + flvFile.GuessFrameRate.ToString());
                        }
                        Console.WriteLine();
                    }
                    if (flvFile.Warnings.Length != 0)
                    {
                        foreach (string warning in flvFile.Warnings)
                        {
                            Console.WriteLine("Warning: " + warning);
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }

            Console.WriteLine("Finished.");
            return 0;
        }

        private static bool PromptOverwrite(string path)
        {
            if (_autoOverwrite) return true;
            bool? overwrite = null;
            Console.Write("Output file \"" + Path.GetFileName(path) + "\" already exists, overwrite? (y/n): ");
            while (overwrite == null)
            {
                char c = Console.ReadKey(true).KeyChar;
                if (c == 'y') overwrite = true;
                if (c == 'n') overwrite = false;
            }
            Console.WriteLine(overwrite.Value ? "y" : "n");
            Console.WriteLine();
            return overwrite.Value;
        }
    }
}
