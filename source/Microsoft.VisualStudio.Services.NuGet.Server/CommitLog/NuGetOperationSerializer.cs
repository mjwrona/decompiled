// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.NuGetOperationSerializer
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog
{
  public class NuGetOperationSerializer : IProtocolSpecificCommitEntrySerializer
  {
    public CommonProtocolOperations CommonProtocolOperations => NuGetOperations.CommonOperations;

    public IDictionary<string, string> Serialize(ICommitOperationData commitOperationData)
    {
      switch (commitOperationData)
      {
        case NuGetAddOperationData addOperationData:
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          dictionary.Add("name", addOperationData.Identity.Name.DisplayName);
          dictionary.Add("version", addOperationData.Identity.Version.DisplayVersion);
          dictionary.Add("protocolOperation", NuGetAddOperation.Instance.ToString());
          dictionary.Add("packageContent", addOperationData.PackageStorageId.ValueString);
          dictionary.Add("packageSize", addOperationData.PackageSize.ToString());
          dictionary.Add("nuspecBytes", Convert.ToBase64String(addOperationData.NuspecBytes));
          dictionary.Add("sourceChain", addOperationData.SourceChain.Serialize<IEnumerable<UpstreamSourceInfo>>());
          ProvenanceInfo provenance = addOperationData.Provenance;
          dictionary.Add("provenance", provenance != null ? provenance.Serialize<ProvenanceInfo>() : (string) null);
          dictionary.Add("addAsDelisted", addOperationData.AddAsDelisted.ToString());
          return (IDictionary<string, string>) dictionary;
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
              "storageId",
              deleteOperationData.StorageId.ValueString
            },
            {
              "protocolOperation",
              NuGetPermanentDeleteOperation.Instance.ToString()
            }
          };
        case NuGetRestoreToFeedOperationData feedOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "name",
              feedOperationData.Identity.Name.DisplayName
            },
            {
              "version",
              feedOperationData.Identity.Version.DisplayVersion
            },
            {
              "protocolOperation",
              NuGetRestoreToFeedOperation.Instance.ToString()
            },
            {
              "nuspecBytes",
              Convert.ToBase64String(feedOperationData.NuspecBytes)
            }
          };
        case IRestoreToFeedOperationData _:
          throw new SerializationFailedException("Unexpected restore-to-feed commit operation type " + commitOperationData.GetType().FullName);
        default:
          return (IDictionary<string, string>) null;
      }
    }
  }
}
