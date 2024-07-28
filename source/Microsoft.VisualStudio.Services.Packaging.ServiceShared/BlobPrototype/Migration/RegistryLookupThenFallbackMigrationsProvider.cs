// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration.RegistryLookupThenFallbackMigrationsProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration
{
  public class RegistryLookupThenFallbackMigrationsProvider : MigrationDefinitionsProviderBase
  {
    private readonly IRegistryService registryService;
    private readonly IMigrationDefinitionsProvider mainProvider;

    public RegistryLookupThenFallbackMigrationsProvider(
      IRegistryService registryService,
      IMigrationDefinitionsProvider mainProvider)
    {
      this.registryService = registryService;
      this.mainProvider = mainProvider;
    }

    public override IEnumerable<MigrationDefinition> GetMigrations() => this.mainProvider.GetMigrations();

    public override MigrationDefinition GetDefaultMigration(IProtocol protocol, FeedId feedId)
    {
      MigrationDefinition migrationDefinition = (MigrationDefinition) null;
      if ((GuidBasedId) feedId != (GuidBasedId) null)
      {
        string migrationName = this.registryService.GetValue<string>(new RegistryQuery(string.Format("{0}/{1}/{2}", (object) CodeOnlyDeploymentsConstants.DefaultMigrationQueryPrefix, (object) protocol, (object) feedId)), (string) null);
        if (migrationName != null)
          migrationDefinition = this.mainProvider.GetMigration(migrationName, protocol);
      }
      string migrationName1 = this.registryService.GetValue<string>(new RegistryQuery(string.Format("{0}/{1}", (object) CodeOnlyDeploymentsConstants.DefaultMigrationQueryPrefix, (object) protocol)), (string) null);
      if (migrationDefinition == null && migrationName1 != null)
        migrationDefinition = this.mainProvider.GetMigration(migrationName1, protocol);
      return migrationDefinition ?? this.mainProvider.GetDefaultMigration(protocol, feedId);
    }
  }
}
