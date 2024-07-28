// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.TypeExtensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal static class TypeExtensions
  {
    public static IEnumerable<MethodInfo> GetRuntimeMethods(this Type type) => (IEnumerable<MethodInfo>) type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

    public static bool IsAbstract(this Type type) => type.IsAbstract;

    public static bool IsGenericType(this Type type) => type.IsGenericType;
  }
}
