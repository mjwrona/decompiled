// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API
{
  [DataContract]
  public class Package : PackagingSecuredObject
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeletedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? PermanentlyDeletedDate { get; set; }

    [DataMember]
    public IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }

    [DataMember]
    public bool? Listed { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IEnumerable<UpstreamSourceInfo> sourceChain = this.SourceChain;
      if (sourceChain != null)
        sourceChain.ToSecuredObject<UpstreamSourceInfo>(securedObject);
      if (this.Links == null)
        return;
      ReferenceLinks target = new ReferenceLinks();
      this.Links.CopyTo(target, securedObject);
      this.Links = target;
    }
  }
}
