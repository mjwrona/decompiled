// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types.API
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

    [DataMember]
    public string DeprecateMessage { get; set; }

    [DataMember]
    public DateTime? UnpublishedDate { get; set; }

    [DataMember]
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
