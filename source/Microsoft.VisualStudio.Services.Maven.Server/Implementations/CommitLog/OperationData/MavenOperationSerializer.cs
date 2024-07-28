// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData.MavenOperationSerializer
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Converters;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData
{
  public class MavenOperationSerializer : IProtocolSpecificCommitEntrySerializer
  {
    public CommonProtocolOperations CommonProtocolOperations => MavenOperations.CommonOperations;

    public IDictionary<string, string> Serialize(ICommitOperationData commitOperationData)
    {
      switch (commitOperationData)
      {
        case IMavenCommitOperationData commitOperationData1:
          byte[] numArray = commitOperationData1.PomBytes;
          bool flag = numArray != null;
          if (flag)
            numArray = CompressionHelper.DeflateByteArray(numArray);
          string base64String = commitOperationData1.PomBytes == null ? (string) null : Convert.ToBase64String(numArray);
          IReadOnlyList<MavenPackageFile> mavenPackageFileList = new MavenPackageFilesNewToLegacyConverter().Convert(commitOperationData1.Files);
          string str1 = (string) null;
          if (!commitOperationData1.SourceChain.IsNullOrEmpty<UpstreamSourceInfo>())
            str1 = commitOperationData1.SourceChain.Serialize<IEnumerable<UpstreamSourceInfo>>();
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          dictionary1.Add("ProtocolOperationName", MavenCommitOperation.Instance.OperationName);
          dictionary1.Add("PackageDisplayName", commitOperationData1.Identity.Name.DisplayName);
          dictionary1.Add("PackageDisplayVersion", commitOperationData1.Identity.Version.DisplayVersion);
          dictionary1.Add("Files", JsonConvert.SerializeObject((object) mavenPackageFileList));
          dictionary1.Add("PomBytes", base64String);
          dictionary1.Add("IsCompressed", flag.ToString());
          ProvenanceInfo provenance = commitOperationData1.Provenance;
          dictionary1.Add("provenance", provenance != null ? provenance.Serialize<ProvenanceInfo>() : (string) null);
          dictionary1.Add("sourceChain", str1);
          return (IDictionary<string, string>) dictionary1;
        case IMavenPermanentDeleteOperationData deleteOperationData1:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "PackageDisplayName",
              deleteOperationData1.Identity.Name.DisplayName
            },
            {
              "PackageDisplayVersion",
              deleteOperationData1.Identity.Version.DisplayVersion
            },
            {
              "ExtraAssetsBlobReferences",
              JsonConvert.SerializeObject((object) deleteOperationData1.BlobReferencesToDelete.ToList<BlobReferenceIdentifier>())
            },
            {
              "ProtocolOperationName",
              MavenPermanentDeleteOperation.Instance.ToString()
            }
          };
        case IMavenSnapshotCleanupOperationData cleanupOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "PackageDisplayName",
              cleanupOperationData.Identity.Name.DisplayName
            },
            {
              "PackageDisplayVersion",
              cleanupOperationData.Identity.Version.DisplayVersion
            },
            {
              "SnapshotInstanceIds",
              JsonConvert.SerializeObject((object) cleanupOperationData.SnapshotInstanceIds)
            },
            {
              "ExtraAssetsBlobReferences",
              JsonConvert.SerializeObject((object) cleanupOperationData.BlobReferencesToDelete.ToList<BlobReferenceIdentifier>())
            },
            {
              "ProtocolOperationName",
              MavenSnapshotCleanupOperation.Instance.ToString()
            }
          };
        case IDeleteOperationData deleteOperationData2:
          Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
          dictionary2.Add("PackageDisplayName", deleteOperationData2.Identity.Name.DisplayName);
          dictionary2.Add("PackageDisplayVersion", deleteOperationData2.Identity.Version.DisplayVersion);
          DateTime dateTime = deleteOperationData2.DeletedDate;
          long binary = dateTime.ToBinary();
          dictionary2.Add("DeletedDate", binary.ToString());
          DateTime? permanentDeleteDate = deleteOperationData2.ScheduledPermanentDeleteDate;
          ref DateTime? local = ref permanentDeleteDate;
          string str2;
          if (!local.HasValue)
          {
            str2 = (string) null;
          }
          else
          {
            dateTime = local.GetValueOrDefault();
            binary = dateTime.ToBinary();
            str2 = binary.ToString();
          }
          dictionary2.Add("ScheduledPermanentDeleteDate", str2);
          dictionary2.Add("ProtocolOperationName", MavenDeleteOperation.Instance.ToString());
          return (IDictionary<string, string>) dictionary2;
        case IMavenRestoreToFeedOperationData feedOperationData:
          return (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "PackageDisplayName",
              feedOperationData.Identity.Name.DisplayName
            },
            {
              "PackageDisplayVersion",
              feedOperationData.Identity.Version.DisplayVersion
            },
            {
              "ProtocolOperationName",
              MavenRestoreToFeedOperation.Instance.ToString()
            }
          };
        default:
          return (IDictionary<string, string>) null;
      }
    }
  }
}
