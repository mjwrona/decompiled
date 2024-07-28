// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmOperationDeserializer
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmOperationDeserializer : 
    IProtocolSpecificCommitEntryDeserializer,
    ISupportPreProtocolOperationCommits
  {
    public CommonProtocolOperations CommonProtocolOperations => NpmOperations.CommonOperations;

    public ICommitOperationData Deserialize(
      CommitLogItem commitLogItem,
      ProtocolOperation operation,
      IItemData storedItemData)
    {
      if (operation.Equals((ProtocolOperation) NpmAddOperation.Instance))
      {
        string json1 = storedItemData["packageJsonOptions"];
        PackageJsonOptions packageJsonOptions = string.IsNullOrWhiteSpace(json1) ? (PackageJsonOptions) null : JsonUtilities.Deserialize<PackageJsonOptions>(json1);
        string json2 = storedItemData["packageManifest"];
        PackageManifest packageManifest = string.IsNullOrWhiteSpace(json2) ? (PackageManifest) null : JsonUtilities.Deserialize<PackageManifest>(json2);
        string json3 = storedItemData["sourceChain"];
        IEnumerable<UpstreamSourceInfo> sourceChain = string.IsNullOrWhiteSpace(json3) ? (IEnumerable<UpstreamSourceInfo>) new List<UpstreamSourceInfo>() : JsonUtilities.Deserialize<IEnumerable<UpstreamSourceInfo>>(json3);
        string json4 = storedItemData["provenance"];
        ProvenanceInfo provenance = string.IsNullOrWhiteSpace(json4) ? (ProvenanceInfo) null : JsonUtilities.Deserialize<ProvenanceInfo>(json4);
        string str = storedItemData["packageViews"];
        IEnumerable<Guid> guids;
        if (!string.IsNullOrWhiteSpace(str))
          guids = ((IEnumerable<string>) str.Split(',')).Select<string, Guid>((Func<string, Guid>) (viewId => Guid.Parse(viewId)));
        else
          guids = (IEnumerable<Guid>) new List<Guid>();
        IEnumerable<Guid> packageViews = guids;
        return (ICommitOperationData) new NpmAddOperationData(StorageId.Parse(storedItemData["packageContent"]), new NpmPackageName(storedItemData["packageName"]), long.Parse(storedItemData["packageSize"]), Convert.FromBase64String(storedItemData["packageJsonBytes"]), storedItemData["packageSha1Sum"], storedItemData["packageDistTag"], Convert.ToBoolean(storedItemData["packageIsCachedFromUpstream"]), packageJsonOptions, packageManifest, sourceChain, provenance, packageViews, storedItemData["deprecateMessage"]);
      }
      if (operation.Equals((ProtocolOperation) NpmDistTagSetOperation.Instance))
        return (ICommitOperationData) new NpmDistTagSetOperationData((IPackageName) new NpmPackageName(storedItemData["packageName"]), NpmVersionUtils.ParseNpmPackageVersion(storedItemData["packageVersion"]), storedItemData["tagName"]);
      if (operation.Equals((ProtocolOperation) NpmDistTagRemoveOperation.Instance))
        return (ICommitOperationData) new NpmDistTagRemoveOperationData((IPackageName) new NpmPackageName(storedItemData["packageName"]), storedItemData["tagName"]);
      if (operation.Equals((ProtocolOperation) NpmPermanentDeleteOperation.Instance))
      {
        string str = storedItemData["extraAssetsBlobReferences"];
        List<BlobReferenceIdentifier> extraAssetsBlobReferences = string.IsNullOrWhiteSpace(str) ? (List<BlobReferenceIdentifier>) null : JsonConvert.DeserializeObject<List<BlobReferenceIdentifier>>(str);
        return (ICommitOperationData) new NpmPermanentDeleteOperationData(CommitLogDeserializationHelpers.ExtractPackageIdentity<NpmPackageIdentity>(storedItemData, new Func<string, string, NpmPackageIdentity>(((IdentityResolverBase<NpmPackageName, SemanticVersion, NpmPackageIdentity, SimplePackageFileName>) NpmIdentityResolver.Instance).ResolvePackageIdentity)), StorageId.Parse(storedItemData["packageContent"]), (IEnumerable<BlobReferenceIdentifier>) extraAssetsBlobReferences);
      }
      if (operation.Equals((ProtocolOperation) NpmDeprecateOperation.Instance))
        return (ICommitOperationData) new NpmDeprecateOperationData((IPackageIdentity) CommitLogDeserializationHelpers.ExtractPackageIdentity<NpmPackageIdentity>(storedItemData, new Func<string, string, NpmPackageIdentity>(((IdentityResolverBase<NpmPackageName, SemanticVersion, NpmPackageIdentity, SimplePackageFileName>) NpmIdentityResolver.Instance).ResolvePackageIdentity)), storedItemData["deprecateMessage"]);
      if (operation.Equals((ProtocolOperation) NpmMetadataDiffOperation.Instance))
        return (ICommitOperationData) new NpmMetadataDiffOperationData((IPackageName) new NpmPackageName(storedItemData["name"]), JsonUtilities.Deserialize<IDictionary<string, VersionMetadata>>(storedItemData["oldVersionMetadata"]), JsonUtilities.Deserialize<IDictionary<string, VersionMetadata>>(storedItemData["newVersionMetadata"]));
      if (operation.Equals((ProtocolOperation) NpmClearUpstreamCacheOperation.Instance))
        return (ICommitOperationData) CommitLogDeserializationHelpers.GetClearUpstreamCacheOperationData(storedItemData);
      if (operation.Equals((ProtocolOperation) NpmUpgradeOperation.Instance))
        return (ICommitOperationData) NpmUpgradeOperation.Instance.GetData(storedItemData);
      if (!operation.Equals((ProtocolOperation) NpmUnpublishOperation.Instance))
        return (ICommitOperationData) null;
      IItemData itemData = storedItemData;
      if (itemData["deletedDate"] == null)
      {
        string str;
        itemData["deletedDate"] = str = storedItemData["unpublishedDate"] ?? commitLogItem.ModifiedDate.ToBinary().ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      return (ICommitOperationData) CommitLogDeserializationHelpers.GetDeleteOperationData(new Func<string, string, IPackageIdentity>(((IdentityResolverBase<NpmPackageName, SemanticVersion, NpmPackageIdentity, SimplePackageFileName>) NpmIdentityResolver.Instance).ResolvePackageIdentity), storedItemData);
    }

    ICommitOperationData ISupportPreProtocolOperationCommits.DeserializePreProtocolOperationCommit(
      IItemData storedItemData)
    {
      NpmCommitLogItemBackCompat logItemBackCompat = new NpmCommitLogItemBackCompat(storedItemData);
      if (logItemBackCompat.CommitOperation == CommitOperation.Add)
        return (ICommitOperationData) new NpmAddOperationData(logItemBackCompat.PackageStorageId, (NpmPackageName) null, logItemBackCompat.PackageSize, logItemBackCompat.PackageJsonBytes, logItemBackCompat.PackageSha1Sum, (string) null, false, (PackageJsonOptions) null, (PackageManifest) null, Enumerable.Empty<UpstreamSourceInfo>(), (ProvenanceInfo) null, (IEnumerable<Guid>) null, (string) null);
      throw new UpConvertFailedException();
    }

    public static CommitEntryDeserializer BootstrapCommitEntryDeserializer(
      IVssRequestContext requestContext)
    {
      return new CommitEntryDeserializer((IProtocolSpecificCommitEntryDeserializer) new NpmOperationDeserializer(), new Func<string, string, IPackageIdentity>(((IdentityResolverBase<NpmPackageName, SemanticVersion, NpmPackageIdentity, SimplePackageFileName>) NpmIdentityResolver.Instance).ResolvePackageIdentity), requestContext.GetTracerFacade());
    }
  }
}
