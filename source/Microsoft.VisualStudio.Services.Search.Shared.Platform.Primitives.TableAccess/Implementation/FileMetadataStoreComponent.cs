// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.FileMetadataStoreComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class FileMetadataStoreComponent : SQLTable<FileMetadataRecord>
  {
    [StaticSafe("Grandfathered")]
    private static string s_serviceName = "Search_FileMetadataStore";
    protected internal const int BatchCountForInsertMetadataRecords = 500;
    protected internal const int BatchCountForDeleteMetadataRecords = 500;
    protected internal const int BatchCountForSelectMetadataRecords = 1000;
    protected internal const int BatchCountForLookupFilePathContentHash = 500;
    protected internal const int BatchCountForLookupFilePathBranch = 500;
    protected internal const int BatchCountForUpdateRecords = 100;
    protected internal const int BatchCountForUpdateRecordsUsingUpdate = 500;
    protected internal StringComparer FilePathComparer = StringComparer.OrdinalIgnoreCase;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<FileMetadataStoreComponent>(1, true),
      (IComponentCreator) new ComponentCreator<FileMetadataStoreComponentV2>(2),
      (IComponentCreator) new ComponentCreator<FileMetadataStoreComponentV3>(3)
    }, FileMetadataStoreComponent.s_serviceName);
    private const string TraceArea = "Indexing Pipeline";
    protected const string TraceLayer = "FileMetadataStoreComponent";
    private static readonly SqlMetaData[] s_fileMetadataInsertTable = new SqlMetaData[3]
    {
      new SqlMetaData("Item1", SqlDbType.BigInt),
      new SqlMetaData("Item2", SqlDbType.NVarChar, 800L),
      new SqlMetaData("Item3", SqlDbType.Xml)
    };
    private static readonly SqlMetaData[] s_fileMetadataUpdateDescriptorTable = new SqlMetaData[4]
    {
      new SqlMetaData("FilePathId", SqlDbType.BigInt),
      new SqlMetaData("FilePath", SqlDbType.NVarChar, 800L),
      new SqlMetaData("ContentHash", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BranchName", SqlDbType.NVarChar, 1000L)
    };
    private static readonly SqlMetaData[] s_filePathContentHashDescriptorTable = new SqlMetaData[2]
    {
      new SqlMetaData("FilePath", SqlDbType.NVarChar, 800L),
      new SqlMetaData("ContentHash", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] s_stringStringDecsriptorTable = new SqlMetaData[2]
    {
      new SqlMetaData("Item0", SqlDbType.NVarChar, 800L),
      new SqlMetaData("Item1", SqlDbType.NVarChar, 800L)
    };
    private static readonly SqlMetaData[] s_typStringTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, 1024L)
    };
    private static readonly SqlMetaData[] s_fileMetadataUpdateTable = new SqlMetaData[2]
    {
      new SqlMetaData("Item1", SqlDbType.BigInt),
      new SqlMetaData("Item2", SqlDbType.Xml)
    };

    public FileMetadataStoreComponent()
      : base()
    {
    }

    internal FileMetadataStoreComponent(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public virtual IEnumerable<FileMetadataRecord> AddRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FileMetadataRecord> records)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      List<FileMetadataRecord> fileMetadataRecordList = new List<FileMetadataRecord>();
      if (records == null || !records.Any<FileMetadataRecord>())
        return (IEnumerable<FileMetadataRecord>) fileMetadataRecordList;
      List<FileMetadataRecord> list = records.ToList<FileMetadataRecord>();
      int num1 = list.Count<FileMetadataRecord>();
      int val1 = num1;
      for (int index = 0; index < num1; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int num2 = Math.Min(val1, 500);
        IList<FileMetadataRecord> range = (IList<FileMetadataRecord>) list.GetRange(index, num2);
        val1 -= num2;
        if (range.Count > 0)
        {
          try
          {
            long nextId = this.GetNextId(num2);
            this.PrepareStoredProcedure("Search.prc_AddFileMetadataRecords");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindFileMetadataRecordsParameter("@metadataRecords", (IEnumerable<FileMetadataRecord>) range, nextId);
            this.ExecuteNonQuery(false);
          }
          catch (Exception ex)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), ex);
            fileMetadataRecordList.AddRange((IEnumerable<FileMetadataRecord>) range);
          }
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.AddRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return (IEnumerable<FileMetadataRecord>) fileMetadataRecordList;
    }

    public virtual List<FileMetadataRecord> UpdateRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FileMetadataRecord> records)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      List<FileMetadataRecord> fileMetadataRecordList = new List<FileMetadataRecord>();
      if (records == null || !records.Any<FileMetadataRecord>())
        return fileMetadataRecordList;
      List<FileMetadataRecord> list = records.ToList<FileMetadataRecord>();
      int num = list.Count<FileMetadataRecord>();
      int val1 = num;
      for (int index = 0; index < num; index += 500)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count = Math.Min(val1, 500);
        IList<FileMetadataRecord> range = (IList<FileMetadataRecord>) list.GetRange(index, count);
        val1 -= count;
        if (range.Count > 0)
        {
          try
          {
            this.PrepareStoredProcedure("Search.prc_UpdateFileMetadataRecordsUsingUpdate");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindUpdateFileMetadataRecordsParameter("@metadataRecords", (IEnumerable<FileMetadataRecord>) range);
            this.ExecuteNonQuery(false);
          }
          catch (Exception ex)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), ex);
            fileMetadataRecordList.AddRange((IEnumerable<FileMetadataRecord>) range);
          }
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.UpdateRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return fileMetadataRecordList;
    }

    public virtual IEnumerable<FileMetadataRecord> GetRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      long startingId,
      int count)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      int val1 = count;
      List<FileMetadataRecord> records = new List<FileMetadataRecord>();
      for (int index = 0; index < count; index += 1000)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int parameterValue = Math.Min(val1, 1000);
        this.PrepareStoredProcedure("Search.prc_QueryFileMetadataRecords");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindLong("@startingId", startingId);
        this.BindInt("@count", parameterValue);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<FileMetadataRecord>((ObjectBinder<FileMetadataRecord>) new FileMetadataStoreComponent.FileMetadataStoreColumns());
          ObjectBinder<FileMetadataRecord> current = resultCollection.GetCurrent<FileMetadataRecord>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
                records.AddRange((IEnumerable<FileMetadataRecord>) current.Items);
            }
          }
        }
        val1 -= parameterValue;
        if (val1 > 0)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.GetRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
          startingId += (long) parameterValue;
        }
        else
          break;
      }
      return (IEnumerable<FileMetadataRecord>) records;
    }

    public virtual IEnumerable<FileMetadataRecord> GetRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<string> filePaths)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (filePaths == null || !filePaths.Any<string>())
        return (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>();
      List<string> list = filePaths.ToList<string>();
      int num = list.Count<string>();
      int val1 = num;
      List<FileMetadataRecord> records = new List<FileMetadataRecord>();
      for (int index = 0; index < num; index += 1000)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count = Math.Min(val1, 1000);
        List<string> range = list.GetRange(index, count);
        this.PrepareStoredProcedure("Search.prc_QueryFileMetadataRecordsOnFilePath");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (row =>
        {
          SqlDataRecord record = new SqlDataRecord(FileMetadataStoreComponent.s_typStringTable);
          record.SetNullableString(0, row);
          return record;
        });
        this.BindTable("@filePathList", "typ_StringTable", range.Select<string, SqlDataRecord>(selector));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<FileMetadataRecord>((ObjectBinder<FileMetadataRecord>) new FileMetadataStoreComponent.FileMetadataStoreColumns());
          ObjectBinder<FileMetadataRecord> current = resultCollection.GetCurrent<FileMetadataRecord>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
                records.AddRange((IEnumerable<FileMetadataRecord>) current.Items);
            }
          }
        }
        val1 -= count;
        if (val1 > 0)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.GetRecordsByFilePath took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        else
          break;
      }
      return (IEnumerable<FileMetadataRecord>) records;
    }

    public virtual void GetMinAndMaxFilePathIds(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      out long minFilePathId,
      out long maxFilePathId)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      Stopwatch stopwatch = Stopwatch.StartNew();
      SqlCommand sqlCommand = this.PrepareStoredProcedure("Search.prc_GetMinMaxFileMetadataStoreIds");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      SqlParameter sqlParameter1 = new SqlParameter("@minId", SqlDbType.BigInt);
      sqlParameter1.Direction = ParameterDirection.Output;
      SqlParameter sqlParameter2 = new SqlParameter("@maxId", SqlDbType.BigInt);
      sqlParameter2.Direction = ParameterDirection.Output;
      sqlCommand.Parameters.Add(sqlParameter1);
      sqlCommand.Parameters.Add(sqlParameter2);
      this.ExecuteNonQuery(false);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.GetMinAndMaxFilePathIds took {0}ms", (object) stopwatch.ElapsedMilliseconds));
      minFilePathId = sqlParameter1.Value is DBNull ? 0L : (long) sqlParameter1.Value;
      maxFilePathId = sqlParameter2.Value is DBNull ? 0L : (long) sqlParameter2.Value;
    }

    public virtual Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>> UpdateRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FileMetadataRecord> recordsToUpdate,
      IEnumerable<FileMetadataRecord> recordsToDelete)
    {
      List<FileMetadataRecord> fileMetadataRecordList1 = new List<FileMetadataRecord>();
      List<FileMetadataRecord> fileMetadataRecordList2 = new List<FileMetadataRecord>();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      recordsToUpdate = recordsToUpdate ?? (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>();
      recordsToDelete = recordsToDelete ?? (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>();
      if (!recordsToUpdate.Any<FileMetadataRecord>() && !recordsToDelete.Any<FileMetadataRecord>())
        return new Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>>(fileMetadataRecordList1, fileMetadataRecordList2);
      int num1 = recordsToUpdate.Count<FileMetadataRecord>();
      int num2 = recordsToDelete.Count<FileMetadataRecord>();
      int val1_1 = num1;
      int val1_2 = num2;
      int num3 = num1;
      List<FileMetadataRecord> list1 = recordsToUpdate.ToList<FileMetadataRecord>();
      for (int index = 0; index < num3; index += 100)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count = Math.Min(val1_1, 100);
        List<FileMetadataRecord> range = list1.GetRange(index, count);
        try
        {
          this.PrepareStoredProcedure("Search.prc_UpdateFileMetadataRecords");
          this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
          this.BindFileMetadataRecordsUpdateDescriptorParameter("@updateDescriptor", (IEnumerable<FileMetadataRecord>) range);
          this.BindFileMetadataRecordsUpdateDescriptorParameter("@deleteDescriptor", (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>());
          this.ExecuteNonQuery(false);
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), ex);
          fileMetadataRecordList1.AddRange((IEnumerable<FileMetadataRecord>) range);
        }
        val1_1 -= count;
        if (val1_1 > 0)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.UpdateRecords.Add took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        else
          break;
      }
      int num4 = num2;
      List<FileMetadataRecord> list2 = recordsToDelete.ToList<FileMetadataRecord>();
      for (int index = 0; index < num4; index += 100)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count = Math.Min(val1_2, 100);
        List<FileMetadataRecord> range = list2.GetRange(index, count);
        try
        {
          this.PrepareStoredProcedure("Search.prc_UpdateFileMetadataRecords");
          this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
          this.BindFileMetadataRecordsUpdateDescriptorParameter("@updateDescriptor", (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>());
          this.BindFileMetadataRecordsUpdateDescriptorParameter("@deleteDescriptor", (IEnumerable<FileMetadataRecord>) range);
          this.ExecuteNonQuery(false);
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), ex);
          fileMetadataRecordList2.AddRange((IEnumerable<FileMetadataRecord>) range);
        }
        val1_2 -= count;
        if (val1_2 > 0)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.UpdateRecords.Delete took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        else
          break;
      }
      return new Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>>(fileMetadataRecordList1, fileMetadataRecordList2);
    }

    public virtual Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>> UpdateRecordsUsingMerge(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FileMetadataRecord> recordsToUpdate,
      IEnumerable<FileMetadataRecord> recordsToDelete)
    {
      List<FileMetadataRecord> fileMetadataRecordList1 = new List<FileMetadataRecord>();
      List<FileMetadataRecord> fileMetadataRecordList2 = new List<FileMetadataRecord>();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      recordsToUpdate = recordsToUpdate ?? (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>();
      recordsToDelete = recordsToDelete ?? (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>();
      if (!recordsToUpdate.Any<FileMetadataRecord>() && !recordsToDelete.Any<FileMetadataRecord>())
        return new Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>>(fileMetadataRecordList1, fileMetadataRecordList2);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) this.FilePathComparer);
      stringSet.AddRange<string, HashSet<string>>(recordsToUpdate.Select<FileMetadataRecord, string>((System.Func<FileMetadataRecord, string>) (x => x.FilePath)));
      stringSet.AddRange<string, HashSet<string>>(recordsToDelete.Select<FileMetadataRecord, string>((System.Func<FileMetadataRecord, string>) (x => x.FilePath)));
      IDictionary<string, FileMetadataRecord> filePathToUpdateRecord = this.GetNewFilePathToFileMetadataRecordsDictionary();
      foreach (FileMetadataRecord fileMetadataRecord in recordsToUpdate)
        filePathToUpdateRecord[fileMetadataRecord.FilePath] = fileMetadataRecord;
      IDictionary<string, FileMetadataRecord> filePathToDeleteRecord = this.GetNewFilePathToFileMetadataRecordsDictionary();
      foreach (FileMetadataRecord fileMetadataRecord in recordsToDelete)
        filePathToDeleteRecord[fileMetadataRecord.FilePath] = fileMetadataRecord;
      IDictionary<string, FileMetadataRecord> recordsDictionary = this.GetNewFilePathToFileMetadataRecordsDictionary();
      foreach (FileMetadataRecord record in this.GetRecords(indexingUnit, (IEnumerable<string>) stringSet))
        recordsDictionary[record.FilePath] = record;
      IDictionary<string, FileMetadataRecord> recordsToBeInserted = this.GetNewFilePathToFileMetadataRecordsDictionary();
      IDictionary<string, FileMetadataRecord> recordsToBeDeleted = this.GetNewFilePathToFileMetadataRecordsDictionary();
      this.MergeExistingRecordsWithRecordsToBeUpdated(recordsDictionary, filePathToUpdateRecord, out recordsToBeInserted);
      this.MergeExistingRecordsWithRecordsToBeDeleted(recordsDictionary, filePathToDeleteRecord, out recordsToBeDeleted);
      if (recordsToBeInserted.Any<KeyValuePair<string, FileMetadataRecord>>())
      {
        IEnumerable<FileMetadataRecord> source = this.AddRecords(indexingUnit, (IEnumerable<FileMetadataRecord>) recordsToBeInserted.Values);
        fileMetadataRecordList1.AddRange(source.Select<FileMetadataRecord, FileMetadataRecord>((System.Func<FileMetadataRecord, FileMetadataRecord>) (record => filePathToUpdateRecord[record.FilePath])));
      }
      if (recordsDictionary.Any<KeyValuePair<string, FileMetadataRecord>>())
      {
        foreach (FileMetadataRecord updateRecord in (IEnumerable<FileMetadataRecord>) this.UpdateRecords(indexingUnit, (IEnumerable<FileMetadataRecord>) recordsDictionary.Values))
        {
          FileMetadataRecord fileMetadataRecord;
          if (filePathToUpdateRecord.TryGetValue(updateRecord.FilePath, out fileMetadataRecord))
            fileMetadataRecordList1.Add(fileMetadataRecord);
          if (filePathToDeleteRecord.TryGetValue(updateRecord.FilePath, out fileMetadataRecord))
            fileMetadataRecordList1.Add(fileMetadataRecord);
        }
      }
      if (recordsToBeDeleted.Any<KeyValuePair<string, FileMetadataRecord>>())
      {
        IEnumerable<FileMetadataRecord> source = this.DeleteRecords(indexingUnit, (IEnumerable<FileMetadataRecord>) recordsToBeDeleted.Values);
        fileMetadataRecordList2.AddRange(source.Select<FileMetadataRecord, FileMetadataRecord>((System.Func<FileMetadataRecord, FileMetadataRecord>) (record => filePathToDeleteRecord[record.FilePath])));
      }
      return new Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>>(fileMetadataRecordList1, fileMetadataRecordList2);
    }

    public virtual void DeleteBranchInfoInRecords(int indexingUnitId, string branchName)
    {
      if (string.IsNullOrWhiteSpace(branchName))
        throw new ArgumentException(nameof (branchName));
      SqlCommand sqlCommand = this.PrepareStoredProcedure("Search.prc_DeleteBranchFileMetadataRecords");
      this.BindInt("@indexingUnitId", indexingUnitId);
      this.BindString("@branchName", branchName, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      SqlParameter sqlParameter = new SqlParameter("@deletedRecordsCount", SqlDbType.BigInt);
      sqlParameter.Direction = ParameterDirection.Output;
      sqlCommand.Parameters.Add(sqlParameter);
      do
      {
        this.ExecuteNonQuery(false);
      }
      while ((sqlParameter.Value is DBNull ? 0L : (long) sqlParameter.Value) != 0L);
    }

    public virtual void DeleteBranchInfoRecordsUsingMerge(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      List<string> branchNames,
      int takeCount)
    {
      List<FileMetadataRecord> fileMetadataRecordList1 = new List<FileMetadataRecord>();
      List<FileMetadataRecord> fileMetadataRecordList2 = new List<FileMetadataRecord>();
      int num1 = 0;
      long minFilePathId;
      long maxFilePathId;
      this.GetMinAndMaxFilePathIds(indexingUnit, out minFilePathId, out maxFilePathId);
      if (minFilePathId > 0L)
      {
        while (minFilePathId <= maxFilePathId)
        {
          IEnumerable<FileMetadataRecord> fileMetadataRecords = this.GetRecords(indexingUnit, minFilePathId, takeCount) ?? (IEnumerable<FileMetadataRecord>) new List<FileMetadataRecord>();
          List<FileMetadataRecord> fileMetadataRecordList3 = new List<FileMetadataRecord>();
          foreach (FileMetadataRecord fileMetadataRecord in fileMetadataRecords)
          {
            if (fileMetadataRecord.Id <= maxFilePathId)
            {
              ContentHashBranchListDictionary source1 = new ContentHashBranchListDictionary();
              foreach (KeyValuePair<string, Branches> keyValuePair in (Dictionary<string, Branches>) fileMetadataRecord?.BranchMetadata?.Metadata)
              {
                if (keyValuePair.Value != null)
                {
                  Branches source2 = new Branches();
                  source2.AddRange(keyValuePair.Value.Intersect<string>((IEnumerable<string>) branchNames));
                  if (source2.Any<string>())
                    source1.Add(keyValuePair.Key, source2);
                }
              }
              if (source1.Any<KeyValuePair<string, Branches>>())
                fileMetadataRecordList3.Add(new FileMetadataRecord()
                {
                  Id = fileMetadataRecord.Id,
                  BranchMetadata = new BranchMetadata()
                  {
                    Metadata = source1
                  },
                  FilePath = fileMetadataRecord.FilePath
                });
            }
            else
              break;
          }
          minFilePathId += (long) takeCount;
          if (fileMetadataRecordList3.Count > 0)
          {
            IDictionary<string, FileMetadataRecord> recordsDictionary1 = this.GetNewFilePathToFileMetadataRecordsDictionary();
            IDictionary<string, FileMetadataRecord> recordsDictionary2 = this.GetNewFilePathToFileMetadataRecordsDictionary();
            foreach (FileMetadataRecord fileMetadataRecord in fileMetadataRecords)
              recordsDictionary1[fileMetadataRecord.FilePath] = fileMetadataRecord;
            foreach (FileMetadataRecord fileMetadataRecord in fileMetadataRecordList3)
              recordsDictionary2[fileMetadataRecord.FilePath] = fileMetadataRecord;
            IDictionary<string, FileMetadataRecord> recordsToBeDeleted;
            this.MergeExistingRecordsWithRecordsToBeDeleted(recordsDictionary1, recordsDictionary2, out recordsToBeDeleted);
            if (recordsDictionary1.Any<KeyValuePair<string, FileMetadataRecord>>())
              fileMetadataRecordList1.AddRange((IEnumerable<FileMetadataRecord>) this.UpdateRecords(indexingUnit, (IEnumerable<FileMetadataRecord>) recordsDictionary1.Values));
            if (recordsToBeDeleted.Any<KeyValuePair<string, FileMetadataRecord>>())
              fileMetadataRecordList2.AddRange(this.DeleteRecords(indexingUnit, (IEnumerable<FileMetadataRecord>) recordsToBeDeleted.Values));
            int num2 = fileMetadataRecordList2.Count + fileMetadataRecordList1.Count;
            num1 += fileMetadataRecordList3.Count - num2;
          }
        }
      }
      if (fileMetadataRecordList2.Count > 0 || fileMetadataRecordList1.Count > 0)
      {
        int num3 = fileMetadataRecordList2.Count + fileMetadataRecordList1.Count;
        throw new Exception(string.Format("Failed to delete branchinfo of branches {0} from FileMetadataStore for indexingunit id {1}. ", (object) string.Join(",", (IEnumerable<string>) branchNames), (object) indexingUnit.IndexingUnitId) + FormattableString.Invariant(FormattableStringFactory.Create("No of records failed to update: {0}", (object) num3)) + FormattableString.Invariant(FormattableStringFactory.Create("No of records deleted succeccfully: {0}", (object) num1)));
      }
    }

    public virtual IEnumerable<FilePathAndContentHash> LookupForDocuments(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FilePathAndContentHash> recordsToLookup,
      DocumentContractType documentContractType)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (recordsToLookup == null || !recordsToLookup.Any<FilePathAndContentHash>())
        return recordsToLookup;
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<FilePathAndContentHash> list = recordsToLookup.ToList<FilePathAndContentHash>();
      int count1 = list.Count;
      int val1 = count1;
      List<FilePathAndContentHash> pathAndContentHashList = new List<FilePathAndContentHash>();
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(val1, 500);
        IList<FilePathAndContentHash> range = (IList<FilePathAndContentHash>) list.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_LookUpFilePathContentHash");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindFilePathContentHashParameter("@filePathContentHashDescriptor", (IEnumerable<FilePathAndContentHash>) range);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<FilePathAndContentHash>((ObjectBinder<FilePathAndContentHash>) new FileMetadataStoreComponent.FilePathContentHashColumns());
          ObjectBinder<FilePathAndContentHash> current = resultCollection.GetCurrent<FilePathAndContentHash>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
                pathAndContentHashList.AddRange((IEnumerable<FilePathAndContentHash>) current.Items);
            }
          }
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.LookupForDocuments took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        val1 -= count2;
        if (val1 <= 0)
          break;
      }
      return (IEnumerable<FilePathAndContentHash>) pathAndContentHashList;
    }

    public virtual IDictionary<FilePathBranchInfo, string> LookupForContentHashUsingFilePathBranch(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FilePathBranchInfo> recordsToLookup,
      DocumentContractType documentContractType)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (recordsToLookup == null || !recordsToLookup.Any<FilePathBranchInfo>())
        return (IDictionary<FilePathBranchInfo, string>) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<FilePathBranchInfo> list = recordsToLookup.ToList<FilePathBranchInfo>();
      int count1 = list.Count;
      int val1 = count1;
      Dictionary<FilePathBranchInfo, string> dictionary = new Dictionary<FilePathBranchInfo, string>();
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(val1, 500);
        IList<FilePathBranchInfo> range = (IList<FilePathBranchInfo>) list.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_LookUpContentHashUsingFilePathBranch");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindFilePathBranchParameter("@filePathBranchDescriptor", (IEnumerable<FilePathBranchInfo>) range);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Tuple<FilePathBranchInfo, string>>((ObjectBinder<Tuple<FilePathBranchInfo, string>>) new FileMetadataStoreComponent.FilePathBranchContentHashColumns());
          ObjectBinder<Tuple<FilePathBranchInfo, string>> current = resultCollection.GetCurrent<Tuple<FilePathBranchInfo, string>>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
              {
                foreach (Tuple<FilePathBranchInfo, string> tuple in current.Items)
                  dictionary.Add(tuple.Item1, tuple.Item2);
              }
            }
          }
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("TempFileMetadataStoreComponent.LookupForContentHashUsingFilePathBranch took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        val1 -= count2;
        if (val1 <= 0)
          break;
      }
      return (IDictionary<FilePathBranchInfo, string>) dictionary;
    }

    public virtual void DeleteFileMetadataRecords(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      this.PrepareStoredProcedure("Search.prc_DeleteFileMetadataRecords");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.ExecuteNonQuery(false);
    }

    public virtual IEnumerable<FileMetadataRecord> DeleteRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FileMetadataRecord> recordsToDelete)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      List<FileMetadataRecord> fileMetadataRecordList = new List<FileMetadataRecord>();
      List<FileMetadataRecord> source = recordsToDelete != null ? recordsToDelete.ToList<FileMetadataRecord>() : throw new ArgumentNullException(nameof (recordsToDelete));
      int num = source.Count<FileMetadataRecord>();
      int val1 = num;
      for (int index = 0; index < num; index += 500)
      {
        int count = Math.Min(val1, 500);
        IList<FileMetadataRecord> range = (IList<FileMetadataRecord>) source.GetRange(index, count);
        val1 -= count;
        if (range.Count > 0)
        {
          try
          {
            this.PrepareStoredProcedure("Search.prc_DeleteFileMetadataRecordsById");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindIds("@idList", range.Select<FileMetadataRecord, long>((System.Func<FileMetadataRecord, long>) (x => x.Id)));
            this.ExecuteNonQuery(false);
          }
          catch (Exception ex)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), ex);
            fileMetadataRecordList.AddRange((IEnumerable<FileMetadataRecord>) range);
          }
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("FileMetadataStoreComponent.DeleteRecordsById took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        }
        else
          break;
      }
      return (IEnumerable<FileMetadataRecord>) fileMetadataRecordList;
    }

    protected virtual long GetNextId(int countToReserve)
    {
      SqlCommand sqlCommand = this.PrepareStoredProcedure("dbo.prc_iCounterGetNext");
      this.BindString("@counterName", FileMetadataStoreComponent.s_serviceName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@countToReserve", countToReserve);
      SqlParameter sqlParameter = new SqlParameter("@firstIdToUse", SqlDbType.BigInt);
      sqlParameter.Direction = ParameterDirection.Output;
      sqlCommand.Parameters.Add(sqlParameter);
      this.ExecuteNonQuery(false);
      return (long) sqlParameter.Value;
    }

    protected virtual SqlParameter BindFileMetadataRecordsParameter(
      string parameterName,
      IEnumerable<FileMetadataRecord> rows,
      long firstIdToUse)
    {
      rows = rows ?? Enumerable.Empty<FileMetadataRecord>();
      System.Func<FileMetadataRecord, SqlDataRecord> selector = (System.Func<FileMetadataRecord, SqlDataRecord>) (metadataRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FileMetadataStoreComponent.s_fileMetadataInsertTable);
        sqlDataRecord.SetInt64(0, firstIdToUse++);
        sqlDataRecord.SetString(1, metadataRecord.FilePath);
        sqlDataRecord.SetSqlXml(2, new SqlXml(SQLTable<FileMetadataRecord>.ToStream((object) metadataRecord.BranchMetadata, typeof (BranchMetadata))));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IdStringXmlTable", rows.Select<FileMetadataRecord, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindUpdateFileMetadataRecordsParameter(
      string parameterName,
      IEnumerable<FileMetadataRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<FileMetadataRecord>();
      System.Func<FileMetadataRecord, SqlDataRecord> selector = (System.Func<FileMetadataRecord, SqlDataRecord>) (metadataRecord =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FileMetadataStoreComponent.s_fileMetadataUpdateTable);
        sqlDataRecord.SetInt64(0, metadataRecord.Id);
        sqlDataRecord.SetSqlXml(1, new SqlXml(SQLTable<FileMetadataRecord>.ToStream((object) metadataRecord.BranchMetadata, typeof (BranchMetadata))));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_IdXmlTable", rows.Select<FileMetadataRecord, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindFileMetadataRecordsUpdateDescriptorParameter(
      string parameterName,
      IEnumerable<FileMetadataRecord> rows)
    {
      rows = rows ?? Enumerable.Empty<FileMetadataRecord>();
      List<SqlDataRecord> rows1 = new List<SqlDataRecord>();
      foreach (FileMetadataRecord row in rows)
      {
        if (row?.BranchMetadata?.Metadata?.Keys != null)
        {
          foreach (string key in row.BranchMetadata.Metadata.Keys)
          {
            List<string> stringList = (List<string>) row.BranchMetadata.Metadata[key];
            if (stringList != null)
            {
              foreach (string str in stringList)
              {
                SqlDataRecord sqlDataRecord = new SqlDataRecord(FileMetadataStoreComponent.s_fileMetadataUpdateDescriptorTable);
                sqlDataRecord.SetSqlInt64(0, (SqlInt64) row.Id);
                sqlDataRecord.SetString(1, row.FilePath);
                sqlDataRecord.SetString(2, key);
                sqlDataRecord.SetString(3, str);
                rows1.Add(sqlDataRecord);
              }
            }
          }
        }
      }
      return this.BindTable(parameterName, "Search.typ_FileMetadataStoreUpdateDescriptor", (IEnumerable<SqlDataRecord>) rows1);
    }

    protected virtual SqlParameter BindFilePathContentHashParameter(
      string parameterName,
      IEnumerable<FilePathAndContentHash> recordsToLookup)
    {
      recordsToLookup = recordsToLookup ?? Enumerable.Empty<FilePathAndContentHash>();
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (FilePathAndContentHash pathAndContentHash in recordsToLookup)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FileMetadataStoreComponent.s_filePathContentHashDescriptorTable);
        sqlDataRecord.SetString(0, pathAndContentHash.FilePath);
        sqlDataRecord.SetString(1, pathAndContentHash.ContentHash);
        rows.Add(sqlDataRecord);
      }
      return this.BindTable(parameterName, "Search.typ_FilePathContentHash", (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual SqlParameter BindFilePathBranchParameter(
      string parameterName,
      IEnumerable<FilePathBranchInfo> recordsToLookup)
    {
      recordsToLookup = recordsToLookup ?? Enumerable.Empty<FilePathBranchInfo>();
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (FilePathBranchInfo filePathBranchInfo in recordsToLookup)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FileMetadataStoreComponent.s_stringStringDecsriptorTable);
        sqlDataRecord.SetString(0, filePathBranchInfo.FilePath);
        sqlDataRecord.SetString(1, filePathBranchInfo.Branch);
        rows.Add(sqlDataRecord);
      }
      return this.BindTable(parameterName, "Search.typ_StringStringDescriptor", (IEnumerable<SqlDataRecord>) rows);
    }

    protected SqlParameter BindIds(string parameterName, IEnumerable<long> rows)
    {
      rows = rows ?? Enumerable.Empty<long>();
      System.Func<long, SqlDataRecord> selector = (System.Func<long, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SQLTable<FileMetadataRecord>.BigIntTable);
        sqlDataRecord.SetValue(0, (object) entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_Int64Descriptor", rows.Select<long, SqlDataRecord>(selector));
    }

    internal virtual void MergeExistingRecordsWithRecordsToBeUpdated(
      IDictionary<string, FileMetadataRecord> existingRecords,
      IDictionary<string, FileMetadataRecord> updateRecords,
      out IDictionary<string, FileMetadataRecord> recordsToBeInserted)
    {
      recordsToBeInserted = this.GetNewFilePathToFileMetadataRecordsDictionary();
      if (updateRecords == null || !updateRecords.Any<KeyValuePair<string, FileMetadataRecord>>())
        return;
      if (existingRecords == null)
        existingRecords = this.GetNewFilePathToFileMetadataRecordsDictionary();
      foreach (KeyValuePair<string, FileMetadataRecord> updateRecord in (IEnumerable<KeyValuePair<string, FileMetadataRecord>>) updateRecords)
      {
        string key1 = updateRecord.Key;
        ContentHashBranchListDictionary metadata = updateRecord.Value.BranchMetadata.Metadata;
        FileMetadataRecord fileMetadataRecord;
        if (existingRecords.TryGetValue(key1, out fileMetadataRecord))
        {
          if (fileMetadataRecord == null)
          {
            existingRecords[key1] = updateRecord.Value;
          }
          else
          {
            ContentHashBranchListDictionary branchListDictionary = fileMetadataRecord.BranchMetadata?.Metadata;
            if (branchListDictionary == null)
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("NULL Metadata found for record {0} while merging for Update records.", (object) fileMetadataRecord));
              branchListDictionary = new ContentHashBranchListDictionary();
              fileMetadataRecord.BranchMetadata = new BranchMetadata()
              {
                Metadata = branchListDictionary
              };
            }
            foreach (KeyValuePair<string, Branches> keyValuePair in (Dictionary<string, Branches>) metadata)
            {
              string key2 = keyValuePair.Key;
              Branches collection1 = keyValuePair.Value;
              Branches collection2;
              if (!branchListDictionary.TryGetValue(key2, out collection2))
                collection2 = new Branches();
              collection2.AddRange((IEnumerable<string>) collection1);
              Branches branches = new Branches();
              branches.AddRange((IEnumerable<string>) new HashSet<string>((IEnumerable<string>) collection2));
              branchListDictionary[key2] = branches;
            }
          }
        }
        else
          recordsToBeInserted[key1] = updateRecord.Value;
      }
    }

    internal virtual void MergeExistingRecordsWithRecordsToBeDeleted(
      IDictionary<string, FileMetadataRecord> existingRecords,
      IDictionary<string, FileMetadataRecord> updateRecords,
      out IDictionary<string, FileMetadataRecord> recordsToBeDeleted)
    {
      recordsToBeDeleted = this.GetNewFilePathToFileMetadataRecordsDictionary();
      if (updateRecords == null || !updateRecords.Any<KeyValuePair<string, FileMetadataRecord>>() || existingRecords == null)
        return;
      foreach (KeyValuePair<string, FileMetadataRecord> updateRecord in (IEnumerable<KeyValuePair<string, FileMetadataRecord>>) updateRecords)
      {
        string key1 = updateRecord.Key;
        ContentHashBranchListDictionary metadata1 = updateRecord.Value.BranchMetadata.Metadata;
        FileMetadataRecord fileMetadataRecord;
        if (existingRecords.TryGetValue(key1, out fileMetadataRecord))
        {
          ContentHashBranchListDictionary metadata2 = fileMetadataRecord?.BranchMetadata?.Metadata;
          if (metadata2 == null || !metadata2.Any<KeyValuePair<string, Branches>>())
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("NULL Metadata found for record {0} while merging for Delete records.", (object) fileMetadataRecord));
            recordsToBeDeleted[key1] = fileMetadataRecord;
            existingRecords.Remove(key1);
          }
          else
          {
            foreach (KeyValuePair<string, Branches> keyValuePair in (Dictionary<string, Branches>) metadata1)
            {
              string key2 = keyValuePair.Key;
              Branches updateRecordBranches = keyValuePair.Value;
              Branches source;
              if (!metadata2.TryGetValue(key2, out source))
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), string.Format("NULL Branches found for record {0} while merging for Delete records.", (object) fileMetadataRecord));
              }
              else
              {
                source.RemoveAll((Predicate<string>) (x => updateRecordBranches.Contains(x)));
                if (!source.Any<string>())
                  metadata2.Remove(key2);
              }
            }
            if (!metadata2.Any<KeyValuePair<string, Branches>>())
            {
              recordsToBeDeleted[key1] = fileMetadataRecord;
              existingRecords.Remove(key1);
            }
          }
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082626, "Indexing Pipeline", nameof (FileMetadataStoreComponent), key1 + " not found in existing records while performing Deletion of Branches.");
      }
    }

    internal virtual IDictionary<string, FileMetadataRecord> GetNewFilePathToFileMetadataRecordsDictionary() => (IDictionary<string, FileMetadataRecord>) new Dictionary<string, FileMetadataRecord>((IEqualityComparer<string>) this.FilePathComparer);

    protected class FileMetadataStoreColumns : ObjectBinder<FileMetadataRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_filePath = new SqlColumnBinder("FilePath");
      private SqlColumnBinder m_metadata = new SqlColumnBinder("Metadata");

      protected override FileMetadataRecord Bind() => new FileMetadataRecord()
      {
        Id = this.m_id.GetInt64((IDataReader) this.Reader),
        FilePath = this.m_filePath.GetString((IDataReader) this.Reader, false),
        BranchMetadata = (BranchMetadata) SQLTable<FileMetadataRecord>.FromString(this.m_metadata.GetString((IDataReader) this.Reader, false), typeof (BranchMetadata))
      };
    }

    protected class FilePathContentHashColumns : ObjectBinder<FilePathAndContentHash>
    {
      private SqlColumnBinder m_filePath = new SqlColumnBinder("FilePath");
      private SqlColumnBinder m_contentHash = new SqlColumnBinder("ContentHash");

      protected override FilePathAndContentHash Bind() => new FilePathAndContentHash(this.m_filePath.GetString((IDataReader) this.Reader, false), this.m_contentHash.GetString((IDataReader) this.Reader, false));
    }

    protected class FilePathBranchContentHashColumns : 
      ObjectBinder<Tuple<FilePathBranchInfo, string>>
    {
      private SqlColumnBinder m_filePath = new SqlColumnBinder("FilePath");
      private SqlColumnBinder m_branch = new SqlColumnBinder("BranchName");
      private SqlColumnBinder m_contentHash = new SqlColumnBinder("ContentHash");

      protected override Tuple<FilePathBranchInfo, string> Bind() => new Tuple<FilePathBranchInfo, string>(new FilePathBranchInfo()
      {
        FilePath = this.m_filePath.GetString((IDataReader) this.Reader, false),
        Branch = this.m_branch.GetString((IDataReader) this.Reader, false)
      }, this.m_contentHash.GetString((IDataReader) this.Reader, false));
    }
  }
}
