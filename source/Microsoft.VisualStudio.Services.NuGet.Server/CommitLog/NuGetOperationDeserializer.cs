// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.NuGetOperationDeserializer
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog
{
  public class NuGetOperationDeserializer : 
    IProtocolSpecificCommitEntryDeserializer,
    ISupportPreProtocolOperationCommits
  {
    public CommonProtocolOperations CommonProtocolOperations => NuGetOperations.CommonOperations;

    public ICommitOperationData Deserialize(
      CommitLogItem commitLogItem,
      ProtocolOperation operation,
      IItemData storedItemData)
    {
      if (operation.Equals((ProtocolOperation) NuGetAddOperation.Instance))
      {
        string json1 = storedItemData["sourceChain"];
        IEnumerable<UpstreamSourceInfo> sourceChain = string.IsNullOrWhiteSpace(json1) ? (IEnumerable<UpstreamSourceInfo>) new List<UpstreamSourceInfo>() : JsonUtilities.Deserialize<IEnumerable<UpstreamSourceInfo>>(json1);
        string json2 = storedItemData["provenance"];
        ProvenanceInfo provenance = string.IsNullOrWhiteSpace(json2) ? (ProvenanceInfo) null : JsonUtilities.Deserialize<ProvenanceInfo>(json2);
        string str = storedItemData["addAsDelisted"];
        bool addAsDelisted = !string.IsNullOrWhiteSpace(str) && bool.Parse(str);
        return (ICommitOperationData) new NuGetAddOperationData(CommitLogDeserializationHelpers.ExtractPackageIdentity<VssNuGetPackageIdentity>(storedItemData, new Func<string, string, VssNuGetPackageIdentity>(((IdentityResolverBase<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, SimplePackageFileName>) NuGetIdentityResolver.Instance).ResolvePackageIdentity)), StorageId.Parse(storedItemData["packageContent"]), long.Parse(storedItemData["packageSize"]), Convert.FromBase64String(storedItemData["nuspecBytes"]), sourceChain, provenance, addAsDelisted);
      }
      if (operation.Equals((ProtocolOperation) NuGetPermanentDeleteOperation.Instance))
        return (ICommitOperationData) new PermanentDeleteOperationData((IPackageIdentity) CommitLogDeserializationHelpers.ExtractPackageIdentity<VssNuGetPackageIdentity>(storedItemData, new Func<string, string, VssNuGetPackageIdentity>(((IdentityResolverBase<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, SimplePackageFileName>) NuGetIdentityResolver.Instance).ResolvePackageIdentity)), StorageId.Parse(storedItemData["storageId"]));
      if (operation.Equals((ProtocolOperation) NuGetDeleteOperation.Instance))
      {
        IItemData itemData1 = storedItemData;
        if (itemData1["deletedDate"] == null)
        {
          IItemData itemData2 = itemData1;
          long binary = commitLogItem.CreatedDate.ToBinary();
          string str1;
          string str2 = str1 = binary.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          itemData2["deletedDate"] = str1;
        }
        return (ICommitOperationData) CommitLogDeserializationHelpers.GetDeleteOperationData(new Func<string, string, IPackageIdentity>(((IdentityResolverBase<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, SimplePackageFileName>) NuGetIdentityResolver.Instance).ResolvePackageIdentity), storedItemData);
      }
      return operation.Equals((ProtocolOperation) NuGetRestoreToFeedOperation.Instance) ? (ICommitOperationData) new NuGetRestoreToFeedOperationData((IPackageIdentity) CommitLogDeserializationHelpers.ExtractPackageIdentity<VssNuGetPackageIdentity>(storedItemData, new Func<string, string, VssNuGetPackageIdentity>(((IdentityResolverBase<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, SimplePackageFileName>) NuGetIdentityResolver.Instance).ResolvePackageIdentity)), Convert.FromBase64String(storedItemData["nuspecBytes"])) : (ICommitOperationData) null;
    }

    ICommitOperationData ISupportPreProtocolOperationCommits.DeserializePreProtocolOperationCommit(
      IItemData storedItemData)
    {
      NuGetCommitLogItem getCommitLogItem = new NuGetCommitLogItem(storedItemData);
      if (getCommitLogItem.CommitOperation == CommitOperation.Add)
      {
        VssNuGetPackageIdentity packageIdentity = (VssNuGetPackageIdentity) null;
        if (storedItemData["name"] != null && storedItemData["version"] != null)
          packageIdentity = CommitLogDeserializationHelpers.ExtractPackageIdentity<VssNuGetPackageIdentity>(storedItemData, new Func<string, string, VssNuGetPackageIdentity>(((IdentityResolverBase<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, SimplePackageFileName>) NuGetIdentityResolver.Instance).ResolvePackageIdentity));
        return (ICommitOperationData) new NuGetAddOperationData(packageIdentity, getCommitLogItem.PackageStorageId, getCommitLogItem.PackageSize, getCommitLogItem.NuspecBytes, (IEnumerable<UpstreamSourceInfo>) new List<UpstreamSourceInfo>(), (ProvenanceInfo) null, false);
      }
      if (getCommitLogItem.CommitOperation == CommitOperation.Delist)
        return (ICommitOperationData) new DelistOperationData((IPackageIdentity) new VssNuGetPackageIdentity(getCommitLogItem.PackageDisplayName, getCommitLogItem.PackageDisplayVersion));
      throw new UpConvertFailedException();
    }

    public static CommitEntryDeserializer BootstrapCommitEntryDeserializer(
      IVssRequestContext requestContext)
    {
      return new CommitEntryDeserializer((IProtocolSpecificCommitEntryDeserializer) new NuGetOperationDeserializer(), new Func<string, string, IPackageIdentity>(((IdentityResolverBase<VssNuGetPackageName, VssNuGetPackageVersion, VssNuGetPackageIdentity, SimplePackageFileName>) NuGetIdentityResolver.Instance).ResolvePackageIdentity), requestContext.GetTracerFacade());
    }
  }
}
