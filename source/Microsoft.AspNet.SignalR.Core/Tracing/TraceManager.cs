// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Tracing.TraceManager
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.AspNet.SignalR.Tracing
{
  public class TraceManager : ITraceManager
  {
    private readonly ConcurrentDictionary<string, TraceSource> _sources = new ConcurrentDictionary<string, TraceSource>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly TextWriterTraceListener _hostTraceListener;

    public TraceManager()
      : this((TextWriterTraceListener) null)
    {
    }

    public TraceManager(TextWriterTraceListener hostTraceListener)
    {
      this.Switch = new SourceSwitch("SignalRSwitch");
      this._hostTraceListener = hostTraceListener;
    }

    public SourceSwitch Switch { get; private set; }

    public TraceSource this[string name] => this._sources.GetOrAdd(name, (Func<string, TraceSource>) (key => this.CreateTraceSource(key)));

    private TraceSource CreateTraceSource(string name)
    {
      TraceSource traceSource = new TraceSource(name, SourceLevels.Off)
      {
        Switch = this.Switch
      };
      if (this._hostTraceListener != null)
      {
        if (traceSource.Listeners.Count > 0 && traceSource.Listeners[0] is DefaultTraceListener)
          traceSource.Listeners.RemoveAt(0);
        traceSource.Listeners.Add((TraceListener) this._hostTraceListener);
      }
      return traceSource;
    }
  }
}
