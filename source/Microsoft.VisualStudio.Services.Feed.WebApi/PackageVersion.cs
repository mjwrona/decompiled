// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.PackageVersion
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class PackageVersion : MinimalPackageVersion
  {
    public PackageVersion()
    {
      this.AddViews = (IEnumerable<string>) new string[0];
      this.RemoveViews = (IEnumerable<string>) new string[0];
    }

    [DataMember(EmitDefaultValue = false)]
    public string Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeletedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProtocolMetadata ProtocolMetadata { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Summary { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<string> Tags { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<PackageDependency> Dependencies { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<MinimalPackageVersion> OtherVersions { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<PackageFile> Files { get; set; }

    [IgnoreDataMember]
    public long UpdateSequenceNumber { get; set; }

    [IgnoreDataMember]
    public IEnumerable<string> AddViews { get; set; }

    [IgnoreDataMember]
    public IEnumerable<string> RemoveViews { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<UpstreamSource> SourceChain { get; set; }
  }
}
