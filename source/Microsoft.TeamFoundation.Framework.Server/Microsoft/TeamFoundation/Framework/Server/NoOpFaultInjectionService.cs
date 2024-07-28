// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.NoOpFaultInjectionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class NoOpFaultInjectionService : IFaultInjectionService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public T[] GetFaults<T>(IVssRequestContext requestContext, string faultPoint, string faultType) where T : IFaultSettings => Array.Empty<T>();

    public bool IsFaultEnabled(
      IVssRequestContext requestContext,
      string faultPoint,
      string faultType)
    {
      return false;
    }

    public bool IsFaultInjectionEnabled(IVssRequestContext requestContext) => false;
  }
}
