// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.SemaphoreSlimToken
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public struct SemaphoreSlimToken : IDisposable
  {
    private SemaphoreSlim _semaphore;

    private SemaphoreSlimToken(SemaphoreSlim semaphore)
      : this()
    {
      this._semaphore = semaphore;
    }

    public static async Task<SemaphoreSlimToken> Wait(SemaphoreSlim semaphore)
    {
      await semaphore.WaitAsync().ConfigureAwait(false);
      return new SemaphoreSlimToken(semaphore);
    }

    public void Dispose()
    {
      if (this._semaphore == null)
        return;
      this._semaphore.Release();
      this._semaphore = (SemaphoreSlim) null;
    }

    public static bool operator ==(SemaphoreSlimToken left, SemaphoreSlimToken right) => throw new InvalidOperationException();

    public static bool operator !=(SemaphoreSlimToken left, SemaphoreSlimToken right) => throw new InvalidOperationException();

    public override bool Equals(object obj) => throw new InvalidOperationException();

    public override int GetHashCode() => throw new InvalidOperationException();
  }
}
