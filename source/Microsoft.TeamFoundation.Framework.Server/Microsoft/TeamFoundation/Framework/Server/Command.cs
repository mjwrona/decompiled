// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Command
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class Command : ICommand, IDisposable
  {
    private IVssRequestContext m_requestContext;
    private IDisposable m_commandExemptionLock;
    private long m_cacheUsage;
    private long m_totalResultSize;
    private bool m_canceled;
    private int m_maxCacheSize = Command.CommandCacheLimit;
    private static int s_commandCacheLimit = 33554432;
    private readonly StackTracer m_constructorStackTrace;

    protected Command(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      if (requestContext.ServiceHost != null && !requestContext.ServiceHost.IsProduction)
        this.m_commandExemptionLock = requestContext.AcquireExemptionLock();
      if (!TeamFoundationTracingService.IsRawTracingEnabled(1379469382, TraceLevel.Info, "Streaming", nameof (Command), (string[]) null))
        return;
      this.m_constructorStackTrace = new StackTracer();
    }

    ~Command()
    {
      if (this.m_constructorStackTrace != null)
        TeamFoundationTracingService.TraceRaw(647916056, TraceLevel.Error, "Streaming", nameof (Command), "{0} was not disposed before finalization - Call stack {1}", (object) this.Name, (object) this.m_constructorStackTrace);
      else
        TeamFoundationTracingService.TraceRaw(647916056, TraceLevel.Error, "Streaming", nameof (Command), "{0} was not disposed before finalization", (object) this.Name);
      this.Dispose(false);
    }

    public string Name => this.GetType().Name;

    internal void Cancel()
    {
      if (this.m_requestContext == null)
        return;
      this.m_requestContext.Cancel((string) null);
    }

    public virtual void ContinueExecution()
    {
    }

    public bool IncrementCacheUsage(int bytes)
    {
      this.m_totalResultSize += (long) bytes;
      return (this.m_cacheUsage += (long) bytes) > (long) this.MaxCacheSize;
    }

    public void DecrementCacheUsage(int bytes) => this.m_cacheUsage -= (long) bytes;

    public void Dispose()
    {
      this.Dispose(true);
      if (this.m_commandExemptionLock != null)
      {
        this.m_commandExemptionLock.Dispose();
        this.m_commandExemptionLock = (IDisposable) null;
      }
      GC.SuppressFinalize((object) this);
    }

    protected abstract void Dispose(bool disposing);

    public long TotalResultSize => this.m_totalResultSize;

    public bool IsCacheFull => this.m_cacheUsage >= (long) this.MaxCacheSize;

    protected bool IsCanceled => this.m_canceled;

    public static int CommandCacheLimit
    {
      get => Command.s_commandCacheLimit;
      set => Command.s_commandCacheLimit = value;
    }

    public virtual int MaxCacheSize
    {
      get => this.m_maxCacheSize;
      set => this.m_maxCacheSize = value;
    }

    protected internal IVssRequestContext RequestContext
    {
      get => this.m_requestContext;
      set => this.m_requestContext = value;
    }
  }
}
