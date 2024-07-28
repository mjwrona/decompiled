// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Security.BCryptAlgorithmHandleCache2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Collections.Generic;
using System.Security;

namespace Microsoft.TeamFoundation.Server.Core.Security
{
  internal sealed class BCryptAlgorithmHandleCache2
  {
    [SecurityCritical]
    private Dictionary<string, WeakReference> m_algorithmHandles;

    [SecurityCritical]
    public BCryptAlgorithmHandleCache2() => this.m_algorithmHandles = new Dictionary<string, WeakReference>();

    [SecuritySafeCritical]
    public SafeBCryptAlgorithmHandle2 GetCachedAlgorithmHandle(
      string algorithm,
      string implementation)
    {
      string key = algorithm + implementation;
      if (this.m_algorithmHandles.ContainsKey(key) && this.m_algorithmHandles[key].Target is SafeBCryptAlgorithmHandle2 target1)
        return target1;
      SafeBCryptAlgorithmHandle2 target2 = BCryptNative2.OpenAlgorithm(algorithm, implementation);
      this.m_algorithmHandles[key] = new WeakReference((object) target2);
      return target2;
    }
  }
}
