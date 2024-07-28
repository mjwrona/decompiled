// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildFolderServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class IBuildFolderServiceExtensions
  {
    public static IList<Folder> GetFolders(
      this IBuildFolderService folderService,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return folderService.GetFolders(requestContext, projectId, "\\", FolderQueryOrder.FolderAscending);
    }

    public static void ValidateFolder(
      this IBuildFolderService folderService,
      Folder folder,
      bool allowWildCards = false)
    {
      FolderValidator.Validate(folder, allowWildCards);
    }

    public static void ValidatePath(
      this IBuildFolderService folderService,
      ref string path,
      bool allowWildCards = false,
      bool allowRootPath = true)
    {
      FolderValidator.CheckValidItemPath(ref path, allowWildCards, allowRootPath);
      if (!allowWildCards && path.Length > GitConstants.MaxGitRefNameLength)
        throw new InvalidFolderException(BuildServerResources.InvalidFolder((object) path));
    }

    public static void ValidateFolderName(
      this IBuildFolderService folderService,
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      bool allowNumericFolderNames = false)
    {
      if (allowNumericFolderNames)
        return;
      string[] strArray = path.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      string path1 = "";
      foreach (string s in strArray)
      {
        path1 = path1 + "\\" + s;
        if (int.TryParse(s, out int _) && folderService.GetFolders(requestContext, projectId, path1, FolderQueryOrder.None).FirstOrDefault<Folder>() == null)
          throw new InvalidFolderException(BuildServerResources.NumericFolderNameNotAllowed());
      }
    }
  }
}
