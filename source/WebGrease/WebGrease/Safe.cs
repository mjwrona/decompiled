// Decompiled with JetBrains decompiler
// Type: WebGrease.Safe
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WebGrease.Extensions;

namespace WebGrease
{
  internal sealed class Safe : IDisposable
  {
    internal const int DefaultLockTimeout = 5000;
    internal const int MaxLockTimeout = 2147483647;
    private static readonly IDictionary<string, object> UniqueKeyLocks = (IDictionary<string, object>) new Dictionary<string, object>();
    private readonly bool[] securedFlags;
    private readonly object[] padlocks;

    public Safe(object[] padlockObjects, int millisecondTimeout)
    {
      this.padlocks = padlockObjects;
      this.securedFlags = new bool[this.padlocks.Length];
      for (int index = 0; index < this.padlocks.Length; ++index)
        this.securedFlags[index] = Monitor.TryEnter(this.padlocks[index], millisecondTimeout);
    }

    private Safe(object padlockObject, int milliSecondTimeout)
      : this(new object[1]{ padlockObject }, milliSecondTimeout)
    {
    }

    private bool Secured => ((IEnumerable<bool>) this.securedFlags).All<bool>((Func<bool, bool>) (s => s));

    public void Dispose()
    {
      for (int index = 0; index < this.securedFlags.Length; ++index)
      {
        if (this.securedFlags[index])
        {
          Monitor.Exit(this.padlocks[index]);
          this.securedFlags[index] = false;
        }
      }
    }

    internal static void Lock(object padlock, Action action) => Safe.Lock(padlock, 5000, action);

    internal static void FileLock(FileSystemInfo fileInfo, Action fileAction) => Safe.FileLock(fileInfo, 5000, fileAction);

    internal static void LockFiles(IEnumerable<FileInfo> fileInfoItems, Action fileAction)
    {
      List<object> uniqueKeyLocks = new List<object>();
      Safe.Lock((object) Safe.UniqueKeyLocks, (Action) (() =>
      {
        foreach (FileSystemInfo fileInfoItem in fileInfoItems)
        {
          string upperInvariant = fileInfoItem.FullName.ToUpperInvariant();
          object obj;
          if (!Safe.UniqueKeyLocks.TryGetValue(upperInvariant, out obj))
            Safe.UniqueKeyLocks.Add(upperInvariant, obj = new object());
          uniqueKeyLocks.Add(obj);
        }
      }));
      Safe.Lock(uniqueKeyLocks.ToArray(), int.MaxValue, fileAction);
    }

    internal static void FileLock(
      FileSystemInfo fileInfo,
      int millisecondTimeout,
      Action fileAction)
    {
      Safe.UniqueKeyLock(fileInfo.FullName.ToUpperInvariant(), millisecondTimeout, fileAction);
    }

    internal static void UniqueKeyLock(string uniqueKey, int millisecondTimeout, Action fileAction)
    {
      object uniqueKeyLock = (object) null;
      Safe.Lock((object) Safe.UniqueKeyLocks, (Action) (() =>
      {
        if (Safe.UniqueKeyLocks.TryGetValue(uniqueKey, out uniqueKeyLock))
          return;
        Safe.UniqueKeyLocks.Add(uniqueKey, uniqueKeyLock = new object());
      }));
      Safe.Lock(uniqueKeyLock, millisecondTimeout, fileAction);
    }

    internal static TResult Lock<TResult>(object padlock, Func<TResult> action) => Safe.Lock<TResult>(padlock, 5000, action);

    internal static TResult Lock<TResult>(
      object padlock,
      int millisecondTimeout,
      Func<TResult> action)
    {
      using (Safe safe = new Safe(padlock, millisecondTimeout))
      {
        if (safe.Secured)
          return action();
        throw new TimeoutException(ResourceStrings.SafeLockFailedMessage.InvariantFormat((object) millisecondTimeout));
      }
    }

    internal static void Lock(object[] padlocks, int millisecondTimeout, Action action)
    {
      using (Safe safe = new Safe(padlocks, millisecondTimeout))
      {
        if (safe.Secured)
          action();
        else
          throw new TimeoutException(ResourceStrings.SafeLockFailedMessage.InvariantFormat((object) millisecondTimeout));
      }
    }

    internal static void Lock(object padlock, int millisecondTimeout, Action action)
    {
      using (Safe safe = new Safe(padlock, millisecondTimeout))
      {
        if (safe.Secured)
          action();
        else
          throw new TimeoutException(ResourceStrings.SafeLockFailedMessage.InvariantFormat((object) millisecondTimeout));
      }
    }

    internal static bool WriteToFileStream(string filePath, Action<FileStream> action) => Safe.WriteToFileStream(filePath, 10, 500, action);

    private static bool WriteToFileStream(
      string fullPath,
      int maxRetries,
      int millisecondsTimeoutBetweenTries,
      Action<FileStream> action)
    {
      int num = 0;
      while (true)
      {
        ++num;
        try
        {
          using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
          {
            fileStream.ReadByte();
            fileStream.Seek(0L, SeekOrigin.Begin);
            action(fileStream);
            return true;
          }
        }
        catch (Exception ex)
        {
          if (num == maxRetries)
            return false;
          Thread.Sleep(millisecondsTimeoutBetweenTries);
        }
      }
    }
  }
}
