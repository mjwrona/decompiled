// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.MethodScope
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal struct MethodScope : IDisposable
  {
    private readonly string m_layer;
    private readonly string m_method;
    private readonly IVssRequestContext m_requestContext;

    public MethodScope(IVssRequestContext requestContext, string layer, [CallerMemberName] string method = null)
    {
      this.m_requestContext = requestContext;
      this.m_layer = layer;
      this.m_method = method;
      this.m_requestContext.TraceEnter(0, "SignalR", this.m_layer, this.m_method);
    }

    public void Dispose() => this.m_requestContext.TraceLeave(0, "SignalR", this.m_layer, this.m_method);
  }
}
