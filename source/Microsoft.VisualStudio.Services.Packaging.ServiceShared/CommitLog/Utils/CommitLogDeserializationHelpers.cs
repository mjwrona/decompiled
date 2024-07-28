// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils.CommitLogDeserializationHelpers
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.Utils
{
  public static class CommitLogDeserializationHelpers
  {
    public static DeleteOperationData GetDeleteOperationData(
      Func<string, string, IPackageIdentity> identityConverter,
      IItemData data)
    {
      DateTime deletedDate = DateTime.FromBinary(long.Parse(data["deletedDate"]));
      string s = data["scheduledPermanentDeleteDate"];
      DateTime? scheduledPermanentDeleteDate = s != null ? new DateTime?(DateTime.FromBinary(long.Parse(s))) : new DateTime?();
      return new DeleteOperationData(CommitLogDeserializationHelpers.ExtractPackageIdentity<IPackageIdentity>(data, identityConverter), deletedDate, scheduledPermanentDeleteDate);
    }

    public static TPackageIdentity ExtractPackageIdentity<TPackageIdentity>(
      IItemData data,
      Func<string, string, TPackageIdentity> identityConverter)
      where TPackageIdentity : class, IPackageIdentity
    {
      return identityConverter(data["name"], data["version"]);
    }

    public static ViewOperationData GetViewOperationData(
      IPackageIdentity packageIdentity,
      IItemData data)
    {
      MetadataSuboperation result;
      if (!Enum.TryParse<MetadataSuboperation>(data["metadataSuboperation"], out result) || !Enum.IsDefined(typeof (MetadataSuboperation), (object) result))
        throw new DeserializationFailedException(data["metadataSuboperation"]);
      return new ViewOperationData(packageIdentity, result, Guid.Parse(data["view"]));
    }

    public static ClearUpstreamCacheOperationData GetClearUpstreamCacheOperationData(IItemData data)
    {
      uint result;
      string upstreamSource = uint.TryParse(data["UpstreamRevision"], out result) ? data["UpstreamSource"] : throw new DeserializationFailedException("UpstreamRevision: " + data["UpstreamRevision"]);
      return !string.IsNullOrEmpty(upstreamSource) ? new ClearUpstreamCacheOperationData(result, upstreamSource) : throw new DeserializationFailedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamSourceEmpty());
    }

    public static MarkerOperationData GetMarkerData(IItemData data) => new MarkerOperationData(new ProtocolOperation(data["protocolOperation"]));

    public static INoOpOperationData GetNoOpOperationData(IItemData data) => (INoOpOperationData) new NoOpOperationData();

    public static ICommitOperationData GetAddProblemPackageOperationData(
      IPackageIdentity packageIdentity,
      IItemData storedItemData)
    {
      string json1 = storedItemData["reasons"];
      if (string.IsNullOrEmpty(json1))
        throw new DeserializationFailedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PropertyNotFound((object) "reasons"));
      string json2 = storedItemData["UpstreamSource"];
      List<TerrapinIngestionValidationReason> reasons = !string.IsNullOrEmpty(json1) ? JsonUtilities.Deserialize<List<TerrapinIngestionValidationReason>>(json1, true) : throw new DeserializationFailedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PropertyNotFound((object) "UpstreamSource"));
      UpstreamSourceInfo upstreamSource = JsonUtilities.Deserialize<UpstreamSourceInfo>(json2, true);
      return (ICommitOperationData) new AddProblemPackageOperationData(packageIdentity, upstreamSource, (IEnumerable<TerrapinIngestionValidationReason>) reasons);
    }

    public static DelistOperationData GetDelistOperationData(
      IItemData storedItemData,
      Func<string, string, IPackageIdentity> identityConverter)
    {
      return new DelistOperationData(CommitLogDeserializationHelpers.ExtractPackageIdentity<IPackageIdentity>(storedItemData, identityConverter));
    }

    public static RelistOperationData GetRelistOperationData(
      IItemData storedItemData,
      Func<string, string, IPackageIdentity> identityConverter)
    {
      return new RelistOperationData(CommitLogDeserializationHelpers.ExtractPackageIdentity<IPackageIdentity>(storedItemData, identityConverter));
    }
  }
}
