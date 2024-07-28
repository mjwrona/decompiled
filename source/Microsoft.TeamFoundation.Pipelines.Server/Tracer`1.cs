// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Tracer`1
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class Tracer<T> : IDisposable
  {
    private IVssRequestContext m_context;
    private string m_layer;
    private int m_enter;
    private int m_leave;
    private string m_methodName;

    public Tracer(IVssRequestContext context, int enter, int leave, [CallerMemberName] string methodName = null)
    {
      this.m_context = context;
      this.m_layer = typeof (T).Name;
      this.m_enter = enter;
      this.m_leave = leave;
      this.m_methodName = methodName;
      this.m_context.TraceEnter(this.m_enter, TracePoints.Area, this.m_layer, this.m_methodName);
    }

    public void Dispose() => this.m_context.TraceLeave(this.m_leave, TracePoints.Area, this.m_layer, this.m_methodName);
  }
}
