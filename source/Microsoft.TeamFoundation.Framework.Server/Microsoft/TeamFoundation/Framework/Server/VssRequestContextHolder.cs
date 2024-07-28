// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssRequestContextHolder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssRequestContextHolder : IDisposable
  {
    private readonly bool m_ownsRequest;

    public VssRequestContextHolder(IVssRequestContext requestContext, bool ownsRequest)
    {
      this.RequestContext = requestContext;
      this.m_ownsRequest = ownsRequest;
    }

    public IVssRequestContext RequestContext { get; private set; }

    public void Dispose()
    {
      IVssRequestContext requestContext = this.RequestContext;
      if (requestContext == null)
        return;
      this.RequestContext = (IVssRequestContext) null;
      if (!this.m_ownsRequest)
        return;
      requestContext.Dispose();
    }
  }
}
