// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildContainerPath
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class BuildContainerPath
  {
    private const string m_root = "#";
    private const string m_pathSeparator = "/";
    private const char m_pathSeparatorChar = '/';

    public static string Combine(params string[] pathParts)
    {
      if (pathParts.Length == 0)
        return BuildContainerPath.RootFolder;
      StringBuilder stringBuilder = new StringBuilder(pathParts[0]);
      for (int index = 1; index < pathParts.Length; ++index)
        stringBuilder.AppendFormat("{0}{1}", (object) "/", (object) pathParts[index]);
      return BuildContainerPath.GetFullPath(stringBuilder.ToString());
    }

    public static bool IsValidPath(ref string path, out string error)
    {
      error = (string) null;
      try
      {
        path = BuildContainerPath.GetFullPath(path);
        if (Wildcard.IsWildcard(path))
          throw new InvalidPathException(BuildTypeResource.InvalidPathContainsWildcards((object) path));
        return true;
      }
      catch (Exception ex)
      {
        error = ex.Message;
      }
      return false;
    }

    public static string GetFullPath(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      StringBuilder stringBuilder = new StringBuilder(BuildContainerPath.RootFolder);
      string[] strArray = path.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
        return BuildContainerPath.RootFolder;
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (index != 0 || !(strArray[0] == "#"))
        {
          string error;
          if (!FileSpec.IsLegalNtfsName(strArray[index], BuildConstants.MaxPathNameLength, true, out error))
            throw new InvalidPathException(error);
          stringBuilder.AppendFormat("{0}{1}", (object) strArray[index], index == strArray.Length - 1 ? (object) string.Empty : (object) "/");
        }
      }
      return stringBuilder.ToString();
    }

    public static void GetContainerIdAndPath(
      string buildPath,
      out long containerId,
      out string itemPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(buildPath, nameof (buildPath));
      if (!buildPath.StartsWith(BuildContainerPath.RootFolder, StringComparison.OrdinalIgnoreCase))
        throw new InvalidPathException(BuildTypeResource.InvalidBuildFileContainerRoot((object) buildPath));
      string empty = string.Empty;
      int startIndex = buildPath.IndexOf('/', 2);
      string s;
      if (startIndex < 0)
      {
        if (buildPath.Length <= 2)
          throw new InvalidPathException(BuildTypeResource.InvalidBuildFileContainerId((object) buildPath));
        itemPath = "/";
        s = buildPath.Substring(2);
      }
      else
      {
        s = buildPath.Substring(2, startIndex - 2);
        itemPath = buildPath.Substring(startIndex);
      }
      if (!long.TryParse(s, out containerId))
        throw new InvalidPathException(BuildTypeResource.InvalidBuildFileContainerId((object) buildPath));
    }

    public static string MakeRelative(string parentPath, string fullPath)
    {
      if (string.IsNullOrEmpty(fullPath))
        throw new ArgumentNullException(nameof (fullPath));
      if (string.IsNullOrEmpty(parentPath))
        throw new ArgumentNullException(nameof (parentPath));
      fullPath = fullPath.Replace("\\", "/");
      parentPath = parentPath.Replace("\\", "/").Trim('/');
      int num = fullPath.IndexOf(parentPath, StringComparison.OrdinalIgnoreCase);
      return num > -1 ? fullPath.Substring(num + parentPath.Length) : fullPath;
    }

    public static string GetFolderName(string fullPath)
    {
      int length1 = !string.IsNullOrEmpty(fullPath) ? fullPath.LastIndexOfAny(new char[2]
      {
        '/',
        '\\'
      }) : throw new ArgumentNullException(nameof (fullPath));
      int length2 = fullPath.LastIndexOf(Uri.EscapeDataString("/"));
      int length3 = fullPath.LastIndexOf(Uri.EscapeDataString("\\"));
      if (length1 > -1)
        return fullPath.Substring(0, length1);
      if (length2 > -1)
        return fullPath.Substring(0, length2);
      return length3 > -1 ? fullPath.Substring(0, length3) : fullPath;
    }

    public static bool IsHttpUrl(string logLocation) => !string.IsNullOrEmpty(logLocation) && logLocation.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase);

    public static void GetContainerId(string buildPath, out long containerId)
    {
      string itemPath = "";
      BuildContainerPath.GetContainerIdAndPath(buildPath, out containerId, out itemPath);
    }

    public static string GetItemName(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      int num = path.LastIndexOf('/');
      return num == -1 ? string.Empty : path.Substring(num + 1);
    }

    public static bool IsServerPath(string path) => !string.IsNullOrEmpty(path) && path.StartsWith(BuildContainerPath.RootFolder, StringComparison.OrdinalIgnoreCase);

    public static bool AreEqual(string path1, string path2) => StringComparer.OrdinalIgnoreCase.Equals(BuildContainerPath.GetFullPath(path1), BuildContainerPath.GetFullPath(path2));

    public static bool IsSubItem(string item, string parent)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(item, "Item");
      ArgumentUtility.CheckStringForNullOrEmpty(parent, "Parent");
      if (!item.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
        return false;
      return item.Length == parent.Length || parent[parent.Length - 1] == '/' || item[parent.Length] == '/';
    }

    public static string RootFolder => "#/";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string PathSeparator => "/";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static char PathSeparatorChar => '/';
  }
}
