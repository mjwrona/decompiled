// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.TempFileMetaDataRecordExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public static class TempFileMetaDataRecordExtensions
  {
    public static readonly IEqualityComparer<BranchInfo> branchComparer = (IEqualityComparer<BranchInfo>) new BranchInfo.BranchInfoComparer();

    public static void RemoveDuplicateBranchInfo(this TempFileMetadataRecord record)
    {
      foreach (KeyValuePair<string, UpdateTypeDictionary> keyValuePair in (Dictionary<string, UpdateTypeDictionary>) record.TemporaryBranchMetadata.BranchMetadata)
      {
        foreach (MetaDataStoreUpdateType key in keyValuePair.Value.Keys.ToArray<MetaDataStoreUpdateType>())
          keyValuePair.Value[key] = keyValuePair.Value[key].Distinct<BranchInfo>(TempFileMetaDataRecordExtensions.branchComparer).ToList<BranchInfo>();
      }
    }

    public static void AddToPipelineDocuments(
      this TempFileMetadataRecord record,
      VersionControlType vcType,
      PipelineDocumentCollection<CodePipelineDocumentId, CodeDocument> codePipelineDocs,
      bool lenient = false)
    {
      string originalFilePath = record.FileAttributes.OriginalFilePath;
      foreach (KeyValuePair<string, UpdateTypeDictionary> keyValuePair1 in (Dictionary<string, UpdateTypeDictionary>) record.TemporaryBranchMetadata.BranchMetadata)
      {
        foreach (KeyValuePair<MetaDataStoreUpdateType, List<BranchInfo>> keyValuePair2 in (Dictionary<MetaDataStoreUpdateType, List<BranchInfo>>) keyValuePair1.Value)
        {
          MetaDataStoreUpdateType key = keyValuePair2.Key;
          if (keyValuePair2.Value != null)
          {
            foreach (BranchInfo branchInfo in keyValuePair2.Value)
            {
              string branchName = branchInfo.BranchName;
              string filePath;
              if (vcType != VersionControlType.TFVC || string.IsNullOrWhiteSpace(branchName))
                filePath = originalFilePath;
              else
                filePath = FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}", (object) branchName, (object) originalFilePath));
              string branch = branchName;
              CodePipelineDocumentId pipelineDocumentId = new CodePipelineDocumentId(filePath, branch);
              CodeDocument doc;
              if (record.FileAttributes.DocumentContractType.IsDedupeFileContract() && codePipelineDocs.TryGetLenient(pipelineDocumentId, out doc) && doc.ShouldProcess)
              {
                if ((doc.UpdateType == MetaDataStoreUpdateType.Add || doc.UpdateType == MetaDataStoreUpdateType.UpdateMetaData) && key == MetaDataStoreUpdateType.Delete || doc.UpdateType == MetaDataStoreUpdateType.Delete && (key == MetaDataStoreUpdateType.Add || key == MetaDataStoreUpdateType.UpdateMetaData))
                {
                  doc.AuditTrail.Record(FormattableString.Invariant(FormattableStringFactory.Create("Consolidating update type from {0} to {1}", (object) doc.UpdateType, (object) MetaDataStoreUpdateType.Edit)));
                  doc.UpdateType = MetaDataStoreUpdateType.Edit;
                  continue;
                }
                if (key == MetaDataStoreUpdateType.Delete && (doc.UpdateType == MetaDataStoreUpdateType.Edit || doc.UpdateType == MetaDataStoreUpdateType.Delete))
                {
                  doc.AuditTrail.Record(FormattableString.Invariant(FormattableStringFactory.Create("Multiple delete entries for the given doc.")));
                  doc.HasMultipleDeleteContentHashes = true;
                  continue;
                }
              }
              if (!lenient || !codePipelineDocs.TryGetLenient(pipelineDocumentId, out CodeDocument _))
                codePipelineDocs.Add(new CodeDocument(vcType, pipelineDocumentId, key));
            }
          }
        }
      }
    }
  }
}
