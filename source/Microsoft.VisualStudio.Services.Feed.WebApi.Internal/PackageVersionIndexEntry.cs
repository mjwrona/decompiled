// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.PackageVersionIndexEntry
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal
{
  [DataContract]
  public class PackageVersionIndexEntry
  {
    [DataMember]
    public string NormalizedVersion { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public string SortableVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StorageId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? PublishDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProtocolMetadata VersionProtocolMetadata { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Summary { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<string> Tags { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<PackageDependency> Dependencies { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsRelease { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<PackageFile> Files { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsCached { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<UpstreamSource> SourceChain { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Provenance Provenance { get; set; }
  }
}
