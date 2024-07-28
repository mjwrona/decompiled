// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.FileNamePathHelper
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  internal static class FileNamePathHelper
  {
    internal const int MaximumFileNameAllowed = 256;
    internal const string JsonFileExtension = ".json";
    private const string JsFileExtension = ".js";
    private static readonly char[] SortedInvalidFileChars = Path.GetInvalidFileNameChars();

    static FileNamePathHelper() => Array.Sort<char>(FileNamePathHelper.SortedInvalidFileChars);

    internal static string ConstructValidFileName(
      string monitoringAccount,
      string metricNamespace,
      string metric,
      string monitorId,
      string fileExtension,
      int maximumFileNameAllowed)
    {
      StringBuilder builder = new StringBuilder(maximumFileNameAllowed);
      builder.Append(monitoringAccount);
      if (!string.IsNullOrWhiteSpace(metricNamespace))
        builder.Append('_').Append(metricNamespace);
      if (!string.IsNullOrWhiteSpace(metric))
        builder.Append('_').Append(metric);
      if (!string.IsNullOrWhiteSpace(monitorId))
        builder.Append('_').Append(monitorId);
      string str1 = builder.ToString();
      int num = builder.Length + fileExtension.Length > maximumFileNameAllowed ? 1 : 0;
      if (num != 0)
      {
        builder.Clear();
        int totalAllowedLength = maximumFileNameAllowed - fileExtension.Length - 16 - 1;
        int partsRemaining = 1;
        if (!string.IsNullOrWhiteSpace(metricNamespace))
          ++partsRemaining;
        if (!string.IsNullOrWhiteSpace(metric))
          ++partsRemaining;
        if (!string.IsNullOrWhiteSpace(monitorId))
          ++partsRemaining;
        FileNamePathHelper.AppendShortedFilePart(builder, monitoringAccount, totalAllowedLength, ref partsRemaining);
        FileNamePathHelper.AppendShortedFilePart(builder, metricNamespace, totalAllowedLength, ref partsRemaining);
        FileNamePathHelper.AppendShortedFilePart(builder, metric, totalAllowedLength, ref partsRemaining);
        FileNamePathHelper.AppendShortedFilePart(builder, monitorId, totalAllowedLength, ref partsRemaining);
      }
      FileNamePathHelper.RepalceInvalidFileChars(builder);
      if (num == 0)
        return builder.Append(fileExtension).ToString();
      string str2 = builder.ToString();
      builder.Clear();
      using (SHA1 shA1 = SHA1.Create())
      {
        byte[] hash = shA1.ComputeHash(Encoding.UTF8.GetBytes(str1.ToLowerInvariant()));
        for (int index = 0; index < hash.Length; ++index)
        {
          if (index < 8)
            builder.Append(hash[index].ToString("x2"));
          else
            break;
        }
      }
      return str2 + "_" + builder?.ToString() + fileExtension;
    }

    internal static string ConstructValidFileName(
      string monitoringAccount,
      int maximumFileNameAllowed)
    {
      return FileNamePathHelper.ConstructValidFileName(monitoringAccount, string.Empty, string.Empty, string.Empty, ".json", maximumFileNameAllowed);
    }

    internal static string ConvertPathToValidFolderName(string path)
    {
      StringBuilder builder = new StringBuilder(path);
      FileNamePathHelper.RepalceInvalidFileChars(builder);
      return builder.ToString();
    }

    private static void AppendShortedFilePart(
      StringBuilder builder,
      string value,
      int totalAllowedLength,
      ref int partsRemaining)
    {
      if (string.IsNullOrWhiteSpace(value))
        return;
      int num = (totalAllowedLength - builder.Length) / partsRemaining;
      --partsRemaining;
      if (value.Length > num)
      {
        int length = (num - 1) / 2;
        builder.AppendFormat("{0}{1}~{2}", builder.Length == 0 ? (object) string.Empty : (object) "_", (object) value.Substring(0, length), (object) value.Substring(value.Length - length));
      }
      else
        builder.AppendFormat("{0}{1}", builder.Length == 0 ? (object) string.Empty : (object) "_", (object) value);
    }

    private static void RepalceInvalidFileChars(StringBuilder builder)
    {
      for (int index = 0; index < builder.Length; ++index)
      {
        if (!char.IsLetter(builder[index]) && !char.IsDigit(builder[index]) && Array.BinarySearch<char>(FileNamePathHelper.SortedInvalidFileChars, builder[index]) >= 0)
          builder[index] = '^';
      }
    }
  }
}
