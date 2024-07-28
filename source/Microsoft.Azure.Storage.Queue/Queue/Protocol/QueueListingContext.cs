// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueListingContext
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Shared.Protocol;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public sealed class QueueListingContext : ListingContext
  {
    public QueueListingContext(string prefix, int? maxResults, QueueListingDetails include)
      : base(prefix, maxResults)
    {
      this.Include = include;
    }

    public QueueListingDetails Include { get; set; }
  }
}
