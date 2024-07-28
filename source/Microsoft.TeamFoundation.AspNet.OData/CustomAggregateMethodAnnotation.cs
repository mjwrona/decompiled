// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.CustomAggregateMethodAnnotation
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.OData
{
  public class CustomAggregateMethodAnnotation
  {
    private readonly Dictionary<string, IDictionary<Type, MethodInfo>> _tokenToMethodMap = new Dictionary<string, IDictionary<Type, MethodInfo>>();

    public CustomAggregateMethodAnnotation AddMethod(
      string methodToken,
      IDictionary<Type, MethodInfo> methods)
    {
      this._tokenToMethodMap.Add(methodToken, methods);
      return this;
    }

    public bool GetMethodInfo(string methodToken, Type returnType, out MethodInfo methodInfo)
    {
      methodInfo = (MethodInfo) null;
      IDictionary<Type, MethodInfo> dictionary;
      return this._tokenToMethodMap.TryGetValue(methodToken, out dictionary) && dictionary.TryGetValue(returnType, out methodInfo);
    }
  }
}
