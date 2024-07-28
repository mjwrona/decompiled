// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueMessage
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using System;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public class QueueMessage
  {
    internal QueueMessage()
    {
    }

    public DateTimeOffset? ExpirationTime { get; internal set; }

    public string Id { get; internal set; }

    public DateTimeOffset? InsertionTime { get; internal set; }

    public DateTimeOffset? NextVisibleTime { get; internal set; }

    public string PopReceipt { get; internal set; }

    public string Text { get; internal set; }

    public int DequeueCount { get; internal set; }
  }
}
