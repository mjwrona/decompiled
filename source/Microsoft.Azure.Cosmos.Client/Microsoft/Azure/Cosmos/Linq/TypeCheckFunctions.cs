// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.TypeCheckFunctions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class TypeCheckFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> TypeCheckFunctionsDefinitions { get; set; }

    static TypeCheckFunctions() => TypeCheckFunctions.TypeCheckFunctionsDefinitions = new Dictionary<string, BuiltinFunctionVisitor>()
    {
      {
        "IsDefined",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_DEFINED", true, new List<Type[]>()
        {
          new Type[1]{ typeof (object) }
        })
      },
      {
        "IsNull",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_NULL", true, new List<Type[]>()
        {
          new Type[1]{ typeof (object) }
        })
      },
      {
        "IsPrimitive",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_PRIMITIVE", true, new List<Type[]>()
        {
          new Type[1]{ typeof (object) }
        })
      }
    };

    public static SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      BuiltinFunctionVisitor builtinFunctionVisitor;
      if (!TypeCheckFunctions.TypeCheckFunctionsDefinitions.TryGetValue(methodCallExpression.Method.Name, out builtinFunctionVisitor))
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
      return builtinFunctionVisitor.Visit(methodCallExpression, context);
    }
  }
}
