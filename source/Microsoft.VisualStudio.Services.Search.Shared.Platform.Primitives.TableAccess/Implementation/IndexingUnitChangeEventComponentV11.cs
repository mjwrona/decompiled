// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventComponentV11
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventComponentV11 : IndexingUnitChangeEventComponentV10
  {
    public IndexingUnitChangeEventComponentV11()
    {
    }

    internal IndexingUnitChangeEventComponentV11(
      string connectionString,
      int partitionId,
      IVssRequestContext requestContext)
      : base(connectionString, partitionId, requestContext)
    {
    }

    public override List<Tuple<IEntityType, int, string, int, ChangeEventData>> RetrieveChangeEventColumnsForJobTriggerAndState(
      List<string> changeTypeList,
      List<int> jobTrigger,
      List<IndexingUnitChangeEventState> stateList)
    {
      this.PrepareStoredProcedure("Search.prc_GetIndexingStatusDetails");
      List<byte> list = ((IEnumerable<byte>) Array.ConvertAll<int, byte>(jobTrigger.ToArray(), (Converter<int, byte>) (value => (byte) value))).ToList<byte>();
      this.BindIndexingUnitChangeEventChangeTypeTable("@changeTypes", (IEnumerable<string>) changeTypeList);
      this.BindIndexingUnitChangeEventJobTriggerTable("@jobTriggers", (IEnumerable<byte>) list);
      this.BindIndexingUnitChangeEventStateTable("@states", (IEnumerable<IndexingUnitChangeEventState>) stateList);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<IEntityType, int, string, int, ChangeEventData>>((ObjectBinder<Tuple<IEntityType, int, string, int, ChangeEventData>>) new IndexingUnitChangeEventComponentV11.IndexingStatusColumnsV11(this.m_entityTypes, this.m_changeEventDataKnownTypes));
        ObjectBinder<Tuple<IEntityType, int, string, int, ChangeEventData>> current = resultCollection.GetCurrent<Tuple<IEntityType, int, string, int, ChangeEventData>>();
        return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<Tuple<IEntityType, int, string, int, ChangeEventData>>();
      }
    }

    public override IDictionary<string, int> GetCountOfEventsBreachingThreshold(
      int bulkIndexThreshold,
      int updateIndexThreshold)
    {
      this.PrepareStoredProcedure("Search.prc_CountOfEventsBreachingThreshold");
      this.BindInt("@bulkIndexThreshold", bulkIndexThreshold);
      this.BindInt("@updateIndexThreshold", updateIndexThreshold);
      IDictionary<string, int> breachingThreshold = (IDictionary<string, int>) new FriendlyDictionary<string, int>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<string, int>>((ObjectBinder<Tuple<string, int>>) new IndexingUnitChangeEventComponentV11.ChangeTypeThreshold());
        ObjectBinder<Tuple<string, int>> current = resultCollection.GetCurrent<Tuple<string, int>>();
        if (current != null)
        {
          if (current.Items != null)
          {
            if (current.Items.Count > 0)
            {
              foreach (Tuple<string, int> tuple in current.Items)
                breachingThreshold[tuple.Item1] = tuple.Item2;
            }
          }
        }
      }
      return breachingThreshold;
    }

    protected class ChangeTypeThreshold : ObjectBinder<Tuple<string, int>>
    {
      private SqlColumnBinder m_changeType = new SqlColumnBinder("ChangeType");
      private SqlColumnBinder m_count = new SqlColumnBinder("Count");

      protected override Tuple<string, int> Bind() => new Tuple<string, int>(this.m_changeType.GetString((IDataReader) this.Reader, false), this.m_count.GetInt32((IDataReader) this.Reader));
    }

    protected class IndexingStatusColumnsV11 : 
      ObjectBinder<Tuple<IEntityType, int, string, int, ChangeEventData>>
    {
      private SqlColumnBinder m_entityType = new SqlColumnBinder("EntityType");
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_changeType = new SqlColumnBinder("ChangeType");
      private SqlColumnBinder m_jobTrigger = new SqlColumnBinder("JobTrigger");
      private SqlColumnBinder m_changeData = new SqlColumnBinder("ChangeData");
      private IEnumerable<IEntityType> m_entityTypes;
      private IEnumerable<Type> m_changeEventDataKnownTypes;

      protected override Tuple<IEntityType, int, string, int, ChangeEventData> Bind()
      {
        IEntityType entityType = EntityPluginsFactory.GetEntityType(this.m_entityTypes, this.m_entityType.GetString((IDataReader) this.Reader, false));
        int int32 = this.m_indexingUnitId.GetInt32((IDataReader) this.Reader);
        string str1 = this.m_changeType.GetString((IDataReader) this.Reader, false);
        int num1 = (int) this.m_jobTrigger.GetByte((IDataReader) this.Reader);
        ChangeEventData changeEventData1 = (ChangeEventData) SQLTable<IndexingUnitChangeEvent>.FromString(this.m_changeData.GetString((IDataReader) this.Reader, false), typeof (ChangeEventData), this.m_changeEventDataKnownTypes);
        int num2 = int32;
        string str2 = str1;
        int num3 = num1;
        ChangeEventData changeEventData2 = changeEventData1;
        return Tuple.Create<IEntityType, int, string, int, ChangeEventData>(entityType, num2, str2, num3, changeEventData2);
      }

      public IndexingStatusColumnsV11(
        IEnumerable<IEntityType> entityTypes,
        IEnumerable<Type> changeEventDataKnownTypes)
      {
        this.m_entityTypes = entityTypes;
        this.m_changeEventDataKnownTypes = changeEventDataKnownTypes;
      }
    }
  }
}
