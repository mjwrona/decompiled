// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts.SkeletalPackageVersion
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts
{
  [DataContract]
  public class SkeletalPackageVersion : FeedSecuredObject
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public DateTime PublishDate { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public string NormalizedVersion { get; set; }

    [DataMember]
    public int DownloadCount { get; set; }

    [DataMember]
    public int UserCount { get; set; }

    [DataMember]
    public Guid? DirectUpstreamSourceId { get; set; }

    [DataMember]
    public List<string> Views { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public bool IsListed { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }
  }
}
