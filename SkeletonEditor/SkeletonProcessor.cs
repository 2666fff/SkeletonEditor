using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SkeletonEditor.Core
{
    public static class SkeletonProcessor
    {
        public static int ProcessDirectory(string basePath, int xOffset, int yOffset, float scale, 
                                         string backupRoot, string boneName = null)
        {
            // 获取所有.skel文件
            var skelFiles = Directory.GetFiles(basePath, "*.skel", SearchOption.AllDirectories);
            
            // 如果有指定骨骼名称，则进行过滤
            if (!string.IsNullOrWhiteSpace(boneName))
            {
                // 支持带或不带扩展名的匹配
                skelFiles = skelFiles.Where(f => 
                    Path.GetFileName(f).Equals(boneName, StringComparison.OrdinalIgnoreCase) ||
                    Path.GetFileNameWithoutExtension(f).Equals(boneName, StringComparison.OrdinalIgnoreCase)
                ).ToArray();
            }
            
            int count = 0;
            foreach (var file in skelFiles)
            {
                ProcessFile(file, xOffset, yOffset, scale, backupRoot, basePath);
                count++;
            }
            return count;
        }

        public static void ProcessFile(string filePath, int xOffsetAdd, int yOffsetAdd, float scaleAdd, 
                                      string backupRoot, string baseDir = null)
        {
            string relativePath = baseDir == null ? Path.GetFileName(filePath) : Path.GetRelativePath(baseDir, filePath);
            string backupPath = Path.Combine(backupRoot, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(backupPath));

            if (File.Exists(backupPath))
                File.Delete(backupPath);
            File.Copy(filePath, backupPath);

            byte[] fileBytes = File.ReadAllBytes(filePath);

            int rootIndex = FindRootIndex(fileBytes);
            if (rootIndex == -1)
                throw new Exception("未找到 root 骨骼标识");

            int dataStartIndex = rootIndex + 8;
            if (dataStartIndex + 16 > fileBytes.Length)
                throw new Exception("数据不足，无法读取偏移和缩放信息");

            float xOffset = ReadBigEndianFloat(fileBytes, dataStartIndex);
            float yOffset = ReadBigEndianFloat(fileBytes, dataStartIndex + 4);
            float scaleX = ReadBigEndianFloat(fileBytes, dataStartIndex + 8);
            float scaleY = ReadBigEndianFloat(fileBytes, dataStartIndex + 12);

            float newX = xOffset + xOffsetAdd;
            float newY = yOffset + yOffsetAdd;
            float newSX = scaleX * scaleAdd;
            float newSY = scaleY * scaleAdd;

            WriteBigEndianFloat(fileBytes, dataStartIndex, newX);
            WriteBigEndianFloat(fileBytes, dataStartIndex + 4, newY);
            WriteBigEndianFloat(fileBytes, dataStartIndex + 8, newSX);
            WriteBigEndianFloat(fileBytes, dataStartIndex + 12, newSY);

            File.WriteAllBytes(filePath, fileBytes);
        }

        public static BoneInfo GetBoneInfo(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("文件不存在");
    
            if (!filePath.EndsWith(".skel", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("仅支持.skel文件");

            byte[] fileBytes = File.ReadAllBytes(filePath);

            int rootIndex = FindRootIndex(fileBytes);
            if (rootIndex == -1)
                throw new Exception("未找到 root 骨骼标识");

            int dataStartIndex = rootIndex + 8;
            if (dataStartIndex + 16 > fileBytes.Length)
                throw new Exception("数据不足，无法读取偏移和缩放信息");

            return new BoneInfo
            {
                XOffset = ReadBigEndianFloat(fileBytes, dataStartIndex),
                YOffset = ReadBigEndianFloat(fileBytes, dataStartIndex + 4),
                ScaleX = ReadBigEndianFloat(fileBytes, dataStartIndex + 8),
                ScaleY = ReadBigEndianFloat(fileBytes, dataStartIndex + 12) // 确保读取Y缩放值
            };
        }

        private static int FindRootIndex(byte[] bytes)
        {
            byte[] root = { 0x72, 0x6F, 0x6F, 0x74 }; // "root"
            for (int i = 0; i <= bytes.Length - root.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < root.Length; j++)
                {
                    if (bytes[i + j] != root[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return i;
            }
            return -1;
        }

        private static float ReadBigEndianFloat(byte[] bytes, int index)
        {
            return BitConverter.ToSingle(new[] {
                bytes[index + 3], bytes[index + 2], bytes[index + 1], bytes[index]
            }, 0);
        }

        private static void WriteBigEndianFloat(byte[] bytes, int index, float value)
        {
            var data = BitConverter.GetBytes(value);
            bytes[index] = data[3];
            bytes[index + 1] = data[2];
            bytes[index + 2] = data[1];
            bytes[index + 3] = data[0];
        }
        
        public class BoneInfo
        {
            public float XOffset { get; set; }
            public float YOffset { get; set; }
            public float ScaleX { get; set; }
            public float ScaleY { get; set; }
        }
    }
}