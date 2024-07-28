// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities.BlockedPackageIdentityToExceptionConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities
{
  public class BlockedPackageIdentityToExceptionConverter : 
    IConverter<IPackageIdentity, Exception>,
    IHaveInputType<IPackageIdentity>,
    IHaveOutputType<Exception>
  {
    private readonly IReadOnlyList<IBlockedPackageIdentityProvider> providers;
    private readonly IFactory<string, Exception> exceptionFactory;

    public BlockedPackageIdentityToExceptionConverter(
      IFactory<string, Exception> exceptionFactory,
      IReadOnlyList<IBlockedPackageIdentityProvider> providers)
    {
      this.providers = providers;
      this.exceptionFactory = exceptionFactory;
    }

    public Exception Convert(IPackageIdentity request)
    {
      foreach (IBlockedPackageIdentityProvider provider in (IEnumerable<IBlockedPackageIdentityProvider>) this.providers)
      {
        bool allVersionsBlocked;
        if (provider.IsIdentityBlocked(request, out allVersionsBlocked))
          return this.exceptionFactory.Get(allVersionsBlocked ? Resources.Error_PackageBlockedDueToSecurityIncidentAllVersions((object) request.Name.DisplayName) : Resources.Error_PackageBlockedDueToSecurityIncident((object) request.DisplayStringForMessages));
      }
      return (Exception) null;
    }
  }
}
