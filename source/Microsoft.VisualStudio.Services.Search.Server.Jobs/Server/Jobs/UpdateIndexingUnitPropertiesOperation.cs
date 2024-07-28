// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.UpdateIndexingUnitPropertiesOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class UpdateIndexingUnitPropertiesOperation : AbstractIndexingOperation
  {
    public UpdateIndexingUnitPropertiesOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(1080610, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        switch (this.IndexingUnit.IndexingUnitType)
        {
          case "Collection":
            if (this.IndexingUnit.TFSEntityAttributes is CollectionAttributes && !string.IsNullOrEmpty((this.IndexingUnit.TFSEntityAttributes as CollectionAttributes).CollectionName))
            {
              this.IndexingUnit.Properties.Name = (this.IndexingUnit.TFSEntityAttributes as CollectionAttributes).CollectionName;
              break;
            }
            string collectionName = coreIndexingExecutionContext.RequestContext.GetCollectionName();
            this.IndexingUnit.Properties.Name = collectionName;
            if (this.IndexingUnit.TFSEntityAttributes is CollectionAttributes)
            {
              ((CollectionAttributes) this.IndexingUnit.TFSEntityAttributes).CollectionName = collectionName;
              break;
            }
            this.IndexingUnit.TFSEntityAttributes = (TFSEntityAttributes) new CollectionAttributes()
            {
              CollectionName = collectionName
            };
            break;
          case "Project":
            switch (this.IndexingUnit.EntityType.Name)
            {
              case "Code":
                this.IndexingUnit.Properties.Name = this.IndexingUnit.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes1 ? entityAttributes1.ProjectName : (string) null;
                break;
              case "WorkItem":
                this.IndexingUnit.Properties.Name = this.IndexingUnit.TFSEntityAttributes is ProjectWorkItemTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null;
                break;
              default:
                throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "EntityType:{0} and IndexingUnitType:{1} are not supported.", (object) this.IndexingUnit.EntityType.Name, (object) this.IndexingUnit.IndexingUnitType));
            }
            if (string.IsNullOrEmpty(this.IndexingUnit.Properties.Name))
            {
              this.IndexingUnit.Properties.Name = executionContext.ProjectName;
              if (this.IndexingUnit.EntityType.Name == "Code" || this.IndexingUnit.EntityType.Name == "Wiki")
              {
                if (this.IndexingUnit.TFSEntityAttributes is ProjectCodeTFSAttributes)
                {
                  (this.IndexingUnit.TFSEntityAttributes as ProjectCodeTFSAttributes).ProjectName = executionContext.ProjectName;
                  break;
                }
                this.IndexingUnit.TFSEntityAttributes = (TFSEntityAttributes) new ProjectCodeTFSAttributes()
                {
                  ProjectName = executionContext.ProjectName
                };
                break;
              }
              if (this.IndexingUnit.TFSEntityAttributes is ProjectWorkItemTFSAttributes)
              {
                (this.IndexingUnit.TFSEntityAttributes as ProjectWorkItemTFSAttributes).ProjectName = executionContext.ProjectName;
                break;
              }
              this.IndexingUnit.TFSEntityAttributes = (TFSEntityAttributes) new ProjectWorkItemTFSAttributes()
              {
                ProjectName = executionContext.ProjectName
              };
              break;
            }
            break;
          case "Git_Repository":
            this.IndexingUnit.Properties.Name = this.IndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes3 ? entityAttributes3.RepositoryName : (string) null;
            break;
          case "TFVC_Repository":
            this.IndexingUnit.Properties.Name = this.IndexingUnit.TFSEntityAttributes is TfvcCodeRepoTFSAttributes entityAttributes4 ? entityAttributes4.RepositoryName : (string) null;
            break;
          case "CustomRepository":
            this.IndexingUnit.Properties.Name = this.IndexingUnit.TFSEntityAttributes is CustomRepoCodeTFSAttributes entityAttributes5 ? entityAttributes5.RepositoryName : (string) null;
            break;
          default:
            throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IndexingUnitType:{0} is not supported", (object) this.IndexingUnit.IndexingUnitType));
        }
        if (string.IsNullOrWhiteSpace(this.IndexingUnit.Properties.Name))
          stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Indexing unit name is null or empty for Id:{0}", (object) this.IndexingUnit.IndexingUnitId));
        this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, this.IndexingUnit);
        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Updated InexingUnit Properties of {0}", (object) this.IndexingUnit.ToString())));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Tracer.TraceLeave(1080610, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }
  }
}
