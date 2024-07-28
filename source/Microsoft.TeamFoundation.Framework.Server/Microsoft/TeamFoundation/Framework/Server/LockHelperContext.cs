// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockHelperContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockHelperContext
  {
    [ThreadStatic]
    private static long s_requestId;
    private static long s_requestIdCounter;

    public static bool RequestIdIsSet => LockHelperContext.s_requestId != 0L;

    public static long RequestId => LockHelperContext.s_requestId != 0L ? LockHelperContext.s_requestId : throw new InvalidOperationException();

    public static long NewRequestId() => Interlocked.Increment(ref LockHelperContext.s_requestIdCounter);

    public static void SetRequestId(long requestId)
    {
      if (requestId == 0L)
        throw new ArgumentOutOfRangeException(nameof (requestId));
      LockHelperContext.s_requestId = LockHelperContext.s_requestId == 0L ? requestId : throw new InvalidOperationException();
    }

    public static void ClearRequestId() => LockHelperContext.s_requestId = LockHelperContext.s_requestId != 0L ? 0L : throw new InvalidOperationException();
  }
}
