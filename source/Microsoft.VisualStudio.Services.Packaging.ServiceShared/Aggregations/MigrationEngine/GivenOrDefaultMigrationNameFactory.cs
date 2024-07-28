// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine.GivenOrDefaultMigrationNameFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine
{
  public class GivenOrDefaultMigrationNameFactory : IFactory<IProtocol, string>
  {
    private readonly IMigrationDefinitionsProvider migrationProvider;
    private readonly string destinationMigration;

    public GivenOrDefaultMigrationNameFactory(
      IMigrationDefinitionsProvider migrationProvider,
      string destinationMigration)
    {
      this.migrationProvider = migrationProvider;
      this.destinationMigration = destinationMigration;
    }

    public string Get(IProtocol protocol) => !string.IsNullOrEmpty(this.destinationMigration) ? this.destinationMigration : this.migrationProvider.GetDefaultMigration(protocol, (FeedId) null).Name;
  }
}
