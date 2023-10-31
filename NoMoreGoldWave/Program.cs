using NAudio.Wave;
using System;
using System.Data.Odbc;
using System.IO;

namespace NoMoreGoldWave
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //No input files
            if (args.Length == 0 || args.Length > 1)
            {
                Console.WriteLine("Please Drag 'n Drop Your Audio File Onto The EXE File.");
                Console.ReadLine();
                Environment.Exit(1);
            }

            //File doesnt exist
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Specified file does not exist.");
                Console.ReadLine();
                Environment.Exit(1);
            }

            try
            {
                FileStream input = File.OpenRead(args[0]);
                MemoryStream  outputStream = new MemoryStream();
                input.CopyTo(outputStream);
                input.Dispose();
                outputStream.Position = outputStream.Length;

                int lastSample = 0;

                using (var waveStream = new WaveFileReader(args[0]))
                {
                    lastSample = (int)(waveStream.SampleCount);
                }

                BinaryWriter bw = new BinaryWriter(outputStream);
                BinaryReader br = new BinaryReader(outputStream);

                //Write Header
                bw.Write("cue ".ToCharArray());
                bw.Write((int)52);
                bw.Write((int)2);

                //Write loop 1
                bw.Write((int)0);
                bw.Write((int)0);
                bw.Write("data".ToCharArray());
                bw.Write((Int64)0);
                bw.Write((int)0);

                //Write loop 2
                bw.Write((int)1);
                bw.Write(lastSample);
                bw.Write("data".ToCharArray());
                bw.Write((Int64)0);
                bw.Write(lastSample);

                //Write List
                bw.Write("LIST".ToCharArray());
                bw.Write((int)46);
                bw.Write("adtl".ToCharArray());

                //Write list 1
                bw.Write("labl".ToCharArray());
                bw.Write("start_cue".Length + 5);
                bw.Write((int)0);
                bw.Write("start_cue".ToCharArray());
                bw.Write((byte)0);

                //Write list 2
                bw.Write("labl".ToCharArray());
                bw.Write("end_cue".Length + 5);
                bw.Write((int)1);
                bw.Write("end_cue".ToCharArray());
                bw.Write((byte)0);

                outputStream.Position = 4;
                int length = br.ReadInt32();
                length += 114;
                outputStream.Position = 4;
                bw.Write(length);

                outputStream.Position = 0;

                FileStream fs = File.Create(args[0].Substring(0, args[0].Length - 4) + ".wav");
                outputStream.CopyTo(fs);
                fs.Dispose();

            }
            catch (NotImplementedException)
            {
                Console.WriteLine("File is in use, check to see if it's being used in another program!");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }
    }
}


