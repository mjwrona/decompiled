// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssLockManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IVssLockManager
  {
    NamedLockFrame Lock(ILockName lockName);

    bool TryGetLock(ILockName lockName, int millisecondsTimeout);

    void ReleaseLock(ILockName lockName);

    bool IsLockHeld(ILockName lockName);

    NamedLockFrame AcquireReaderLock(ILockName lockName);

    bool IsReaderLockHeld(ILockName lockName);

    NamedLockFrame AcquireWriterLock(ILockName lockName);

    bool IsWriterLockHeld(ILockName lockName);

    NamedLockFrame AcquireConnectionLock(ConnectionLockNameType type);

    NamedLockFrame AcquireExemptionLock();
  }
}
