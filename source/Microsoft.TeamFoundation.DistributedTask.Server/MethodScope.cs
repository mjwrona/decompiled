// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MethodScope
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.DistributedTask.Server
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
      this.m_requestContext.TraceEnter(this.m_layer, this.m_method);
    }

    public void Dispose() => this.m_requestContext.TraceLeave(this.m_layer, this.m_method);
  }
}
