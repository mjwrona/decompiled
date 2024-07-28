// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Serialization.TfsGitRepositoryInfo
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Serialization
{
  [XmlType("TfsGitRepositoryInfo")]
  [ClientType("TfsGitRepositoryInfo")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class TfsGitRepositoryInfo
  {
    public TfsGitRepositoryInfo()
    {
    }

    public TfsGitRepositoryInfo(Microsoft.TeamFoundation.Git.Server.TfsGitRepositoryInfo info, string remoteUrl)
    {
      this.Name = info.Name;
      this.RepositoryId = info.Key.RepoId;
      this.TeamProjectUri = info.Key.GetProjectUri();
      this.RemoteUrl = remoteUrl;
    }

    public string Name { get; set; }

    public Guid RepositoryId { get; set; }

    public string TeamProjectUri { get; set; }

    public string RemoteUrl { get; set; }
  }
}
