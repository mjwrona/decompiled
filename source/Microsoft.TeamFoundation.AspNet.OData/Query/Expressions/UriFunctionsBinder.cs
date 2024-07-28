// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.UriFunctionsBinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  public static class UriFunctionsBinder
  {
    private static Dictionary<string, MethodInfo> methodLiteralSignaturesToMethodInfo = new Dictionary<string, MethodInfo>();
    private static object locker = new object();

    public static void BindUriFunctionName(string functionName, MethodInfo methodInfo)
    {
      if (string.IsNullOrEmpty(functionName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (functionName));
      string key = !(methodInfo == (MethodInfo) null) ? UriFunctionsBinder.GetMethodLiteralSignature(functionName, methodInfo) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (methodInfo));
      lock (UriFunctionsBinder.locker)
      {
        if (UriFunctionsBinder.methodLiteralSignaturesToMethodInfo.ContainsKey(key))
          throw new ODataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SRResources.UriFunctionClrBinderAlreadyBound, new object[1]
          {
            (object) key
          }));
        UriFunctionsBinder.methodLiteralSignaturesToMethodInfo.Add(key, methodInfo);
      }
    }

    public static bool UnbindUriFunctionName(string functionName, MethodInfo methodInfo)
    {
      if (string.IsNullOrEmpty(functionName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (functionName));
      string key = !(methodInfo == (MethodInfo) null) ? UriFunctionsBinder.GetMethodLiteralSignature(functionName, methodInfo) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (methodInfo));
      lock (UriFunctionsBinder.locker)
        return UriFunctionsBinder.methodLiteralSignaturesToMethodInfo.Remove(key);
    }

    public static bool TryGetMethodInfo(
      string functionName,
      IEnumerable<Type> methodArgumentsType,
      out MethodInfo methodInfo)
    {
      if (string.IsNullOrEmpty(functionName))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (functionName));
      string key = methodArgumentsType != null ? UriFunctionsBinder.GetMethodLiteralSignature(functionName, methodArgumentsType) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (methodArgumentsType));
      lock (UriFunctionsBinder.locker)
        return UriFunctionsBinder.methodLiteralSignaturesToMethodInfo.TryGetValue(key, out methodInfo);
    }

    private static string GetMethodLiteralSignature(string methodName, MethodInfo methodInfo)
    {
      IEnumerable<Type> types = ((IEnumerable<ParameterInfo>) methodInfo.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (parameter => parameter.ParameterType));
      if (!methodInfo.IsStatic)
        types = ((IEnumerable<Type>) new Type[1]
        {
          methodInfo.DeclaringType
        }).Concat<Type>(types);
      return UriFunctionsBinder.GetMethodLiteralSignature(methodName, types);
    }

    private static string GetMethodLiteralSignature(
      string methodName,
      IEnumerable<Type> methodArgumentsType)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str = string.Empty;
      stringBuilder.Append(methodName);
      stringBuilder.Append('(');
      foreach (Type type in methodArgumentsType)
      {
        stringBuilder.Append(str);
        str = ", ";
        stringBuilder.Append(type.FullName);
      }
      stringBuilder.Append(')');
      return stringBuilder.ToString();
    }
  }
}
