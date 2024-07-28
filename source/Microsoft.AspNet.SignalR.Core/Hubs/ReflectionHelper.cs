// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.ReflectionHelper
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public static class ReflectionHelper
  {
    private static readonly Type[] _excludeTypes = new Type[2]
    {
      typeof (Hub),
      typeof (object)
    };
    private static readonly Type[] _excludeInterfaces = new Type[2]
    {
      typeof (IHub),
      typeof (IDisposable)
    };

    public static IEnumerable<MethodInfo> GetExportedHubMethods(Type type) => !typeof (IHub).IsAssignableFrom(type) ? Enumerable.Empty<MethodInfo>() : ((IEnumerable<MethodInfo>) type.GetMethods(BindingFlags.Instance | BindingFlags.Public)).Except<MethodInfo>(((IEnumerable<Type>) ReflectionHelper._excludeInterfaces).SelectMany<Type, MethodInfo>((Func<Type, IEnumerable<MethodInfo>>) (i => ReflectionHelper.GetInterfaceMethods(type, i)))).Where<MethodInfo>(new Func<MethodInfo, bool>(ReflectionHelper.IsValidHubMethod));

    private static bool IsValidHubMethod(MethodInfo methodInfo) => !((IEnumerable<Type>) ReflectionHelper._excludeTypes).Contains<Type>(methodInfo.GetBaseDefinition().DeclaringType) && !methodInfo.IsSpecialName;

    private static IEnumerable<MethodInfo> GetInterfaceMethods(Type type, Type iface) => !iface.IsAssignableFrom(type) ? Enumerable.Empty<MethodInfo>() : (IEnumerable<MethodInfo>) type.GetInterfaceMap(iface).TargetMethods;

    public static TResult GetAttributeValue<TAttribute, TResult>(
      ICustomAttributeProvider source,
      Func<TAttribute, TResult> valueGetter)
      where TAttribute : Attribute
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (valueGetter == null)
        throw new ArgumentNullException(nameof (valueGetter));
      List<TAttribute> list = source.GetCustomAttributes(typeof (TAttribute), false).Cast<TAttribute>().ToList<TAttribute>();
      return list.Any<TAttribute>() ? valueGetter(list[0]) : default (TResult);
    }
  }
}
