// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerPathHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class FileContainerPathHelper
  {
    private static readonly char[] s_forbiddenPathChars = new char[39]
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
      ':',
      '<',
      '>',
      '|',
      '*',
      '?'
    };
    private static readonly bool[] s_validPathCharsTruthTable = new bool[128];

    static FileContainerPathHelper()
    {
      for (int index = 0; index < 128; ++index)
        FileContainerPathHelper.s_validPathCharsTruthTable[index] = true;
      foreach (char forbiddenPathChar in FileContainerPathHelper.s_forbiddenPathChars)
        FileContainerPathHelper.s_validPathCharsTruthTable[(int) forbiddenPathChar] = false;
    }

    public static string NormalizePath(string itemPath)
    {
      ArgumentUtility.CheckForNull<string>(itemPath, nameof (itemPath));
      string[] strArray = !string.IsNullOrWhiteSpace(itemPath) ? itemPath.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries) : throw new InvalidPathException(FrameworkResources.InvalidPathEmpty((object) itemPath));
      if (strArray.Length == 0)
        throw new InvalidPathException(FrameworkResources.InvalidPathEmpty((object) itemPath));
      for (int index = 0; index < strArray.Length; ++index)
      {
        string str = strArray[index];
        if (str.Length > FileContainerPathHelper.MaxPathSegmentLength)
          throw new InvalidPathException(FrameworkResources.InvalidPathSegmentTooLong((object) itemPath, (object) FileContainerPathHelper.MaxPathSegmentLength));
        bool flag = false;
        foreach (char c in str)
        {
          if (!FileContainerPathHelper.IsValidPathChar(c))
            throw new InvalidPathException(TFCommonResources.InvalidPathInvalidChar((object) itemPath, (object) c));
          if (!char.IsWhiteSpace(c) && c != '.')
            flag = true;
        }
        if (!flag)
          throw new InvalidPathException(FrameworkResources.InvalidPathSpacePeriodSegment((object) itemPath));
      }
      string str1 = string.Join("/", strArray);
      if (str1.Length > FileContainerPathHelper.MaxPathLength)
        throw new InvalidPathException(TFCommonResources.InvalidPathTooLongVariable((object) str1, (object) FileContainerPathHelper.MaxPathLength));
      return str1;
    }

    public static bool IsValidPathChar(char c) => c > '\u007F' || FileContainerPathHelper.s_validPathCharsTruthTable[(int) c];

    public static int MaxPathLength => 399;

    public static int MaxPathSegmentLength => (int) byte.MaxValue;

    public static string GetParentFolderPath(string absolutePath)
    {
      string parentFolderPath = string.Empty;
      int num = absolutePath.LastIndexOf("/");
      if (num != -1)
        parentFolderPath = absolutePath.Substring(0, num + 1);
      return parentFolderPath;
    }

    public static string RemovePathPrefix(string prefix, string itemPath)
    {
      if (!string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(itemPath))
      {
        if (itemPath.StartsWith(prefix))
        {
          try
          {
            return itemPath.Substring(prefix.Length);
          }
          catch (ArgumentOutOfRangeException ex)
          {
            return itemPath;
          }
        }
      }
      return itemPath;
    }
  }
}
