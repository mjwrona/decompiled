// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ReaderWriterLockSlimExtensionMethods
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class ReaderWriterLockSlimExtensionMethods
  {
    public static IDisposable AcquireReadToken(this ReaderWriterLockSlim readerWriterlock) => (IDisposable) new ReaderWriterLockSlimExtensionMethods.ReadLockToken(readerWriterlock);

    public static IDisposable AcquireWriteToken(this ReaderWriterLockSlim readerWriterlock) => (IDisposable) new ReaderWriterLockSlimExtensionMethods.WriteLockToken(readerWriterlock);

    private struct ReadLockToken : IDisposable
    {
      private readonly ReaderWriterLockSlim readerWriterlock;

      public ReadLockToken(ReaderWriterLockSlim readerWriterlock)
      {
        this.readerWriterlock = readerWriterlock;
        this.readerWriterlock.EnterReadLock();
      }

      public void Dispose() => this.readerWriterlock.ExitReadLock();
    }

    private struct WriteLockToken : IDisposable
    {
      private readonly ReaderWriterLockSlim readerWriterlock;

      public WriteLockToken(ReaderWriterLockSlim readerWriterlock)
      {
        this.readerWriterlock = readerWriterlock;
        this.readerWriterlock.EnterWriteLock();
      }

      public void Dispose() => this.readerWriterlock.ExitWriteLock();
    }
  }
}
