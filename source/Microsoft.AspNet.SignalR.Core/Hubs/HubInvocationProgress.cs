// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubInvocationProgress
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubInvocationProgress
  {
    private static readonly ConcurrentDictionary<Type, Func<Func<object, Task>, HubInvocationProgress>> _progressCreateCache = new ConcurrentDictionary<Type, Func<Func<object, Task>, HubInvocationProgress>>();
    private volatile bool _complete;
    private readonly object _statusLocker = new object();
    private readonly Func<object, Task> _sendProgressFunc;

    protected HubInvocationProgress(Func<object, Task> sendProgressFunc) => this._sendProgressFunc = sendProgressFunc;

    private TraceSource Trace { get; set; }

    public static HubInvocationProgress Create(
      Type progressGenericType,
      Func<object, Task> sendProgressFunc,
      TraceSource traceSource)
    {
      Func<Func<object, Task>, HubInvocationProgress> func;
      if (!HubInvocationProgress._progressCreateCache.TryGetValue(progressGenericType, out func))
      {
        func = (Func<Func<object, Task>, HubInvocationProgress>) typeof (HubInvocationProgress).GetMethod(nameof (Create), new Type[1]
        {
          typeof (Func<object, Task>)
        }).MakeGenericMethod(progressGenericType).CreateDelegate(typeof (Func<Func<object, Task>, HubInvocationProgress>));
        HubInvocationProgress._progressCreateCache[progressGenericType] = func;
      }
      HubInvocationProgress invocationProgress = func(sendProgressFunc);
      invocationProgress.Trace = traceSource;
      return invocationProgress;
    }

    public static HubInvocationProgress<T> Create<T>(Func<object, Task> sendProgressFunc) => new HubInvocationProgress<T>(sendProgressFunc);

    public void SetComplete()
    {
      lock (this._statusLocker)
        this._complete = true;
    }

    protected void DoReport(object value)
    {
      lock (this._statusLocker)
      {
        if (this._complete)
          throw new InvalidOperationException(Resources.Error_HubProgressOnlyReportableBeforeMethodReturns);
        this._sendProgressFunc(value).Catch<Task>(this.Trace);
      }
    }
  }
}
