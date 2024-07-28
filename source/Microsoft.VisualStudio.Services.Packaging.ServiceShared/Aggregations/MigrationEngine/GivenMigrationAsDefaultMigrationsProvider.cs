// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine.GivenMigrationAsDefaultMigrationsProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine
{
  public class GivenMigrationAsDefaultMigrationsProvider : MigrationDefinitionsProviderBase
  {
    private readonly IMigrationDefinitionsProvider migrationsProvider;
    private readonly string defaultMigration;

    public GivenMigrationAsDefaultMigrationsProvider(
      IMigrationDefinitionsProvider migrationsProvider,
      string defaultMigration)
    {
      this.migrationsProvider = migrationsProvider;
      this.defaultMigration = defaultMigration;
    }

    public override MigrationDefinition GetDefaultMigration(IProtocol protocol, FeedId feedId) => this.migrationsProvider.GetMigration(this.defaultMigration, protocol);

    public override IEnumerable<MigrationDefinition> GetMigrations() => (IEnumerable<MigrationDefinition>) this.migrationsProvider.GetMigrations().ToList<MigrationDefinition>();
  }
}
