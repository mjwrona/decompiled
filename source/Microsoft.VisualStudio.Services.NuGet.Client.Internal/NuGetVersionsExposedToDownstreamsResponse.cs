// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.NuGetVersionsExposedToDownstreamsResponse
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  public class NuGetVersionsExposedToDownstreamsResponse : PackagingSecuredObject
  {
    [DataMember]
    public IReadOnlyList<string> Versions { get; set; }

    [DataMember]
    public IReadOnlyList<NuGetRawVersionWithSourceChainAndListed> VersionInfo { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      foreach (PackagingSecuredObject packagingSecuredObject in (IEnumerable<NuGetRawVersionWithSourceChainAndListed>) this.VersionInfo)
        packagingSecuredObject.SetSecuredObject(securedObject);
    }
  }
}
