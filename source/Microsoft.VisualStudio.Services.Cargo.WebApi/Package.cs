// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API.Package
// Assembly: Microsoft.VisualStudio.Services.Cargo.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79D1C655-766F-4F71-AAEA-7C02E794C2F8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API
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
