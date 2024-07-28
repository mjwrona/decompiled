// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.BranchLinkExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class BranchLinkExtensions
  {
    public static void UpdateBranchMapPaths<T>(
      this List<T> branchLinkData,
      IVssRequestContext requestContext,
      string projectName,
      Guid projectGuid,
      ProjectMapCache projectMapCache)
      where T : IBranchMap
    {
      if (string.IsNullOrEmpty(projectName))
        return;
      foreach (T branchMapItem in branchLinkData)
        BranchLinkExtensions.UpdateServerPaths(requestContext, projectName, projectGuid, projectMapCache, (IBranchMap) branchMapItem);
    }

    private static void UpdateServerPaths(
      IVssRequestContext requestContext,
      string projectName,
      Guid projectGuid,
      ProjectMapCache projectMapCache,
      IBranchMap branchMapItem)
    {
      string embeddedProjectGuid1 = branchMapItem.SourcePath.GetEmbeddedProjectGuid();
      string embeddedProjectGuid2 = branchMapItem.TargetPath.GetEmbeddedProjectGuid();
      if (embeddedProjectGuid1.Equals(embeddedProjectGuid2, StringComparison.OrdinalIgnoreCase))
      {
        if (embeddedProjectGuid1.Equals(projectGuid.ToString(), StringComparison.OrdinalIgnoreCase))
        {
          branchMapItem.SourcePath = branchMapItem.SourcePath.ReplaceProjectName(projectName);
          branchMapItem.TargetPath = branchMapItem.TargetPath.ReplaceProjectName(projectName);
        }
        else
        {
          Guid result = Guid.Empty;
          if (!Guid.TryParse(embeddedProjectGuid1, out result))
            return;
          string projectName1 = ProjectServiceHelper.GetProjectName(requestContext, result, projectMapCache);
          if (string.IsNullOrEmpty(projectName1))
            return;
          branchMapItem.SourcePath = branchMapItem.SourcePath.ReplaceProjectName(projectName1);
          branchMapItem.TargetPath = branchMapItem.TargetPath.ReplaceProjectName(projectName1);
        }
      }
      else if (embeddedProjectGuid1.Equals(projectGuid.ToString(), StringComparison.OrdinalIgnoreCase))
      {
        branchMapItem.SourcePath = branchMapItem.SourcePath.ReplaceProjectName(projectName);
        branchMapItem.TargetPath = BranchLinkExtensions.UpdateServerPath(requestContext, branchMapItem.TargetPath, embeddedProjectGuid2, projectMapCache);
      }
      else if (embeddedProjectGuid2.Equals(projectGuid.ToString(), StringComparison.OrdinalIgnoreCase))
      {
        branchMapItem.TargetPath = branchMapItem.TargetPath.ReplaceProjectName(projectName);
        branchMapItem.SourcePath = BranchLinkExtensions.UpdateServerPath(requestContext, branchMapItem.SourcePath, embeddedProjectGuid1, projectMapCache);
      }
      else
      {
        branchMapItem.SourcePath = BranchLinkExtensions.UpdateServerPath(requestContext, branchMapItem.SourcePath, embeddedProjectGuid1, projectMapCache);
        branchMapItem.TargetPath = BranchLinkExtensions.UpdateServerPath(requestContext, branchMapItem.TargetPath, embeddedProjectGuid2, projectMapCache);
      }
    }

    private static string UpdateServerPath(
      IVssRequestContext requestContext,
      string serverPath,
      string embeddedProjectGuid,
      ProjectMapCache projectMapCache)
    {
      if (string.IsNullOrEmpty(embeddedProjectGuid))
        return serverPath;
      Guid result = Guid.Empty;
      if (Guid.TryParse(embeddedProjectGuid, out result))
      {
        string projectName = ProjectServiceHelper.GetProjectName(requestContext, result, projectMapCache);
        if (!string.IsNullOrEmpty(projectName))
          return serverPath.ReplaceProjectName(projectName);
      }
      return serverPath;
    }
  }
}
