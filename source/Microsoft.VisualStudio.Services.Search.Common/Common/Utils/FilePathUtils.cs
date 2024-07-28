// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.FilePathUtils
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
{
  public static class FilePathUtils
  {
    public static readonly char DirectorySeparatorChar = '\\';
    public static readonly char AltDirectorySeparatorChar = '/';
    public static readonly char VolumeSeparatorChar = ':';

    public static string GetFileExtension(string filePath)
    {
      try
      {
        return Path.GetExtension(filePath);
      }
      catch (ArgumentException ex)
      {
        return FilePathUtils.ComputeFileExtension(filePath);
      }
    }

    public static string GetFileName(string filePath)
    {
      try
      {
        return Path.GetFileName(filePath);
      }
      catch (ArgumentException ex)
      {
        return FilePathUtils.ComputeFileName(filePath);
      }
    }

    internal static string ComputeFileExtension(string filePath)
    {
      string fileName = FilePathUtils.GetFileName(filePath);
      if (string.IsNullOrEmpty(fileName))
        return fileName;
      int startIndex = fileName.LastIndexOf(".", StringComparison.Ordinal);
      return 0 > startIndex || startIndex >= fileName.Length - 1 ? string.Empty : fileName.Substring(startIndex);
    }

    internal static string ComputeFileName(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath))
        return filePath;
      int num1 = filePath.LastIndexOf("/", StringComparison.Ordinal);
      if (num1 >= 0)
        return filePath.Substring(num1 + 1);
      int num2 = filePath.LastIndexOf("\\", StringComparison.Ordinal);
      return num2 < 0 ? filePath : filePath.Substring(num2 + 1);
    }

    public static bool HasExtension(string path)
    {
      if (path != null)
      {
        int length = path.Length;
        while (--length >= 0)
        {
          char ch = path[length];
          if (ch == '.')
            return length != path.Length - 1;
          if ((int) ch == (int) FilePathUtils.DirectorySeparatorChar || (int) ch == (int) FilePathUtils.AltDirectorySeparatorChar || (int) ch == (int) FilePathUtils.VolumeSeparatorChar)
            break;
        }
      }
      return false;
    }

    public static string GetFileNameWithoutExtension(string path)
    {
      path = FilePathUtils.GetFileName(path);
      if (path == null)
        return (string) null;
      int length;
      return (length = path.LastIndexOf('.')) == -1 ? path : path.Substring(0, length);
    }
  }
}
