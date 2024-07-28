// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.TypedHubConnectionContext`1
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class TypedHubConnectionContext<T> : IHubConnectionContext<T>
  {
    private IHubConnectionContext<object> _dynamicContext;

    public TypedHubConnectionContext(IHubConnectionContext<object> dynamicContext) => this._dynamicContext = dynamicContext;

    public T All
    {
      get
      {
        if (TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1 == null)
          TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
        Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1.Target;
        CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__1;
        if (TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
          TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__3.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.All);
        return target((CallSite) p1, obj);
      }
    }

    public T AllExcept(params string[] excludeConnectionIds)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__4.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.AllExcept(excludeConnectionIds));
      return target((CallSite) p1, obj);
    }

    public T Client(string connectionId)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__5.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.Client(connectionId));
      return target((CallSite) p1, obj);
    }

    public T Clients(IList<string> connectionIds)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__6.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.Clients(connectionIds));
      return target((CallSite) p1, obj);
    }

    public T Group(string groupName, params string[] excludeConnectionIds)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__7.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.Group(groupName, excludeConnectionIds));
      return target((CallSite) p1, obj);
    }

    public T Groups(IList<string> groupNames, params string[] excludeConnectionIds)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__8.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.Groups(groupNames, excludeConnectionIds));
      return target((CallSite) p1, obj);
    }

    public T User(string userId)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__9.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.User(userId));
      return target((CallSite) p1, obj);
    }

    public T Users(IList<string> userIds)
    {
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (T), typeof (TypedHubConnectionContext<T>)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, T> target = TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, T>> p1 = TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Build", (IEnumerable<Type>) null, typeof (TypedHubConnectionContext<T>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) TypedHubConnectionContext<T>.\u003C\u003Eo__10.\u003C\u003Ep__0, typeof (TypedClientBuilder<T>), this._dynamicContext.Users(userIds));
      return target((CallSite) p1, obj);
    }
  }
}
