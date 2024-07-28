// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Data.CodeSenseSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Data
{
  public class CodeSenseSqlResourceComponent : 
    TeamFoundationSqlResourceComponent,
    ICodeSenseSqlResourceComponent,
    IDisposable
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<CodeSenseSqlResourceComponent>(1, true),
      (IComponentCreator) new ComponentCreator<CodeSenseSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<CodeSenseSqlResourceComponent3>(3)
    }, "CodeSense");
    protected readonly CodeSenseSqlResourceComponent.Columns columns = new CodeSenseSqlResourceComponent.Columns();
    private SqlMetaData[] typ_SliceDescriptorTable = new SqlMetaData[4]
    {
      new SqlMetaData("SliceFileId", SqlDbType.Int),
      new SqlMetaData("AggregatedPath", SqlDbType.NVarChar, 400L),
      new SqlMetaData("ChangesId", SqlDbType.Int),
      new SqlMetaData("Source", SqlDbType.Int)
    };
    protected SqlMetaData[] typ_AggregateDescriptor = new SqlMetaData[3]
    {
      new SqlMetaData("AggregatePath", SqlDbType.NVarChar, 400L),
      new SqlMetaData("AggregateFileId", SqlDbType.Int),
      new SqlMetaData("ProjectGuid", SqlDbType.UniqueIdentifier)
    };
    private SqlMetaData[] typ_AggregateDescriptorTable = new SqlMetaData[2]
    {
      new SqlMetaData("AggregatePath", SqlDbType.NVarChar, 400L),
      new SqlMetaData("AggregateFileId", SqlDbType.Int)
    };
    private SqlMetaData[] typ_WorkItemAssociationLookupEntryTable = new SqlMetaData[2]
    {
      new SqlMetaData("ChangesId", SqlDbType.Int),
      new SqlMetaData("WorkItemId", SqlDbType.Int)
    };
    private SqlMetaData[] typ_MopupItemEntryTable = new SqlMetaData[1]
    {
      new SqlMetaData("ChangesetId", SqlDbType.Int)
    };

    public CodeSenseSqlResourceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override string TraceArea => "CodeSense";

    public override void Dispose() => base.Dispose();

    public IEnumerable<SliceDescriptor> GetSlices(int sliceCount)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024700, TraceLayer.ExternalSql, "GetSlices {0}", new object[1]
      {
        (object) sliceCount
      }))
        return this.GetSlicesCore(sliceCount);
    }

    public void AddSlices(IEnumerable<SliceDescriptor> slices)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024705, TraceLayer.ExternalSql, "AddSlices {0}", new object[1]
      {
        (object) slices.Count<SliceDescriptor>()
      }))
        this.AddSlicesCore(slices);
    }

    public void RemoveSlices(IEnumerable<SliceDescriptor> slices)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024710, TraceLayer.ExternalSql, "RemoveSlices {0}", new object[1]
      {
        (object) slices.Count<SliceDescriptor>()
      }))
        this.RemoveSlicesCore(slices);
    }

    public void RemoveAllSlices()
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024745, TraceLayer.ExternalSql, nameof (RemoveAllSlices), Array.Empty<object>()))
        this.RemoveAllSlicesCore();
    }

    public virtual AggregateDescriptor GetAggregate(string aggregatePath, Guid projectGuid)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024715, TraceLayer.ExternalSql, "GetAggregate {0}", new object[1]
      {
        (object) aggregatePath
      }))
        return this.GetAggregateCore(aggregatePath, projectGuid);
    }

    public virtual void AddAggregates(IEnumerable<AggregateDescriptor> aggregates)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024720, TraceLayer.ExternalSql, "AddAggregates {0}", new object[1]
      {
        (object) aggregates.Count<AggregateDescriptor>()
      }))
        this.AddAggregatesCore(aggregates);
    }

    public int DeleteOrphanedFiles(DateTime createdBefore)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024725, TraceLayer.ExternalSql, "DeleteOrphanedFiles {0}", new object[1]
      {
        (object) createdBefore
      }))
        return this.DeleteOrphanedFilesCore(createdBefore);
    }

    public virtual IEnumerable<IgnoreListedItem> GetIgnoreListedPaths()
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024730, TraceLayer.ExternalSql, "GetIgnoreListedPaths()", Array.Empty<object>()))
        return this.GetIgnoreListedPathsCore();
    }

    public void AddWorkItemAssociationLookupEntries(
      IEnumerable<WorkItemAssociationLookupEntry> lookupEntries)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024735, TraceLayer.ExternalSql, "AddWorkItemAssociationLookupEntries()", Array.Empty<object>()))
        this.AddWorkItemAssociationLookupEntriesCore(lookupEntries);
    }

    public IEnumerable<int> GetWorkItemAssociationLookupEntries(int changesId)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024740, TraceLayer.ExternalSql, "GetWorkItemAssociationLookupEntries()", Array.Empty<object>()))
        return this.GetWorkItemAssociationLookupEntriesCore(changesId);
    }

    public int RemoveWorkItemAssociationLookupEntries(int minChangesId, int maxChangesId)
    {
      if (minChangesId <= 0 || minChangesId > maxChangesId)
        throw new ArgumentOutOfRangeException("minChangesId must be greater than zero and less than or equal to maxChangesId");
      using (new CodeSenseTraceWatch(this.RequestContext, 1024745, TraceLayer.ExternalSql, "RemoveWorkItemAssociationLookupEntries()", Array.Empty<object>()))
        return this.RemoveWorkItemAssociationLookupEntriesCore(minChangesId, maxChangesId);
    }

    public void AddMopupItemToQueue(int changesetId, int sliceSourceId)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024750, TraceLayer.ExternalSql, "AddMopupItemToQueue {0}", new object[1]
      {
        (object) changesetId
      }))
        this.AddMopupItemToQueueCore(changesetId, sliceSourceId);
    }

    public IEnumerable<int> GetMopupQueueItems()
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024755, TraceLayer.ExternalSql, "GetMopupQueueItems()", Array.Empty<object>()))
        return this.GetMopupQueueItemsCore();
    }

    public void RemoveMopupItems(List<int> changesetIds)
    {
      using (new CodeSenseTraceWatch(this.RequestContext, 1024760, TraceLayer.ExternalSql, "RemoveMopupItems()", Array.Empty<object>()))
        this.RemoveMopupItemsCore(changesetIds);
    }

    public virtual void AddFilesToDelete(IEnumerable<int> fileIds)
    {
    }

    public virtual int GetFilesPendingForDeletion(DateTime createdBefore) => 0;

    private void AddMopupItemToQueueCore(int changesetId, int sliceSourceId)
    {
      this.PrepareStoredProcedure("CodeSense.prc_AddMopupItem");
      this.BindInt("@changesetId", changesetId);
      this.BindInt("@sourceId", sliceSourceId);
      this.ExecuteNonQuery(false);
    }

    private void RemoveMopupItemsCore(List<int> changesetIds)
    {
      this.PrepareStoredProcedure("CodeSense.prc_RemoveMopupItems");
      this.BindMopupItemEntryTable("@itemList", (IEnumerable<int>) changesetIds);
      this.ExecuteNonQuery(false);
    }

    private IEnumerable<int> GetMopupQueueItemsCore()
    {
      this.PrepareStoredProcedure("CodeSense.prc_GetMopupItems");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ProjectionBinder<int>((System.Func<SqlDataReader, int>) (reader => this.columns.MopupChangesId.GetInt32((IDataReader) reader))));
        return (IEnumerable<int>) resultCollection.GetCurrent<int>().Items.ToReadOnlyList<int>();
      }
    }

    private IEnumerable<SliceDescriptor> GetSlicesCore(int sliceCount)
    {
      this.PrepareStoredProcedure("CodeSense.prc_GetSlices");
      this.BindInt("@sliceCount", sliceCount);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SliceDescriptor>((ObjectBinder<SliceDescriptor>) new ProjectionBinder<SliceDescriptor>((System.Func<SqlDataReader, SliceDescriptor>) (r => new SliceDescriptor(this.columns.GetSlicesSliceFileId.GetInt32((IDataReader) r), this.columns.GetSlicesAggregatedPath.GetString((IDataReader) r, false), this.columns.GetSlicesChangesId.GetInt32((IDataReader) r), (SliceSource) this.columns.GetSlicesSource.GetInt32((IDataReader) r)))));
        return (IEnumerable<SliceDescriptor>) resultCollection.GetCurrent<SliceDescriptor>().Items.ToReadOnlyList<SliceDescriptor>();
      }
    }

    private void AddSlicesCore(IEnumerable<SliceDescriptor> slices)
    {
      this.PrepareStoredProcedure("CodeSense.prc_AddSlices");
      this.BindSliceDescriptorTable("@itemList", slices);
      this.ExecuteNonQuery(false);
    }

    private void RemoveSlicesCore(IEnumerable<SliceDescriptor> slices)
    {
      this.PrepareStoredProcedure("CodeSense.prc_RemoveSlices");
      this.BindSliceDescriptorTable("@itemList", slices);
      this.ExecuteNonQuery(false);
    }

    private void RemoveAllSlicesCore()
    {
      this.PrepareStoredProcedure("CodeSense.prc_RemoveAllSlices");
      this.ExecuteNonQuery(false);
    }

    private AggregateDescriptor GetAggregateCore(string aggregatePath, Guid projectGuid)
    {
      this.PrepareStoredProcedure("CodeSense.prc_GetAggregate");
      this.BindString("@aggregatePath", aggregatePath, 400, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AggregateDescriptor>((ObjectBinder<AggregateDescriptor>) new ProjectionBinder<AggregateDescriptor>((System.Func<SqlDataReader, AggregateDescriptor>) (r => new AggregateDescriptor(aggregatePath.GetCompletePath(this.RequestContext, projectGuid), this.columns.GetAggregateFileIdAggregateFileId.GetInt32((IDataReader) r), projectGuid))));
        return resultCollection.GetCurrent<AggregateDescriptor>().Items.SingleOrDefault<AggregateDescriptor>();
      }
    }

    private void AddAggregatesCore(IEnumerable<AggregateDescriptor> aggregates)
    {
      this.PrepareStoredProcedure("CodeSense.prc_AddAggregates");
      this.BindAggregateDescriptorTable("@itemList", aggregates);
      this.ExecuteNonQuery(false);
    }

    private int DeleteOrphanedFilesCore(DateTime createdBefore)
    {
      this.PrepareStoredProcedure("CodeSense.prc_DeleteOrphanedFiles", 3600);
      this.BindDateTime("@createdBefore", createdBefore);
      return (int) this.ExecuteScalar();
    }

    private IEnumerable<IgnoreListedItem> GetIgnoreListedPathsCore()
    {
      this.PrepareStoredProcedure("CodeSense.prc_GetIgnoreList");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IgnoreListedItem>((ObjectBinder<IgnoreListedItem>) new ProjectionBinder<IgnoreListedItem>((System.Func<SqlDataReader, IgnoreListedItem>) (reader => new IgnoreListedItem(this.columns.IgnoreListServerPath.GetString((IDataReader) reader, false)))));
        return (IEnumerable<IgnoreListedItem>) resultCollection.GetCurrent<IgnoreListedItem>().Items.ToReadOnlyList<IgnoreListedItem>();
      }
    }

    private void AddWorkItemAssociationLookupEntriesCore(
      IEnumerable<WorkItemAssociationLookupEntry> lookupEntries)
    {
      this.PrepareStoredProcedure("CodeSense.prc_AddWorkItemAssociationLookupEntries");
      this.BindWorkItemAssociationLookupTable("@entryList", lookupEntries);
      this.ExecuteNonQuery();
    }

    private IEnumerable<int> GetWorkItemAssociationLookupEntriesCore(int changesId)
    {
      this.PrepareStoredProcedure("CodeSense.prc_GetWorkItemAssociationLookupEntries");
      this.BindInt("@changesId", changesId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ProjectionBinder<int>((System.Func<SqlDataReader, int>) (reader => this.columns.GetWorkItemAssociationLookupEntriesWorkItemId.GetInt32((IDataReader) reader))));
        return (IEnumerable<int>) resultCollection.GetCurrent<int>().Items.ToReadOnlyList<int>();
      }
    }

    private int RemoveWorkItemAssociationLookupEntriesCore(int minChangesId, int maxChangesId)
    {
      this.PrepareStoredProcedure("CodeSense.prc_RemoveWorkItemAssociationLookupEntries");
      this.BindInt("@minChangesId", minChangesId);
      this.BindInt("@maxChangesId", maxChangesId);
      return this.ExecuteNonQuery();
    }

    private SqlParameter BindSliceDescriptorTable(
      string parameterName,
      IEnumerable<SliceDescriptor> rows)
    {
      rows = rows ?? Enumerable.Empty<SliceDescriptor>();
      System.Func<SliceDescriptor, SqlDataRecord> selector = (System.Func<SliceDescriptor, SqlDataRecord>) (sliceDescriptor =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_SliceDescriptorTable);
        sqlDataRecord.SetInt32(0, sliceDescriptor.SliceFileId);
        sqlDataRecord.SetString(1, sliceDescriptor.AggregatePath);
        sqlDataRecord.SetInt32(2, sliceDescriptor.ChangesId);
        sqlDataRecord.SetInt32(3, (int) sliceDescriptor.Source);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "CodeSense.typ_SliceDescriptorTable", rows.Select<SliceDescriptor, SqlDataRecord>(selector));
    }

    protected SqlParameter BindAggregateDescriptor(
      string parameterName,
      IEnumerable<AggregateDescriptor> rows)
    {
      rows = rows ?? Enumerable.Empty<AggregateDescriptor>();
      System.Func<AggregateDescriptor, SqlDataRecord> selector = (System.Func<AggregateDescriptor, SqlDataRecord>) (aggregateDescriptor =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_AggregateDescriptor);
        sqlDataRecord.SetString(0, aggregateDescriptor.Path);
        sqlDataRecord.SetInt32(1, aggregateDescriptor.FileId);
        sqlDataRecord.SetGuid(2, aggregateDescriptor.ProjectGuid);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "CodeSense.typ_AggregateDescriptor", rows.Select<AggregateDescriptor, SqlDataRecord>(selector));
    }

    private SqlParameter BindAggregateDescriptorTable(
      string parameterName,
      IEnumerable<AggregateDescriptor> rows)
    {
      rows = rows ?? Enumerable.Empty<AggregateDescriptor>();
      System.Func<AggregateDescriptor, SqlDataRecord> selector = (System.Func<AggregateDescriptor, SqlDataRecord>) (aggregateDescriptor =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_AggregateDescriptorTable);
        sqlDataRecord.SetString(0, aggregateDescriptor.Path);
        sqlDataRecord.SetInt32(1, aggregateDescriptor.FileId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "CodeSense.typ_AggregateDescriptorTable", rows.Select<AggregateDescriptor, SqlDataRecord>(selector));
    }

    private SqlParameter BindWorkItemAssociationLookupTable(
      string parameterName,
      IEnumerable<WorkItemAssociationLookupEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<WorkItemAssociationLookupEntry>();
      System.Func<WorkItemAssociationLookupEntry, SqlDataRecord> selector = (System.Func<WorkItemAssociationLookupEntry, SqlDataRecord>) (entry =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_WorkItemAssociationLookupEntryTable);
        sqlDataRecord.SetInt32(0, entry.ChangesId);
        sqlDataRecord.SetInt32(1, entry.WorkItemId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "CodeSense.typ_WorkItemAssociationLookupEntryTable", rows.Select<WorkItemAssociationLookupEntry, SqlDataRecord>(selector));
    }

    private SqlParameter BindMopupItemEntryTable(string parameterName, IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (changesetId =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_MopupItemEntryTable);
        sqlDataRecord.SetInt32(0, changesetId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "CodeSense.typ_MopupItemEntryTable", rows.Select<int, SqlDataRecord>(selector));
    }

    protected class Columns
    {
      public SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      public SqlColumnBinder GetSlicesSliceFileId = new SqlColumnBinder("SliceFileId");
      public SqlColumnBinder GetSlicesAggregatedPath = new SqlColumnBinder("AggregatedPath");
      public SqlColumnBinder GetSlicesChangesId = new SqlColumnBinder("ChangesId");
      public SqlColumnBinder MopupChangesId = new SqlColumnBinder("ChangesetId");
      public SqlColumnBinder GetSlicesSource = new SqlColumnBinder("Source");
      public SqlColumnBinder GetAggregateFileIdAggregateFileId = new SqlColumnBinder("AggregateFileId");
      public SqlColumnBinder IgnoreListServerPath = new SqlColumnBinder("ServerPath");
      public SqlColumnBinder ProjectName = new SqlColumnBinder("Name");
      public SqlColumnBinder GetWorkItemAssociationLookupEntriesChangesId = new SqlColumnBinder("ChangesId");
      public SqlColumnBinder GetWorkItemAssociationLookupEntriesWorkItemId = new SqlColumnBinder("WorkItemId");
    }
  }
}
