// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.MigrationDefinitionsProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class MigrationDefinitionsProviderBootstrapper : 
    IBootstrapper<IMigrationDefinitionsProvider>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IMigrationDefinitionsProvider protocolProvider;

    public MigrationDefinitionsProviderBootstrapper(
      IVssRequestContext requestContext,
      IMigrationDefinitionsProvider protocolProvider)
    {
      this.requestContext = requestContext;
      this.protocolProvider = protocolProvider;
    }

    public IMigrationDefinitionsProvider Bootstrap() => (IMigrationDefinitionsProvider) new RegistryLookupThenFallbackMigrationsProvider((IRegistryService) new RegistryServiceFacade(this.requestContext), this.protocolProvider);
  }
}
