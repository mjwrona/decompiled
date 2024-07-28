// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types.UpstreamVersionsDataVersion
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types
{
  public class UpstreamVersionsDataVersion : PackagingSecuredObject
  {
    [DataMember]
    public string DisplayVersion { get; set; }

    [DataMember]
    public string NormalizedVersion { get; set; }

    [DataMember]
    public IList<UpstreamVersionsDataVersionUpstream> Upstreams { get; set; }

    [DataMember]
    public UpstreamVersionsDataVersionLocalInstance LocalInstance { get; set; }

    [DataMember]
    public Guid? SelectedUpstreamId { get; set; }

    [DataMember]
    public bool IsLocal { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.LocalInstance?.SetSecuredObject(securedObject);
      foreach (PackagingSecuredObject upstream in (IEnumerable<UpstreamVersionsDataVersionUpstream>) this.Upstreams)
        upstream.SetSecuredObject(securedObject);
    }
  }
}
