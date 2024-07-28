// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData.MavenOperationDeserializer
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Maven.Server.Converters;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData
{
  public class MavenOperationDeserializer : 
    IProtocolSpecificCommitEntryDeserializer,
    IHaveLegacyProtocolOperationKey,
    IHaveLegacyPackageIdentityKeys,
    IHaveLegacyProtocolOperationValues
  {
    public 
    #nullable disable
    CommonProtocolOperations CommonProtocolOperations => MavenOperations.CommonOperations;

    public ICommitOperationData Deserialize(
      CommitLogItem commitLogItem,
      ProtocolOperation operation,
      IItemData storedItemData)
    {
      if (operation.Equals((ProtocolOperation) MavenCommitOperation.Instance))
      {
        MavenPackageFilesLegacyToNewConverter legacyToNewConverter = new MavenPackageFilesLegacyToNewConverter();
        string str1 = storedItemData["Files"];
        IReadOnlyList<MavenPackageFileNew> files = string.IsNullOrWhiteSpace(str1) ? (IReadOnlyList<MavenPackageFileNew>) null : legacyToNewConverter.Convert((IEnumerable<MavenPackageFile>) JsonConvert.DeserializeObject<List<MavenPackageFile>>(str1));
        if (files == null)
        {
          string str2 = storedItemData["PackageFiles"];
          files = string.IsNullOrWhiteSpace(str2) ? (IReadOnlyList<MavenPackageFileNew>) null : legacyToNewConverter.Convert((IEnumerable<MavenPackageFile>) JsonConvert.DeserializeObject<List<MavenPackageFile>>(str2));
        }
        string s = storedItemData["PomBytes"];
        byte[] numArray = string.IsNullOrWhiteSpace(s) ? (byte[]) null : Convert.FromBase64String(s);
        if ((string.IsNullOrWhiteSpace(storedItemData["IsCompressed"]) || numArray == null ? 0 : (bool.Parse(storedItemData["IsCompressed"]) ? 1 : 0)) != 0)
          numArray = CompressionHelper.InflateByteArray(numArray);
        string json1 = storedItemData["provenance"];
        ProvenanceInfo provenance = string.IsNullOrWhiteSpace(json1) ? (ProvenanceInfo) null : JsonUtilities.Deserialize<ProvenanceInfo>(json1);
        string json2 = storedItemData["sourceChain"];
        IEnumerable<UpstreamSourceInfo> sourceChain = string.IsNullOrWhiteSpace(json2) ? (IEnumerable<UpstreamSourceInfo>) new List<UpstreamSourceInfo>() : JsonUtilities.Deserialize<IEnumerable<UpstreamSourceInfo>>(json2);
        return (ICommitOperationData) new MavenCommitOperationData(MavenOperationDeserializer.ReadIdentity(storedItemData), (IEnumerable<MavenPackageFileNew>) files, numArray, provenance, sourceChain);
      }
      if (operation.Equals((ProtocolOperation) MavenPermanentDeleteOperation.Instance))
      {
        string str = storedItemData["ExtraAssetsBlobReferences"];
        List<BlobReferenceIdentifier> extraAssetsBlobReferences = string.IsNullOrWhiteSpace(str) ? (List<BlobReferenceIdentifier>) null : JsonConvert.DeserializeObject<List<BlobReferenceIdentifier>>(str);
        return (ICommitOperationData) new MavenPermanentDeleteOperationData(MavenOperationDeserializer.ReadIdentity(storedItemData), (IEnumerable<BlobReferenceIdentifier>) extraAssetsBlobReferences);
      }
      if (operation.Equals((ProtocolOperation) MavenSnapshotCleanupOperation.Instance))
      {
        string str3 = storedItemData["SnapshotInstanceIds"];
        List<MavenSnapshotInstanceId> snapshotInstanceIds = string.IsNullOrWhiteSpace(str3) ? (List<MavenSnapshotInstanceId>) null : JsonConvert.DeserializeObject<List<MavenSnapshotInstanceId>>(str3);
        string str4 = storedItemData["ExtraAssetsBlobReferences"];
        List<BlobReferenceIdentifier> extraAssetsBlobReferences = string.IsNullOrWhiteSpace(str4) ? (List<BlobReferenceIdentifier>) null : JsonConvert.DeserializeObject<List<BlobReferenceIdentifier>>(str4);
        return (ICommitOperationData) new MavenSnapshotCleanupOperationData(MavenOperationDeserializer.ReadIdentity(storedItemData), (IList<MavenSnapshotInstanceId>) snapshotInstanceIds, (IEnumerable<BlobReferenceIdentifier>) extraAssetsBlobReferences);
      }
      if (operation.Equals((ProtocolOperation) MavenDeleteOperation.Instance))
      {
        DateTime deletedDate = DateTime.FromBinary(long.Parse(storedItemData["DeletedDate"]));
        DateTime? scheduledPermanentDeleteDate = new DateTime?();
        if (storedItemData["ScheduledPermanentDeleteDate"] != null)
          scheduledPermanentDeleteDate = new DateTime?(DateTime.FromBinary(long.Parse(storedItemData["ScheduledPermanentDeleteDate"])));
        return (ICommitOperationData) new DeleteOperationData((IPackageIdentity) MavenOperationDeserializer.ReadIdentity(storedItemData), deletedDate, scheduledPermanentDeleteDate);
      }
      return operation.Equals((ProtocolOperation) MavenRestoreToFeedOperation.Instance) ? (ICommitOperationData) new MavenRestoreToFeedOperationData(MavenOperationDeserializer.ReadIdentity(storedItemData)) : (ICommitOperationData) null;
    }

    string IHaveLegacyPackageIdentityKeys.LegacyPackageNameKey => "PackageDisplayName";

    string IHaveLegacyPackageIdentityKeys.LegacyPackageVersionKey => "PackageDisplayVersion";

    private static MavenPackageIdentity ReadIdentity(IItemData storedItemData) => new MavenPackageIdentity(new MavenPackageName(storedItemData["PackageDisplayName"] ?? storedItemData["name"]), new MavenPackageVersion(storedItemData["PackageDisplayVersion"] ?? storedItemData["version"]));

    string IHaveLegacyProtocolOperationKey.LegacyProtocolOperationKey => "ProtocolOperationName";

    ProtocolOperation IHaveLegacyProtocolOperationValues.TranslateProtocolOperationValueOrDefault(
      string value)
    {
      return value == MavenCommitOperation.Instance.OperationName ? (ProtocolOperation) MavenCommitOperation.Instance : (ProtocolOperation) null;
    }

    public static CommitEntryDeserializer BootstrapCommitEntryDeserializer(
      IVssRequestContext requestContext)
    {
      return new CommitEntryDeserializer((IProtocolSpecificCommitEntryDeserializer) new MavenOperationDeserializer(), (Func<string, string, IPackageIdentity>) ((name, version) => (IPackageIdentity) new MavenPackageIdentity(new MavenPackageName(name), new MavenPackageVersion(version))), requestContext.GetTracerFacade());
    }
  }
}
