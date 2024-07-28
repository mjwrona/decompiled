// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3PackageVersionsInternalToPublicConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class V3PackageVersionsInternalToPublicConverter : 
    IConverter<NuGetVersionsExposedToDownstreamsResponse, PackageVersionsResponse>,
    IHaveInputType<NuGetVersionsExposedToDownstreamsResponse>,
    IHaveOutputType<PackageVersionsResponse>
  {
    public PackageVersionsResponse Convert(NuGetVersionsExposedToDownstreamsResponse input)
    {
      PackageVersionsResponse versionsResponse = new PackageVersionsResponse();
      versionsResponse.Versions = input.Versions.ToArray<string>();
      versionsResponse.SetSecuredObject((ISecuredObject) input);
      return versionsResponse;
    }
  }
}
