// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV6
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV6 : IndexingUnitChangeEventComponentV5
  {
    public IndexingUnitChangeEventComponentV6()
    {
    }

    internal IndexingUnitChangeEventComponentV6(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override int GetCountOfIndexingUnitChangeEventsInProgressOrQueued(IEntityType entityType)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryCountOfChangeEventsInProgressOrQueued");
        this.BindString("@entityType", entityType.Name, 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve count of IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    public override IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      int count,
      IEntityType entityType)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryNextSetOfEventsToBeProcessedForEntityType");
        this.BindInt("@count", count);
        this.BindString("@entityType", entityType.Name.ToString(), 32, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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
  }
}
