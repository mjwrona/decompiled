// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Package
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
  public class Package : FeedSecuredObject, IPackageInfo
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string NormalizedName { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ProtocolType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? StarCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsCached { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<MinimalPackageVersion> Versions { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }
  }
}
