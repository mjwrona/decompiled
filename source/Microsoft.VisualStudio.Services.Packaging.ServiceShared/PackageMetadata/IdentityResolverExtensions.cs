// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.IdentityResolverExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public static class IdentityResolverExtensions
  {
    public static IConverter<IRawPackageRequest, TPackageIdentity> GetRawPackageRequestToIdentityConverter<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName>(
      this IIdentityResolver<TPackageName, TPackageVersion, TPackageIdentity, TPackageFileName> identityResolver,
      IVssRequestContext requestContext)
      where TPackageName : class, IPackageName
      where TPackageVersion : class, IPackageVersion
      where TPackageIdentity : class, IPackageIdentity<TPackageName, TPackageVersion>
      where TPackageFileName : class, IPackageFileName
    {
      return new RequestContextPopulatingRawRequestToIdentityConverterBootstrapper<IRawPackageRequest, TPackageIdentity>(requestContext, ByFuncConverter.Create<IRawPackageRequest, TPackageIdentity>((Func<IRawPackageRequest, TPackageIdentity>) (rawRequest => identityResolver.ResolvePackageIdentity(rawRequest.PackageName, rawRequest.PackageVersion)))).Bootstrap();
    }
  }
}
