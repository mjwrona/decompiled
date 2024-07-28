// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities.BlockedPackageIdentityToExceptionConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities
{
  public class BlockedPackageIdentityToExceptionConverterBootstrapper : 
    IBootstrapper<IConverter<IPackageIdentity, Exception>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly BlockedIdentityContext context;

    public BlockedPackageIdentityToExceptionConverterBootstrapper(
      IVssRequestContext requestContext,
      BlockedIdentityContext context = BlockedIdentityContext.Download)
    {
      this.requestContext = requestContext;
      this.context = context;
    }

    public IConverter<IPackageIdentity, Exception> Bootstrap()
    {
      if (this.context == BlockedIdentityContext.Delete)
        return ByFuncConverter.Create<IPackageIdentity, Exception>((Func<IPackageIdentity, Exception>) (_ => (Exception) null));
      IFactory<IProtocol, PrecomputedBlockedPackageIdentityProvider> registryBlockedIdentityProviderFactory = new BlockedPackagesRegistryReader((IDeploymentLevelRegistryService) new DeploymentLevelRegistryServiceFacade(this.requestContext)).ConvertBy<IProtocol, IEnumerable<BlockedPackageIdentity>, PrecomputedBlockedPackageIdentityProvider>((Func<IEnumerable<BlockedPackageIdentity>, PrecomputedBlockedPackageIdentityProvider>) (blockedPackages => new PrecomputedBlockedPackageIdentityProvider(blockedPackages)));
      IFactory<string, Exception> exceptionFactory = ByFuncInputFactory.For<string, Exception>((Func<string, Exception>) (message =>
      {
        switch (this.context)
        {
          case BlockedIdentityContext.Download:
          case BlockedIdentityContext.Update:
            return (Exception) new PackageBlockedException(message);
          case BlockedIdentityContext.Upload:
            return (Exception) new PackageBlockedPublishException(message);
          case BlockedIdentityContext.Delete:
            return (Exception) null;
          default:
            throw new ArgumentOutOfRangeException("context", (object) this.context, "Unknown BlockedIdentityContext");
        }
      }));
      IFactory<IProtocol, IConverter<IPackageIdentity, Exception>> mainFactory = new ByFuncInputFactory<IProtocol, IConverter<IPackageIdentity, Exception>>((Func<IProtocol, IConverter<IPackageIdentity, Exception>>) (protocol => (IConverter<IPackageIdentity, Exception>) new BlockedPackageIdentityToExceptionConverter(exceptionFactory, (IReadOnlyList<IBlockedPackageIdentityProvider>) new PrecomputedBlockedPackageIdentityProvider[2]
      {
        new PrecomputedBlockedPackageIdentityProvider((IEnumerable<BlockedPackageIdentity>) protocol.PermanentlyBlockedPackageIdentities),
        registryBlockedIdentityProviderFactory.Get(protocol)
      }))).SingleElementCache<IProtocol, IConverter<IPackageIdentity, Exception>>();
      return ConvertFrom.InputTypeOf<IPackageIdentity>((IBootstrapper<IHaveInputType<IPackageIdentity>>) this).By<IPackageIdentity, Exception>((Func<IPackageIdentity, Exception>) (input => mainFactory.Get(input.Name.Protocol).Convert(input)));
    }
  }
}
