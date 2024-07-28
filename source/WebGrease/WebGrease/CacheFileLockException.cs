// Decompiled with JetBrains decompiler
// Type: WebGrease.CacheFileLockException
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using WebGrease.Extensions;

namespace WebGrease
{
  public sealed class CacheFileLockException : Exception
  {
    public CacheFileLockException(string lockFile, Exception innerException = null)
      : base("Could not create the cache lock file because it already exists: {0}\r\nThis usually indicates that you another process is already running using this lockfile.".InvariantFormat((object) lockFile), innerException)
    {
    }
  }
}
