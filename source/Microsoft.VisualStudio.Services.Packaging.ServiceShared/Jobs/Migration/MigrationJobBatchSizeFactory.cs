// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration.MigrationJobBatchSizeFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration
{
  public class MigrationJobBatchSizeFactory : IFactory<int>
  {
    public const string CodeOnlyMigrationJobBatchSize = "/Configuration/Packaging/CodeOnly/MigrationJobBatchSize";
    public const int DefaultBatchSize = 20000;
    private readonly IRegistryService registryService;

    public MigrationJobBatchSizeFactory(IRegistryService registryService) => this.registryService = registryService;

    public int Get() => this.registryService.GetValue<int>((RegistryQuery) "/Configuration/Packaging/CodeOnly/MigrationJobBatchSize", 20000);
  }
}
