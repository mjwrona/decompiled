// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.Telemetry.ItemUploaderRecord
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

namespace Microsoft.VisualStudio.Services.ItemStore.Common.Telemetry
{
  public class ItemUploaderRecord
  {
    public const long InvalidTime = -1;

    public ItemUploaderRecord()
    {
      this.AssociateElapsedTimeMs = -1L;
      this.TransferTimeMs = -1L;
      this.SealTimeMs = -1L;
    }

    public ItemUploaderRecord(ItemUploaderRecord record)
    {
      this.FilesTransferredCount = record.FilesTransferredCount;
      this.SumCachedBytes = record.SumCachedBytes;
      this.SumTransferredBytes = record.SumTransferredBytes;
      this.TransferTimeMs = record.TransferTimeMs;
      this.TransferFailureCount = record.TransferFailureCount;
      this.ItemsCount = record.ItemsCount;
      this.AssociateElapsedTimeMs = record.AssociateElapsedTimeMs;
      this.AssociateSuccess = record.AssociateSuccess;
      this.SealResult = record.SealResult;
      this.SealTimeMs = record.SealTimeMs;
      this.IsChunked = record.IsChunked;
    }

    public long FilesTransferredCount { get; set; }

    public long SumTransferredBytes { get; set; }

    public long SumCachedBytes { get; set; }

    public long TransferTimeMs { get; set; }

    public long TransferFailureCount { get; set; }

    public long ItemsCount { get; set; }

    public long AssociateElapsedTimeMs { get; set; }

    public bool AssociateSuccess { get; set; }

    public SealResultType SealResult { get; set; }

    public long SealTimeMs { get; set; }

    public bool IsChunked { get; set; }
  }
}
