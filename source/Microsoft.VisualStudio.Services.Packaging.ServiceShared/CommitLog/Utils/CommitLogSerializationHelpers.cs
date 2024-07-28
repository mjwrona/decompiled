// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils.CommitLogSerializationHelpers
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils
{
  public static class CommitLogSerializationHelpers
  {
    public static Dictionary<string, string> GetDeleteOperationDictionary(
      ProtocolOperation protocolOperation,
      IDeleteOperationData deleteOperationData)
    {
      CommitLogUtils.CheckDateIsUtc(deleteOperationData.DeletedDate, "DeletedDate");
      DateTime? permanentDeleteDate;
      if (deleteOperationData.ScheduledPermanentDeleteDate.HasValue)
      {
        permanentDeleteDate = deleteOperationData.ScheduledPermanentDeleteDate;
        CommitLogUtils.CheckDateIsUtc(permanentDeleteDate.Value, "ScheduledPermanentDeleteDate");
      }
      Dictionary<string, string> operationDictionaryCore = CommitLogSerializationHelpers.GetPackageVersionOperationDictionaryCore((IPackageVersionOperationData) deleteOperationData, protocolOperation);
      operationDictionaryCore.Add("deletedDate", deleteOperationData.DeletedDate.ToBinary().ToString());
      permanentDeleteDate = deleteOperationData.ScheduledPermanentDeleteDate;
      ref DateTime? local = ref permanentDeleteDate;
      operationDictionaryCore.Add("scheduledPermanentDeleteDate", local.HasValue ? local.GetValueOrDefault().ToBinary().ToString() : (string) null);
      return operationDictionaryCore;
    }

    public static Dictionary<string, string> GetRestoreToFeedOperationDictionary(
      ProtocolOperation protocolOperation,
      IRestoreToFeedOperationData restoreToFeedOperationData)
    {
      return new Dictionary<string, string>()
      {
        {
          "name",
          restoreToFeedOperationData.Identity.Name.DisplayName
        },
        {
          "version",
          restoreToFeedOperationData.Identity.Version.DisplayVersion
        },
        {
          nameof (protocolOperation),
          protocolOperation.ToString()
        }
      };
    }

    public static Dictionary<string, string> GetViewOperationDictionary(
      ProtocolOperation protocolOperation,
      IViewOperationData viewOperationData)
    {
      Dictionary<string, string> operationDictionaryCore = CommitLogSerializationHelpers.GetPackageVersionOperationDictionaryCore((IPackageVersionOperationData) viewOperationData, protocolOperation);
      operationDictionaryCore.Add("view", viewOperationData.ViewId.ToString());
      operationDictionaryCore.Add("metadataSuboperation", viewOperationData.MetadataSuboperation.ToString());
      return operationDictionaryCore;
    }

    public static Dictionary<string, string> GetClearUpstreamCacheOperationDictionary(
      ProtocolOperation protocolOperation,
      ClearUpstreamCacheOperationData clearUpstreamCacheOperationData)
    {
      return new Dictionary<string, string>()
      {
        {
          nameof (protocolOperation),
          protocolOperation.ToString()
        },
        {
          "UpstreamRevision",
          clearUpstreamCacheOperationData.UpstreamRevision.ToString()
        },
        {
          "UpstreamSource",
          clearUpstreamCacheOperationData.UpstreamSource
        }
      };
    }

    public static Dictionary<string, string> GetNoOpOperationDictionary(
      ProtocolOperation protocolOperation,
      INoOpOperationData noOpOperationData)
    {
      return new Dictionary<string, string>()
      {
        {
          nameof (protocolOperation),
          protocolOperation.ToString()
        }
      };
    }

    public static Dictionary<string, string> GetMarkerOperationDictionary(
      MarkerOperationData markerOperationData)
    {
      return new Dictionary<string, string>()
      {
        {
          "protocolOperation",
          new MarkerProtocolOperation(markerOperationData.OperationName, markerOperationData.Version).ToString()
        }
      };
    }

    public static IDictionary<string, string> GetAddProblemPackageOperationDictionary(
      AddProblemPackageOperationData addProblemPackageOperationData)
    {
      Dictionary<string, string> operationDictionaryCore = CommitLogSerializationHelpers.GetPackageVersionOperationDictionaryCore((IPackageVersionOperationData) addProblemPackageOperationData, (ProtocolOperation) AddProblemPackageOperation.Instance);
      operationDictionaryCore.Add("reasons", addProblemPackageOperationData.Reasons.Serialize<IEnumerable<TerrapinIngestionValidationReason>>(true));
      operationDictionaryCore.Add("UpstreamSource", addProblemPackageOperationData.UpstreamSource.Serialize<UpstreamSourceInfo>(true));
      return (IDictionary<string, string>) operationDictionaryCore;
    }

    public static IDictionary<string, string> GetDelistOperationDictionary(
      ProtocolOperation operationId,
      IDelistOperationData delistOperationData)
    {
      return (IDictionary<string, string>) CommitLogSerializationHelpers.GetPackageVersionOperationDictionaryCore((IPackageVersionOperationData) delistOperationData, operationId);
    }

    public static IDictionary<string, string> GetRelistOperationDictionary(
      ProtocolOperation operationId,
      IRelistOperationData relistOperationData)
    {
      return (IDictionary<string, string>) CommitLogSerializationHelpers.GetPackageVersionOperationDictionaryCore((IPackageVersionOperationData) relistOperationData, operationId);
    }

    private static Dictionary<string, string> GetPackageVersionOperationDictionaryCore(
      IPackageVersionOperationData opData,
      ProtocolOperation operationId)
    {
      return new Dictionary<string, string>()
      {
        {
          "name",
          opData.Identity.Name.DisplayName
        },
        {
          "version",
          opData.Identity.Version.DisplayVersion
        },
        {
          "protocolOperation",
          operationId.ToString()
        }
      };
    }
  }
}
