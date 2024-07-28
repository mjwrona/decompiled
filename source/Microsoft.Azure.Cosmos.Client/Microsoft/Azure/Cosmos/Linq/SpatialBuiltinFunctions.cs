// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.SpatialBuiltinFunctions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class SpatialBuiltinFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> SpatialBuiltinFunctionDefinitions { get; set; }

    static SpatialBuiltinFunctions()
    {
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions = new Dictionary<string, BuiltinFunctionVisitor>();
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("Distance", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_DISTANCE", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Geometry) }
      }));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("Within", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_WITHIN", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Geometry) }
      }));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("IsValidDetailed", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_ISVALIDDETAILED", true, new List<Type[]>()));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("IsValid", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_ISVALID", true, new List<Type[]>()));
      SpatialBuiltinFunctions.SpatialBuiltinFunctionDefinitions.Add("Intersects", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ST_INTERSECTS", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Geometry) }
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
