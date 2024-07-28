// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CommitLog.CargoOperationDeserializer
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Index;
using Microsoft.VisualStudio.Services.Cargo.Server.Ingestion;
using Microsoft.VisualStudio.Services.Cargo.Server.Operations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CommitLog
{
  public class CargoOperationDeserializer : IProtocolSpecificCommitEntryDeserializer
  {
    public CommonProtocolOperations CommonProtocolOperations => CargoOperations.CommonOperations;

    public ICommitOperationData? Deserialize(
      CommitLogItem commitLogItem,
      ProtocolOperation operation,
      IItemData storedItemData)
    {
      if (operation.Equals(CargoOperations.CargoAddOperation))
      {
        string json1 = storedItemData["sourceChain"];
        IEnumerable<UpstreamSourceInfo> sourceChain = string.IsNullOrWhiteSpace(json1) ? (IEnumerable<UpstreamSourceInfo>) ImmutableList<UpstreamSourceInfo>.Empty : JsonUtilities.Deserialize<IEnumerable<UpstreamSourceInfo>>(json1);
        string json2 = storedItemData["provenance"];
        ProvenanceInfo provenance = string.IsNullOrWhiteSpace(json2) ? (ProvenanceInfo) null : JsonUtilities.Deserialize<ProvenanceInfo>(json2);
        string json3 = storedItemData["hashes"];
        IEnumerable<HashAndType> hashes = string.IsNullOrWhiteSpace(json3) ? (IEnumerable<HashAndType>) ImmutableList<HashAndType>.Empty : JsonUtilities.Deserialize<IEnumerable<HashAndType>>(json3);
        string innerFilesJson = storedItemData["innerFiles"];
        ImmutableArray<InnerFileReference> innerFiles = string.IsNullOrWhiteSpace(innerFilesJson) ? ImmutableArray<InnerFileReference>.Empty : PackageFileSerialization.DeserializeInnerFileReferences(innerFilesJson);
        string str1 = storedItemData["manifestReal"];
        int num = string.IsNullOrWhiteSpace(str1) ? 0 : (bool.Parse(str1) ? 1 : 0);
        string deflatedBase64String1 = storedItemData["manifestBytes"];
        LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest> PublishManifest = num == 0 || !string.IsNullOrWhiteSpace(deflatedBase64String1) ? CargoPublishManifest.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(deflatedBase64String1)) : (LazySerDesValue<DeflateCompressibleBytes, CargoPublishManifest>) null;
        string deflatedBase64String2 = storedItemData["upsIndexBytes"];
        LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> lazySerDesValue1 = string.IsNullOrWhiteSpace(deflatedBase64String2) ? (LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow>) null : CargoIndexVersionRow.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(deflatedBase64String2));
        string deflatedBase64String3 = storedItemData["cargoTomlBytes"];
        LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> lazySerDesValue2 = string.IsNullOrWhiteSpace(deflatedBase64String3) ? (LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest>) null : CargoTomlManifest.LazyDeserialize(DeflateCompressibleBytes.FromDeflatedBase64String(deflatedBase64String3));
        LazySerDesValue<DeflateCompressibleBytes, CargoIndexVersionRow> UpstreamIndexRow = lazySerDesValue1;
        LazySerDesValue<DeflateCompressibleBytes, CargoTomlManifest> CargoToml = lazySerDesValue2;
        string ActualReadmeFilePath = storedItemData["readmePath"];
        string ActualLicenseFilePath = storedItemData["licensePath"];
        LazySerDesValue<CargoRawPackageMetadata, CargoPackageMetadata> metadata = CargoPackageMetadata.LazyDeserialize(new CargoRawPackageMetadata(PublishManifest, UpstreamIndexRow, CargoToml, ActualReadmeFilePath, ActualLicenseFilePath));
        string str2 = storedItemData["addAsDelisted"];
        bool addAsYanked = !string.IsNullOrWhiteSpace(str2) && bool.Parse(str2);
        return (ICommitOperationData) new CargoAddOperationData(new CargoPackageIdentity(CargoPackageNameParser.Parse(storedItemData["name"]), CargoPackageVersionParser.Parse(storedItemData["version"])), StorageId.Parse(storedItemData["packageContent"]), long.Parse(storedItemData["packageSize"]), metadata, hashes, sourceChain, provenance, innerFiles, addAsYanked);
      }
      return operation.Equals(CargoOperations.CargoPermanentDeleteOperation) ? (ICommitOperationData) new PermanentDeleteOperationData((IPackageIdentity) new CargoPackageIdentity(CargoPackageNameParser.Parse(storedItemData["name"]), CargoPackageVersionParser.Parse(storedItemData["version"])), StorageId.Parse(storedItemData["packageContent"])) : (ICommitOperationData) null;
    }

    public static CommitEntryDeserializer BootstrapCommitEntryDeserializer(
      IVssRequestContext requestContext)
    {
      return new CommitEntryDeserializer((IProtocolSpecificCommitEntryDeserializer) new CargoOperationDeserializer(), new Func<string, string, IPackageIdentity>(((IdentityResolverBase<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, SimplePackageFileName>) CargoIdentityResolver.Instance).ResolvePackageIdentity), requestContext.GetTracerFacade());
    }
  }
}
