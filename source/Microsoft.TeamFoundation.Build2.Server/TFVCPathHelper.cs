// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TFVCPathHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class TFVCPathHelper
  {
    public static string ConvertToPathWithProjectGuid(
      IVssRequestContext requestContext,
      string path)
    {
      if (string.IsNullOrEmpty(path) || !path.StartsWith("$/"))
        return path;
      bool checkCanonicalization = true;
      try
      {
        path = VersionControlPath.GetFullPath(path);
      }
      catch (InvalidPathException ex)
      {
        if (!BuildCommonUtil.VariableInUse(path))
          throw ex;
        requestContext.TraceVerbose(12030079, "TfvcPathHelper", "The path {0} has variables in it, no canonicalization is done, using the path as is.", (object) path);
        checkCanonicalization = false;
      }
      Guid empty = Guid.Empty;
      string pathWithProjectGuid = path;
      if (!VersionControlPath.Equals(path, "$/"))
      {
        string teamProjectName = VersionControlPath.GetTeamProjectName(path, checkCanonicalization: checkCanonicalization);
        if (teamProjectName != null)
        {
          try
          {
            pathWithProjectGuid = "$/" + TFVCPathHelper.GetProjectIdFromName(requestContext, teamProjectName).ToString() + path.Substring(teamProjectName.Length + "$/".Length);
          }
          catch (ProjectDoesNotExistWithNameException ex)
          {
          }
        }
      }
      return pathWithProjectGuid;
    }

    public static string ConvertToPathWithProjectName(
      IVssRequestContext requestContext,
      string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      string pathWithProjectName = path;
      if (pathWithProjectName.StartsWith("$/") && !VersionControlPath.Equals(path, "$/"))
      {
        string teamProjectName = VersionControlPath.GetTeamProjectName(path, checkCanonicalization: !BuildCommonUtil.VariableInUse(path));
        if (teamProjectName != null)
        {
          Guid result;
          if (Guid.TryParse(teamProjectName, out result))
          {
            try
            {
              pathWithProjectName = "$/" + TFVCPathHelper.GetProjectNameFromId(requestContext, result) + path.Substring(36 + "$/".Length);
            }
            catch (ProjectDoesNotExistException ex)
            {
              requestContext.TraceError(nameof (TFVCPathHelper), "Unable to find a project for GUID {0}, so using path {1}", (object) teamProjectName, (object) pathWithProjectName);
            }
          }
        }
      }
      return pathWithProjectName;
    }

    public static string ConvertServerPathToLocalPath(string serverPath, string commonRoot)
    {
      if (string.IsNullOrEmpty(serverPath))
        return serverPath;
      string str1 = serverPath;
      if (str1.IndexOf(commonRoot) == 0)
        str1 = str1.Substring(commonRoot.Length);
      else if (str1.IndexOf("$") == 0)
        str1 = str1.Substring(1);
      string str2 = str1.Replace('/', '\\');
      return !string.IsNullOrEmpty(str2) ? str2 : "\\";
    }

    public static string GetCommonServerPath(IEnumerable<string> serverItems)
    {
      if (serverItems == null)
        return string.Empty;
      using (IEnumerator<string> enumerator = serverItems.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return string.Empty;
        string path1 = enumerator.Current;
        while (enumerator.MoveNext())
        {
          path1 = TFVCPathHelper.GetCommonServerPath(path1, enumerator.Current);
          if (string.IsNullOrEmpty(path1))
            break;
        }
        return path1;
      }
    }

    private static string GetCommonServerPath(string path1, string path2)
    {
      if (!VersionControlPath.IsServerItem(path1) || !VersionControlPath.IsServerItem(path2))
        return "$/";
      string parent;
      string str;
      if (VersionControlPath.GetFolderDepth(path1) >= VersionControlPath.GetFolderDepth(path2))
      {
        parent = path2;
        str = path1;
      }
      else
      {
        parent = path1;
        str = path2;
      }
      while (!VersionControlPath.IsSubItem(str, parent))
        parent = VersionControlPath.GetFolderName(parent);
      return parent;
    }

    private static Guid GetProjectIdFromName(IVssRequestContext requestContext, string projectName) => requestContext.GetService<IProjectService>().GetProjectId(requestContext.Elevate(), projectName);

    private static string GetProjectNameFromId(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), projectId);
  }
}
