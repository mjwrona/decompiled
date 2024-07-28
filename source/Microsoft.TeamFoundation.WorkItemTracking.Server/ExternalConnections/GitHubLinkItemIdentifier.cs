// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.GitHubLinkItemIdentifier
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  [DataContract]
  public class GitHubLinkItemIdentifier
  {
    [DataMember]
    public GitHubLinkItemType ItemType { get; set; }

    [DataMember]
    public Guid RepoInternalId { get; set; }

    [DataMember]
    public string RepoExternalId { get; set; }

    [DataMember]
    public string NumberOrSHA { get; set; }

    [DataMember]
    public string Url { get; set; }

    [IgnoreDataMember]
    public string HostName { get; set; }

    [IgnoreDataMember]
    public string RepoNameWithOwner { get; set; }
  }
}
