// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.SpatialBuiltinFunctions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Spatial;
using Microsoft.Azure.Documents.Sql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class SpatialBuiltinFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> SpatialBuiltinFunctionDefinitions { get; set; }

    static SpatialBuiltinFunctions()
    {
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions = new Dictionary<string, BuiltinFunctionVisitor>();
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("Distance", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_Distance", true, new List<Type[]>()
      {
        new Type[2]{ typeof (Geometry), typeof (Geometry) }
      }));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("Within", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_Within", true, new List<Type[]>()
      {
        new Type[2]{ typeof (Geometry), typeof (Geometry) }
      }));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("IsValidDetailed", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_IsValidDetailed", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Geometry) }
      }));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("IsValid", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_IsValid", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Geometry) }
      }));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("Intersects", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_Intersects", true, new List<Type[]>()
      {
        new Type[2]{ typeof (Geometry), typeof (Geometry) }
      }));
    }

    public static SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      BuiltinFunctionVisitor builtinFunctionVisitor = (BuiltinFunctionVisitor) null;
      if (SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.TryGetValue(methodCallExpression.Method.Name, out builtinFunctionVisitor))
        return builtinFunctionVisitor.Visit(methodCallExpression, context);
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
    }
  }
}
