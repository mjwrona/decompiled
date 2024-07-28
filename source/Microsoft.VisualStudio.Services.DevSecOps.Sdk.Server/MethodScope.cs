// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.MethodScope
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
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
