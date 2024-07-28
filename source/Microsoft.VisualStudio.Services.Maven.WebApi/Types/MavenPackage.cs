// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.WebApi.Types.MavenPackage
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
  public class MavenPackage
  {
    public MavenPackage(string groupId, string artifactId, string version)
    {
      this.GroupId = groupId;
      this.ArtifactId = artifactId;
      this.Version = version;
      if (this.HasVersion())
        this.Files = new ReferenceLinks();
      else
        this.Versions = new ReferenceLinks();
    }

    [DataMember(Name = "groupId", IsRequired = true)]
    public string GroupId { get; set; }

    [DataMember(Name = "artifactId", IsRequired = true)]
    public string ArtifactId { get; set; }

    [DataMember(Name = "version", EmitDefaultValue = false)]
    public string Version { get; set; }

    [DataMember(Name = "versionsIndex", EmitDefaultValue = false)]
    public ReferenceLink VersionsIndex { get; set; }

    [DataMember(Name = "artifactIndex", EmitDefaultValue = false)]
    public ReferenceLink ArtifactIndex { get; set; }

    [DataMember(Name = "artifactMetadata", EmitDefaultValue = false)]
    public ReferenceLink ArtifactMetadata { get; set; }

    [DataMember(Name = "deletedDate")]
    public DateTime? DeletedDate { get; set; }

    [DataMember(Name = "requestedFile", EmitDefaultValue = false)]
    public ReferenceLink RequestedFile { get; set; }

    [DataMember(Name = "snapshotMetadata", EmitDefaultValue = false)]
    public ReferenceLink SnapshotMetadata { get; set; }

    [DataMember(Name = "files", EmitDefaultValue = false)]
    public ReferenceLinks Files { get; set; }

    [DataMember(Name = "versions", EmitDefaultValue = false)]
    public ReferenceLinks Versions { get; set; }

    [DataMember(Name = "pom", EmitDefaultValue = false)]
    public MavenPomMetadata Pom { get; set; }

    public bool HasVersion() => !string.IsNullOrWhiteSpace(this.Version);
  }
}
