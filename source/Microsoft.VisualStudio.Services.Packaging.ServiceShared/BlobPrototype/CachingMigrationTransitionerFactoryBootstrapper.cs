// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CachingMigrationTransitionerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class CachingMigrationTransitionerFactoryBootstrapper : 
    IBootstrapper<IFactory<IProtocol, IMigrationTransitionerInternal>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IMigrationDefinitionsProvider migrationsProvider;

    public CachingMigrationTransitionerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IMigrationDefinitionsProvider migrationsProvider)
    {
      this.requestContext = requestContext;
      this.migrationsProvider = migrationsProvider;
    }

    public IFactory<IProtocol, IMigrationTransitionerInternal> Bootstrap() => (IFactory<IProtocol, IMigrationTransitionerInternal>) new CachingMigrationTransitionerFactory((ICache<string, object>) new RequestContextItemsAsCacheFacade(this.requestContext), (IFactory<IMigrationTransitionerInternal>) new ByFuncFactory<IMigrationTransitionerInternal>((Func<IMigrationTransitionerInternal>) (() => (IMigrationTransitionerInternal) new CachingMigrationTransitioner(new NoCachingMigrationTransitionerBootstrapper(this.requestContext, this.migrationsProvider).Bootstrap(), (ICache<string, MigrationEntry>) new DictionaryAsCache<string, MigrationEntry>()))));
  }
}
