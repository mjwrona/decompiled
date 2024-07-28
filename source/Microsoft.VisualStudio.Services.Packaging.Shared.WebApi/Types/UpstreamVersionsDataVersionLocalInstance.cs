// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types.UpstreamVersionsDataVersionLocalInstance
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types
{
  public class UpstreamVersionsDataVersionLocalInstance : PackagingSecuredObject
  {
    [DataMember]
    public IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }

    [DataMember]
    public PackageOrigin Origin { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      foreach (PackagingSecuredObject packagingSecuredObject in this.SourceChain)
        packagingSecuredObject.SetSecuredObject(securedObject);
    }
  }
}
