// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.MathBuiltinFunctions
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
  internal static class MathBuiltinFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> MathBuiltinFunctionDefinitions { get; set; }

    static MathBuiltinFunctions() => MathBuiltinFunctions.MathBuiltinFunctionDefinitions = new Dictionary<string, BuiltinFunctionVisitor>()
    {
      {
        "Abs",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ABS", true, new List<Type[]>()
        {
          new Type[1]{ typeof (Decimal) },
          new Type[1]{ typeof (double) },
          new Type[1]{ typeof (float) },
          new Type[1]{ typeof (int) },
          new Type[1]{ typeof (long) },
          new Type[1]{ typeof (sbyte) },
          new Type[1]{ typeof (short) }
        })
      },
      {
        "Acos",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ACOS", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Asin",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ASIN", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Atan",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ATAN", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Atan2",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ATN2", true, new List<Type[]>()
        {
          new Type[2]{ typeof (double), typeof (double) }
        })
      },
      {
        "Ceiling",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("CEILING", true, new List<Type[]>()
        {
          new Type[1]{ typeof (Decimal) },
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Cos",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("COS", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Exp",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("EXP", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Floor",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("FLOOR", true, new List<Type[]>()
        {
          new Type[1]{ typeof (Decimal) },
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Log",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("LOG", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) },
          new Type[2]{ typeof (double), typeof (double) }
        })
      },
      {
        "Log10",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("LOG10", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Pow",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("POWER", true, new List<Type[]>()
        {
          new Type[2]{ typeof (double), typeof (double) }
        })
      },
      {
        "Round",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ROUND", true, new List<Type[]>()
        {
          new Type[1]{ typeof (Decimal) },
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Sign",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SIGN", true, new List<Type[]>()
        {
          new Type[1]{ typeof (Decimal) },
          new Type[1]{ typeof (double) },
          new Type[1]{ typeof (float) },
          new Type[1]{ typeof (int) },
          new Type[1]{ typeof (long) },
          new Type[1]{ typeof (sbyte) },
          new Type[1]{ typeof (short) }
        })
      },
      {
        "Sin",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SIN", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Sqrt",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SQRT", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Tan",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("TAN", true, new List<Type[]>()
        {
          new Type[1]{ typeof (double) }
        })
      },
      {
        "Truncate",
        (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("TRUNC", true, new List<Type[]>()
        {
          new Type[1]{ typeof (Decimal) },
          new Type[1]{ typeof (double) }
        })
      }
    };

    public static SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      BuiltinFunctionVisitor builtinFunctionVisitor;
      if (!MathBuiltinFunctions.MathBuiltinFunctionDefinitions.TryGetValue(methodCallExpression.Method.Name, out builtinFunctionVisitor))
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
      return builtinFunctionVisitor.Visit(methodCallExpression, context);
    }
  }
}
