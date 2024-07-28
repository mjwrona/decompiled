// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.TypeCheckFunctions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class TypeCheckFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> TypeCheckFunctionsDefinitions { get; set; }

    static TypeCheckFunctions()
    {
      TypeCheckFunctions.TypeCheckFunctionsDefinitions = new Dictionary<string, BuiltinFunctionVisitor>();
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsArray", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_ARRAY", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsBool", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_BOOL", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsDefined", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_DEFINED", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsNull", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_NULL", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsNumber", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_NUMBER", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsObject", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_OBJECT", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsPrimitive", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_PRIMITIVE", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
      TypeCheckFunctions.TypeCheckFunctionsDefinitions.Add("IsString", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("IS_STRING", true, new List<Type[]>()
      {
        new Type[1]{ typeof (object) }
      }));
    }

    public static SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      BuiltinFunctionVisitor builtinFunctionVisitor = (BuiltinFunctionVisitor) null;
      if (TypeCheckFunctions.TypeCheckFunctionsDefinitions.TryGetValue(methodCallExpression.Method.Name, out builtinFunctionVisitor))
        return builtinFunctionVisitor.Visit(methodCallExpression, context);
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
    }
  }
}
