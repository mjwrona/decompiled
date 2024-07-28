// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types.VersionsExposedToDownstreamsResponse
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types
{
  [DataContract]
  public class VersionsExposedToDownstreamsResponse : PackagingSecuredObject
  {
    [DataMember]
    public IReadOnlyList<string> Versions { get; set; }

    [DataMember]
    public IReadOnlyList<RawVersionWithSourceChain> VersionInfo { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      foreach (PackagingSecuredObject packagingSecuredObject in (IEnumerable<RawVersionWithSourceChain>) this.VersionInfo)
        packagingSecuredObject.SetSecuredObject(securedObject);
    }
  }
}
