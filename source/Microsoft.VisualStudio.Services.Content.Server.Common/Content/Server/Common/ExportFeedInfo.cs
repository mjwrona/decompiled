// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ExportFeedInfo
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class ExportFeedInfo
  {
    public string BlobHash { get; }

    public string ReferenceIdDetails { get; }

    public string FeedId { get; }

    public DateTimeOffset? ModifiedTime { get; }

    public ExportFeedInfo(
      string blobHash,
      string feedId,
      string referenceIdDetails,
      DateTimeOffset? modifiedTime)
    {
      this.BlobHash = blobHash;
      this.FeedId = feedId;
      this.ReferenceIdDetails = referenceIdDetails;
      this.ModifiedTime = modifiedTime;
    }

    public override string ToString()
    {
      string[] strArray = new string[7];
      strArray[0] = this.BlobHash;
      strArray[1] = ",";
      DateTimeOffset? modifiedTime = this.ModifiedTime;
      ref DateTimeOffset? local = ref modifiedTime;
      strArray[2] = local.HasValue ? local.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss.fff") : (string) null;
      strArray[3] = ",";
      strArray[4] = this.FeedId;
      strArray[5] = ",";
      strArray[6] = this.ReferenceIdDetails;
      return string.Concat(strArray);
    }
  }
}
