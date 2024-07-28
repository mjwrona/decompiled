// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssLockManagerInternal
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface IVssLockManagerInternal
  {
    bool TryGetLock(ILockName lockName, int millisecondsTimeout);

    void ReleaseLock(ILockName lockName);

    bool TryGetReaderLock(ILockName lockName, int millisecondsTimeout);

    void ReleaseReaderLock(ILockName lockName);

    bool TryGetWriterLock(ILockName lockName, int millisecondsTimeout);

    void ReleaseWriterLock(ILockName lockName);
  }
}
