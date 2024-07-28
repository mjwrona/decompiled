// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TFVCPathHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.Build.Server
{
  public class TFVCPathHelper
  {
    public static string ConvertToPathWithProjectGuid(
      IVssRequestContext requestContext,
      string path)
    {
      return TFVCPathHelper.ConvertToPathWithProjectGuid((Func<string, Guid>) (x => TFVCPathHelper.GetProjectIdFromName(requestContext, x)), path);
    }

    public static string ConvertToPathWithProjectGuid(
      Func<string, Guid> getProjectIdFromName,
      string path)
    {
      if (!string.IsNullOrEmpty(path))
      {
        if (path.StartsWith("$/"))
        {
          try
          {
            path = VersionControlPath.GetFullPath(path);
          }
          catch (InvalidPathException ex)
          {
            return path;
          }
          Guid empty = Guid.Empty;
          string pathWithProjectGuid = path;
          if (!VersionControlPath.Equals(path, "$/"))
          {
            string teamProjectName = VersionControlPath.GetTeamProjectName(path);
            if (teamProjectName != null)
            {
              try
              {
                pathWithProjectGuid = "$/" + getProjectIdFromName(teamProjectName).ToString() + path.Substring(teamProjectName.Length + "$/".Length);
              }
              catch (ProjectDoesNotExistWithNameException ex)
              {
              }
            }
          }
          return pathWithProjectGuid;
        }
      }
      return path;
    }

    public static string ConvertToPathWithProjectName(
      IVssRequestContext requestContext,
      string path)
    {
      return TFVCPathHelper.ConvertToPathWithProjectName((Func<Guid, string>) (x => TFVCPathHelper.GetProjectNameFromId(requestContext, x)), path);
    }

    public static string ConvertToPathWithProjectName(
      Func<Guid, string> getProjectNameFromId,
      string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      string pathWithProjectName = path;
      if (pathWithProjectName.StartsWith("$/") && !VersionControlPath.Equals(path, "$/"))
      {
        string teamProjectName = VersionControlPath.GetTeamProjectName(path);
        if (teamProjectName != null)
        {
          Guid result;
          if (Guid.TryParse(teamProjectName, out result))
          {
            try
            {
              pathWithProjectName = "$/" + getProjectNameFromId(result) + path.Substring(36 + "$/".Length);
            }
            catch (ProjectDoesNotExistException ex)
            {
            }
          }
        }
      }
      return pathWithProjectName;
    }

    private static Guid GetProjectIdFromName(IVssRequestContext requestContext, string projectName) => requestContext.GetService<IProjectService>().GetProjectId(requestContext.Elevate(), projectName);

    private static string GetProjectNameFromId(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), projectId);
  }
}
