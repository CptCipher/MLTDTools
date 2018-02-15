﻿using System;
using System.IO;
using OpenMLTD.AllStarsTheater;
using OpenMLTD.AllStarsTheater.Database;

namespace ManifestExport {
    internal static class Program {

        private static void Main(string[] args) {
            if (args.Length < 1) {
                PrintUsage();
                return;
            }

            var data = File.ReadAllBytes(args[0]);

            var b = AssetInfoList.TryParse(data, MltdConstants.Utf8WithoutBom, out var assetInfoList);

            if (!b || assetInfoList == null) {
                Console.WriteLine("Manifest parsing failed.");
                return;
            }

            string outputFilePath;

            if (args.Length > 1) {
                outputFilePath = args[1];
            } else {
                var fileInfo = new FileInfo(args[0]);
                outputFilePath = fileInfo.FullName + ".txt";
            }

            using (var writer = new StreamWriter(outputFilePath, false, MltdConstants.Utf8WithoutBom)) {
                writer.WriteLine("Asset count: {0}", assetInfoList.Assets.Count);

                foreach (var asset in assetInfoList.Assets) {
                    writer.WriteLine();
                    writer.WriteLine("Resource name: {0}", asset.ResourceName);
                    writer.WriteLine("Resource hash: {0}", asset.ContentHash);
                    writer.WriteLine("Remote name: {0}", asset.RemoteName);
                    writer.WriteLine("File size: {0} ({1})", asset.Size, BytesToString(asset.Size));
                }
            }
        }

        private static void PrintUsage() {
            Console.WriteLine("Usage: ManifestExport <input manifest> [<output txt>]");
        }

        private static string BytesToString(long byteCount) {
            if (byteCount == 0) {
                return "0 B";
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(byteCount) * num).ToString("0.#") + SizeSuffixes[place];
        }

        private static readonly string[] SizeSuffixes = { " B", " KB", " MB", " GB", " TB", " PB", " EB" };

    }
}
