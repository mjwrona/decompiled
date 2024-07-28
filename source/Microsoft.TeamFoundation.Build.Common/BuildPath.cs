// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildPath
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class BuildPath
  {
    private const string m_recursionOperator = "**";
    private const string m_slashRecursionOperator = "\\**";
    private const string m_pathSeparator = "\\";
    private const char m_pathSeparatorChar = '\\';
    private static readonly char[] m_splitSeparators = new char[2]
    {
      '\\',
      '/'
    };

    public static string Combine(string parent, string relative) => BuildPath.GetFullPath(Path.Combine(parent, relative));

    public static bool IsRecursive(string queryPath)
    {
      int num = queryPath.LastIndexOf("\\**", StringComparison.OrdinalIgnoreCase);
      return num > 0 && (queryPath.Length == num + "\\**".Length || queryPath[num + "\\**".Length] == '\\');
    }

    public static bool IsValidPath(ref string path, bool allowWildcards, out string error)
    {
      error = (string) null;
      try
      {
        path = BuildPath.GetFullPath(path);
        if (!allowWildcards && Wildcard.IsWildcard(path))
          throw new InvalidPathException(BuildTypeResource.InvalidPathContainsWildcards((object) path));
        return true;
      }
      catch (Exception ex)
      {
        error = ex.Message;
      }
      return false;
    }

    public static DropType GetDropType(string dropLocation)
    {
      if (string.IsNullOrEmpty(dropLocation))
        return DropType.None;
      string error;
      if (Validation.IsValidUncPath(dropLocation, out error))
        return DropType.Unc;
      if (Validation.IsValidBuildContainerPath(dropLocation, out error))
        return DropType.Server;
      if (Validation.IsValidDropLocationUri(dropLocation, out error))
        return DropType.Uri;
      return Validation.IsValidServerPath(dropLocation, out error) ? DropType.VersionControl : DropType.Unknown;
    }

    public static string GetFullPath(string buildPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(buildPath, nameof (buildPath));
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = buildPath.Split(BuildPath.m_splitSeparators, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
        throw new InvalidPathException(BuildTypeResource.InvalidPathTeamProjectRequired((object) buildPath));
      for (int index = 0; index < strArray.Length; ++index)
      {
        string error;
        if (!FileSpec.IsLegalNtfsName(strArray[index], BuildConstants.MaxPathNameLength, true, out error))
          throw new InvalidPathException(error);
        stringBuilder.AppendFormat("{0}{1}", (object) "\\", (object) strArray[index]);
      }
      return stringBuilder.ToString();
    }

    public static string GetRelativePath(string basePath, string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      string[] strArray = string.IsNullOrEmpty(basePath) ? Array.Empty<string>() : basePath.Split(BuildPath.m_splitSeparators, StringSplitOptions.RemoveEmptyEntries);
      string[] source = path.Split(BuildPath.m_splitSeparators, StringSplitOptions.RemoveEmptyEntries);
      int count = 0;
      while (count < strArray.Length && count < source.Length && strArray[count] == source[count])
        ++count;
      List<string> values = new List<string>();
      values.AddRange(Enumerable.Repeat<string>("..", strArray.Length - count));
      values.AddRange(((IEnumerable<string>) source).Skip<string>(count));
      return string.Join("/", (IEnumerable<string>) values);
    }

    public static string GetTeamProject(string buildPath)
    {
      string teamProject;
      BuildPath.SplitTeamProject(buildPath, out teamProject, out string _);
      return teamProject;
    }

    public static int GetItemDepth(string buildPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(buildPath, nameof (buildPath));
      int itemDepth = 0;
      for (int index = 0; index < buildPath.Length; ++index)
      {
        if (buildPath[index] == '\\')
          ++itemDepth;
      }
      return itemDepth;
    }

    public static string GetItemName(string buildPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(buildPath, nameof (buildPath));
      int num = buildPath.LastIndexOf('\\');
      return num == 0 ? string.Empty : buildPath.Substring(num + 1);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string RemoveTeamProject(string path)
    {
      string groupPath;
      BuildPath.SplitTeamProject(path, out string _, out groupPath);
      return groupPath;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string Root(string rootPath, string relativePath)
    {
      string parent = rootPath;
      string relative;
      if (string.IsNullOrEmpty(relativePath))
        relative = relativePath;
      else
        relative = relativePath.TrimStart('\\');
      return BuildPath.Combine(parent, relative);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string RootNoCanonicalize(string rootPath, string relativePath)
    {
      if (string.IsNullOrEmpty(rootPath) || rootPath[0] != '\\')
        rootPath = BuildPath.RootFolder + rootPath;
      string path1 = rootPath;
      string path2;
      if (string.IsNullOrEmpty(relativePath))
        path2 = relativePath;
      else
        path2 = relativePath.TrimStart('\\');
      return Path.Combine(path1, path2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SplitTeamProject(
      string buildPath,
      out string teamProject,
      out string groupPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(buildPath, nameof (buildPath));
      int startIndex = buildPath[0] == '\\' ? buildPath.IndexOf('\\', 1) : throw new InvalidPathException(BuildTypeResource.InvalidPathMustContainRoot((object) buildPath));
      if (startIndex < 0)
      {
        teamProject = buildPath.Length > 1 ? buildPath.Substring(1) : throw new InvalidPathException(BuildTypeResource.InvalidPathTeamProjectRequired((object) buildPath));
        groupPath = BuildPath.RootFolder;
      }
      else
      {
        teamProject = buildPath.Substring(1, startIndex - 1);
        groupPath = buildPath.Substring(startIndex);
      }
    }

    public static string RootFolder => "\\";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string RecursionOperator => "**";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string PathSeparator => "\\";

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static char PathSeparatorChar => '\\';
  }
}
