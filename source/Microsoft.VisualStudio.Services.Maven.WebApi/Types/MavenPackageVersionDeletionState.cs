// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPackageVersionDeletionState
// Assembly: Microsoft.VisualStudio.Services.Maven.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62CDE373-A3CE-478E-B824-A307191D9BE2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.WebApi.Types
{
  [DataContract]
  [ClientIncludeModel]
  public class MavenPackageVersionDeletionState
  {
    public MavenPackageVersionDeletionState(
      string groupId,
      string artifactId,
      string version,
      DateTime deletedDate)
    {
      this.GroupId = groupId;
      this.ArtifactId = artifactId;
      this.Version = version;
      this.DeletedDate = deletedDate;
    }

    [DataMember(Name = "groupId", IsRequired = true)]
    public string GroupId { get; set; }

    [DataMember(Name = "artifactId", IsRequired = true)]
    public string ArtifactId { get; set; }

    [DataMember(Name = "version", IsRequired = true)]
    public string Version { get; set; }

    [DataMember(Name = "deletedDate", IsRequired = true)]
    public DateTime DeletedDate { get; set; }
  }
}
