// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRepositoryRef
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitRepositoryRef : VersionControlSecuredObject
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool IsFork { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public string RemoteUrl { get; set; }

    [DataMember]
    public string SshUrl { get; set; }

    [DataMember(Name = "Project")]
    public TeamProjectReference ProjectReference { get; set; }

    [DataMember]
    public TeamProjectCollectionReference Collection { get; set; }
  }
}
