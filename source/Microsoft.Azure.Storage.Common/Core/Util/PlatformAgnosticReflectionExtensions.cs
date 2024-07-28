// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.PlatformAgnosticReflectionExtensions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class PlatformAgnosticReflectionExtensions
  {
    public static IEnumerable<MethodInfo> FindStaticMethods(this Type type, string name) => ((IEnumerable<MethodInfo>) type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == name));

    public static IEnumerable<MethodInfo> FindMethods(this Type type) => (IEnumerable<MethodInfo>) type.GetMethods();

    public static MethodInfo FindMethod(this Type type, string name, Type[] parameters) => type.GetMethod(name, parameters);

    public static PropertyInfo FindProperty(this Type type, string name) => type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    public static MethodInfo FindGetProp(this PropertyInfo property) => property.GetGetMethod();

    public static MethodInfo FindSetProp(this PropertyInfo property) => property.GetSetMethod();
  }
}
