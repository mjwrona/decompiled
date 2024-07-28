// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CopyState
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class CopyState
  {
    public string CopyId { get; internal set; }

    public DateTimeOffset? CompletionTime { get; internal set; }

    public CopyStatus Status { get; internal set; }

    public Uri Source { get; internal set; }

    public long? BytesCopied { get; internal set; }

    public long? TotalBytes { get; internal set; }

    public string StatusDescription { get; internal set; }

    public DateTimeOffset? DestinationSnapshotTime { get; internal set; }
  }
}
