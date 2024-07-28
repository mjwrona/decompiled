// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.NameVersionHelper
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Dynamic;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public static class NameVersionHelper
  {
    internal static string GetDefaultMethodName(
      MethodInfo methodInfo,
      bool useFullyQualifiedMethodNames)
    {
      string defaultMethodName = methodInfo.Name;
      if (useFullyQualifiedMethodNames && methodInfo.DeclaringType != (Type) null)
        defaultMethodName = NameVersionHelper.GetFullyQualifiedMethodName(methodInfo.DeclaringType.Name, methodInfo.Name);
      return defaultMethodName;
    }

    public static string GetDefaultName(object obj) => NameVersionHelper.GetDefaultName(obj, false);

    public static string GetDefaultName(object obj, bool useFullyQualifiedMethodNames)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      Type type;
      MethodInfo methodInfo;
      return !((type = obj as Type) != (Type) null) ? (!((methodInfo = obj as MethodInfo) != (MethodInfo) null) ? (!(obj is InvokeMemberBinder invokeMemberBinder) ? obj.GetType().ToString() : invokeMemberBinder.Name) : NameVersionHelper.GetDefaultMethodName(methodInfo, useFullyQualifiedMethodNames)) : type.ToString();
    }

    public static string GetDefaultVersion(object obj) => string.Empty;

    internal static string GetFullyQualifiedMethodName(string declaringType, string methodName) => string.IsNullOrEmpty(declaringType) ? methodName : declaringType + "." + methodName;
  }
}
