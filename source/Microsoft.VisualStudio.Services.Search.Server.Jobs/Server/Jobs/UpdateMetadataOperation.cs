// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.UpdateMetadataOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class UpdateMetadataOperation : AbstractIndexingOperation
  {
    private RegistryManager m_registryManager;

    internal UpdateMetadataOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      RegistryManager registryManager)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_registryManager = registryManager;
    }

    public UpdateMetadataOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, new RegistryManager(executionContext.RequestContext, "IndexingOperation"))
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080610, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (this.IndexingUnitChangeEvent.ChangeData is UpdateMetadataEventData changeData && !this.HandleOperation(executionContext, changeData, resultMessage))
        {
          IndexingProperties properties = this.IndexingUnit.Properties;
          switch (changeData.UpdateType)
          {
            case UpdateMetaDataOperationType.SetIndexingState:
              this.UpdateIndexingState(executionContext, properties, changeData, resultMessage);
              break;
            case UpdateMetaDataOperationType.SetIndexContractType:
              this.UpdateIndexContractType(executionContext, properties, changeData, resultMessage);
              break;
            case UpdateMetaDataOperationType.SetESConnectionString:
              this.UpdateESConnectionString(executionContext, properties, changeData, false, resultMessage);
              break;
            case UpdateMetaDataOperationType.AppendESConnectionString:
              this.UpdateESConnectionString(executionContext, properties, changeData, true, resultMessage);
              break;
            case UpdateMetaDataOperationType.SendSqlNotification:
              this.SendSqlNotification(executionContext, resultMessage);
              break;
            case UpdateMetaDataOperationType.ClearIndexingWatermarks:
              this.EraseIndexingWatermarks(executionContext);
              break;
            case UpdateMetaDataOperationType.SetESQueryConnectionString:
              this.UpdateESConnectionString(executionContext, properties, changeData, false, resultMessage, true);
              break;
            case UpdateMetaDataOperationType.SetESIndexingConnectionString:
              this.UpdateESConnectionString(executionContext, properties, changeData, false, resultMessage, false);
              break;
          }
        }
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080610, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual bool HandleOperation(
      IndexingExecutionContext executionContext,
      UpdateMetadataEventData eventData,
      StringBuilder resultMessage)
    {
      return false;
    }

    internal virtual void EraseIndexingWatermarks(IndexingExecutionContext indexingExecutionContext)
    {
      this.IndexingUnit.EraseIndexingWatermarksOfTree(indexingExecutionContext, this.IndexingUnitDataAccess);
      if (!this.IndexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext) || !(this.IndexingUnit.IndexingUnitType == "Git_Repository"))
        return;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitTree = this.IndexingUnit.GetIndexingUnitTree(indexingExecutionContext, indexingExecutionContext.IndexingUnitDataAccess);
      indexingUnitTree.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.Properties.IndexIndices.Clear()));
      indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitTree);
      this.m_registryManager.RemoveRegistryEntry("LargeRepositoryMultipeBranchIndexing", this.IndexingUnit.TFSEntityId.ToString());
      this.m_registryManager.RemoveRegistryEntry("LargeRepositoryMultipeBranchIndexing", this.IndexingUnit.IndexingUnitId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    private void UpdateIndexContractType(
      IndexingExecutionContext executionContext,
      IndexingProperties properties,
      UpdateMetadataEventData eventData,
      StringBuilder resultMessage)
    {
      if (!("Collection" == this.IndexingUnit.IndexingUnitType))
        throw new InvalidOperationException("Set-IndexContractType not valid for " + this.IndexingUnit.ToString());
      if (!(properties is CollectionIndexingProperties indexingProperties) || !("Code" == this.IndexingUnit.EntityType.Name))
        return;
      indexingProperties.IndexContractTypePreReindexing = indexingProperties.IndexContractType;
      indexingProperties.QueryContractTypePreReindexing = indexingProperties.QueryContractType;
      indexingProperties.IndexContractType = (DocumentContractType) Enum.Parse(typeof (DocumentContractType), eventData.UpdatedData);
      this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated IndexContractType to {0} of {1}", (object) indexingProperties.IndexContractType, (object) this.IndexingUnit.ToString())));
    }

    private void UpdateESConnectionString(
      IndexingExecutionContext executionContext,
      IndexingProperties properties,
      UpdateMetadataEventData eventData,
      bool append,
      StringBuilder resultMessage)
    {
      if (!("Collection" == this.IndexingUnit.IndexingUnitType))
        throw new InvalidOperationException("Set-ESConnectionString not valid for " + this.IndexingUnit.ToString());
      if (!(properties is CollectionIndexingProperties indexingProperties))
        return;
      if (append)
      {
        indexingProperties.IndexESConnectionString = indexingProperties.IndexESConnectionString + ";" + eventData.UpdatedData;
        indexingProperties.QueryESConnectionString = indexingProperties.QueryESConnectionString + ";" + eventData.UpdatedData;
      }
      else
        indexingProperties.IndexESConnectionString = indexingProperties.QueryESConnectionString = eventData.UpdatedData;
      this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated IndexESConnectionString to {0} and QueryESConnectionString to {1} of {2}", (object) indexingProperties.IndexESConnectionString, (object) indexingProperties.QueryESConnectionString, (object) this.IndexingUnit.ToString())));
      this.SendSqlNotificationToDocumentContractTypeService(executionContext);
    }

    private void UpdateESConnectionString(
      IndexingExecutionContext executionContext,
      IndexingProperties properties,
      UpdateMetadataEventData eventData,
      bool append,
      StringBuilder resultMessage,
      bool QueryOnly)
    {
      if (!("Collection" == this.IndexingUnit.IndexingUnitType))
        throw new InvalidOperationException("Set-ESConnectionString not valid for " + this.IndexingUnit.ToString());
      if (!(properties is CollectionIndexingProperties indexingProperties))
        return;
      if (append)
      {
        if (QueryOnly)
          indexingProperties.QueryESConnectionString = indexingProperties.QueryESConnectionString + ";" + eventData.UpdatedData;
        else
          indexingProperties.IndexESConnectionString = indexingProperties.IndexESConnectionString + ";" + eventData.UpdatedData;
      }
      else if (QueryOnly)
        indexingProperties.QueryESConnectionString = eventData.UpdatedData;
      else
        indexingProperties.IndexESConnectionString = eventData.UpdatedData;
      this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated IndexESConnectionString to {0} and QueryESConnectionString to {1} of {2}", (object) indexingProperties.IndexESConnectionString, (object) indexingProperties.QueryESConnectionString, (object) this.IndexingUnit.ToString())));
      this.SendSqlNotificationToDocumentContractTypeService(executionContext);
    }

    private void SendSqlNotification(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      if (!("Collection" == this.IndexingUnit.IndexingUnitType))
        throw new InvalidOperationException("Send-SqlNotification not valid for " + this.IndexingUnit.ToString());
      this.SendSqlNotificationToDocumentContractTypeService(executionContext);
      resultMessage.Append("Sent SQL Notification to DocumentContractTypeService.");
    }

    private void SendSqlNotificationToDocumentContractTypeService(
      IndexingExecutionContext executionContext)
    {
      executionContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(executionContext.RequestContext, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.DocumentContractTypeChanged, (string) null);
    }

    private void UpdateIndexingState(
      IndexingExecutionContext executionContext,
      IndexingProperties properties,
      UpdateMetadataEventData eventData,
      StringBuilder resultMessage)
    {
      if (!this.IndexingUnit.IsRepository())
        throw new InvalidOperationException("Set-IndexingState not valid for " + this.IndexingUnit.ToString());
      properties.IsDisabled = bool.Parse(eventData.UpdatedData);
      this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      string str = properties.IsDisabled ? "Disabled" : "Enabled";
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} indexing of {1}", (object) str, (object) this.IndexingUnit.ToString())));
    }
  }
}
