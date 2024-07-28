// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueEntry
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public sealed class QueueEntry
  {
    internal QueueEntry()
    {
    }

    internal QueueEntry(string name, Uri uri, IDictionary<string, string> metadata)
    {
      this.Name = name;
      this.Uri = uri;
      this.Metadata = metadata;
    }

    public IDictionary<string, string> Metadata { get; internal set; }

    public string Name { get; internal set; }

    public Uri Uri { get; internal set; }
  }
}
