// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmOperationSerializer
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmOperationSerializer : IProtocolSpecificCommitEntrySerializer
  {
    public CommonProtocolOperations CommonProtocolOperations => NpmOperations.CommonOperations;

    public IDictionary<string, string> Serialize(ICommitOperationData commitOperationData)
    {
      switch (commitOperationData)
      {
        case NpmAddOperationData addOperationData:
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          dictionary.Add("packageName", addOperationData.PackageName.DisplayName);
          dictionary.Add("protocolOperation", NpmAddOperation.Instance.ToString());
          dictionary.Add("packageSize", addOperationData.PackageSize.ToString());
          dictionary.Add("packageContent", addOperationData.PackageStorageId.ValueString);
          dictionary.Add("packageJsonBytes", Convert.ToBase64String(addOperationData.PackageJsonBytes));
          dictionary.Add("packageSha1Sum", addOperationData.PackageSha1Sum);
          dictionary.Add("packageDistTag", addOperationData.DistTag);
          dictionary.Add("packageIsCachedFromUpstream", addOperationData.IsUpstreamCached.ToString());
          dictionary.Add("deprecateMessage", addOperationData.DeprecateMessage);
          dictionary.Add("packageJsonOptions", addOperationData.PackageJsonOptions.Serialize<PackageJsonOptions>());
          dictionary.Add("packageManifest", addOperationData.PackageManifest.Serialize<PackageManifest>());
          dictionary.Add("sourceChain", addOperationData.SourceChain.Serialize<IEnumerable<UpstreamSourceInfo>>());
          ProvenanceInfo provenance = addOperationData.Provenance;
          dictionary.Add("provenance", provenance != null ? provenance.Serialize<ProvenanceInfo>() : (string) null);
          dictionary.Add("packageViews", string.Join<Guid>(",", addOperationData.PackageViews ?? Enumerable.Empty<Guid>()));
          return (IDictionary<string, string>) dictionary;
        case NpmDistTagSetOperationData setOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "protocolOperation",
              NpmDistTagSetOperation.Instance.ToString()
            },
            {
              "packageName",
              setOperationData.PackageName.DisplayName
            },
            {
              "packageVersion",
              setOperationData.PackageVersion.ToString()
            },
            {
              "tagName",
              setOperationData.Tag
            }
          };
        case NpmDistTagRemoveOperationData removeOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "protocolOperation",
              NpmDistTagRemoveOperation.Instance.ToString()
            },
            {
              "packageName",
              removeOperationData.PackageName.DisplayName
            },
            {
              "tagName",
              removeOperationData.Tag
            }
          };
        case NpmPermanentDeleteOperationData deleteOperationData1:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "name",
              ((IPackageIdentity) deleteOperationData1.Identity).Name.DisplayName
            },
            {
              "version",
              deleteOperationData1.Identity.Version.DisplayVersion
            },
            {
              "packageContent",
              deleteOperationData1.StorageId.ValueString
            },
            {
              "protocolOperation",
              NpmPermanentDeleteOperation.Instance.ToString()
            },
            {
              "extraAssetsBlobReferences",
              JsonConvert.SerializeObject((object) deleteOperationData1.BlobReferencesToDelete.ToList<BlobReferenceIdentifier>())
            }
          };
        case NpmDeprecateOperationData deprecateOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "name",
              deprecateOperationData.Identity.Name.DisplayName
            },
            {
              "version",
              deprecateOperationData.Identity.Version.DisplayVersion
            },
            {
              "deprecateMessage",
              deprecateOperationData.DeprecateMessage
            },
            {
              "protocolOperation",
              NpmDeprecateOperation.Instance.ToString()
            }
          };
        case NpmMetadataDiffOperationData diffOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "name",
              diffOperationData.PackageName.DisplayName
            },
            {
              "newVersionMetadata",
              diffOperationData.NewVersionMetadata.Serialize<IDictionary<string, VersionMetadata>>()
            },
            {
              "oldVersionMetadata",
              diffOperationData.OldVersionMetadata.Serialize<IDictionary<string, VersionMetadata>>()
            },
            {
              "protocolOperation",
              NpmMetadataDiffOperation.Instance.ToString()
            }
          };
        case ClearUpstreamCacheOperationData clearUpstreamCacheOperationData:
          return (IDictionary<string, string>) CommitLogSerializationHelpers.GetClearUpstreamCacheOperationDictionary((ProtocolOperation) NpmClearUpstreamCacheOperation.Instance, clearUpstreamCacheOperationData);
        case NpmUpgradeOperationData upgradeOperationData:
          return (IDictionary<string, string>) upgradeOperationData.ToDictionary();
        case IDeleteOperationData deleteOperationData2:
          Dictionary<string, string> operationDictionary = CommitLogSerializationHelpers.GetDeleteOperationDictionary((ProtocolOperation) NpmUnpublishOperation.Instance, deleteOperationData2);
          operationDictionary["unpublishedDate"] = operationDictionary["deletedDate"];
          return (IDictionary<string, string>) operationDictionary;
        default:
          return (IDictionary<string, string>) null;
      }
    }
  }
}
