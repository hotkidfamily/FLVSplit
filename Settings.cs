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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JDP {
    internal static class SettingsShared {
        public static string GetMyAppDataDir(string appName) {
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string myAppDataDir = Path.Combine(appDataDir, appName);

            if (Directory.Exists(myAppDataDir) == false) {
                Directory.CreateDirectory(myAppDataDir);
            }

            return myAppDataDir;
        }
    }

    internal class SettingsReader {
        private Dictionary<string, string> _settings;

        public SettingsReader(string appName, string fileName) {
            _settings = new Dictionary<string, string>();

            string path = Path.Combine(SettingsShared.GetMyAppDataDir(appName), fileName);
            if (!File.Exists(path)) {
                return;
            }

            using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {
                string line, name, val;
                int pos;

                while ((line = sr.ReadLine()) != null) {
                    pos = line.IndexOf('=');
                    if (pos != -1) {
                        name = line.Substring(0, pos);
                        val = line.Substring(pos + 1);

                        if (!_settings.ContainsKey(name)) {
                            _settings.Add(name, val);
                        }
                    }
                }
            }
        }

        public string Load(string name) {
            return _settings.ContainsKey(name) ? _settings[name] : null;
        }
    }

    internal class SettingsWriter {
        private StreamWriter _sw;

        public SettingsWriter(string appName, string fileName) {
            string path = Path.Combine(SettingsShared.GetMyAppDataDir(appName), fileName);

            _sw = new StreamWriter(path, false, Encoding.UTF8);
        }

        public void Save(string name, string value) {
            _sw.WriteLine(name + "=" + value);
        }

        public void Close() {
            _sw.Close();
        }
    }
}
