// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.TempFileMetadataStoreComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class TempFileMetadataStoreComponent : SQLTable<TempFileMetadataRecord>
  {
    private static readonly string s_serviceName = "Search_TempFileMetadataStore";
    protected internal const int BatchCountForInsertMetadataRecords = 500;
    protected internal const int BatchCountForSelectMetadataRecords = 500;
    protected internal const int BatchCountForDeleteMetadataRecords = 500;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<TempFileMetadataStoreComponent>(1, true),
      (IComponentCreator) new ComponentCreator<TempFileMetadataStoreComponentV2>(2),
      (IComponentCreator) new ComponentCreator<TempFileMetadataStoreComponentV3>(3)
    }, TempFileMetadataStoreComponent.s_serviceName);
    protected const string s_traceArea = "Indexing Pipeline";
    protected const string s_traceLayer = "TempFileMetadataStoreComponent";
    private static readonly SqlMetaData[] s_indexingUnitIdTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] s_tempFileMetadataInsertTable = new SqlMetaData[3]
    {
      new SqlMetaData("Item1", SqlDbType.BigInt),
      new SqlMetaData("Item2", SqlDbType.NVarChar, 800L),
      new SqlMetaData("Item3", SqlDbType.Xml)
    };
    private static readonly SqlMetaData[] s_tempFileMetadataUpdateDescriptorTable = new SqlMetaData[6]
    {
      new SqlMetaData("FilePath", SqlDbType.NVarChar, 800L),
      new SqlMetaData("ContentHash", SqlDbType.NVarChar, 256L),
      new SqlMetaData("UpdateType", SqlDbType.NVarChar, 100L),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 1000L),
      new SqlMetaData("ChangeId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ChangeTime", SqlDbType.VarChar, 30L)
    };
    protected static readonly SqlMetaData[] typ_StringTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, 1024L)
    };

    public TempFileMetadataStoreComponent()
      : base()
    {
    }

    internal TempFileMetadataStoreComponent(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public virtual void GetMinAndMaxIds(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit, out long minId, out long maxId)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      Stopwatch stopwatch = Stopwatch.StartNew();
      SqlCommand sqlCommand = this.PrepareStoredProcedure("Search.prc_GetMinMaxIds");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      SqlParameter sqlParameter1 = new SqlParameter("@minId", SqlDbType.BigInt);
      sqlParameter1.Direction = ParameterDirection.Output;
      SqlParameter sqlParameter2 = sqlParameter1;
      SqlParameter sqlParameter3 = new SqlParameter("@maxId", SqlDbType.BigInt);
      sqlParameter3.Direction = ParameterDirection.Output;
      SqlParameter sqlParameter4 = sqlParameter3;
      sqlCommand.Parameters.Add(sqlParameter2);
      sqlCommand.Parameters.Add(sqlParameter4);
      this.ExecuteNonQuery(false);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.GetMinAndMaxIds took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      minId = sqlParameter2.Value is DBNull ? 0L : (long) sqlParameter2.Value;
      maxId = sqlParameter4.Value is DBNull ? 0L : (long) sqlParameter4.Value;
    }

    public virtual int AddFileMetadataRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<TempFileMetadataRecord> records)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (records == null || !records.Any<TempFileMetadataRecord>())
        return 0;
      List<TempFileMetadataRecord> list = records.ToList<TempFileMetadataRecord>();
      int num = list.Count<TempFileMetadataRecord>();
      int val1 = num;
      for (int index = 0; index < num; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count = Math.Min(val1, 500);
        IList<TempFileMetadataRecord> range = (IList<TempFileMetadataRecord>) list.GetRange(index, count);
        if (range.Count > 0)
        {
          try
          {
            this.PrepareStoredProcedure("Search.prc_AddTempFileMetadataRecords");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindTempFileMetadataRecordsParameter("@metadataRecords", (IEnumerable<TempFileMetadataRecord>) range);
            this.ExecuteNonQuery(false);
          }
          catch
          {
            return num - val1;
          }
          val1 -= count;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.AddFileFailureRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return num;
    }

    public virtual IEnumerable<TempFileMetadataRecord> GetTempFileMetadataRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      long startingId,
      int count,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<TempFileMetadataRecord> GetFilesWithMinAttemptCount(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      short minAttemptCount,
      string indexingUnitType,
      DocumentContractType contractType,
      int hours = 0)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<TempFileMetadataRecord> GetRecordsByFilePath(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FileAttributes> fileAttributes,
      long startingId,
      long endingId,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      if (contractType != DocumentContractType.DedupeFileContractV4 && contractType != DocumentContractType.DedupeFileContractV5)
        throw new NotSupportedException(nameof (contractType));
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (fileAttributes == null || !fileAttributes.Any<FileAttributes>())
        return (IEnumerable<TempFileMetadataRecord>) new List<TempFileMetadataRecord>();
      List<string> list = fileAttributes.Select<FileAttributes, string>((System.Func<FileAttributes, string>) (x => x.GetFilePathOrHash())).ToList<string>();
      int num = list.Count<string>();
      int val1 = num;
      List<TempFileMetadataRecord> recordsByFilePath = new List<TempFileMetadataRecord>();
      for (int index = 0; index < num; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count = Math.Min(val1, 500);
        List<string> range = list.GetRange(index, count);
        this.PrepareStoredProcedure("Search.prc_QueryTempFileMetadataRecordsOnFilePath");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindLong("@endingId", endingId);
        System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (row =>
        {
          SqlDataRecord record = new SqlDataRecord(TempFileMetadataStoreComponent.typ_StringTable);
          record.SetNullableString(0, row);
          return record;
        });
        this.BindTable("@filePathList", "typ_StringTable", range.Select<string, SqlDataRecord>(selector));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TempFileMetadataRecord>((ObjectBinder<TempFileMetadataRecord>) new TempFileMetadataStoreComponent.TempFileMetadataStoreColumnsV2(indexingUnitType, contractType));
          ObjectBinder<TempFileMetadataRecord> current = resultCollection.GetCurrent<TempFileMetadataRecord>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
                recordsByFilePath.AddRange((IEnumerable<TempFileMetadataRecord>) current.Items);
            }
          }
        }
        val1 -= count;
        if (val1 > 0)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.GetRecordsByFilePath took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        else
          break;
      }
      return (IEnumerable<TempFileMetadataRecord>) recordsByFilePath;
    }

    public virtual void DeleteRecords(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit, long startingId, long endingId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      if (startingId > endingId)
        throw new ArgumentException(nameof (endingId));
      this.PrepareStoredProcedure("Search.prc_DeleteTempFileMetadataRecords");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindLong("@startingId", startingId);
      this.BindLong("@endingId", endingId);
      this.ExecuteNonQuery(false);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.DeleteRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
    }

    public virtual void DeleteRecordsExcludingSome(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      long startingId,
      long endingId,
      IEnumerable<long> idsNotToDelete)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (startingId <= -1L)
        throw new ArgumentException(nameof (startingId));
      if (startingId > endingId)
        throw new ArgumentException(nameof (endingId));
      this.PrepareStoredProcedure("Search.prc_DeleteTempFileMetadataRecordsByRangeExcludingSome");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindLong("@startingId", startingId);
      this.BindLong("@endingId", endingId);
      this.BindIds("@idList", idsNotToDelete);
      this.ExecuteNonQuery(false);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.DeleteTempFileMetadataRecordsByRangeExcludingSome took {0}ms", (object) stopwatch.ElapsedMilliseconds));
    }

    public virtual void UpdateRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<TempFileMetadataRecord> records)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (records == null || !records.Any<TempFileMetadataRecord>())
        return;
      List<TempFileMetadataRecord> list = records.ToList<TempFileMetadataRecord>();
      int num = list.Count<TempFileMetadataRecord>();
      int val1 = num;
      for (int index = 0; index < num; index += 500)
      {
        int count = Math.Min(val1, 500);
        IList<TempFileMetadataRecord> range = (IList<TempFileMetadataRecord>) list.GetRange(index, count);
        val1 -= count;
        if (range.Count <= 0)
          break;
        this.PrepareStoredProcedure("Search.prc_UpdateTempFileMetadataRecords");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindTempFileMetadataRecordsUpdateDescriptorParameter("@updateDescriptor", (IEnumerable<TempFileMetadataRecord>) range);
        this.ExecuteNonQuery(false);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.UpdateRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      }
    }

    public virtual void UpdateRecordsByOverwrite(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<TempFileMetadataRecord> records)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (records == null || !records.Any<TempFileMetadataRecord>())
        return;
      this.PrepareStoredProcedure("Search.prc_UpdateTempFileMetadataRecordsUsingUpdate");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindUpdateTempFileMetadataRecordsParameter("@metadataRecords", records);
      this.ExecuteNonQuery(false);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.UpdateRecordsByOverwrite took {0}ms", (object) stopwatch.ElapsedMilliseconds));
    }

    public virtual IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int> GetNumberOfRecords(
      IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnits == null || !indexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        throw new ArgumentException(nameof (indexingUnits));
      Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits)
        dictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
      this.PrepareStoredProcedure("Search.prc_GetNumberOfRecords");
      this.BindIndexingUnitIdTable("@indexingUnitIdList", (IEnumerable<int>) dictionary.Keys);
      Dictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int> numberOfRecords = new Dictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Tuple<int, int>>((ObjectBinder<Tuple<int, int>>) new TempFileMetadataStoreComponent.RecordsPerIndexingUnit());
        ObjectBinder<Tuple<int, int>> current = resultCollection.GetCurrent<Tuple<int, int>>();
        if (current != null)
        {
          if (current.Items != null)
          {
            if (current.Items.Count > 0)
            {
              foreach (Tuple<int, int> tuple in current.Items)
                numberOfRecords.Add(dictionary[tuple.Item1], tuple.Item2);
            }
          }
        }
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082625, "Indexing Pipeline", nameof (TempFileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.GetNumberOfRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits)
      {
        if (!numberOfRecords.ContainsKey(indexingUnit))
          numberOfRecords.Add(indexingUnit, 0);
      }
      return (IDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) numberOfRecords;
    }

    protected virtual SqlParameter BindTempFileMetadataRecordsParameter(
      string parameterName,
      IEnumerable<TempFileMetadataRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<TempFileMetadataRecord>();
      System.Func<TempFileMetadataRecord, SqlDataRecord> selector = (System.Func<TempFileMetadataRecord, SqlDataRecord>) (metadataRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TempFileMetadataStoreComponent.s_tempFileMetadataInsertTable);
        if (metadataRecord.FilePathId.HasValue)
          sqlDataRecord.SetInt64(0, metadataRecord.FilePathId.Value);
        else
          sqlDataRecord.SetDBNull(0);
        sqlDataRecord.SetString(1, metadataRecord.FileAttributes.GetFilePathOrHash());
        sqlDataRecord.SetSqlXml(2, new SqlXml(SQLTable<TempFileMetadataRecord>.ToStream((object) metadataRecord.TemporaryBranchMetadata, typeof (TemporaryBranchMetadata))));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IdStringXmlTable", rows.Select<TempFileMetadataRecord, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindTempFileMetadataRecordsUpdateDescriptorParameter(
      string parameterName,
      IEnumerable<TempFileMetadataRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<TempFileMetadataRecord>();
      List<SqlDataRecord> rows1 = new List<SqlDataRecord>();
      foreach (TempFileMetadataRecord row in rows)
      {
        if (row?.TemporaryBranchMetadata?.BranchMetadata?.Keys != null)
        {
          foreach (string key1 in row.TemporaryBranchMetadata.BranchMetadata.Keys)
          {
            UpdateTypeDictionary updateTypeDictionary = row.TemporaryBranchMetadata.BranchMetadata[key1];
            if (updateTypeDictionary?.Keys != null)
            {
              foreach (MetaDataStoreUpdateType key2 in updateTypeDictionary.Keys)
              {
                List<BranchInfo> branchInfoList = updateTypeDictionary[key2];
                if (branchInfoList != null)
                {
                  foreach (BranchInfo branchInfo in branchInfoList)
                  {
                    SqlDataRecord sqlDataRecord = new SqlDataRecord(TempFileMetadataStoreComponent.s_tempFileMetadataUpdateDescriptorTable);
                    sqlDataRecord.SetString(0, row.FileAttributes.GetFilePathOrHash());
                    sqlDataRecord.SetString(1, key1);
                    sqlDataRecord.SetString(2, key2.ToString());
                    sqlDataRecord.SetString(3, branchInfo.BranchName);
                    sqlDataRecord.SetString(4, branchInfo.ChangeId);
                    sqlDataRecord.SetString(5, branchInfo.ChangeTime.ToString("s"));
                    rows1.Add(sqlDataRecord);
                  }
                }
              }
            }
          }
        }
      }
      return this.BindTable(parameterName, "Search.typ_TempFileMetadataStoreUpdateDescriptor", (IEnumerable<SqlDataRecord>) rows1);
    }

    protected virtual SqlParameter BindUpdateTempFileMetadataRecordsParameter(
      string parameterName,
      IEnumerable<TempFileMetadataRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<TempFileMetadataRecord>();
      System.Func<TempFileMetadataRecord, SqlDataRecord> selector = (System.Func<TempFileMetadataRecord, SqlDataRecord>) (metadataRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TempFileMetadataStoreComponent.s_tempFileMetadataInsertTable);
        sqlDataRecord.SetInt64(0, metadataRecord.Id);
        sqlDataRecord.SetString(1, metadataRecord.FileAttributes.GetFilePathOrHash());
        sqlDataRecord.SetSqlXml(2, new SqlXml(SQLTable<TempFileMetadataRecord>.ToStream((object) metadataRecord.TemporaryBranchMetadata, typeof (TemporaryBranchMetadata))));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IdStringXmlTable", rows.Select<TempFileMetadataRecord, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIndexingUnitIdTable(string parameterName, IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(TempFileMetadataStoreComponent.s_indexingUnitIdTable);
        sqlDataRecord.SetInt32(0, entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Int32Table", rows.Select<int, SqlDataRecord>(selector));
    }

    protected SqlParameter BindIds(string parameterName, IEnumerable<long> rows)
    {
      rows = rows ?? Enumerable.Empty<long>();
      System.Func<long, SqlDataRecord> selector = (System.Func<long, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SQLTable<TempFileMetadataRecord>.BigIntTable);
        sqlDataRecord.SetValue(0, (object) entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_Int64Descriptor", rows.Select<long, SqlDataRecord>(selector));
    }

    protected class TempFileMetadataStoreColumns : ObjectBinder<TempFileMetadataRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_filePathId = new SqlColumnBinder("FilePathId");
      private SqlColumnBinder m_filePath = new SqlColumnBinder("FilePath");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");
      private readonly string m_indexingUnitType;
      private readonly DocumentContractType m_documentContractType;

      protected override TempFileMetadataRecord Bind()
      {
        string filePath = this.m_filePath.GetString((IDataReader) this.Reader, false);
        return new TempFileMetadataRecord()
        {
          Id = this.m_id.GetInt64((IDataReader) this.Reader),
          FilePathId = this.Reader.IsDBNull(1) ? new long?() : new long?(this.m_filePathId.GetInt64((IDataReader) this.Reader)),
          FileAttributes = new FileAttributes(filePath, this.m_indexingUnitType, this.m_documentContractType),
          TemporaryBranchMetadata = (TemporaryBranchMetadata) SQLTable<TempFileMetadataRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (TemporaryBranchMetadata))
        };
      }

      public TempFileMetadataStoreColumns(
        string indexingUnitType,
        DocumentContractType contractType)
      {
        this.m_indexingUnitType = indexingUnitType;
        this.m_documentContractType = contractType;
      }
    }

    protected class RecordsPerIndexingUnit : ObjectBinder<Tuple<int, int>>
    {
      private SqlColumnBinder m_indexingUnitId = new SqlColumnBinder("IndexingUnitId");
      private SqlColumnBinder m_count = new SqlColumnBinder("Count");

      protected override Tuple<int, int> Bind() => Tuple.Create<int, int>(this.m_indexingUnitId.GetInt32((IDataReader) this.Reader), this.m_count.GetInt32((IDataReader) this.Reader));
    }

    protected class FilePathAndAttemptCount : ObjectBinder<TempFileMetadataRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_filePath = new SqlColumnBinder("FilePath");
      private SqlColumnBinder m_attemptCount = new SqlColumnBinder("AttemptCount");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");
      private readonly string m_indexingUnitType;
      private readonly DocumentContractType m_documentContractType;

      protected override TempFileMetadataRecord Bind()
      {
        string filePath = this.m_filePath.GetString((IDataReader) this.Reader, false);
        TemporaryBranchMetadata temporaryBranchMetadata = (TemporaryBranchMetadata) SQLTable<TempFileMetadataRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (TemporaryBranchMetadata));
        if (temporaryBranchMetadata == null)
          throw new Exception("temporaryBranchMetadata can not be null");
        if (this.m_documentContractType == DocumentContractType.DedupeFileContractV4 || this.m_documentContractType == DocumentContractType.DedupeFileContractV5)
          filePath = temporaryBranchMetadata.FilePath;
        return new TempFileMetadataRecord()
        {
          Id = this.m_id.GetInt64((IDataReader) this.Reader),
          FileAttributes = new FileAttributes(filePath, this.m_indexingUnitType, this.m_documentContractType),
          AttemptCount = this.m_attemptCount.GetByte((IDataReader) this.Reader),
          TemporaryBranchMetadata = temporaryBranchMetadata
        };
      }

      public FilePathAndAttemptCount(string indexingUnitType, DocumentContractType contractType)
      {
        this.m_indexingUnitType = indexingUnitType;
        this.m_documentContractType = contractType;
      }
    }

    protected class TempFileMetadataStoreColumnsV2 : ObjectBinder<TempFileMetadataRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_filePathId = new SqlColumnBinder("FilePathId");
      private SqlColumnBinder m_filePath = new SqlColumnBinder("FilePath");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");
      private SqlColumnBinder m_attemptCount = new SqlColumnBinder("AttemptCount");
      private readonly string m_indexingUnitType;
      private readonly DocumentContractType m_documentContractType;

      protected override TempFileMetadataRecord Bind()
      {
        string filePath = this.m_filePath.GetString((IDataReader) this.Reader, false);
        TemporaryBranchMetadata temporaryBranchMetadata = (TemporaryBranchMetadata) SQLTable<TempFileMetadataRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (TemporaryBranchMetadata));
        if (temporaryBranchMetadata == null)
          throw new Exception("temporaryBranchMetadata can not be null");
        if (this.m_documentContractType == DocumentContractType.DedupeFileContractV4 || this.m_documentContractType == DocumentContractType.DedupeFileContractV5)
          filePath = temporaryBranchMetadata.FilePath;
        return new TempFileMetadataRecord()
        {
          Id = this.m_id.GetInt64((IDataReader) this.Reader),
          FilePathId = this.Reader.IsDBNull(1) ? new long?() : new long?(this.m_filePathId.GetInt64((IDataReader) this.Reader)),
          FileAttributes = new FileAttributes(filePath, this.m_indexingUnitType, this.m_documentContractType),
          TemporaryBranchMetadata = temporaryBranchMetadata,
          AttemptCount = this.m_attemptCount.GetByte((IDataReader) this.Reader)
        };
      }

      public TempFileMetadataStoreColumnsV2(
        string indexingUnitType,
        DocumentContractType contractType)
      {
        this.m_indexingUnitType = indexingUnitType;
        this.m_documentContractType = contractType;
      }
    }
  }
}
