// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.FileSpec
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class FileSpec
  {
    private static FileSpecBottomUpComparer s_bottomUpComparer;
    private static FileSpecTopDownComparer s_topDownComparer;
    public static readonly char[] IllegalNtfsChars = new char[39]
    {
      char.MinValue,
      '\u0001',
      '\u0002',
      '\u0003',
      '\u0004',
      '\u0005',
      '\u0006',
      '\a',
      '\b',
      '\t',
      '\n',
      '\v',
      '\f',
      '\r',
      '\u000E',
      '\u000F',
      '\u0010',
      '\u0011',
      '\u0012',
      '\u0013',
      '\u0014',
      '\u0015',
      '\u0016',
      '\u0017',
      '\u0018',
      '\u0019',
      '\u001A',
      '\u001B',
      '\u001C',
      '\u001D',
      '\u001E',
      '\u001F',
      '"',
      '/',
      ':',
      '<',
      '>',
      '\\',
      '|'
    };
    public static readonly char[] IllegalNtfsCharsAndWildcards = new char[41]
    {
      char.MinValue,
      '\u0001',
      '\u0002',
      '\u0003',
      '\u0004',
      '\u0005',
      '\u0006',
      '\a',
      '\b',
      '\t',
      '\n',
      '\v',
      '\f',
      '\r',
      '\u000E',
      '\u000F',
      '\u0010',
      '\u0011',
      '\u0012',
      '\u0013',
      '\u0014',
      '\u0015',
      '\u0016',
      '\u0017',
      '\u0018',
      '\u0019',
      '\u001A',
      '\u001B',
      '\u001C',
      '\u001D',
      '\u001E',
      '\u001F',
      '"',
      '/',
      ':',
      '<',
      '>',
      '\\',
      '|',
      '*',
      '?'
    };
    public static readonly bool[] ValidNtfsCharTable = new bool[128]
    {
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      false,
      true,
      true,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      false,
      true,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      true,
      true
    };
    public static readonly string[] ReservedNames = new string[22]
    {
      "CON",
      "PRN",
      "AUX",
      "NUL",
      "COM1",
      "COM2",
      "COM3",
      "COM4",
      "COM5",
      "COM6",
      "COM7",
      "COM8",
      "COM9",
      "LPT1",
      "LPT2",
      "LPT3",
      "LPT4",
      "LPT5",
      "LPT6",
      "LPT7",
      "LPT8",
      "LPT9"
    };
    internal static readonly string[] reservedNamesLength3 = new string[4]
    {
      "CON",
      "PRN",
      "AUX",
      "NUL"
    };
    private static readonly char[] s_invalidPathChars = Path.GetInvalidPathChars();
    private static readonly char[] s_versionControlSeparators = new char[1]
    {
      ';'
    };
    [ThreadStatic]
    private static Random s_random;
    private static string s_tempDirectory;

    public static string GetFullPath(string path) => FileSpec.GetFullPath(path, false);

    public static string GetFullPath(string path, bool checkForIllegalDollar)
    {
      if (path.Length >= 2)
      {
        char ch = path[path.Length - 1];
        if (((int) ch == (int) Path.DirectorySeparatorChar || (int) ch == (int) Path.AltDirectorySeparatorChar) && (int) path[path.Length - 2] != (int) Path.VolumeSeparatorChar)
          path = path.Substring(0, path.Length - 1);
      }
      string fileName = Path.GetFileName(path);
      string path1;
      if (!Wildcard.IsWildcard(fileName))
      {
        path1 = FileSpec.GetFullPathWrapper(path);
      }
      else
      {
        string path2 = Path.GetDirectoryName(path);
        switch (path2)
        {
          case null:
            path1 = FileSpec.GetFullPathWrapper(path);
            goto label_9;
          case "":
            path2 = ".";
            break;
        }
        path1 = Path.Combine(FileSpec.GetFullPathWrapper(path2), fileName);
      }
label_9:
      if (checkForIllegalDollar)
        FileSpec.CheckForIllegalDollarInPath(path1);
      return path1;
    }

    public static void CheckForIllegalDollarInPath(string path)
    {
      if (path == null)
        return;
      int length = path.Length;
      for (int index = 1; index < length; ++index)
      {
        if (path[index] == '$' && ((int) path[index - 1] == (int) Path.DirectorySeparatorChar || (int) path[index - 1] == (int) Path.AltDirectorySeparatorChar))
          throw new InvalidPathException(TFCommonResources.InvalidPathDollarSign((object) path));
      }
    }

    public static string Combine(string parent, string relative) => FileSpec.GetFullPath(FileSpec.UncanonicalizedCombine(parent, relative));

    public static string UncanonicalizedCombine(string parent, string relative) => relative.Length > 0 && ((int) relative[0] == (int) Path.DirectorySeparatorChar || (int) relative[0] == (int) Path.AltDirectorySeparatorChar) && (relative.Length == 1 || (int) relative[1] != (int) Path.DirectorySeparatorChar && (int) relative[1] != (int) Path.AltDirectorySeparatorChar) ? Path.Combine(Path.GetPathRoot(parent), relative.Substring(1)) : Path.Combine(parent, relative);

    public static string[] SplitPath(string path)
    {
      List<string> stringList = new List<string>();
      string folder = path;
      while (!string.IsNullOrEmpty(folder))
      {
        string fileName;
        FileSpec.Parse(folder, out folder, out fileName);
        if (!string.IsNullOrEmpty(fileName))
        {
          stringList.Insert(0, fileName);
        }
        else
        {
          stringList.Insert(0, folder.TrimEnd(Path.DirectorySeparatorChar));
          break;
        }
      }
      return stringList.ToArray();
    }

    public static void Parse(string path, out string folder, out string fileName)
    {
      int folderEnd;
      int fileNameStart;
      FileSpec.FindFileNameIndex(path, out folderEnd, out fileNameStart);
      folder = path.Substring(0, Math.Max(folderEnd, 0));
      fileName = path.Substring(fileNameStart);
    }

    private static void FindFileNameIndex(string path, out int folderEnd, out int fileNameStart)
    {
      folderEnd = path.LastIndexOf(Path.DirectorySeparatorChar);
      if (path.StartsWith("\\\\", StringComparison.Ordinal) && path.LastIndexOf(Path.DirectorySeparatorChar, folderEnd - 1) < 2)
        folderEnd = path.Length;
      fileNameStart = Math.Min(path.Length, folderEnd + 1);
      if (folderEnd != 2 || (int) path[1] != (int) Path.VolumeSeparatorChar)
        return;
      folderEnd = 3;
    }

    public static string GetDirectoryName(string path)
    {
      int folderEnd;
      FileSpec.FindFileNameIndex(path, out folderEnd, out int _);
      return path.Substring(0, folderEnd);
    }

    [Conditional("DEBUG")]
    private static void AssertPathCanonicalized(string parameter, string value)
    {
      try
      {
        FileSpec.GetFullPath(value, false);
      }
      catch (Exception ex)
      {
      }
    }

    public static string GetFileName(string path)
    {
      int fileNameStart;
      FileSpec.FindFileNameIndex(path, out int _, out fileNameStart);
      return path.Substring(fileNameStart);
    }

    public static int GetFolderDepth(string item)
    {
      if (string.IsNullOrEmpty(item))
        return 0;
      int num = 0;
      int length = item.Length;
      for (int index = 0; index < length; ++index)
      {
        if ((int) item[index] == (int) Path.DirectorySeparatorChar)
          ++num;
      }
      if (item.StartsWith("\\\\", StringComparison.Ordinal))
        num -= 3;
      else if (item.Length == 3 && (int) item[1] == (int) Path.VolumeSeparatorChar && (int) item[2] == (int) Path.DirectorySeparatorChar)
        --num;
      return num < 0 ? 0 : num;
    }

    public static string MakeRelative(string path, string folder)
    {
      int relativeStartIndex = FileSpec.GetRelativeStartIndex(path, folder);
      if (relativeStartIndex < 0)
        return string.Empty;
      return relativeStartIndex == 0 ? path : path.Substring(relativeStartIndex);
    }

    public static int GetRelativeStartIndex(string path, string folder)
    {
      if (path.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
      {
        if (path.Length == folder.Length)
          return -1;
        if (folder.Length > 0 && folder[folder.Length - 1] == '\\')
          return folder.Length;
        if (path[folder.Length] == '\\')
          return folder.Length + 1;
      }
      return 0;
    }

    public static string MakeRelative(string item, string folder, bool recursive)
    {
      string str = FileSpec.MakeRelative(item, folder);
      if (!recursive || !FileSpec.Equals(str, item))
        return str;
      string empty = string.Empty;
      while (FileSpec.Equals(str, item))
      {
        string directoryName = FileSpec.GetDirectoryName(folder);
        if (folder == directoryName)
          return item;
        folder = directoryName;
        str = FileSpec.MakeRelative(item, folder);
        empty += "..\\";
      }
      return empty + str;
    }

    public static bool Equals(string item1, string item2) => (object) item1 == (object) item2 || string.Equals(item1, item2, StringComparison.OrdinalIgnoreCase);

    public static int Compare(string item1, string item2) => FileSpec.Compare(item1, item2, item1.Length, item2.Length, '\\', StringComparison.OrdinalIgnoreCase);

    public static int Compare(string item1, string item2, int item1Length, int item2Length) => FileSpec.Compare(item1, item2, item1Length, item2Length, '\\', StringComparison.OrdinalIgnoreCase);

    public static int CompareUI(string item1, string item2) => FileSpec.Compare(item1, item2, item1.Length, item2.Length, '\\', StringComparison.CurrentCultureIgnoreCase);

    public static int CompareUI(string item1, string item2, int item1Length, int item2Length) => FileSpec.Compare(item1, item2, item1Length, item2Length, '\\', StringComparison.CurrentCultureIgnoreCase);

    public static int CompareCaseSensitive(string item1, string item2)
    {
      int num = FileSpec.Compare(FileSpec.GetDirectoryName(item1), FileSpec.GetDirectoryName(item2), '\\', StringComparison.OrdinalIgnoreCase);
      return num == 0 ? string.CompareOrdinal(FileSpec.GetFileName(item1), FileSpec.GetFileName(item2)) : num;
    }

    public static bool EqualsCaseSensitive(string item1, string item2) => FileSpec.CompareCaseSensitive(item1, item2) == 0;

    public static int Compare(string item1, string item2, char slash, StringComparison comparison) => FileSpec.Compare(item1, item2, item1.Length, item2.Length, slash, comparison);

    public static int Compare(
      string item1,
      string item2,
      int item1Length,
      int item2Length,
      char slash,
      StringComparison comparison)
    {
      return (object) item1 == (object) item2 ? 0 : FileSpec.Compare(item1, item2, item1Length, item2Length, out int _, out int _, slash, comparison);
    }

    public static int Compare(
      string item1,
      string item2,
      int item1Length,
      int item2Length,
      out int item1ComparedLength,
      out int item2ComparedLength,
      char slash,
      StringComparison comparison)
    {
      CompareOptions options = CompareOptions.None;
      CompareInfo compareInfo;
      switch (comparison)
      {
        case StringComparison.CurrentCulture:
          compareInfo = CultureInfo.CurrentCulture.CompareInfo;
          break;
        case StringComparison.CurrentCultureIgnoreCase:
          compareInfo = CultureInfo.CurrentCulture.CompareInfo;
          options = CompareOptions.IgnoreCase;
          break;
        case StringComparison.InvariantCulture:
          compareInfo = CultureInfo.InvariantCulture.CompareInfo;
          break;
        case StringComparison.InvariantCultureIgnoreCase:
          compareInfo = CultureInfo.InvariantCulture.CompareInfo;
          options = CompareOptions.IgnoreCase;
          break;
        case StringComparison.Ordinal:
          compareInfo = CultureInfo.InvariantCulture.CompareInfo;
          options = CompareOptions.Ordinal;
          break;
        case StringComparison.OrdinalIgnoreCase:
          compareInfo = CultureInfo.InvariantCulture.CompareInfo;
          options = CompareOptions.OrdinalIgnoreCase;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (comparison));
      }
      int num1 = 0;
      int num2 = 0;
      try
      {
        int num3 = 0;
        int num4;
        for (num4 = 0; num3 < item1Length && num4 < item2Length; num4 = num2 + 1)
        {
          num1 = item1.IndexOf(slash, num3, item1Length - num3);
          num2 = item2.IndexOf(slash, num4, item2Length - num4);
          if (num1 == -1)
            num1 = item1Length;
          if (num2 == -1)
            num2 = item2Length;
          int num5 = compareInfo.Compare(item1, num3, num1 - num3, item2, num4, num2 - num4, options);
          if (num5 != 0)
            return num5;
          num3 = num1 + 1;
        }
        if (num3 < item1Length)
          return 1;
        return num4 < item2Length ? -1 : 0;
      }
      finally
      {
        item1ComparedLength = num1;
        item2ComparedLength = num2;
      }
    }

    public static StringComparer FullPathComparer => StringComparer.OrdinalIgnoreCase;

    public static int FullPathDescendingComparison(string x, string y) => -FileSpec.FullPathComparer.Compare(x, y);

    public static StringComparer PartialPathComparer => StringComparer.OrdinalIgnoreCase;

    public static int CompareTopDown(string item1, string item2) => FileSpec.CompareTopDown(item1, item2, '\\', StringComparison.OrdinalIgnoreCase);

    public static int CompareTopDownUI(string item1, string item2) => FileSpec.CompareTopDown(item1, item2, '\\', StringComparison.CurrentCultureIgnoreCase);

    public static int CompareTopDown(
      string item1,
      string item2,
      char slash,
      StringComparison comparison)
    {
      if ((object) item1 == (object) item2)
        return 0;
      int length1 = item1.Length;
      int length2 = item2.Length;
      int item1ComparedLength;
      int item2ComparedLength;
      int num = FileSpec.Compare(item1, item2, length1, length2, out item1ComparedLength, out item2ComparedLength, slash, comparison);
      if (num < 0)
      {
        if (length1 > item1ComparedLength && length2 == item2ComparedLength)
          num = 1;
      }
      else if (num > 0)
      {
        if (length1 == item1ComparedLength && length2 > item2ComparedLength)
          num = -1;
      }
      else if (num == 0)
        num = length1 - length2;
      return num;
    }

    public static int CompareBottomUp(string item1, string item2) => FileSpec.CompareBottomUp(item1, item2, '\\', StringComparison.OrdinalIgnoreCase);

    public static int CompareBottomUpUI(string item1, string item2) => FileSpec.CompareBottomUp(item1, item2, '\\', StringComparison.CurrentCultureIgnoreCase);

    public static int CompareBottomUp(
      string item1,
      string item2,
      char slash,
      StringComparison comparison)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      for (int index = 0; index < item1.Length; ++index)
      {
        if ((int) item1[index] == (int) slash)
        {
          ++num1;
          num3 = index;
        }
      }
      for (int index = 0; index < item2.Length; ++index)
      {
        if ((int) item2[index] == (int) slash)
        {
          ++num2;
          num4 = index;
        }
      }
      if (num1 > num2)
      {
        if (string.Compare(item1, 0, item2, 0, num4, comparison) != 0)
          return FileSpec.Compare(item1, item2, slash, comparison);
        return (int) item1[num4] == (int) slash ? -1 : 1;
      }
      if (num2 > num1)
      {
        if (string.Compare(item1, 0, item2, 0, num3, comparison) != 0)
          return FileSpec.Compare(item1, item2, slash, comparison);
        return (int) item2[num3] == (int) slash ? 1 : -1;
      }
      if (num1 == 1)
      {
        if ((int) item1[item1.Length - 1] == (int) slash)
          return (int) item2[item2.Length - 1] == (int) slash ? 0 : 1;
        if ((int) item2[item2.Length - 1] == (int) slash)
          return -1;
      }
      return FileSpec.Compare(item1, item2, slash, comparison);
    }

    public static FileSpecBottomUpComparer BottomUpComparer
    {
      get
      {
        if (FileSpec.s_bottomUpComparer == null)
          FileSpec.s_bottomUpComparer = new FileSpecBottomUpComparer();
        return FileSpec.s_bottomUpComparer;
      }
    }

    public static FileSpecTopDownComparer TopDownComparer
    {
      get
      {
        if (FileSpec.s_topDownComparer == null)
          FileSpec.s_topDownComparer = new FileSpecTopDownComparer();
        return FileSpec.s_topDownComparer;
      }
    }

    public static bool IsSubItem(string item, string parent)
    {
      if (!item.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
        return false;
      return item.Length == parent.Length || parent.Length > 0 && parent[parent.Length - 1] == '\\' || item[parent.Length] == '\\';
    }

    public static bool IsImmediateChild(string item, string parent)
    {
      if (!FileSpec.IsSubItem(item, parent) || item.Length <= parent.Length)
        return false;
      int startIndex = item.Length - 1;
      if ('\\' == item[item.Length - 1] && item.Length > 1)
        --startIndex;
      int num = item.LastIndexOf('\\', startIndex);
      return num < 0 || num == parent.Length - 1 || num == parent.Length;
    }

    public static bool IsWildcard(string path)
    {
      int num = Math.Max(path.LastIndexOf('\\'), path.LastIndexOf('/'));
      return num != path.Length - 1 && Wildcard.IsWildcard(path, num + 1, path.Length - (num + 1));
    }

    public static bool Match(string item, string matchFolder, string matchPattern, bool recursive)
    {
      string folder;
      string fileName;
      if (matchPattern == null || matchPattern.Length == 0)
      {
        folder = item;
        fileName = (string) null;
      }
      else
        FileSpec.Parse(item, out folder, out fileName);
      if (recursive)
      {
        if (!FileSpec.IsSubItem(folder, matchFolder))
          return false;
      }
      else if (FileSpec.Compare(matchFolder, folder) != 0)
        return false;
      return fileName == null || FileSpec.Match(fileName, matchPattern);
    }

    public static bool Match(string itemName, string matchPattern) => FileSpec.Match(itemName, 0, matchPattern);

    internal static bool Match(string itemName, int itemIndex, string matchPattern) => Wildcard.Match(itemName, itemIndex, matchPattern);

    public static bool MatchFileName(string item, string matchPattern) => FileSpec.Match(FileSpec.GetFileName(item), matchPattern);

    public static string RemoveInvalidFileNameChars(string fileName)
    {
      int startIndex = 0;
      int num;
      if ((num = fileName.IndexOfAny(FileSpec.IllegalNtfsCharsAndWildcards)) < 0)
        return fileName;
      StringBuilder stringBuilder = new StringBuilder();
      for (; num >= 0; num = fileName.IndexOfAny(FileSpec.IllegalNtfsCharsAndWildcards, startIndex))
      {
        if (num > startIndex)
          stringBuilder.Append(fileName.Substring(startIndex, num - startIndex));
        startIndex = num + 1;
      }
      if (startIndex < fileName.Length)
        stringBuilder.Append(fileName.Substring(startIndex));
      return stringBuilder.ToString();
    }

    public static string GetTempFileName()
    {
      string prefix = "v" + DateTime.UtcNow.Second.ToString();
      int num = 0;
      while (true)
      {
        try
        {
          return Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetTempFileName(FileSpec.GetTempDirectory(), prefix, 0U);
        }
        catch (IOException ex)
        {
          if (num < 3)
            Thread.Sleep(10);
          else
            throw;
        }
        ++num;
      }
    }

    public static string GetTempFileNameWithExtension(string extension)
    {
      if (!string.IsNullOrEmpty(extension) && extension[0] != '.')
        extension = "." + extension;
      string tempDirectory = FileSpec.GetTempDirectory();
      string str = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetCurrentThreadId().ToString((IFormatProvider) CultureInfo.InvariantCulture) + "_";
      int num = 0;
      string path;
      do
      {
        path = Path.Combine(tempDirectory, "vctmp" + str + FileSpec.NextRandomNumber.ToString() + extension);
      }
      while (File.Exists(path) && ++num < 100);
      return !File.Exists(path) ? path : throw new IOException(TFCommonResources.ErrorCreatingTempFile((object) FileSpec.GetTempDirectory()));
    }

    public static string GetTempDirectory()
    {
      if (FileSpec.s_tempDirectory == null)
        FileSpec.s_tempDirectory = Path.Combine(Path.GetTempPath(), "TFSTemp");
      Directory.CreateDirectory(FileSpec.s_tempDirectory);
      return FileSpec.s_tempDirectory;
    }

    public static string CurrentDirectory
    {
      get
      {
        string path = Environment.CurrentDirectory;
        if (path.IndexOf('~') >= 0)
          path = Path.GetFullPath(path);
        return path;
      }
    }

    public static string GetCommonPathPrefix(string path1, string path2)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path1, nameof (path1));
      ArgumentUtility.CheckStringForNullOrEmpty(path2, nameof (path2));
      bool flag1 = path1.StartsWith("\\\\", StringComparison.Ordinal);
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (; num1 < path1.Length && num1 < path2.Length && (int) char.ToUpperInvariant(path1[num1]) == (int) char.ToUpperInvariant(path2[num1]); ++num1)
      {
        if ((int) path1[num1] == (int) Path.DirectorySeparatorChar)
        {
          num2 = num1;
          ++num3;
        }
      }
      if (num1 == 0 || flag1 && num3 < 3)
        return (string) null;
      int num4 = num1 == path1.Length ? 1 : ((int) path1[num1] == (int) Path.DirectorySeparatorChar ? 1 : 0);
      bool flag2 = num1 == path2.Length || (int) path2[num1] == (int) Path.DirectorySeparatorChar;
      if (num4 == 0 || !flag2)
      {
        num1 = num2;
        --num3;
      }
      if (num1 == 0 || flag1 && num3 < 3)
        return (string) null;
      string commonPathPrefix = path1.Substring(0, num1);
      if (2 == commonPathPrefix.Length && (int) commonPathPrefix[1] == (int) Path.VolumeSeparatorChar)
        commonPathPrefix += Path.DirectorySeparatorChar.ToString();
      return commonPathPrefix;
    }

    public static string GetFilesystemPathCasing(string path)
    {
      string directoryName = FileSpec.GetDirectoryName(path);
      if (!FileSpec.Equals(directoryName, path))
      {
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FIND_DATA lpFindFileData = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FIND_DATA();
        using (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SafeFindHandle firstFileEx = Microsoft.TeamFoundation.Common.Internal.NativeMethods.FindFirstFileEx(path, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FINDEX_INFO_LEVELS.FindExInfoStandard, ref lpFindFileData, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0U))
        {
          if (!firstFileEx.IsInvalid)
            path = Path.Combine(FileSpec.GetFilesystemPathCasing(directoryName), lpFindFileData.strFileName);
        }
      }
      return path;
    }

    public static bool IsReservedName(string name)
    {
      if (name.Length == 4 && char.IsDigit(name[3]) && name[3] != '0' && (name.StartsWith("LPT", StringComparison.OrdinalIgnoreCase) || name.StartsWith("COM", StringComparison.OrdinalIgnoreCase)))
        return true;
      return name.Length == 3 && Array.IndexOf<string>(FileSpec.reservedNamesLength3, name.ToUpperInvariant()) >= 0;
    }

    public static bool IsLegalNtfsName(string name, int maxLength) => FileSpec.IsLegalNtfsName(name, maxLength, false);

    public static bool IsLegalNtfsName(string name, int maxLength, bool permitWildcards) => FileSpec.IsLegalNtfsName(name, maxLength, permitWildcards, out string _);

    public static bool IsLegalNtfsName(
      string name,
      int maxLength,
      bool permitWildcards,
      out string error)
    {
      error = (string) null;
      if (string.IsNullOrEmpty(name))
      {
        error = TFCommonResources.InvalidPath((object) string.Empty);
        return false;
      }
      if (name.Length > maxLength)
      {
        error = TFCommonResources.InvalidPathTooLongVariable((object) name, (object) maxLength);
        return false;
      }
      if (name.IndexOfAny(permitWildcards ? FileSpec.IllegalNtfsChars : FileSpec.IllegalNtfsCharsAndWildcards) >= 0)
      {
        error = !permitWildcards ? TFCommonResources.InvalidPathInvalidCharactersAndWildcards((object) name) : TFCommonResources.InvalidPathInvalidCharacters((object) name);
        return false;
      }
      switch (name[name.Length - 1])
      {
        case ' ':
        case '.':
          error = TFCommonResources.InvalidPathTermination((object) name);
          return false;
        default:
          return true;
      }
    }

    public static bool IsValidNtfsChar(char c) => c > '\u007F' || FileSpec.ValidNtfsCharTable[(int) c];

    public static bool IsValidPathChar(char c) => Array.IndexOf<char>(FileSpec.s_invalidPathChars, c) < 0;

    public static void ValidatePath(string path)
    {
      foreach (char c in path)
      {
        if (Wildcard.IsWildcard(c))
          throw new InvalidPathException(TFCommonResources.WildcardsNotAllowed());
        if (!FileSpec.IsValidPathChar(c))
          throw new InvalidPathException(TFCommonResources.InvalidPathInvalidChar((object) path, (object) c));
      }
      FileSpec.GetFullPath(path);
    }

    public static bool IsEmptyDirectory(string path) => Directory.Exists(path) ? Microsoft.TeamFoundation.Common.Internal.NativeMethods.PathIsDirectoryEmpty(path) : throw new InvalidPathException(TFCommonResources.PathIsNotADirectory((object) path));

    public static void CopyFile(string oldPath, string newPath) => FileSpec.CopyFile(oldPath, newPath, false);

    public static void CopyFile(string oldPath, string newPath, bool overwriteExisting)
    {
      if (overwriteExisting && File.Exists(newPath))
        FileSpec.DeleteFile(newPath);
      Directory.CreateDirectory(FileSpec.GetDirectoryName(newPath));
      File.Copy(oldPath, newPath, overwriteExisting);
    }

    private static void ResetAttributes(string fileName)
    {
      System.IO.FileAttributes fileAttributes1 = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Archive;
      System.IO.FileAttributes attributes = File.GetAttributes(fileName);
      string directoryName = FileSpec.GetDirectoryName(fileName);
      System.IO.FileAttributes fileAttributes2 = File.GetAttributes(directoryName) & ~fileAttributes1 | attributes & fileAttributes1;
      if (attributes == fileAttributes2)
        return;
      if ((fileAttributes2 & System.IO.FileAttributes.Encrypted) == (System.IO.FileAttributes) 0 && (attributes & System.IO.FileAttributes.Encrypted) != (System.IO.FileAttributes) 0)
        File.Decrypt(fileName);
      else if ((fileAttributes2 & System.IO.FileAttributes.Encrypted) != (System.IO.FileAttributes) 0 && (attributes & System.IO.FileAttributes.Encrypted) == (System.IO.FileAttributes) 0)
      {
        File.Encrypt(fileName);
        attributes &= ~System.IO.FileAttributes.Compressed;
      }
      if ((fileAttributes2 & System.IO.FileAttributes.Compressed) == (System.IO.FileAttributes) 0 && (attributes & System.IO.FileAttributes.Compressed) != (System.IO.FileAttributes) 0)
        FileSpec.SetCompressionFormat(fileName, CompressionFormat.None);
      else if ((fileAttributes2 & System.IO.FileAttributes.Compressed) != (System.IO.FileAttributes) 0 && (attributes & System.IO.FileAttributes.Compressed) == (System.IO.FileAttributes) 0)
      {
        CompressionFormat compressionFormat = FileSpec.GetCompressionFormat(directoryName);
        FileSpec.SetCompressionFormat(fileName, compressionFormat);
      }
      if (fileAttributes2 == (System.IO.FileAttributes) 0)
        fileAttributes2 = System.IO.FileAttributes.Normal;
      File.SetAttributes(fileName, fileAttributes2);
    }

    private static void ResetFileDACL(string fileName)
    {
      FileSecurity fileSecurity = new FileSecurity(fileName, AccessControlSections.Access);
      foreach (AuthorizationRule accessRule in (ReadOnlyCollectionBase) fileSecurity.GetAccessRules(true, false, typeof (SecurityIdentifier)))
        fileSecurity.PurgeAccessRules(accessRule.IdentityReference);
      fileSecurity.SetAccessRuleProtection(false, true);
      new FileInfo(fileName).SetAccessControl(fileSecurity);
    }

    private static bool IsRetryableOperation(int errorCode)
    {
      if (errorCode <= 32)
      {
        if (errorCode != 5 && errorCode != 32)
          goto label_4;
      }
      else if (errorCode != 80 && errorCode != 82 && errorCode != 183)
        goto label_4;
      return true;
label_4:
      return false;
    }

    public static void MoveFile(string oldPath, string newPath) => FileSpec.MoveFile(oldPath, newPath, false);

    public static void MoveFile(string oldPath, string newPath, bool overwriteExisting)
    {
      int num = 0;
      Directory.CreateDirectory(FileSpec.GetDirectoryName(newPath));
      while (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.MoveFileEx(oldPath, newPath, 0))
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (lastWin32Error == 17)
        {
          if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.MoveFileEx(oldPath, newPath, 2))
            lastWin32Error = Marshal.GetLastWin32Error();
          else
            goto label_11;
        }
        if (lastWin32Error != 0)
        {
          if (overwriteExisting && FileSpec.IsRetryableOperation(lastWin32Error) && num < 5)
          {
            FileSpec.DeleteFile(newPath);
            if (num++ > 0)
              Thread.Sleep(3 * num * num * num * num);
          }
          else
          {
            File.Move(oldPath, newPath);
            goto label_11;
          }
        }
      }
      FileSpec.ResetFileDACL(newPath);
label_11:
      FileSpec.ResetAttributes(newPath);
    }

    public static void CreateHardLink(string oldName, string newName, bool overwriteExisting)
    {
      int num = 0;
      while (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateHardLink(newName, oldName, IntPtr.Zero))
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (3 == lastWin32Error)
        {
          string directoryName = FileSpec.GetDirectoryName(newName);
          if (uint.MaxValue != Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileAttributes(directoryName))
            break;
          Directory.CreateDirectory(directoryName);
        }
        else
        {
          if (!(183 == lastWin32Error & overwriteExisting) || num >= 5)
            throw new Win32Exception(lastWin32Error);
          FileSpec.DeleteFile(newName);
          if (num++ > 0)
            Thread.Sleep(3 * num * num * num * num);
        }
      }
    }

    public static string GetLongPathForm(string fileName) => fileName.StartsWith("\\\\") ? "\\\\?\\UNC\\" + fileName.Substring(2) : "\\\\?\\" + fileName;

    public static bool IsLongPathForm(string path) => path.StartsWith("\\\\?\\");

    public static void DeleteFile(string path) => FileSpec.DeleteFile(path, true);

    public static void DeleteFileWithoutException(string path) => FileSpec.DeleteFile(path, false);

    private static void DeleteFile(string path, bool throwExceptionOnFailure)
    {
      if (string.IsNullOrEmpty(path))
        return;
      path = FileSpec.NormalizePath(path, false);
      int num1 = 0;
      int num2 = 0;
      uint dwFileAttributes = uint.MaxValue;
label_3:
      while (true)
      {
        ++num1;
        if (num1 >= 3)
        {
          if (num1 <= 15)
          {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FILE_SPEC_OMIT_SLEEP")))
              Thread.Sleep(num1 * num1 * 6);
          }
          else
            break;
        }
        if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.DeleteFile(path))
        {
          num2 = Marshal.GetLastWin32Error();
          if (5 == num2)
          {
            uint fileAttributes = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileAttributes(path);
            if (uint.MaxValue != fileAttributes)
            {
              if (((int) fileAttributes & 1) != 0)
              {
                if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetFileAttributes(path, 128U))
                  dwFileAttributes = fileAttributes;
                else
                  goto label_18;
              }
              else
                goto label_21;
            }
            else
              goto label_15;
          }
          else
            goto label_21;
        }
        else
          goto label_1;
      }
      if (FileSpec.IsLongPathForm(path))
      {
        if (num2 == 5)
          throw new UnauthorizedAccessException(TFCommonResources.AccessToPathDenied((object) LongPathUtility.RemoveExtendedLengthPathPrefix(path)), (Exception) new Win32Exception());
        if (num2 == 32)
          throw new IOException(TFCommonResources.FileInUse((object) LongPathUtility.RemoveExtendedLengthPathPrefix(path)), (Exception) new Win32Exception());
        goto label_23;
      }
      else
        goto label_23;
label_1:
      return;
label_15:
      num2 = Marshal.GetLastWin32Error();
      goto label_21;
label_18:
      num2 = Marshal.GetLastWin32Error();
label_21:
      switch (num2)
      {
        case 2:
        case 3:
          return;
        case 5:
        case 32:
          goto label_3;
      }
label_23:
      if (throwExceptionOnFailure)
      {
        try
        {
          File.Delete(path);
        }
        catch (Exception ex)
        {
          if (uint.MaxValue != dwFileAttributes)
            Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetFileAttributes(path, dwFileAttributes);
          throw;
        }
      }
      else
      {
        if (uint.MaxValue == dwFileAttributes)
          return;
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetFileAttributes(path, dwFileAttributes);
      }
    }

    private static string NormalizePath(string path, bool throwIfTooLong = true)
    {
      if (!string.IsNullOrEmpty(path))
      {
        if (!FileSpec.IsLongPathForm(path))
        {
          try
          {
            path = Path.GetFullPath(path);
          }
          catch (PathTooLongException ex)
          {
            if (throwIfTooLong)
              throw;
            else
              path = FileSpec.GetLongPathForm(path);
          }
        }
      }
      return path;
    }

    public static void DeleteDirectory(string path) => FileSpec.DeleteDirectory(path, false);

    public static void DeleteDirectory(string path, bool recursive) => FileSpec.DeleteDirectory(path, recursive, false);

    public static void DeleteDirectory(string path, bool recursive, bool followJunctionPoints) => FileSpec.DeleteDirectoryLongPath(path, recursive, followJunctionPoints);

    private static void DeleteDirectoryLongPath(
      string path,
      bool recursive,
      bool followJunctionPoints)
    {
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FIND_DATA wiN32FindData = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FIND_DATA();
      path = FileSpec.IsLongPathForm(path) ? path : FileSpec.GetLongPathForm(path);
      using (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SafeFindHandle firstFile = Microsoft.TeamFoundation.Common.Internal.NativeMethods.FindFirstFile(path, ref wiN32FindData))
      {
        if (firstFile.IsInvalid)
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          switch (lastWin32Error)
          {
            case 2:
              return;
            case 3:
              return;
            default:
              throw new Win32Exception(lastWin32Error);
          }
        }
        else if (((int) wiN32FindData.dwFileAttributes & 16) == 0)
          throw new Win32Exception(267);
      }
      if (((int) wiN32FindData.dwFileAttributes & 1) != 0)
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetFileAttributes(path, wiN32FindData.dwFileAttributes & 4294967294U);
      if (followJunctionPoints || ((int) wiN32FindData.dwFileAttributes & 1024) == 0 || ((int) wiN32FindData.dwReserved0 & 536870912) == 0)
      {
        using (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SafeFindHandle firstFile = Microsoft.TeamFoundation.Common.Internal.NativeMethods.FindFirstFile(path.EndsWith("\\") ? path + "*" : path + "\\*", ref wiN32FindData))
        {
          if (firstFile.IsInvalid)
          {
            int lastWin32Error = Marshal.GetLastWin32Error();
            switch (lastWin32Error)
            {
              case 0:
                return;
              case 2:
                return;
              case 3:
                return;
              default:
                throw new Win32Exception(lastWin32Error);
            }
          }
          else
          {
            do
            {
              if (!string.Equals(wiN32FindData.strFileName, ".", StringComparison.Ordinal) && !string.Equals(wiN32FindData.strFileName, "..", StringComparison.Ordinal))
              {
                string path1 = path + "\\" + wiN32FindData.strFileName;
                if (((int) wiN32FindData.dwFileAttributes & 16) != 0)
                {
                  if (!recursive)
                    throw new IOException(TFCommonResources.CannotDeleteDirectoryWithContents((object) path1));
                  FileSpec.DeleteDirectoryLongPath(path1, recursive, followJunctionPoints);
                }
                else
                  FileSpec.DeleteFile(path1);
              }
            }
            while (Microsoft.TeamFoundation.Common.Internal.NativeMethods.FindNextFile(firstFile, ref wiN32FindData));
          }
        }
        int lastWin32Error1 = Marshal.GetLastWin32Error();
        if (18 != lastWin32Error1)
          throw new Win32Exception(lastWin32Error1);
      }
      if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.RemoveDirectory(path))
        return;
      switch (Marshal.GetLastWin32Error())
      {
        case 2:
        case 3:
          break;
        case 5:
          throw new IOException(TFCommonResources.AccessToPathDenied((object) LongPathUtility.RemoveExtendedLengthPathPrefix(path)), (Exception) new Win32Exception());
        case 32:
          throw new IOException(TFCommonResources.FileInUse((object) LongPathUtility.RemoveExtendedLengthPathPrefix(path)), (Exception) new Win32Exception());
        default:
          throw new Win32Exception();
      }
    }

    private static string GetFullPathWrapper(string path)
    {
      try
      {
        string fullPath = Path.GetFullPath(path);
        if (fullPath.IndexOf('~') >= 0)
          fullPath = Path.GetFullPath(fullPath);
        return fullPath;
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentException _:
          case NotSupportedException _:
            throw new InvalidPathException(TFCommonResources.InvalidPath((object) path), ex);
          case PathTooLongException _:
            throw new InvalidPathException(TFCommonResources.InvalidServerPathTooLong((object) path), ex);
          default:
            throw;
        }
      }
    }

    public static System.IO.FileAttributes GetAttributes(string path)
    {
      System.IO.FileAttributes attributes = (System.IO.FileAttributes) -1;
      if (File.Exists(path) || Directory.Exists(path))
        attributes = File.GetAttributes(path);
      return attributes;
    }

    public static CompressionFormat GetCompressionFormat(string path)
    {
      using (SafeFileHandle file = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile(path, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericRead, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.Read | Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.Write | Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.Delete, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.OpenExisting, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.BackupSemantics, IntPtr.Zero))
        return !file.IsInvalid ? (CompressionFormat) Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileCompression((SafeHandle) file) : throw new Win32Exception();
    }

    public static void SetCompressionFormat(string path, CompressionFormat compressionFormat)
    {
      using (SafeFileHandle file = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile(path, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericRead | Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericWrite, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.None, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.OpenExisting, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.BackupSemantics, IntPtr.Zero))
      {
        if (file.IsInvalid)
          throw new Win32Exception();
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetFileCompression((SafeHandle) file, (short) compressionFormat);
      }
    }

    public static bool IsVersionControlReservedCharacter(char c)
    {
      for (int index = 0; index < FileSpec.s_versionControlSeparators.Length; ++index)
      {
        if ((int) FileSpec.s_versionControlSeparators[index] == (int) c)
          return true;
      }
      return false;
    }

    public static bool HasVersionControlReservedCharacter(string path, out char c)
    {
      for (int index = 0; index < FileSpec.s_versionControlSeparators.Length; ++index)
      {
        if (path.IndexOf(FileSpec.s_versionControlSeparators[index]) != -1)
        {
          c = FileSpec.s_versionControlSeparators[index];
          return true;
        }
      }
      c = char.MinValue;
      return false;
    }

    public static StringComparer StringComparer => StringComparer.OrdinalIgnoreCase;

    private static int NextRandomNumber
    {
      get
      {
        if (FileSpec.s_random == null)
          FileSpec.s_random = new Random(Environment.TickCount);
        return FileSpec.s_random.Next(1000000);
      }
    }
  }
}
