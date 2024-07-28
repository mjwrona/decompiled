// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV3
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV3 : IndexingUnitChangeEventComponentV2
  {
    public IndexingUnitChangeEventComponentV3()
    {
    }

    internal IndexingUnitChangeEventComponentV3(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override Dictionary<long, IndexingUnitChangeEventState> RetrieveStateOfIndexingUnitChangeEventsForListOfIds(
      IList<long> ids)
    {
      if (ids == null || ids.Count == 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("ids is null or empty"));
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        int count = ids.Count;
        List<IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<IndexingUnitChangeEvent>();
        for (int index = 0; index <= count / 500; ++index)
        {
          IList<long> list = (IList<long>) ids.Skip<long>(index * 500).Take<long>(500).ToList<long>();
          this.PrepareStoredProcedure("Search.prc_GetStateOfIndexingUnitChangeEvents");
          this.BindIndexingUnitChangeEventIdTable("@idList", (IEnumerable<long>) list);
          stopwatch.Start();
          using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          {
            resultCollection.AddBinder<IndexingUnitChangeEvent>((ObjectBinder<IndexingUnitChangeEvent>) new IndexingUnitChangeEventComponentV3.IndexingUnitChangeEventColumns());
            ObjectBinder<IndexingUnitChangeEvent> current = resultCollection.GetCurrent<IndexingUnitChangeEvent>();
            if (current != null)
            {
              if (current.Items != null)
              {
                if (current.Items.Count > 0)
                  indexingUnitChangeEventList.AddRange((IEnumerable<IndexingUnitChangeEvent>) current.Items);
              }
            }
          }
        }
        Dictionary<long, IndexingUnitChangeEventState> dictionary = new Dictionary<long, IndexingUnitChangeEventState>();
        foreach (IndexingUnitChangeEvent indexingUnitChangeEvent in indexingUnitChangeEventList)
          dictionary[indexingUnitChangeEvent.Id] = indexingUnitChangeEvent.State;
        return dictionary;
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve State and Ids of IndexingUnitChangeEvents from SQL Azure Platform");
      }
    }

    private class IndexingUnitChangeEventColumns : ObjectBinder<IndexingUnitChangeEvent>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_state = new SqlColumnBinder("State");

      protected override IndexingUnitChangeEvent Bind()
      {
        if (this.m_id.IsNull((IDataReader) this.Reader))
          return (IndexingUnitChangeEvent) null;
        return new IndexingUnitChangeEvent()
        {
          Id = this.m_id.GetInt64((IDataReader) this.Reader),
          State = (IndexingUnitChangeEventState) Enum.Parse(typeof (IndexingUnitChangeEventState), this.m_state.GetString((IDataReader) this.Reader, false), true)
        };
      }
    }
  }
}
