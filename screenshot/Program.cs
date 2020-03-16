using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace screenshot
{
    class Program
    {
        public static string CreateTempFileName()
        {
            string result = Path.GetTempPath() + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString() + ".jpeg";
            return result;
        }

        public static void CaptureImage(string outFile)
        {
            try
            {
                Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Bitmap res = null;
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    try
                    {
                        g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                        bmp.Save(outFile);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[X] Error: {0}", ex);
            }
        }

        public static List<string> GetFiles()
        {
            List<string> results = new List<string> { };
            string searchPattern = $"{DateTime.Now.ToString("yyyyMMdd")}*.*";
            DirectoryInfo di = new DirectoryInfo(Path.GetTempPath());
            FileInfo[] files = di.GetFiles(searchPattern);
            foreach (FileInfo file in files)
            {
                results.Add(file.FullName);
            }
            return results;
        }

        internal static void ZipFile(string filename, ZipOutputStream stream, int offset)
        {
            try
            {
                using (var sReader = File.Open(filename, FileMode.Open, FileAccess.Read))
                {
                    var info = new FileInfo(filename);
                    var entryName = ZipEntry.CleanName(filename.Substring(offset));
                    var zEntry = new ZipEntry(entryName)
                    {
                        DateTime = info.LastWriteTime,
                        Size = info.Length
                    };

                    stream.PutNextEntry(zEntry);
                    var buffer = new byte[4096];
                    StreamUtils.Copy(sReader, stream, buffer);

                    stream.CloseEntry();
                }
            }
            catch
            {
                //Access denied probably
            }
        }

        public static void Zip(List<string> files)
        {
            var fsOut = File.Create(Path.GetTempPath() + DateTime.Now.ToString("yyyyMMdd") + ".zip");
            var zipstream = new ZipOutputStream(fsOut);
            zipstream.SetLevel(7);
            foreach (var fpath in files)
            {
                ZipFile(fpath, zipstream, 0);
            }
            zipstream.IsStreamOwner = true;
            zipstream.Close();
            fsOut.Close();
        }

        public static void RemoveFiles(List<string> files)
        {
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static void Help()
        {
            Console.WriteLine("[!] Incorrect arguments provided!\n\tUsage: screenshot <interval> <timer>\n\tExamples:\n\t\tscreenshot.exe 0 (takes a single screenshot and saves to AppData\\Temp)\n\t\tscreenshot.exe 30 600 (take a screenshot every 30 seconds for 5 minutes and save to <date>.zip)");
        }

        static void Main(string[] args)
        {
            try
            {
                if (Int32.Parse(args[0]) == 0)
                {
                    string file = CreateTempFileName();
                    CaptureImage(file);
                }
                else if (args.Length == 2)
                {
                    int i = 0;
                    while (true)
                    {
                        string file = CreateTempFileName();
                        CaptureImage(file);
                        Thread.Sleep(Int32.Parse(args[0]) * 1000);
                        i++;
                        if ((i * (Int32.Parse(args[0]) * 1000)) == (Int32.Parse(args[1]) * 1000))
                        {
                            break;
                        }
                    }
                    List<string> files = GetFiles();
                    Zip(files);
                    RemoveFiles(files);
                }
                else
                {
                    Help();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[-] Error: " + e.Message);
                Help();
            }
        }
    }
}
