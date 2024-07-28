// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.MethodScope
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Deployment
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
      this.m_requestContext.TraceEnter(0, this.m_layer, this.m_method, ".ctor");
    }

    public void Dispose() => this.m_requestContext.TraceLeave(0, this.m_layer, this.m_method, nameof (Dispose));
  }
}
