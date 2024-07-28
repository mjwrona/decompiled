// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CommitLog.CargoOperationSerializer
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Operations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CommitLog
{
  public class CargoOperationSerializer : IProtocolSpecificCommitEntrySerializer
  {
    public CommonProtocolOperations CommonProtocolOperations => CargoOperations.CommonOperations;

    public IDictionary<string, string?>? Serialize(ICommitOperationData commitOperationData)
    {
      switch (commitOperationData)
      {
        case CargoAddOperationData addOperationData:
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          dictionary.Add("name", addOperationData.Identity.Name.DisplayName);
          dictionary.Add("version", addOperationData.Identity.Version.DisplayVersion);
          dictionary.Add("protocolOperation", CargoOperations.CargoAddOperation.ToString());
          dictionary.Add("packageContent", addOperationData.PackageStorageId.ValueString);
          dictionary.Add("packageSize", addOperationData.PackageSize.ToString());
          dictionary.Add("manifestBytes", addOperationData.Metadata.Serialized.PublishManifest?.Serialized.AsDeflatedBase64String() ?? addOperationData.Metadata.Value.SynthesizePublishManifestFromIndexProperties().SerializeToDeflateCompressibleBytes().AsDeflatedBase64String());
          bool flag = addOperationData.Metadata.Serialized.PublishManifest != null;
          dictionary.Add("manifestReal", flag.ToString());
          dictionary.Add("cargoTomlBytes", addOperationData.Metadata.Serialized.CargoToml?.Serialized.AsDeflatedBase64String());
          dictionary.Add("upsIndexBytes", addOperationData.Metadata.Serialized.UpstreamIndexRow?.Serialized.AsDeflatedBase64String());
          dictionary.Add("readmePath", addOperationData.Metadata.Serialized.ActualReadmeFilePath);
          dictionary.Add("licensePath", addOperationData.Metadata.Serialized.ActualLicenseFilePath);
          dictionary.Add("sourceChain", addOperationData.SourceChain.Serialize<IEnumerable<UpstreamSourceInfo>>());
          ProvenanceInfo provenance = addOperationData.Provenance;
          dictionary.Add("provenance", provenance != null ? provenance.Serialize<ProvenanceInfo>() : (string) null);
          dictionary.Add("hashes", addOperationData.Hashes.Serialize<IReadOnlyList<HashAndType>>(true));
          dictionary.Add("innerFiles", PackageFileSerialization.SerializeInnerFileReferences((IEnumerable<InnerFileReference>) addOperationData.InnerFiles));
          flag = addOperationData.AddAsYanked;
          dictionary.Add("addAsDelisted", flag.ToString());
          return (IDictionary<string, string>) dictionary;
        case PermanentDeleteOperationData deleteOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "name",
              deleteOperationData.Identity.Name.DisplayName
            },
            {
              "version",
              deleteOperationData.Identity.Version.DisplayVersion
            },
            {
              "protocolOperation",
              CargoOperations.CargoPermanentDeleteOperation.ToString()
            },
            {
              "packageContent",
              deleteOperationData.StorageId.ValueString
            }
          };
        default:
          return (IDictionary<string, string>) null;
      }
    }
  }
}
