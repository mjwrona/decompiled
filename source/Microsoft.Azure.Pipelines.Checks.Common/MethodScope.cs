// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.MethodScope
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  public struct MethodScope : IDisposable
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
