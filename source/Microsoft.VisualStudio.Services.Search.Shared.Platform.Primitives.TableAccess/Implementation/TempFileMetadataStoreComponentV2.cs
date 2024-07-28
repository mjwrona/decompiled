// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.TempFileMetadataStoreComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  internal class TempFileMetadataStoreComponentV2 : TempFileMetadataStoreComponent
  {
    private static readonly SqlMetaData[] s_tempFileMetadataInsertTable = new SqlMetaData[4]
    {
      new SqlMetaData("Item1", SqlDbType.BigInt),
      new SqlMetaData("Item2", SqlDbType.NVarChar, 800L),
      new SqlMetaData("Item3", SqlDbType.Xml),
      new SqlMetaData("Item4", SqlDbType.TinyInt)
    };

    public TempFileMetadataStoreComponentV2()
    {
    }

    internal TempFileMetadataStoreComponentV2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IEnumerable<TempFileMetadataRecord> GetTempFileMetadataRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      long startingId,
      int count,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      int val1 = count > -1 ? count : throw new ArgumentException(nameof (count));
      List<TempFileMetadataRecord> fileMetadataRecords = new List<TempFileMetadataRecord>();
      for (int index = 0; index < count; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int parameterValue = Math.Min(val1, 500);
        this.PrepareStoredProcedure("Search.prc_QueryTempFileMetadataRecords");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindInt("@count", parameterValue);
        List<TempFileMetadataRecord> collection = new List<TempFileMetadataRecord>();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TempFileMetadataRecord>((ObjectBinder<TempFileMetadataRecord>) new TempFileMetadataStoreComponent.TempFileMetadataStoreColumnsV2(indexingUnitType, contractType));
          ObjectBinder<TempFileMetadataRecord> current = resultCollection.GetCurrent<TempFileMetadataRecord>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
                collection.AddRange((IEnumerable<TempFileMetadataRecord>) current.Items);
            }
          }
        }
        val1 -= parameterValue;
        fileMetadataRecords.AddRange((IEnumerable<TempFileMetadataRecord>) collection);
        if (val1 > 0)
        {
          startingId += (long) parameterValue;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", "TempFileMetadataStoreComponent", string.Format("TempFileMetadataStoreComponent.GetTempFileMetadataRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return (IEnumerable<TempFileMetadataRecord>) fileMetadataRecords;
    }

    public override IEnumerable<TempFileMetadataRecord> GetFilesWithMinAttemptCount(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      short minAttemptCount,
      string indexingUnitType,
      DocumentContractType contractType,
      int hours = 0)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (minAttemptCount <= (short) 0)
        throw new ArgumentException(nameof (minAttemptCount));
      List<TempFileMetadataRecord> withMinAttemptCount = new List<TempFileMetadataRecord>();
      this.PrepareStoredProcedure("Search.prc_GetFilesWithMinAttemptCount");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindShort("@attemptCount", minAttemptCount);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TempFileMetadataRecord>((ObjectBinder<TempFileMetadataRecord>) new TempFileMetadataStoreComponent.FilePathAndAttemptCount(indexingUnitType, contractType));
        ObjectBinder<TempFileMetadataRecord> current = resultCollection.GetCurrent<TempFileMetadataRecord>();
        if (current != null)
        {
          if (current.Items != null)
          {
            if (current.Items.Count > 0)
              withMinAttemptCount.AddRange((IEnumerable<TempFileMetadataRecord>) current.Items);
          }
        }
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", "TempFileMetadataStoreComponent", string.Format("TempFileMetadataStoreComponent.GetFilesWithMinAttemptCount took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      return (IEnumerable<TempFileMetadataRecord>) withMinAttemptCount;
    }

    public virtual void DeleteRecords(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit, IEnumerable<long> idsToDelete)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      List<long> source = idsToDelete != null ? idsToDelete.ToList<long>() : throw new ArgumentNullException(nameof (idsToDelete));
      int num = source.Count<long>();
      int val1 = num;
      for (int index = 0; index < num; index += 500)
      {
        int count = Math.Min(val1, 500);
        IList<long> range = (IList<long>) source.GetRange(index, count);
        val1 -= count;
        if (range.Count <= 0)
          break;
        this.PrepareStoredProcedure("Search.prc_DeleteTempFileMetadataRecordsById");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindIds("@idList", (IEnumerable<long>) range);
        this.ExecuteNonQuery(false);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", "TempFileMetadataStoreComponent", string.Format("TempFileMetadataStoreComponent.DeleteRecordsById took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      }
    }

    public virtual void DeleteBranchInfoInRecords(int indexingUnitId, string branchName)
    {
      if (string.IsNullOrWhiteSpace(branchName))
        throw new ArgumentException(nameof (branchName));
      this.PrepareStoredProcedure("Search.prc_DeleteBranchFromTempFileMetadataRecords");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.BindString("@branchName", branchName, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery(false);
    }

    protected override SqlParameter BindTempFileMetadataRecordsParameter(
      string parameterName,
      IEnumerable<TempFileMetadataRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<TempFileMetadataRecord>();
      System.Func<TempFileMetadataRecord, SqlDataRecord> selector = (System.Func<TempFileMetadataRecord, SqlDataRecord>) (metadataRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TempFileMetadataStoreComponentV2.s_tempFileMetadataInsertTable);
        if (metadataRecord.FilePathId.HasValue)
          sqlDataRecord.SetInt64(0, metadataRecord.FilePathId.Value);
        else
          sqlDataRecord.SetDBNull(0);
        sqlDataRecord.SetString(1, metadataRecord.FileAttributes.GetFilePathOrHash());
        sqlDataRecord.SetSqlXml(2, new SqlXml(SQLTable<TempFileMetadataRecord>.ToStream((object) metadataRecord.TemporaryBranchMetadata, typeof (TemporaryBranchMetadata))));
        sqlDataRecord.SetByte(3, metadataRecord.AttemptCount);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_TempFileMetadataRecord", rows.Select<TempFileMetadataRecord, SqlDataRecord>(selector));
    }
  }
}
