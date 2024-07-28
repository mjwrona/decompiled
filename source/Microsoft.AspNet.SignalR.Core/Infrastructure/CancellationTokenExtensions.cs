// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.CancellationTokenExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Reflection;
using System.Threading;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal static class CancellationTokenExtensions
  {
    private static readonly CancellationTokenExtensions.RegisterDelegate _tokenRegister = CancellationTokenExtensions.ResolveRegisterDelegate();

    public static IDisposable SafeRegister(
      this CancellationToken cancellationToken,
      Action<object> callback,
      object state)
    {
      CancellationTokenExtensions.CancellationCallbackWrapper cancellationCallbackWrapper = new CancellationTokenExtensions.CancellationCallbackWrapper(callback, state);
      CancellationTokenRegistration registration = CancellationTokenExtensions._tokenRegister(ref cancellationToken, (Action<object>) (s => CancellationTokenExtensions.InvokeCallback(s)), (object) cancellationCallbackWrapper);
      return (IDisposable) new DisposableAction((Action<object>) (s => CancellationTokenExtensions.Dispose(s)), (object) new CancellationTokenExtensions.DiposeCancellationState(cancellationCallbackWrapper, registration));
    }

    private static void InvokeCallback(object state) => ((CancellationTokenExtensions.CancellationCallbackWrapper) state).TryInvoke();

    private static void Dispose(object state) => ((CancellationTokenExtensions.DiposeCancellationState) state).TryDispose();

    private static CancellationTokenExtensions.RegisterDelegate ResolveRegisterDelegate()
    {
      CancellationTokenExtensions.RegisterDelegate registerDelegate = (CancellationTokenExtensions.RegisterDelegate) ((ref CancellationToken token, Action<object> callback, object state) => token.Register(callback, state));
      MethodInfo method = (MethodInfo) null;
      try
      {
        method = typeof (CancellationToken).GetMethod("InternalRegisterWithoutEC", BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[2]
        {
          typeof (Action<object>),
          typeof (object)
        }, (ParameterModifier[]) null);
      }
      catch
      {
      }
      if (method == (MethodInfo) null)
        return registerDelegate;
      try
      {
        return (CancellationTokenExtensions.RegisterDelegate) Delegate.CreateDelegate(typeof (CancellationTokenExtensions.RegisterDelegate), (object) null, method);
      }
      catch
      {
        return registerDelegate;
      }
    }

    private delegate CancellationTokenRegistration RegisterDelegate(
      ref CancellationToken token,
      Action<object> callback,
      object state);

    private class DiposeCancellationState
    {
      private readonly CancellationTokenExtensions.CancellationCallbackWrapper _callbackWrapper;
      private readonly CancellationTokenRegistration _registration;

      public DiposeCancellationState(
        CancellationTokenExtensions.CancellationCallbackWrapper callbackWrapper,
        CancellationTokenRegistration registration)
      {
        this._callbackWrapper = callbackWrapper;
        this._registration = registration;
      }

      public void TryDispose()
      {
        if (!this._callbackWrapper.TrySetInvoked())
          return;
        this._registration.Dispose();
      }
    }

    private class CancellationCallbackWrapper
    {
      private readonly Action<object> _callback;
      private readonly object _state;
      private int _callbackInvoked;

      public CancellationCallbackWrapper(Action<object> callback, object state)
      {
        this._callback = callback;
        this._state = state;
      }

      public bool TrySetInvoked() => Interlocked.Exchange(ref this._callbackInvoked, 1) == 0;

      public void TryInvoke()
      {
        if (!this.TrySetInvoked())
          return;
        this._callback(this._state);
      }
    }
  }
}
