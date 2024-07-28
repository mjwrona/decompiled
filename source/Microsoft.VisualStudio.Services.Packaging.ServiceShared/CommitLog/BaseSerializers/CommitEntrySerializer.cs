// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers.CommitEntrySerializer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers
{
  public class CommitEntrySerializer : ICommitEntrySerializer
  {
    private readonly IProtocolSpecificCommitEntrySerializer protocolSpecificSerializer;

    public CommitEntrySerializer(
      IProtocolSpecificCommitEntrySerializer protocolSpecificSerializer)
    {
      this.protocolSpecificSerializer = protocolSpecificSerializer;
    }

    public IDictionary<string, string?> Serialize(ICommitOperationData commitOperationData) => this.SerializeCore(commitOperationData);

    private IDictionary<string, string?> SerializeCore(ICommitOperationData commitOperationData)
    {
      IDictionary<string, string> dictionary = this.protocolSpecificSerializer.Serialize(commitOperationData);
      if (dictionary != null)
        return dictionary;
      return this.SerializeCommonOperations(commitOperationData) ?? throw new InvalidOperationException(Resources.Error_UnknownOperationType((object) commitOperationData.GetType().Name));
    }

    private IDictionary<string, string?>? SerializeCommonOperations(
      ICommitOperationData commitOperationData)
    {
      switch (commitOperationData)
      {
        case BatchCommitOperationData batchOperation:
          return (IDictionary<string, string>) this.SerializeBatchCommitOperation(batchOperation);
        case MarkerOperationData markerOperationData:
          return (IDictionary<string, string>) CommitLogSerializationHelpers.GetMarkerOperationDictionary(markerOperationData);
        case INoOpOperationData noOpOperationData:
          return (IDictionary<string, string>) CommitLogSerializationHelpers.GetNoOpOperationDictionary((ProtocolOperation) NoOpProtocolOperation.Instance, noOpOperationData);
        case AddProblemPackageOperationData addProblemPackageOperationData:
          return CommitLogSerializationHelpers.GetAddProblemPackageOperationDictionary(addProblemPackageOperationData);
        default:
          ProtocolOperation view = this.protocolSpecificSerializer.CommonProtocolOperations.View;
          if (view != null && commitOperationData is ViewOperationData viewOperationData)
            return (IDictionary<string, string>) CommitLogSerializationHelpers.GetViewOperationDictionary(view, (IViewOperationData) viewOperationData);
          ProtocolOperation delete = this.protocolSpecificSerializer.CommonProtocolOperations.Delete;
          if (delete != null && commitOperationData is DeleteOperationData deleteOperationData)
            return (IDictionary<string, string>) CommitLogSerializationHelpers.GetDeleteOperationDictionary(delete, (IDeleteOperationData) deleteOperationData);
          ProtocolOperation restoreToFeed = this.protocolSpecificSerializer.CommonProtocolOperations.RestoreToFeed;
          if (restoreToFeed != null && commitOperationData is RestoreToFeedOperationData restoreToFeedOperationData)
            return (IDictionary<string, string>) CommitLogSerializationHelpers.GetRestoreToFeedOperationDictionary(restoreToFeed, (IRestoreToFeedOperationData) restoreToFeedOperationData);
          ProtocolOperation delist = this.protocolSpecificSerializer.CommonProtocolOperations.Delist;
          if (delist != null && commitOperationData is DelistOperationData delistOperationData)
            return CommitLogSerializationHelpers.GetDelistOperationDictionary(delist, (IDelistOperationData) delistOperationData);
          ProtocolOperation relist = this.protocolSpecificSerializer.CommonProtocolOperations.Relist;
          return relist != null && commitOperationData is RelistOperationData relistOperationData ? CommitLogSerializationHelpers.GetRelistOperationDictionary(relist, (IRelistOperationData) relistOperationData) : (IDictionary<string, string>) null;
      }
    }

    private Dictionary<string, string?> SerializeBatchCommitOperation(
      BatchCommitOperationData batchOperation)
    {
      List<IDictionary<string, string>> dictionaryList = new List<IDictionary<string, string>>();
      foreach (ICommitOperationData operation in batchOperation.Operations)
      {
        IDictionary<string, string> dictionary = this.SerializeCore(operation);
        dictionaryList.Add(dictionary);
      }
      return new Dictionary<string, string>()
      {
        {
          "protocolOperation",
          BatchOperation.Instance.ToString()
        },
        {
          "operations",
          dictionaryList.Serialize<List<IDictionary<string, string>>>()
        }
      };
    }
  }
}
