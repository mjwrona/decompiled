// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV5
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV5 : IndexingUnitChangeEventComponentV4
  {
    public IndexingUnitChangeEventComponentV5()
    {
    }

    internal IndexingUnitChangeEventComponentV5(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count,
      int indexingUnitId)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryNextSetOfEventsToBeProcessedForIndexingUnit");
        this.BindInt("@count", count);
        this.BindInt("@indexingUnitId", indexingUnitId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetailsWithEntityType>((ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventDetailsWithEntityTypeColumns(this.m_entityTypes, this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetailsWithEntityType>();
          return current != null && current.Items != null && current.Items.Count > 0 ? (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) current.Items : (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryNextSetOfEventsToBeProcessed");
        this.BindInt("@count", count);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IndexingUnitChangeEventDetailsWithEntityType>((ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType>) new IndexingUnitChangeEventComponentV2.IndexingUnitChangeEventDetailsWithEntityTypeColumns(this.m_entityTypes, this.m_changeEventDataKnownTypes));
          ObjectBinder<IndexingUnitChangeEventDetailsWithEntityType> current = resultCollection.GetCurrent<IndexingUnitChangeEventDetailsWithEntityType>();
          return current?.Items != null && current.Items.Count > 0 ? (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) current.Items : (IEnumerable<IndexingUnitChangeEventDetailsWithEntityType>) new List<IndexingUnitChangeEventDetailsWithEntityType>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override int GetCountOfIndexingUnitChangeEventsInProgressOrQueued(IEntityType entityType) => this.GetCountOfIndexingUnitChangeEvents(IndexingUnitChangeEventState.InProgress) + this.GetCountOfIndexingUnitChangeEvents(IndexingUnitChangeEventState.Queued);

    public override IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count,
      IEntityType entityType)
    {
      return this.GetNextSetOfEventsToProcess(count);
    }
  }
}
