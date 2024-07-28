// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.QueueAccountExtensions
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using System;

namespace Microsoft.Azure.Storage.Queue
{
  public static class QueueAccountExtensions
  {
    public static CloudQueueClient CreateCloudQueueClient(this CloudStorageAccount account)
    {
      if (account.QueueEndpoint == (Uri) null)
        throw new InvalidOperationException("No queue endpoint configured.");
      return new CloudQueueClient(account.QueueStorageUri, account.Credentials);
    }
  }
}
