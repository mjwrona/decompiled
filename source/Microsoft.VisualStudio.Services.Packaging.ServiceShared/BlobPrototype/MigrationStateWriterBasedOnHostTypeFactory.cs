// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationStateWriterBasedOnHostTypeFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationStateWriterBasedOnHostTypeFactory : IFactory<IMigrationStateWriter>
  {
    private readonly IFactory<IVssRequestContext, IFactory<CollectionId, IMigrationStateWriter>> migrationStateWriterFromRequestContextFactory;
    private readonly IFactory<CollectionId, IVssRequestContext> collectionRequestContextFactory;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IHostInfoService hostInfoService;

    public MigrationStateWriterBasedOnHostTypeFactory(
      IFactory<IVssRequestContext, IFactory<CollectionId, IMigrationStateWriter>> migrationStateWriterFromRequestContextFactory,
      IFactory<CollectionId, IVssRequestContext> collectionRequestContextFactory,
      IExecutionEnvironment executionEnvironment,
      IHostInfoService hostInfoService)
    {
      this.migrationStateWriterFromRequestContextFactory = migrationStateWriterFromRequestContextFactory;
      this.collectionRequestContextFactory = collectionRequestContextFactory;
      this.executionEnvironment = executionEnvironment;
      this.hostInfoService = hostInfoService;
    }

    public IMigrationStateWriter Get() => this.executionEnvironment.IsHostType(TeamFoundationHostType.ProjectCollection) ? this.migrationStateWriterFromRequestContextFactory.Get((IVssRequestContext) null).Get((CollectionId) this.executionEnvironment.HostId) : (IMigrationStateWriter) new MultiCollectionMigrationStateWriter(this.hostInfoService, this.migrationStateWriterFromRequestContextFactory, this.collectionRequestContextFactory);
  }
}
