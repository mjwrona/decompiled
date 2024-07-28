// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.CoreIndexingExecutionContext
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public class CoreIndexingExecutionContext : ExecutionContext, ICoreIndexingExecutionContext
  {
    private IIndexingUnitDataAccess m_indexingUnitDataAccess;

    public CoreIndexingExecutionContext()
    {
    }

    public CoreIndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      IDataAccessFactory dataAccessFactory)
      : base(requestContext, tracerCICorrelationDetails)
    {
      this.Initialize(indexingUnit, dataAccessFactory, (IIndexingUnitChangeEventHandler) null);
    }

    public CoreIndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails)
      : base(requestContext, tracerCICorrelationDetails)
    {
      this.Initialize(indexingUnit, (IDataAccessFactory) null, (IIndexingUnitChangeEventHandler) null);
    }

    public CoreIndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(requestContext, tracerCICorrelationDetails)
    {
      this.Initialize(indexingUnit, (IDataAccessFactory) null, indexingUnitChangeEventHandler);
    }

    public Microsoft.VisualStudio.Services.Search.Common.IndexingUnit IndexingUnit { get; set; }

    public virtual IIndexingUnitDataAccess IndexingUnitDataAccess
    {
      get
      {
        if (this.m_indexingUnitDataAccess == null)
          this.m_indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
        return this.m_indexingUnitDataAccess;
      }
      set => this.m_indexingUnitDataAccess = value;
    }

    public virtual ICorePipelineFailureHandler OperationFailureHandler { get; set; }

    public IIndexingUnitChangeEventHandler IndexingUnitChangeEventHandler { get; set; }

    public string CollectionName
    {
      get
      {
        if (this.CollectionIndexingUnit == null)
          return this.RequestContext.GetCollectionName();
        if (string.IsNullOrWhiteSpace(this.CollectionIndexingUnit.Properties?.Name))
        {
          if (!(this.CollectionIndexingUnit.TFSEntityAttributes is CollectionAttributes) || string.IsNullOrWhiteSpace((this.CollectionIndexingUnit.TFSEntityAttributes as CollectionAttributes).CollectionName))
            return this.RequestContext.GetCollectionName();
          return !(this.CollectionIndexingUnit.TFSEntityAttributes is CollectionAttributes entityAttributes) ? (string) null : entityAttributes.CollectionName;
        }
        return this.CollectionIndexingUnit.Properties?.Name;
      }
    }

    public virtual Guid CollectionId => this.CollectionIndexingUnit.TFSEntityId;

    public Microsoft.VisualStudio.Services.Search.Common.IndexingUnit CollectionIndexingUnit { get; set; }

    public Microsoft.VisualStudio.Services.Search.Common.IndexingUnit OrganizationIndexingUnit { get; set; }

    public CoreIndexingExecutionContext.OutputLog Log { get; set; }

    public IDataAccessFactory DataAccessFactory { get; private set; }

    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Supported entity types do not make sense in entity agnostic class")]
    public virtual IEntityType[] SupportedEntityTypes => throw new NotImplementedException();

    public virtual void InitializeNameAndIds(IDataAccessFactory dataAccessFactory)
    {
      dataAccessFactory.GetIndexingUnitDataAccess();
      switch (this.IndexingUnit.IndexingUnitType)
      {
        case "Organization":
          this.OrganizationIndexingUnit = this.IndexingUnit;
          break;
        case "Collection":
          this.CollectionIndexingUnit = this.IndexingUnit;
          break;
      }
    }

    private void Initialize(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      this.IndexingUnit = indexingUnit;
      this.DataAccessFactory = dataAccessFactory ?? Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance();
      this.Log = new CoreIndexingExecutionContext.OutputLog();
      this.IndexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
    }

    public class OutputLog
    {
      private readonly StringBuilder m_stringBuilder;

      public OutputLog() => this.m_stringBuilder = new StringBuilder();

      public void Append(string s) => this.m_stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("[{0}]", (object) s)));

      public string Content => this.m_stringBuilder.ToString();
    }
  }
}
