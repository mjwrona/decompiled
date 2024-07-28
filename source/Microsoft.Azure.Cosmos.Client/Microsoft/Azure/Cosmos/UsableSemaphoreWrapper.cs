// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.UsableSemaphoreWrapper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;

namespace Microsoft.Azure.Cosmos
{
  internal class UsableSemaphoreWrapper : IDisposable
  {
    private readonly SemaphoreSlim semaphore;
    private bool disposed;

    public UsableSemaphoreWrapper(SemaphoreSlim semaphore) => this.semaphore = semaphore;

    public void Dispose()
    {
      if (this.disposed)
        return;
      this.semaphore.Release();
      this.disposed = true;
    }
  }
}
