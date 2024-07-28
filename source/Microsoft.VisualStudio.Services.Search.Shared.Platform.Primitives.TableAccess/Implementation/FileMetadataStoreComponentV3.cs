// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.FileMetadataStoreComponentV3
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
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class FileMetadataStoreComponentV3 : FileMetadataStoreComponentV2
  {
    public FileMetadataStoreComponentV3()
    {
    }

    internal FileMetadataStoreComponentV3(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override IEnumerable<FilePathAndContentHash> LookupForDocuments(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FilePathAndContentHash> recordsToLookup,
      DocumentContractType documentContractType)
    {
      if (documentContractType != DocumentContractType.DedupeFileContractV4 && documentContractType != DocumentContractType.DedupeFileContractV5)
        return base.LookupForDocuments(indexingUnit, recordsToLookup, documentContractType);
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (recordsToLookup == null || !recordsToLookup.Any<FilePathAndContentHash>())
        return recordsToLookup;
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<FilePathAndContentHash> pathAndContentHashList1 = new List<FilePathAndContentHash>();
      foreach (FilePathAndContentHash pathAndContentHash in recordsToLookup)
        pathAndContentHashList1.Add(new FilePathAndContentHash(FileAttributes.GetSHA2Hash(pathAndContentHash.FilePath).HexHash, pathAndContentHash.ContentHash));
      int count1 = pathAndContentHashList1.Count;
      int val1 = count1;
      List<FilePathAndContentHash> pathAndContentHashList2 = new List<FilePathAndContentHash>();
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(val1, 500);
        IList<FilePathAndContentHash> range = (IList<FilePathAndContentHash>) pathAndContentHashList1.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_LookUpFilePathContentHashV2");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindFilePathContentHashParameter("@filePathContentHashDescriptor", (IEnumerable<FilePathAndContentHash>) range);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<FilePathAndContentHash>((ObjectBinder<FilePathAndContentHash>) new FileMetadataStoreComponentV3.FilePathContentHashColumnsV2());
          ObjectBinder<FilePathAndContentHash> current = resultCollection.GetCurrent<FilePathAndContentHash>();
          if (current != null)
          {
            if (current.Items != null)
            {
              if (current.Items.Count > 0)
                pathAndContentHashList2.AddRange((IEnumerable<FilePathAndContentHash>) current.Items);
            }
          }
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, this.TraceArea, "FileMetadataStoreComponent", string.Format("TempFileMetadataStoreComponent.LookupForDocuments took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        val1 -= count2;
        if (val1 <= 0)
          break;
      }
      return (IEnumerable<FilePathAndContentHash>) pathAndContentHashList2;
    }

    public override IDictionary<FilePathBranchInfo, string> LookupForContentHashUsingFilePathBranch(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<FilePathBranchInfo> recordsToLookup,
      DocumentContractType documentContractType)
    {
      if (documentContractType != DocumentContractType.DedupeFileContractV4 && documentContractType != DocumentContractType.DedupeFileContractV5)
        return base.LookupForContentHashUsingFilePathBranch(indexingUnit, recordsToLookup, documentContractType);
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (recordsToLookup == null || !recordsToLookup.Any<FilePathBranchInfo>())
        return (IDictionary<FilePathBranchInfo, string>) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<FilePathBranchInfo> filePathBranchInfoList = new List<FilePathBranchInfo>();
      foreach (FilePathBranchInfo filePathBranchInfo in recordsToLookup)
        filePathBranchInfoList.Add(new FilePathBranchInfo()
        {
          FilePath = FileAttributes.GetSHA2Hash(filePathBranchInfo.FilePath).HexHash,
          Branch = filePathBranchInfo.Branch
        });
      int count1 = filePathBranchInfoList.Count;
      int val1 = count1;
      Dictionary<FilePathBranchInfo, string> dictionary = new Dictionary<FilePathBranchInfo, string>();
      for (int index = 0; index < count1; index += 500)
      {
        int count2 = Math.Min(val1, 500);
        IList<FilePathBranchInfo> range = (IList<FilePathBranchInfo>) filePathBranchInfoList.GetRange(index, count2);
        this.PrepareStoredProcedure("Search.prc_LookUpContentHashUsingFilePathBranchV2");
        this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
        this.BindFilePathBranchParameter("@filePathBranchDescriptor", (IEnumerable<FilePathBranchInfo>) range);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Tuple<FilePathBranchInfo, string>>((ObjectBinder<Tuple<FilePathBranchInfo, string>>) new FileMetadataStoreComponentV3.FilePathBranchContentHashColumnsV2());
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
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082626, this.TraceArea, "FileMetadataStoreComponent", string.Format("TempFileMetadataStoreComponent.LookupForContentHashUsingFilePathBranch took {0}ms", (object) stopwatch.ElapsedMilliseconds));
        val1 -= count2;
        if (val1 <= 0)
          break;
      }
      return (IDictionary<FilePathBranchInfo, string>) dictionary;
    }

    protected class FilePathBranchContentHashColumnsV2 : 
      ObjectBinder<Tuple<FilePathBranchInfo, string>>
    {
      private SqlColumnBinder m_metaData = new SqlColumnBinder("BranchMetaData");
      private SqlColumnBinder m_branch = new SqlColumnBinder("BranchName");
      private SqlColumnBinder m_contentHash = new SqlColumnBinder("ContentHash");

      protected override Tuple<FilePathBranchInfo, string> Bind()
      {
        BranchMetadata branchMetadata = (BranchMetadata) SQLTable<FileMetadataRecord>.FromString(this.m_metaData.GetString((IDataReader) this.Reader, false), typeof (BranchMetadata));
        return new Tuple<FilePathBranchInfo, string>(new FilePathBranchInfo()
        {
          FilePath = branchMetadata.FilePath,
          Branch = this.m_branch.GetString((IDataReader) this.Reader, false)
        }, this.m_contentHash.GetString((IDataReader) this.Reader, false));
      }
    }

    protected class FilePathContentHashColumnsV2 : ObjectBinder<FilePathAndContentHash>
    {
      private SqlColumnBinder m_metaData = new SqlColumnBinder("Metadata");
      private SqlColumnBinder m_contentHash = new SqlColumnBinder("ContentHash");

      protected override FilePathAndContentHash Bind() => new FilePathAndContentHash(((BranchMetadata) SQLTable<FileMetadataRecord>.FromString(this.m_metaData.GetString((IDataReader) this.Reader, false), typeof (BranchMetadata))).FilePath, this.m_contentHash.GetString((IDataReader) this.Reader, false));
    }
  }
}
