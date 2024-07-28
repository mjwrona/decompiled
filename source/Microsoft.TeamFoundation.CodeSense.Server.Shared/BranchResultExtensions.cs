// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.BranchResultExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class BranchResultExtensions
  {
    public static void UpdateServerPaths<T>(
      this List<T> branchDetailsResult,
      IVssRequestContext requestContext,
      string projectName,
      Guid projectGuid,
      ProjectMapCache projectMapCache)
      where T : IBranchResult
    {
      if (string.IsNullOrEmpty(projectName))
        return;
      T obj1;
      foreach (T obj2 in branchDetailsResult)
      {
        if (obj2.ServerPath.GetEmbeddedProjectGuid().Equals(projectGuid.ToString(), StringComparison.OrdinalIgnoreCase))
        {
          ref T local = ref obj2;
          obj1 = default (T);
          if ((object) obj1 == null)
          {
            obj1 = local;
            local = ref obj1;
          }
          string completePath = obj2.ServerPath.GetRelativePath().GetCompletePath(projectName);
          local.ServerPath = completePath;
        }
        else
        {
          string embeddedProjectGuid = obj2.ServerPath.GetEmbeddedProjectGuid();
          Guid empty = Guid.Empty;
          ref Guid local1 = ref empty;
          if (Guid.TryParse(embeddedProjectGuid, out local1))
          {
            string projectName1 = ProjectServiceHelper.GetProjectName(requestContext, empty, projectMapCache);
            if (!string.IsNullOrEmpty(projectName1))
            {
              ref T local2 = ref obj2;
              obj1 = default (T);
              if ((object) obj1 == null)
              {
                obj1 = local2;
                local2 = ref obj1;
              }
              string str = obj2.ServerPath.ReplaceProjectName(projectName1);
              local2.ServerPath = str;
            }
          }
        }
      }
    }
  }
}
