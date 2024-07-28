// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.CommitLog.PyPiCommitLogEntrySerializer
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.Operations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.CommitLog
{
  internal class PyPiCommitLogEntrySerializer : IProtocolSpecificCommitEntrySerializer
  {
    public CommonProtocolOperations CommonProtocolOperations => PyPiOperations.CommonOperations;

    public IDictionary<string, string> Serialize(ICommitOperationData commitOperationData)
    {
      switch (commitOperationData)
      {
        case PyPiAddOperationData addOperationData:
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          dictionary1.Add("name", addOperationData.Identity.Name.DisplayName);
          dictionary1.Add("version", addOperationData.Identity.Version.DisplayVersion);
          dictionary1.Add("protocolOperation", PyPiOperations.PyPiAddOperation.ToString());
          dictionary1.Add("packageContent", addOperationData.PackageStorageId.ValueString);
          dictionary1.Add("packageSize", addOperationData.PackageSize.ToString());
          dictionary1.Add("metadataFields", JsonConvert.SerializeObject((object) addOperationData.MetadataFields));
          dictionary1.Add("fileName", addOperationData.FileName);
          dictionary1.Add("computedSha256", addOperationData.ComputedSha256);
          dictionary1.Add("computedMd5", addOperationData.ComputedMd5);
          dictionary1.Add("gpgSignature", addOperationData.GpgSignature?.AsDeflatedBase64String());
          ProvenanceInfo provenance = addOperationData.Provenance;
          dictionary1.Add("provenance", provenance != null ? provenance.Serialize<ProvenanceInfo>() : (string) null);
          Dictionary<string, string> dictionary2 = dictionary1;
          if (!addOperationData.SourceChain.IsNullOrEmpty<UpstreamSourceInfo>())
            dictionary2.Add("sourceChain", addOperationData.SourceChain.Serialize<IEnumerable<UpstreamSourceInfo>>());
          return (IDictionary<string, string>) dictionary2;
        case IPermanentDeleteOperationData deleteOperationData:
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
              PyPiOperations.PyPiPermanentDeleteOperation.ToString()
            },
            {
              "extraAssetsBlobReferences",
              JsonConvert.SerializeObject((object) deleteOperationData.BlobReferencesToDelete.ToList<BlobReferenceIdentifier>())
            }
          };
        default:
          return (IDictionary<string, string>) null;
      }
    }
  }
}
