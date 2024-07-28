// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.ConnectionMessage
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR
{
  public struct ConnectionMessage
  {
    public string Signal { get; private set; }

    public IList<string> Signals { get; private set; }

    public object Value { get; private set; }

    public IList<string> ExcludedSignals { get; private set; }

    public ConnectionMessage(IList<string> signals, object value)
      : this(signals, value, ListHelper<string>.Empty)
    {
    }

    public ConnectionMessage(IList<string> signals, object value, IList<string> excludedSignals)
      : this()
    {
      this.Signals = signals;
      this.Value = value;
      this.ExcludedSignals = excludedSignals;
    }

    public ConnectionMessage(string signal, object value)
      : this(signal, value, ListHelper<string>.Empty)
    {
    }

    public ConnectionMessage(string signal, object value, IList<string> excludedSignals)
      : this()
    {
      this.Signal = signal;
      this.Value = value;
      this.ExcludedSignals = excludedSignals;
    }
  }
}
