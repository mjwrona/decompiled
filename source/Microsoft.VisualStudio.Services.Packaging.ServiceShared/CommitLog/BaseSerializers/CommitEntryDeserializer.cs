// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers.CommitEntryDeserializer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers
{
  public class CommitEntryDeserializer : ICommitEntryDeserializer
  {
    private readonly IProtocolSpecificCommitEntryDeserializer protocolSpecificDeserializer;
    private readonly Func<string, string, IPackageIdentity> identityConverter;
    private readonly ITracerService tracerService;

    public CommitEntryDeserializer(
      IProtocolSpecificCommitEntryDeserializer protocolSpecificDeserializer,
      Func<string, string, IPackageIdentity> identityConverter,
      ITracerService tracerService)
    {
      this.protocolSpecificDeserializer = protocolSpecificDeserializer ?? throw new ArgumentNullException(nameof (protocolSpecificDeserializer));
      this.identityConverter = identityConverter ?? throw new ArgumentNullException(nameof (identityConverter));
      this.tracerService = tracerService;
    }

    public ICommitOperationData Deserialize(CommitLogItem commitLogItem)
    {
      using (ITracerBlock tracerBlock = this.tracerService.Enter((object) this, nameof (Deserialize)))
        return this.DeserializeCore(tracerBlock, commitLogItem, commitLogItem.ItemType, commitLogItem.PackagingCommitId, commitLogItem.SequenceNumber, new int?(), commitLogItem.Data);
    }

    private ICommitOperationData DeserializeCore(
      ITracerBlock tracerBlock,
      CommitLogItem commitLogItem,
      string commitLogItemType,
      PackagingCommitId commitId,
      long sequenceNumber,
      int? batchElementIndex,
      IItemData storedItemData)
    {
      string compressedForm = storedItemData["protocolOperation"];
      if (compressedForm == null && this.protocolSpecificDeserializer is IHaveLegacyProtocolOperationKey specificDeserializer1)
        compressedForm = storedItemData[specificDeserializer1.LegacyProtocolOperationKey];
      if (compressedForm == null)
      {
        if (this.protocolSpecificDeserializer is ISupportPreProtocolOperationCommits specificDeserializer2)
          return specificDeserializer2.DeserializePreProtocolOperationCommit(storedItemData);
        List<string> values = new List<string>()
        {
          "Tried key 'protocolOperation'",
          this.protocolSpecificDeserializer is IHaveLegacyProtocolOperationKey specificDeserializer3 ? "Tried legacy key '" + specificDeserializer3.LegacyProtocolOperationKey + "'" : "No legacy key for this protocol",
          this.protocolSpecificDeserializer is ISupportPreProtocolOperationCommits ? "Tried pre-ProtocolOperation legacy deserializer" : "No pre-ProtocolOperation legacy deserializer for this protocol"
        };
        string operation = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CommitDeserializerCannotDetermineOperation((object) CommitEntryDeserializer.GetCommitIdentifierForLogs(commitId, sequenceNumber, batchElementIndex));
        tracerBlock.TraceError(operation + "\n" + string.Join("\n", (IEnumerable<string>) values));
        throw new DeserializationFailedException(operation);
      }
      ProtocolOperation operation1 = (ProtocolOperation) null;
      if (this.protocolSpecificDeserializer is IHaveLegacyProtocolOperationValues specificDeserializer4)
        operation1 = specificDeserializer4.TranslateProtocolOperationValueOrDefault(compressedForm);
      if (operation1 == null)
        operation1 = ProtocolOperation.ParseFromCompressedForm(compressedForm);
      ICommitOperationData commitOperationData = this.protocolSpecificDeserializer.Deserialize(commitLogItem, operation1, storedItemData);
      if (commitOperationData != null)
        return commitOperationData;
      return this.DeserializeCommonOperations(commitId, sequenceNumber, batchElementIndex, commitLogItemType, operation1, storedItemData, tracerBlock) ?? throw new DeserializationFailedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CommitDeserializerUnknownOperation((object) operation1, (object) CommitEntryDeserializer.GetCommitIdentifierForLogs(commitId, sequenceNumber, batchElementIndex)));
    }

    private static string GetCommitIdentifierForLogs(
      PackagingCommitId commitId,
      long sequenceNumber,
      int? batchElementIndex)
    {
      string str = batchElementIndex.HasValue ? string.Format("/{0}", (object) batchElementIndex.Value) : string.Empty;
      return string.Format("{0}#{1}{2}", (object) commitId, (object) sequenceNumber, (object) str);
    }

    private ICommitOperationData? DeserializeCommonOperations(
      PackagingCommitId commitId,
      long sequenceNumber,
      int? batchElementIndex,
      string commitLogItemType,
      ProtocolOperation operation,
      IItemData itemData,
      ITracerBlock tracerBlock)
    {
      if (operation.Equals((ProtocolOperation) BatchOperation.Instance))
      {
        if (batchElementIndex.HasValue)
          throw new DeserializationFailedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CommitDeserializerNestedBatch((object) CommitEntryDeserializer.GetCommitIdentifierForLogs(commitId, sequenceNumber, batchElementIndex)));
        return (ICommitOperationData) this.DeserializeBatchCommitOperation(commitId, sequenceNumber, commitLogItemType, itemData, tracerBlock);
      }
      if (operation.Equals((ProtocolOperation) NoOpProtocolOperation.Instance))
        return (ICommitOperationData) CommitLogDeserializationHelpers.GetNoOpOperationData(itemData);
      if (operation.Equals((ProtocolOperation) PackageBlobsDereferencedMarkerProtocolOperation.Instance))
        return (ICommitOperationData) CommitLogDeserializationHelpers.GetMarkerData(itemData);
      if (operation.Equals((ProtocolOperation) AddProblemPackageOperation.Instance))
        return CommitLogDeserializationHelpers.GetAddProblemPackageOperationData(ExtractIdentity(), itemData);
      ProtocolOperation view = this.protocolSpecificDeserializer.CommonProtocolOperations.View;
      if (view != null && operation.Equals(view))
        return (ICommitOperationData) CommitLogDeserializationHelpers.GetViewOperationData(ExtractIdentity(), itemData);
      ProtocolOperation delete = this.protocolSpecificDeserializer.CommonProtocolOperations.Delete;
      if (delete != null && operation.Equals(delete))
        return (ICommitOperationData) CommitLogDeserializationHelpers.GetDeleteOperationData(this.identityConverter, itemData);
      ProtocolOperation restoreToFeed = this.protocolSpecificDeserializer.CommonProtocolOperations.RestoreToFeed;
      if (restoreToFeed != null && operation.Equals(restoreToFeed))
        return (ICommitOperationData) new RestoreToFeedOperationData(ExtractIdentity());
      ProtocolOperation delist = this.protocolSpecificDeserializer.CommonProtocolOperations.Delist;
      if (delist != null && operation.Equals(delist))
        return (ICommitOperationData) CommitLogDeserializationHelpers.GetDelistOperationData(itemData, this.identityConverter);
      ProtocolOperation relist = this.protocolSpecificDeserializer.CommonProtocolOperations.Relist;
      return relist != null && operation.Equals(relist) ? (ICommitOperationData) CommitLogDeserializationHelpers.GetRelistOperationData(itemData, this.identityConverter) : (ICommitOperationData) null;

      IPackageIdentity ExtractIdentity()
      {
        string str1 = itemData["name"];
        string str2 = itemData["version"];
        if (this.protocolSpecificDeserializer is IHaveLegacyPackageIdentityKeys specificDeserializer1)
        {
          if (str1 == null)
            str1 = itemData[specificDeserializer1.LegacyPackageNameKey];
          if (str2 == null)
            str2 = itemData[specificDeserializer1.LegacyPackageVersionKey];
        }
        if (str1 == null || str2 == null)
        {
          List<string> values = new List<string>()
          {
            "Tried keys 'name' and 'version'",
            this.protocolSpecificDeserializer is IHaveLegacyPackageIdentityKeys specificDeserializer2 ? "Tried legacy keys '" + specificDeserializer2.LegacyPackageNameKey + "' and '" + specificDeserializer2.LegacyPackageVersionKey : "No legacy keys for this protocol",
            string.Format("Operation type is {0}", (object) operation)
          };
          string identity = Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CommitDeserializerCannotDetermineIdentity((object) CommitEntryDeserializer.GetCommitIdentifierForLogs(commitId, sequenceNumber, batchElementIndex));
          tracerBlock.TraceError(identity + "\n" + string.Join("\n", (IEnumerable<string>) values));
          throw new DeserializationFailedException(identity);
        }
        return this.identityConverter(str1, str2);
      }
    }

    private BatchCommitOperationData DeserializeBatchCommitOperation(
      PackagingCommitId commitId,
      long sequenceNumber,
      string commitLogItemType,
      IItemData storedItemData,
      ITracerBlock tracerBlock)
    {
      List<Dictionary<string, object>> dictionaryList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(storedItemData["operations"]);
      List<ICommitOperationData> operations = new List<ICommitOperationData>(dictionaryList.Count);
      foreach (IDictionary<string, object> data in dictionaryList)
      {
        IItemData itemData = (IItemData) new DictionaryItemData(data);
        CommitLogItem commitLogItem = new CommitLogItem(itemData, commitLogItemType);
        operations.Add(this.DeserializeCore(tracerBlock, commitLogItem, commitLogItemType, commitId, sequenceNumber, new int?(operations.Count), itemData));
      }
      return new BatchCommitOperationData((IReadOnlyCollection<ICommitOperationData>) operations);
    }
  }
}
