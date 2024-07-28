// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV8
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV8 : IndexingUnitChangeEventComponentV7
  {
    public IndexingUnitChangeEventComponentV8()
    {
    }

    internal IndexingUnitChangeEventComponentV8(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public virtual List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> GetChangeEventsCountOfAChangeTypeAndState()
    {
      this.PrepareStoredProcedure("Search.prc_QueryChangeEventsCountOfAChangeTypeAndStatus");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>>((ObjectBinder<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>>) new IndexingUnitChangeEventComponentV8.ChangeEventsColumns());
        ObjectBinder<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> current = resultCollection.GetCurrent<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>>();
        return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>>();
      }
    }

    protected class ChangeEventsColumns : 
      ObjectBinder<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>>
    {
      private SqlColumnBinder m_state = new SqlColumnBinder("State");
      private SqlColumnBinder m_changeType = new SqlColumnBinder("ChangeType");
      private SqlColumnBinder m_count = new SqlColumnBinder("Count");
      private SqlColumnBinder m_maxTimeTaken = new SqlColumnBinder("MaxTimeTaken");

      protected override Tuple<string, IndexingUnitChangeEventState, int, TimeSpan> Bind()
      {
        string str = this.m_changeType.GetString((IDataReader) this.Reader, false);
        IndexingUnitChangeEventState changeEventState = (IndexingUnitChangeEventState) Enum.Parse(typeof (IndexingUnitChangeEventState), this.m_state.GetString((IDataReader) this.Reader, false), true);
        int int32 = this.m_count.GetInt32((IDataReader) this.Reader);
        TimeSpan timeSpan1 = TimeSpan.FromMinutes((double) this.m_maxTimeTaken.GetInt32((IDataReader) this.Reader));
        int num1 = (int) changeEventState;
        int num2 = int32;
        TimeSpan timeSpan2 = timeSpan1;
        return Tuple.Create<string, IndexingUnitChangeEventState, int, TimeSpan>(str, (IndexingUnitChangeEventState) num1, num2, timeSpan2);
      }
    }
  }
}
