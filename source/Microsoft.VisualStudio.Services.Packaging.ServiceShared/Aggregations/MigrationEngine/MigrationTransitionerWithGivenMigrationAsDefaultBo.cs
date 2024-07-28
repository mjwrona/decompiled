// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine.MigrationTransitionerWithGivenMigrationAsDefaultBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine
{
  public class MigrationTransitionerWithGivenMigrationAsDefaultBootstrapper : 
    IBootstrapper<IMigrationTransitionerInternal>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IMigrationDefinitionsProvider migrationsDefinitionsProvider;
    private readonly string defaultMigration;

    public MigrationTransitionerWithGivenMigrationAsDefaultBootstrapper(
      IVssRequestContext requestContext,
      IMigrationDefinitionsProvider migrationsDefinitionsProvider,
      string defaultMigration)
    {
      this.requestContext = requestContext;
      this.migrationsDefinitionsProvider = migrationsDefinitionsProvider;
      this.defaultMigration = defaultMigration;
    }

    public IMigrationTransitionerInternal Bootstrap() => new NoCachingMigrationTransitionerBootstrapper(this.requestContext, (IMigrationDefinitionsProvider) new GivenMigrationAsDefaultMigrationsProvider(this.migrationsDefinitionsProvider, this.defaultMigration)).Bootstrap();
  }
}
