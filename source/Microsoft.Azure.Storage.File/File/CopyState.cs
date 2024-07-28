// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CopyState
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;

namespace Microsoft.Azure.Storage.File
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
