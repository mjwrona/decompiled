// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRemoteUrlUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitRemoteUrlUtility
  {
    public static IEnumerable<string> GetRepositoryRemoteUrls(
      this GitRepository repo,
      IVssRequestContext rc)
    {
      string repoName = repo.Name;
      string repoId = repo.Id.ToString();
      string projectName = repo.ProjectReference.Name;
      string projectId = repo.ProjectReference.Id.ToString();
      ILocationService locationService = rc.GetService<ILocationService>();
      foreach (AccessMapping accessMapping in locationService.GetAccessMappings(rc))
      {
        string collectionBaseUrl = locationService.GetSelfReferenceUrl(rc, accessMapping);
        string[] strArray1 = new string[3]
        {
          "",
          "/_full",
          "/_optimized"
        };
        for (int index1 = 0; index1 < strArray1.Length; ++index1)
        {
          string endpoint = strArray1[index1];
          string[] strArray2 = new string[2]
          {
            repoName,
            repoId
          };
          for (int index2 = 0; index2 < strArray2.Length; ++index2)
          {
            string repoValue = strArray2[index2];
            string[] strArray3 = new string[2]
            {
              projectName,
              projectId
            };
            for (int index3 = 0; index3 < strArray3.Length; ++index3)
            {
              string projectValue = strArray3[index3];
              yield return collectionBaseUrl + projectValue + "/_git" + endpoint + "/" + repoValue;
              yield return collectionBaseUrl + projectValue + "/[TEAM]/_git" + endpoint + "/" + repoValue;
              projectValue = (string) null;
            }
            strArray3 = (string[]) null;
            repoValue = (string) null;
          }
          strArray2 = (string[]) null;
          endpoint = (string) null;
        }
        strArray1 = (string[]) null;
        collectionBaseUrl = (string) null;
      }
    }
  }
}
