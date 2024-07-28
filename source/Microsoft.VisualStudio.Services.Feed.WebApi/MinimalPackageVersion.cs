// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.MinimalPackageVersion
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class MinimalPackageVersion : FeedSecuredObject
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string NormalizedVersion { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsLatest { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsListed { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StorageId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PackageDescription { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsCachedVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<FeedView> Views { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? PublishDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? DirectUpstreamSourceId { get; set; }
  }
}
