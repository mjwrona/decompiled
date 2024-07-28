// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.MathBuiltinFunctions
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
  internal static class MathBuiltinFunctions
  {
    private static Dictionary<string, BuiltinFunctionVisitor> MathBuiltinFunctionDefinitions { get; set; }

    static MathBuiltinFunctions()
    {
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions = new Dictionary<string, BuiltinFunctionVisitor>();
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Abs", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ABS", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Decimal) },
        new Type[1]{ typeof (double) },
        new Type[1]{ typeof (float) },
        new Type[1]{ typeof (int) },
        new Type[1]{ typeof (long) },
        new Type[1]{ typeof (sbyte) },
        new Type[1]{ typeof (short) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Acos", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ACOS", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Asin", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ASIN", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Atan", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ATAN", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Atan2", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ATN2", true, new List<Type[]>()
      {
        new Type[2]{ typeof (double), typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Ceiling", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("CEILING", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Decimal) },
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Cos", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("COS", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Exp", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("EXP", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Floor", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("FLOOR", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Decimal) },
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Log", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("LOG", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) },
        new Type[2]{ typeof (double), typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Log10", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("LOG10", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Pow", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("POWER", true, new List<Type[]>()
      {
        new Type[2]{ typeof (double), typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Round", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("ROUND", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Decimal) },
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Sign", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SIGN", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Decimal) },
        new Type[1]{ typeof (double) },
        new Type[1]{ typeof (float) },
        new Type[1]{ typeof (int) },
        new Type[1]{ typeof (long) },
        new Type[1]{ typeof (sbyte) },
        new Type[1]{ typeof (short) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Sin", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SIN", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Sqrt", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("SQRT", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Tan", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("TAN", true, new List<Type[]>()
      {
        new Type[1]{ typeof (double) }
      }));
      MathBuiltinFunctions.MathBuiltinFunctionDefinitions.Add("Truncate", (BuiltinFunctionVisitor) new SqlBuiltinFunctionVisitor("TRUNC", true, new List<Type[]>()
      {
        new Type[1]{ typeof (Decimal) },
        new Type[1]{ typeof (double) }
      }));
    }

    public static SqlScalarExpression Visit(
      MethodCallExpression methodCallExpression,
      TranslationContext context)
    {
      BuiltinFunctionVisitor builtinFunctionVisitor = (BuiltinFunctionVisitor) null;
      if (MathBuiltinFunctions.MathBuiltinFunctionDefinitions.TryGetValue(methodCallExpression.Method.Name, out builtinFunctionVisitor))
        return builtinFunctionVisitor.Visit(methodCallExpression, context);
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ClientResources.MethodNotSupported, (object) methodCallExpression.Method.Name));
    }
  }
}
