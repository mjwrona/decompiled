// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlPath
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public static class VersionControlPath
  {
    public const char Separator = '/';
    public const string RootFolder = "$/";
    private const int s_maxAgeTempFilesToKeep = 7;
    internal const int s_maxTemporaryFileRetries = 70;
    private const int s_maxTempDirPlusExtensionPlusVersionPlusBaseFileLength = 254;
    private const int s_maxTempDirPlusExtensionPlusVersionLength = 249;
    private const int s_maxTempDirPlusExtensionLength = 241;
    private const int s_maxTempDirLength = 236;
    private static object s_getTempFileNameLock = new object();
    private static IComparer<string> s_depthFirstTreeComparer;

    public static bool IsServerItem(string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      if (path.Length < 2 || path[0] != '$')
        return false;
      return path[1] == '/' || path[1] == '\\';
    }

    public static bool IsValidFolderName(string name, out string error)
    {
      error = (string) null;
      if (name == null || name.Length == 0)
      {
        error = Resources.Get("FolderNameEmpty");
        return false;
      }
      if (name.Length > 0 && name[0] == '$')
      {
        error = Resources.Format("InvalidPathInvalidFolderStartChar", (object) name, (object) '$');
        return false;
      }
      for (int index = 0; index < name.Length; ++index)
      {
        if (!FileSpec.IsValidNtfsChar(name[index]) || Wildcard.IsWildcard(name[index]))
        {
          error = TFCommonResources.InvalidPathInvalidChar((object) name, (object) name[index]);
          return false;
        }
      }
      if (!FileSpec.IsReservedName(name))
        return true;
      error = Resources.Format("ReservedPathName", (object) name);
      return false;
    }

    public static string GetCommonParent(string path1, string path2)
    {
      if (path1 == null && path2 == null)
        throw new ArgumentNullException(nameof (path1));
      if (path1 == null)
        return path2;
      if (path2 == null)
        return path1;
      string parent = path1;
      while (!VersionControlPath.IsSubItem(path2, parent))
        parent = VersionControlPath.GetFolderName(parent);
      return parent;
    }

    public static string GetFullPath(string item) => VersionControlPath.GetFullPath(item, false, PathLength.Length399);

    public static string GetFullPath(string item, bool checkReservedCharacters) => VersionControlPath.GetFullPath(item, checkReservedCharacters, PathLength.Length399);

    public static string GetFullPath(string item, PathLength maxPathLength) => VersionControlPath.GetFullPath(item, false, maxPathLength);

    public static string GetFullPath(
      string item,
      bool checkReservedCharacters,
      PathLength maxPathLength)
    {
      int capacity = item != null ? item.Length : throw new ArgumentNullException(nameof (item));
      if (capacity == 0)
        throw new InvalidPathException(Resources.Get("InvalidPathNoHatterasItem"));
      if (VersionControlPath.IsCanonicalizedPath(item, !checkReservedCharacters, maxPathLength))
        return item;
      StringBuilder sb = new StringBuilder(capacity);
      int index = 0;
      sb.Append('$');
      if (item[index] == '$')
        ++index;
      if (index >= capacity || item[index] != '/' && item[index] != '\\')
        throw new InvalidPathException(Resources.Format("InvalidPathMissingRoot", (object) item));
      bool flag1 = false;
      bool flag2 = false;
      VersionControlPath.ParseState parseState = VersionControlPath.ParseState.Normal;
      int startIndex = 0;
      for (; index < capacity; ++index)
      {
        char c = item[index];
        switch (c)
        {
          case '.':
            switch (parseState)
            {
              case VersionControlPath.ParseState.Normal:
                sb.Append('.');
                continue;
              case VersionControlPath.ParseState.Slash:
                parseState = VersionControlPath.ParseState.Dot;
                continue;
              case VersionControlPath.ParseState.Dot:
                parseState = VersionControlPath.ParseState.DotDot;
                continue;
              case VersionControlPath.ParseState.DotDot:
                sb.Append("...");
                parseState = VersionControlPath.ParseState.Normal;
                continue;
              default:
                continue;
            }
          case '/':
          case '\\':
            switch (parseState)
            {
              case VersionControlPath.ParseState.Normal:
                if (flag1 || VersionControlPath.StripTrailingChars(sb))
                {
                  string name = sb.ToString(startIndex, sb.Length - startIndex);
                  if (name.Length > VersionControlUtil.GetMaxPathLengthComponent(maxPathLength))
                  {
                    int num = maxPathLength != PathLength.Length259 ? VersionControlUtil.ConvertPathLengthToInt(maxPathLength) : throw new InvalidPathException(Resources.Format("RepositoryPathTooLongDetailed", (object) name));
                    throw new InvalidPathException(Resources.Format("RepositoryPathTooLong", (object) name, (object) (num - 11), (object) num));
                  }
                  if (FileSpec.IsReservedName(name))
                    throw new InvalidPathException(Resources.Format("ReservedPathName", (object) name));
                  sb.Append('/');
                  startIndex = sb.Length;
                  flag1 = false;
                  break;
                }
                break;
              case VersionControlPath.ParseState.DotDot:
                VersionControlPath.BackupOneElement(sb, item);
                startIndex = sb.Length;
                flag1 = false;
                break;
            }
            parseState = VersionControlPath.ParseState.Slash;
            break;
          default:
            if (!FileSpec.IsValidNtfsChar(c) || checkReservedCharacters && FileSpec.IsVersionControlReservedCharacter(c))
              throw new InvalidPathException(TFCommonResources.InvalidPathInvalidChar((object) item, (object) c));
            switch (parseState)
            {
              case VersionControlPath.ParseState.Dot:
                sb.Append('.');
                break;
              case VersionControlPath.ParseState.DotDot:
                sb.Append("..");
                break;
            }
            flag1 = flag1 || c == '*' || c == '?';
            if (c == '$' && sb[sb.Length - 1] == '/')
              flag2 = true;
            sb.Append(c);
            parseState = VersionControlPath.ParseState.Normal;
            break;
        }
      }
      switch (parseState)
      {
        case VersionControlPath.ParseState.Normal:
          if (flag1 || VersionControlPath.StripTrailingChars(sb))
          {
            string name = sb.ToString(startIndex, sb.Length - startIndex);
            if (name.Length > VersionControlUtil.GetMaxPathLengthComponent(maxPathLength))
            {
              int num = maxPathLength != PathLength.Length259 ? VersionControlUtil.ConvertPathLengthToInt(maxPathLength) : throw new InvalidPathException(Resources.Format("RepositoryPathTooLongDetailed", (object) name));
              throw new InvalidPathException(Resources.Format("RepositoryPathTooLong", (object) name, (object) (num - 11), (object) num));
            }
            if (FileSpec.IsReservedName(name))
              throw new InvalidPathException(Resources.Format("ReservedPathName", (object) name));
            break;
          }
          break;
        case VersionControlPath.ParseState.DotDot:
          VersionControlPath.BackupOneElement(sb, item);
          break;
      }
      if (sb.Length > 2 && sb[sb.Length - 1] == '/')
        --sb.Length;
      if (sb.Length > VersionControlUtil.ConvertPathLengthToInt(maxPathLength))
      {
        int num = maxPathLength != PathLength.Length259 ? VersionControlUtil.ConvertPathLengthToInt(maxPathLength) : throw new InvalidPathException(Resources.Format("RepositoryPathTooLongDetailed", (object) sb));
        throw new InvalidPathException(Resources.Format("RepositoryPathTooLong", (object) sb, (object) (num - 11), (object) num));
      }
      if (flag2)
        throw new InvalidPathException(TFCommonResources.InvalidPathDollarSign((object) sb.ToString()));
      return sb.ToString();
    }

    public static bool IsCanonicalizedPath(string serverItem, bool allowSemicolon) => VersionControlPath.IsCanonicalizedPath(serverItem, allowSemicolon, PathLength.Length399);

    public static bool IsCanonicalizedPath(
      string serverItem,
      bool allowSemicolon,
      PathLength maxPathLength)
    {
      if (serverItem.Length > VersionControlUtil.ConvertPathLengthToInt(maxPathLength) || !serverItem.StartsWith("$/", StringComparison.Ordinal))
        return false;
      if (2 == serverItem.Length)
        return true;
      if (serverItem.Length > 2 && serverItem[serverItem.Length - 1] == '/')
        return false;
      int pathPartLength = 0;
      for (int index = 2; index < serverItem.Length; ++index)
      {
        char c = serverItem[index];
        if (c == '/')
        {
          if (!VersionControlPath.IsCanonicalizedPathPart(serverItem, index, pathPartLength, maxPathLength))
            return false;
          pathPartLength = 0;
        }
        else
        {
          if (pathPartLength == 0 && c == '$' || !FileSpec.IsValidNtfsChar(c) || !allowSemicolon && c == ';' || c == '*' || c == '?')
            return false;
          ++pathPartLength;
        }
      }
      return VersionControlPath.IsCanonicalizedPathPart(serverItem, serverItem.Length, pathPartLength, maxPathLength);
    }

    private static bool IsCanonicalizedPathPart(
      string serverItem,
      int i,
      int pathPartLength,
      PathLength maxPathLength)
    {
      if (pathPartLength == 0)
        return false;
      if (2 == pathPartLength)
      {
        if (serverItem[i - 1] == '.' && serverItem[i - 2] == '.')
          return false;
      }
      else
      {
        if (serverItem[i - 1] == '.' || char.IsWhiteSpace(serverItem[i - 1]))
          return false;
        if (pathPartLength >= 3)
        {
          if (FileSpec.IsReservedName(serverItem.Substring(i - pathPartLength, pathPartLength)))
            return false;
        }
        else if (pathPartLength > VersionControlUtil.GetMaxPathLengthComponent(maxPathLength))
          return false;
      }
      return true;
    }

    public static bool IsValidPath(string serverItem) => VersionControlPath.IsValidPath(serverItem, PathLength.Length399);

    public static bool IsValidPath(string serverItem, PathLength maxPathLength)
    {
      try
      {
        VersionControlPath.ValidatePath(serverItem, maxPathLength);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static void ValidatePath(string serverItem) => VersionControlPath.ValidatePath(serverItem, PathLength.Length399);

    public static void ValidatePath(string serverItem, PathLength maxPathLength)
    {
      if (Wildcard.IsWildcard(VersionControlPath.GetFullPath(serverItem, maxPathLength)))
        throw new InvalidPathException(TFCommonResources.WildcardsNotAllowed());
    }

    public static void CheckForIllegalDollarInPath(string path) => FileSpec.CheckForIllegalDollarInPath(path);

    public static string Combine(string parent, string relative) => VersionControlPath.Combine(parent, relative, PathLength.Length399);

    public static string Combine(string parent, string relative, PathLength maxPathLength)
    {
      if (parent == null)
        throw new ArgumentNullException(nameof (parent));
      string str1;
      switch (relative)
      {
        case null:
          throw new ArgumentNullException(nameof (relative));
        case "":
          str1 = parent;
          break;
        default:
          if (relative.Length != 1 || relative[0] != '$')
          {
            if (relative[0] == '/' || relative[0] == '\\' || relative[0] == '$' && relative.Length >= 2 && (relative[1] == '/' || relative[1] == '\\'))
            {
              str1 = relative;
              break;
            }
            string str2 = string.Empty;
            if (parent.Length > 0 && parent[parent.Length - 1] != '/')
              str2 = "/";
            str1 = relative[0] != '$' ? parent + str2 + relative : parent + str2 + relative.Substring(1, relative.Length - 1);
            break;
          }
          goto case "";
      }
      return VersionControlPath.GetFullPath(str1, maxPathLength);
    }

    public static void Parse(string item, out string parent, out string name)
    {
      int length = item != null ? item.LastIndexOf('/') : throw new ArgumentNullException(nameof (item));
      if (length < 0)
        throw new InvalidPathException(item);
      if (length == 1)
      {
        parent = "$/";
        if (length == item.Length - 1)
        {
          name = "";
          return;
        }
      }
      else
        parent = item.Substring(0, length);
      name = item.Substring(length + 1);
    }

    public static string GetFolderName(string item)
    {
      int length = item != null ? item.LastIndexOf('/') : throw new ArgumentNullException(nameof (item));
      if (length < 0)
        throw new InvalidPathException(item);
      return length == 1 ? "$/" : item.Substring(0, length);
    }

    public static int GetFolderDepth(string item) => VersionControlPath.GetFolderDepth(item, int.MaxValue);

    public static int GetFolderDepth(string item, int maxDepth)
    {
      int folderDepth = 0;
      if (!VersionControlPath.Equals(item, "$/"))
      {
        for (int index = item.IndexOf('/'); index != -1 && maxDepth > folderDepth; index = item.IndexOf('/', index + 1))
          ++folderDepth;
      }
      return folderDepth;
    }

    public static string GetFileName(string item) => VersionControlPath.GetFileName(item, false);

    public static string GetFileName(string item, bool containsGuid = false)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      int num1 = containsGuid ? 1 : 0;
      int num2 = item.LastIndexOf('/');
      if (num2 < 0)
        throw new InvalidPathException(item);
      return num2 == item.Length - 1 ? string.Empty : item.Substring(num2 + 1);
    }

    public static string GetExtension(string item)
    {
      if (item == null)
        return (string) null;
      if (item == string.Empty)
        return string.Empty;
      string fileName = VersionControlPath.GetFileName(item);
      int startIndex = fileName.LastIndexOf('.');
      return startIndex != -1 && startIndex + 1 < fileName.Length ? fileName.Substring(startIndex) : string.Empty;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetTempFileName(string item, int version)
    {
      string version1 = version != 0 ? version.ToString((IFormatProvider) CultureInfo.InvariantCulture) : string.Empty;
      return VersionControlPath.GetTempFileName(item, version1);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetTempFileName(string item, string version)
    {
      switch (item)
      {
        case null:
          throw new ArgumentNullException(nameof (item));
        case "":
          throw new InvalidPathException(Resources.Get("InvalidPathNoHatterasItem"));
        default:
          version = version != null ? FileSpec.RemoveInvalidFileNameChars(version.Replace(';', '_')) : throw new ArgumentNullException(nameof (version));
          string tempDirectory = FileSpec.GetTempDirectory();
          string fileName = VersionControlPath.GetFileName(item);
          string extension = VersionControlPath.GetExtension(item);
          string fileWithNoExtension = fileName.Substring(0, fileName.Length - extension.Length);
          if (tempDirectory.Length + version.Length + fileName.Length > 236)
          {
            if (tempDirectory.Length > 236)
              throw new InvalidPathException(TFCommonResources.InvalidServerPathTooLong((object) tempDirectory));
            int num1 = tempDirectory.Length + extension.Length;
            if (num1 > 249)
            {
              int startIndex = extension.Length - (num1 - 249);
              extension = extension.Remove(startIndex);
            }
            int num2 = tempDirectory.Length + extension.Length + version.Length;
            if (num2 > 249)
            {
              int startIndex = version.Length - (num2 - 249);
              version = version.Remove(startIndex);
            }
            int num3 = tempDirectory.Length + extension.Length + version.Length + fileWithNoExtension.Length;
            if (num3 > 254)
            {
              int startIndex = fileWithNoExtension.Length - (num3 - 254);
              fileWithNoExtension = fileWithNoExtension.Remove(startIndex);
            }
          }
          string path = VersionControlPath.CombinePathParts(tempDirectory, fileWithNoExtension, version, -1, extension);
          string currentOldestFile = path;
          DateTime maxValue = DateTime.MaxValue;
          lock (VersionControlPath.s_getTempFileNameLock)
          {
            bool flag = false;
            if (!VersionControlPath.RecentFileExists(path, ref currentOldestFile, ref maxValue))
              flag = true;
            for (int retryNumber = 0; retryNumber < 70 && !flag; ++retryNumber)
            {
              path = VersionControlPath.CombinePathParts(tempDirectory, fileWithNoExtension, version, retryNumber, extension);
              if (!VersionControlPath.RecentFileExists(path, ref currentOldestFile, ref maxValue))
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              FileSpec.DeleteFile(currentOldestFile);
              path = currentOldestFile;
            }
            using (File.Create(path))
              return path;
          }
      }
    }

    private static string CombinePathParts(
      string temporaryDirPath,
      string fileWithNoExtension,
      string version,
      int retryNumber,
      string extension)
    {
      if (retryNumber == -1)
        return !string.IsNullOrEmpty(version) ? Path.Combine(temporaryDirPath, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_{1}{2}", (object) fileWithNoExtension, (object) version, (object) extension)) : Path.Combine(temporaryDirPath, fileWithNoExtension + extension);
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0:00}", (object) retryNumber);
      if (string.IsNullOrEmpty(version))
        return Path.Combine(temporaryDirPath, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}.{1}{2}", (object) fileWithNoExtension, (object) str, (object) extension));
      return Path.Combine(temporaryDirPath, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_{1}.{2}{3}", (object) fileWithNoExtension, (object) version, (object) str, (object) extension));
    }

    private static bool RecentFileExists(
      string path,
      ref string currentOldestFile,
      ref DateTime currentOldestTime)
    {
      if (!File.Exists(path))
        return false;
      try
      {
        DateTime creationTime = File.GetCreationTime(path);
        if (creationTime < DateTime.Now - TimeSpan.FromDays(7.0))
        {
          Trace.WriteLine("Deleting old temp file: " + path);
          FileSpec.DeleteFile(path);
          return false;
        }
        if (creationTime < currentOldestTime)
        {
          currentOldestFile = path;
          currentOldestTime = creationTime;
        }
      }
      catch (Exception ex)
      {
      }
      return true;
    }

    public static string GetTeamProjectName(string item) => VersionControlPath.GetTeamProjectName(item, false, true);

    public static string GetTeamProjectName(
      string item,
      bool containsGuid = false,
      bool checkCanonicalization = true)
    {
      return item != null && item.Length <= 2 ? string.Empty : VersionControlPath.GetTeamProject(item, containsGuid, checkCanonicalization).Substring(2);
    }

    public static string GetTeamProject(string item) => VersionControlPath.GetTeamProject(item, false, true);

    public static string GetTeamProject(string item, bool containsGuid = false, bool checkCanonicalization = true)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      if (checkCanonicalization)
      {
        int num = containsGuid ? 1 : 0;
      }
      if (item.Length <= 2)
        return item;
      int length = item.IndexOf('/', 2);
      return length < 0 ? item : item.Substring(0, length);
    }

    public static string GetTeamProjectNameAndRemainingPath(string item, out string remainingPath)
    {
      string teamProject = VersionControlPath.GetTeamProject(item);
      remainingPath = (string) null;
      if (teamProject.Length <= 2)
        return (string) null;
      if (item.Length > teamProject.Length)
        remainingPath = item.Substring(teamProject.Length);
      return teamProject.Substring(2);
    }

    public static bool IsRootFolder(string item) => VersionControlPath.Equals(item, "$/");

    public static bool IsTeamProject(string item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      return VersionControlPath.GetFolderDepth(item, 2) == 1;
    }

    public static string MakeRelative(string item, string folder)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      int startIndex = folder != null ? VersionControlPath.GetRelativeStartIndex(item, folder) : throw new ArgumentNullException(nameof (folder));
      if (startIndex < 0)
        return string.Empty;
      return startIndex == 0 ? item : item.Substring(startIndex);
    }

    public static int GetRelativeStartIndex(string item, string folder)
    {
      if (item.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
      {
        if (item.Length == folder.Length)
          return -1;
        if (folder.Length > 0 && folder[folder.Length - 1] == '/')
          return folder.Length;
        if (item[folder.Length] == '/')
          return folder.Length + 1;
      }
      return 0;
    }

    public static string MakeRelative(string item, string folder, bool recursive)
    {
      string str1 = VersionControlPath.MakeRelative(item, folder);
      if (!recursive || !VersionControlPath.Equals(str1, item))
        return str1;
      string str2 = "";
      while (VersionControlPath.Equals(str1, item) && folder.Length > 2)
      {
        folder = VersionControlPath.GetFolderName(folder);
        str1 = VersionControlPath.MakeRelative(item, folder);
        str2 += "../";
      }
      return str2 + str1;
    }

    public static string Combine(
      string baseLocalPath,
      string targetLocalPath,
      string baseServerPath)
    {
      return VersionControlPath.Combine(baseLocalPath, targetLocalPath, baseServerPath, PathLength.Length399);
    }

    public static string Combine(
      string baseLocalPath,
      string targetLocalPath,
      string baseServerPath,
      PathLength maxServerPathLength)
    {
      if (baseLocalPath == null)
        throw new ArgumentNullException(nameof (baseLocalPath));
      if (targetLocalPath == null)
        throw new ArgumentNullException(nameof (targetLocalPath));
      if (baseServerPath == null)
        throw new ArgumentNullException(nameof (baseServerPath));
      string str = FileSpec.MakeRelative(targetLocalPath, baseLocalPath, true);
      string relative = str != null && !str.Equals(targetLocalPath) ? str.Replace('\\', '/') : throw new InvalidPathException(Resources.Format("InvalidPathNoCommonParent", (object) baseLocalPath, (object) targetLocalPath));
      return VersionControlPath.Combine(baseServerPath, relative, maxServerPathLength);
    }

    public static bool Equals(string item1, string item2) => (object) item1 == (object) item2 || string.Equals(item1, item2, StringComparison.OrdinalIgnoreCase);

    public static int Compare(string item1, string item2) => FileSpec.Compare(item1, item2, item1.Length, item2.Length, '/', StringComparison.OrdinalIgnoreCase);

    public static int Compare(string item1, string item2, int item1Length, int item2Length) => FileSpec.Compare(item1, item2, item1Length, item2Length, '/', StringComparison.OrdinalIgnoreCase);

    public static int CompareUI(string item1, string item2) => FileSpec.Compare(item1, item2, item1.Length, item2.Length, '/', StringComparison.CurrentCultureIgnoreCase);

    public static int CompareUI(string item1, string item2, int item1Length, int item2Length) => FileSpec.Compare(item1, item2, item1Length, item2Length, '/', StringComparison.CurrentCultureIgnoreCase);

    public static int CompareCaseSensitive(string item1, string item2)
    {
      int num = FileSpec.Compare(VersionControlPath.GetFolderName(item1), VersionControlPath.GetFolderName(item2), '/', StringComparison.OrdinalIgnoreCase);
      return num == 0 ? string.CompareOrdinal(VersionControlPath.GetFileName(item1), VersionControlPath.GetFileName(item2)) : num;
    }

    public static bool EqualsCaseSensitive(string item1, string item2) => VersionControlPath.CompareCaseSensitive(item1, item2) == 0;

    public static StringComparer FullPathComparer => StringComparer.OrdinalIgnoreCase;

    internal static StringComparer PartialPathComparer => StringComparer.OrdinalIgnoreCase;

    public static int CompareTopDown(string item1, string item2) => FileSpec.CompareTopDown(item1, item2, '/', StringComparison.OrdinalIgnoreCase);

    public static int CompareTopDownUI(string item1, string item2) => FileSpec.CompareTopDown(item1, item2, '/', StringComparison.CurrentCultureIgnoreCase);

    internal static int CompareBottomUp(string item1, string item2) => FileSpec.CompareBottomUp(item1, item2, '/', StringComparison.OrdinalIgnoreCase);

    internal static int CompareBottomUpUI(string item1, string item2) => FileSpec.CompareBottomUp(item1, item2, '/', StringComparison.CurrentCultureIgnoreCase);

    public static bool IsSubItem(string item, string parent)
    {
      if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(parent) || !item.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
        return false;
      return item.Length == parent.Length || parent[parent.Length - 1] == '/' || item[parent.Length] == '/';
    }

    public static bool IsImmediateChild(string item, string parent) => VersionControlPath.IsSubItem(item, parent) && parent.Length < item.Length && !item.Substring(parent.Length + 1).Contains(new string('/', 1));

    public static bool IsWildcard(string path) => FileSpec.IsWildcard(path);

    public static bool Match(string item, string matchFolder, string matchPattern, bool recursive)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      if (matchFolder == null)
        throw new ArgumentNullException(nameof (matchFolder));
      string parent;
      string name;
      if (matchPattern == null || matchPattern.Length == 0)
      {
        parent = item;
        name = (string) null;
      }
      else
        VersionControlPath.Parse(item, out parent, out name);
      if (recursive)
      {
        if (!VersionControlPath.IsSubItem(parent, matchFolder))
          return false;
      }
      else if (VersionControlPath.Compare(matchFolder, parent) != 0)
        return false;
      return name == null || Wildcard.Match(name, matchPattern);
    }

    public static bool MatchFileName(string item, string matchPattern)
    {
      int num = item != null ? item.LastIndexOf('/') : throw new ArgumentNullException(nameof (item));
      if (num < 0)
        throw new InvalidPathException(item);
      return num != item.Length - 1 && Wildcard.Match(item, num + 1, matchPattern);
    }

    public static string PrependRootIfNeeded(string folder) => VersionControlPath.PrependRootIfNeeded(folder, PathLength.Length399);

    public static string PrependRootIfNeeded(string folder, PathLength maxPathLength)
    {
      if (!folder.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
        folder = VersionControlPath.Combine("$/", folder, maxPathLength);
      return folder;
    }

    private static void BackupOneElement(StringBuilder sb, string item)
    {
      int index = sb.Length - 2;
      if (index == 0)
        throw new InvalidPathException(Resources.Format("InvalidPathTooManyDotDots", (object) item));
      while (sb[index] != '/')
        --index;
      sb.Length = index + 1;
    }

    private static bool StripTrailingChars(StringBuilder sb)
    {
      int index = sb.Length - 1;
      while (sb[index] == '.' || char.IsWhiteSpace(sb[index]))
        --index;
      sb.Length = index + 1;
      return sb[sb.Length - 1] != '/';
    }

    public static StringComparer StringComparer => StringComparer.OrdinalIgnoreCase;

    public static IComparer<string> DepthFirstTreeComparer
    {
      get
      {
        if (VersionControlPath.s_depthFirstTreeComparer == null)
          VersionControlPath.s_depthFirstTreeComparer = (IComparer<string>) new VersionControlPath.VersionControlDepthFirstTreeComparer();
        return VersionControlPath.s_depthFirstTreeComparer;
      }
    }

    private enum ParseState
    {
      Normal,
      Slash,
      Dot,
      DotDot,
    }

    private class VersionControlDepthFirstTreeComparer : IComparer<string>
    {
      public int Compare(string x, string y) => VersionControlPath.Compare(x, y);
    }
  }
}
