// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackagingEtwTracesUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class PackagingEtwTracesUtils
  {
    public static void TraceMigrationState(
      MigrationState state,
      IProtocol protocol,
      ICache<string, object> cache,
      IMigrationDefinitionsProvider provider)
    {
      cache.Set("Packaging.DataCurrentVersion", (object) state.CurrentMigration);
      cache.Set("Packaging.DataDestinationVersion", (object) state.VNextMigration);
      cache.Set("Packaging.DataMigrationState", (object) state.VNextState);
      if (!PackagingEtwTracesUtils.GetConsumedReadMigration(state, provider, protocol).IsDeprecated)
        return;
      cache.Set("Packaging.Properties.DataMigrationIsDeprecated", (object) true);
    }

    private static MigrationDefinition GetConsumedReadMigration(
      MigrationState state,
      IMigrationDefinitionsProvider provider,
      IProtocol protocol)
    {
      string migrationName = state.VNextState == MigrationStateEnum.ReadVNext ? state.VNextMigration : state.CurrentMigration;
      return provider.GetMigration(migrationName, protocol);
    }
  }
}
