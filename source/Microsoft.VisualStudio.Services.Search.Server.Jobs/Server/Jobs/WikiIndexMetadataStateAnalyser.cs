// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.WikiIndexMetadataStateAnalyser
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class WikiIndexMetadataStateAnalyser : IndexMetadataStateAnalyser
  {
    public WikiIndexMetadataStateAnalyser()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    public WikiIndexMetadataStateAnalyser(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(dataAccessFactory, indexingUnitChangeEventHandler)
    {
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionWikiFinalizeHelper();

    protected override TraceMetaData TraceMetadata => new TraceMetaData(1080481, "Indexing Pipeline", "IndexingOperation");
  }
}
