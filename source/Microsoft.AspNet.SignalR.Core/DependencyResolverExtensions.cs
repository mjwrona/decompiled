// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.DependencyResolverExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.SignalR
{
  public static class DependencyResolverExtensions
  {
    public static T Resolve<T>(this IDependencyResolver resolver) => resolver != null ? (T) resolver.GetService(typeof (T)) : throw new ArgumentNullException(nameof (resolver));

    public static object Resolve(this IDependencyResolver resolver, Type type)
    {
      if (resolver == null)
        throw new ArgumentNullException(nameof (resolver));
      return !(type == (Type) null) ? resolver.GetService(type) : throw new ArgumentNullException(nameof (type));
    }

    public static IEnumerable<T> ResolveAll<T>(this IDependencyResolver resolver) => resolver != null ? resolver.GetServices(typeof (T)).Cast<T>() : throw new ArgumentNullException(nameof (resolver));

    public static IEnumerable<object> ResolveAll(this IDependencyResolver resolver, Type type)
    {
      if (resolver == null)
        throw new ArgumentNullException(nameof (resolver));
      return !(type == (Type) null) ? resolver.GetServices(type) : throw new ArgumentNullException(nameof (type));
    }
  }
}
