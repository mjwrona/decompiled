// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.FolderValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class FolderValidator
  {
    private const string m_pathSeparator = "\\";
    private static readonly char[] IllegalNameChars = new char[2]
    {
      '%',
      '+'
    };

    public static void Validate(Folder folder, bool allowWildCards = false, bool allowRootPath = true)
    {
      ArgumentUtility.CheckForNull<Folder>(folder, nameof (folder));
      ArgumentUtility.CheckForEmptyGuid(folder.ProjectId, "folder.Project.Id");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(folder.Path, "folder.Path");
      string path = folder.Path;
      FolderValidator.CheckValidItemPath(ref path, allowWildCards, allowRootPath);
      folder.Path = path;
    }

    public static void CheckValidItemPath(ref string path, bool allowWildcards, bool allowRootPath)
    {
      string error;
      if (!FolderValidator.IsValidPath(ref path, allowWildcards, out error, allowRootPath))
        throw new InvalidPathException(error);
    }

    public static bool IsValidPath(
      ref string path,
      bool allowWildcards,
      out string error,
      bool allowRootPath)
    {
      error = (string) null;
      try
      {
        path = FolderValidator.GetFullPath(path, allowRootPath);
        ArgumentUtility.CheckStringForInvalidCharacters(path, nameof (path), FolderValidator.IllegalNameChars);
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

    public static string GetFullPath(
      string path,
      bool allowEmptyRootPath = true,
      bool isRootPathRecursive = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = path.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
      {
        if (!allowEmptyRootPath)
          throw new InvalidPathException(BuildServerResources.FolderPathNotSupported((object) path));
        stringBuilder.Append("\\");
      }
      for (int index = 0; index < strArray.Length; ++index)
      {
        string error;
        if (!FileSpec.IsLegalNtfsName(strArray[index], BuildConstants.MaxPathNameLength, true, out error))
          throw new InvalidPathException(error);
        stringBuilder.AppendFormat("{0}{1}", (object) "\\", (object) strArray[index]);
      }
      return stringBuilder.ToString();
    }
  }
}
