// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.TypedHubCallerConnectionContext`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class TypedHubCallerConnectionContext<T> : 
    TypedHubConnectionContext<T>,
    IHubCallerConnectionContext<T>,
    IHubConnectionContext<T>
  {
    private IHubCallerConnectionContext<object> _dynamicContext;

    public TypedHubCallerConnectionContext(IHubCallerConnectionContext<object> dynamicContext)
      : base((IHubConnectionContext<object>) dynamicContext)
    {
      this._dynamicContext = dynamicContext;
    }

    public T Caller
    {
      get
      {
        if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1 == null)
          TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubCallerConnectionContext<T>)));
        Func<CallSite, object, T> target = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1.Target;
        CallSite<Func<CallSite, object, T>> p1 = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1;
        if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
          TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubCallerConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        object obj = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) TypedHubCallerConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.Caller);
        return target((CallSite) p1, obj);
      }
    }

    public object CallerState => this._dynamicContext.CallerState;

    public T Others
    {
      get
      {
        if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
          TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubCallerConnectionContext<T>)));
        Func<CallSite, object, T> target = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1.Target;
        CallSite<Func<CallSite, object, T>> p1 = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1;
        if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
          TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubCallerConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        object obj = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) TypedHubCallerConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.Others);
        return target((CallSite) p1, obj);
      }
    }

    public T OthersInGroup(string groupName)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubCallerConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubCallerConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) TypedHubCallerConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.OthersInGroup(groupName));
      return target((CallSite) p1, obj);
    }

    public T OthersInGroups(IList<string> groupNames)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubCallerConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubCallerConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) TypedHubCallerConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.OthersInGroups(groupNames));
      return target((CallSite) p1, obj);
    }
  }
}
