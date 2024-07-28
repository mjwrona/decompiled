// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetPackageNameParsingConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetPackageNameParsingConverterBootstrapper : 
    IBootstrapper<IConverter<string, VssNuGetPackageName>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetPackageNameParsingConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<string, VssNuGetPackageName> Bootstrap() => (IConverter<string, VssNuGetPackageName>) new PopulateRequestContextItemDelegatingConverter<string, VssNuGetPackageName>(this.requestContext, "Packaging.PackageName", NuGetIdentityResolver.Instance.NameResolver);
  }
}
