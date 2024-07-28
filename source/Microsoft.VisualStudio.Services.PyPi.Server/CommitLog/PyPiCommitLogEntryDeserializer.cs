// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.CommitLog.PyPiCommitLogEntryDeserializer
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.PyPi.Server.Operations;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.CommitLog
{
  internal class PyPiCommitLogEntryDeserializer : IProtocolSpecificCommitEntryDeserializer
  {
    public 
    #nullable disable
    CommonProtocolOperations CommonProtocolOperations => PyPiOperations.CommonOperations;

    public ICommitOperationData Deserialize(
      CommitLogItem commitLogItem,
      ProtocolOperation operation,
      IItemData storedItemData)
    {
      if (operation.Equals(PyPiOperations.PyPiAddOperation))
      {
        string str = storedItemData["metadataFields"];
        IReadOnlyDictionary<string, string[]> metadataFields = string.IsNullOrWhiteSpace(str) ? (IReadOnlyDictionary<string, string[]>) null : (IReadOnlyDictionary<string, string[]>) JsonConvert.DeserializeObject<Dictionary<string, string[]>>(str);
        string json1 = storedItemData["sourceChain"];
        IEnumerable<UpstreamSourceInfo> sourceChain = string.IsNullOrWhiteSpace(json1) ? (IEnumerable<UpstreamSourceInfo>) new List<UpstreamSourceInfo>() : JsonUtilities.Deserialize<IEnumerable<UpstreamSourceInfo>>(json1);
        string json2 = storedItemData["provenance"];
        ProvenanceInfo provenance = string.IsNullOrWhiteSpace(json2) ? (ProvenanceInfo) null : JsonUtilities.Deserialize<ProvenanceInfo>(json2);
        string deflatedBase64String = storedItemData["gpgSignature"];
        DeflateCompressibleBytes gpgSignature = !string.IsNullOrWhiteSpace(deflatedBase64String) ? DeflateCompressibleBytes.FromDeflatedBase64String(deflatedBase64String) : (DeflateCompressibleBytes) null;
        return (ICommitOperationData) new PyPiAddOperationData(new PyPiPackageIdentity(new PyPiPackageName(storedItemData["name"]), PyPiPackageVersionParser.Parse(storedItemData["version"])), metadataFields, StorageId.Parse(storedItemData["packageContent"]), long.Parse(storedItemData["packageSize"]), storedItemData["fileName"], storedItemData["computedSha256"], storedItemData["computedMd5"], gpgSignature, sourceChain, provenance);
      }
      if (!operation.Equals(PyPiOperations.PyPiPermanentDeleteOperation))
        return (ICommitOperationData) null;
      string str1 = storedItemData["extraAssetsBlobReferences"];
      List<BlobReferenceIdentifier> extraAssetsBlobReferences = string.IsNullOrWhiteSpace(str1) ? (List<BlobReferenceIdentifier>) null : JsonConvert.DeserializeObject<List<BlobReferenceIdentifier>>(str1);
      return (ICommitOperationData) new PyPiPermanentDeleteOperationData(new PyPiPackageIdentity(new PyPiPackageName(storedItemData["name"]), PyPiPackageVersionParser.Parse(storedItemData["version"])), (IEnumerable<BlobReferenceIdentifier>) extraAssetsBlobReferences);
    }

    public static CommitEntryDeserializer BootstrapCommitEntryDeserializer(
      IVssRequestContext requestContext)
    {
      return new CommitEntryDeserializer((IProtocolSpecificCommitEntryDeserializer) new PyPiCommitLogEntryDeserializer(), (Func<string, string, IPackageIdentity>) ((name, version) => (IPackageIdentity) new PyPiPackageIdentity(new PyPiPackageName(name), PyPiPackageVersionParser.Parse(version))), requestContext.GetTracerFacade());
    }
  }
}
