// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.AggregatepathExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class AggregatepathExtensions
  {
    public static string GetProjectName(this string aggregatePath) => AggregatepathExtensions.GetRootElement(aggregatePath);

    public static string GetEmbeddedProjectGuid(this string aggregatePath) => AggregatepathExtensions.GetRootElement(aggregatePath);

    public static string ReplaceProjectName(this string aggregatePath, string projectName)
    {
      if (string.IsNullOrEmpty(projectName))
        return aggregatePath;
      aggregatePath = aggregatePath.Replace('\\', '/');
      if (!aggregatePath.IsServerItem() || !aggregatePath.HasProjectName())
        return aggregatePath;
      string str = string.Format("$/{0}/", (object) projectName);
      return !aggregatePath.StartsWith(str) ? aggregatePath.GetRelativePath().GetCompletePath(projectName) : aggregatePath;
    }

    public static string GetRelativePath(this string aggregatePath)
    {
      aggregatePath = aggregatePath.Replace('\\', '/');
      if (aggregatePath.IsServerItem())
      {
        string str = aggregatePath.Substring(2);
        int startIndex = str.IndexOf('/');
        if (startIndex > 0)
          return str.Substring(startIndex);
      }
      return string.Empty;
    }

    public static string GetCompletePath(
      this string aggregatePath,
      IVssRequestContext requestContext,
      Guid projectGuid)
    {
      if (aggregatePath.IsServerItem())
        return aggregatePath;
      if (projectGuid.Equals(Guid.Empty))
        return aggregatePath;
      try
      {
        string projectName = ProjectServiceHelper.GetProjectName(requestContext, projectGuid);
        if (!string.IsNullOrEmpty(projectName))
          return string.Format("$/{0}{1}", (object) projectName, (object) aggregatePath);
      }
      catch (Exception ex)
      {
        string.Format("GetCompletePath() failed with the following exception : {0}", (object) ex.ToString());
      }
      return aggregatePath;
    }

    public static string ReplaceProjectNameWithGuid(
      this string serverPath,
      IVssRequestContext requestContext,
      ProjectMapCache projectMapCache)
    {
      string projectName = serverPath.GetProjectName();
      if (!string.IsNullOrEmpty(projectName))
      {
        Guid projectId = ProjectServiceHelper.GetProjectId(requestContext, projectName, projectMapCache);
        if (!projectId.Equals(Guid.Empty))
          return serverPath.GetRelativePath().GetCompletePath(projectId.ToString());
      }
      return serverPath;
    }

    public static string ReplaceProjectGuidWithName(
      this string serverPath,
      IVssRequestContext requestContext,
      ProjectMapCache projectMapCache)
    {
      string projectName1 = serverPath.GetProjectName();
      if (!string.IsNullOrEmpty(projectName1))
      {
        Guid result = Guid.Empty;
        if (Guid.TryParse(projectName1, out result))
        {
          string projectName2 = ProjectServiceHelper.GetProjectName(requestContext, result, projectMapCache);
          if (!string.IsNullOrEmpty(projectName2))
            return serverPath.GetRelativePath().GetCompletePath(projectName2);
        }
      }
      return serverPath;
    }

    public static string GetCompletePath(this string aggregatePath, string projectName) => aggregatePath.IsServerItem() ? aggregatePath : string.Format("$/{0}{1}", (object) projectName, (object) aggregatePath);

    private static string GetRootElement(string path)
    {
      path = path.Replace('\\', '/');
      if (path.IsServerItem() && path.HasProjectName())
      {
        string str = path.Substring(2);
        int length = str.IndexOf('/');
        if (length > 0)
          return str.Substring(0, length);
      }
      return string.Empty;
    }

    private static bool IsServerItem(this string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path));
      return path.Length >= 2 && path[0] == '$' && path[1] == '/';
    }

    private static bool HasProjectName(this string path)
    {
      path = path.Replace('\\', '/');
      return path.IsServerItem() && path.IndexOf('/', 2) > 0;
    }
  }
}
