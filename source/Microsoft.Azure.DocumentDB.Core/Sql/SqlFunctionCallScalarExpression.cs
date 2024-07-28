// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlFunctionCallScalarExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlFunctionCallScalarExpression : SqlScalarExpression
  {
    private const string UdfNamespaceQualifier = "udf.";
    private static readonly Dictionary<string, SqlIdentifier> FunctionIdentifiers = new Dictionary<string, SqlIdentifier>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "_COMPARE_BSON_BINARYDATA",
        SqlFunctionCallScalarExpression.Identifiers.InternalCompareBsonBinaryData
      },
      {
        "_COMPARE_OBJECTS",
        SqlFunctionCallScalarExpression.Identifiers.InternalCompareObjects
      },
      {
        "_ObjectToArray",
        SqlFunctionCallScalarExpression.Identifiers.InternalObjectToArray
      },
      {
        "_PROXY_PROJECTION",
        SqlFunctionCallScalarExpression.Identifiers.InternalProxyProjection
      },
      {
        "_REGEX_MATCH",
        SqlFunctionCallScalarExpression.Identifiers.InternalRegexMatch
      },
      {
        "_ST_DISTANCE",
        SqlFunctionCallScalarExpression.Identifiers.InternalStDistance
      },
      {
        "_ST_INTERSECTS",
        SqlFunctionCallScalarExpression.Identifiers.InternalStIntersects
      },
      {
        "_ST_WITHIN",
        SqlFunctionCallScalarExpression.Identifiers.InternalStWithin
      },
      {
        "_TRY_ARRAY_CONTAINS",
        SqlFunctionCallScalarExpression.Identifiers.InternalTryArrayContains
      },
      {
        "ABS",
        SqlFunctionCallScalarExpression.Identifiers.Abs
      },
      {
        "ACOS",
        SqlFunctionCallScalarExpression.Identifiers.Acos
      },
      {
        "ALL",
        SqlFunctionCallScalarExpression.Identifiers.All
      },
      {
        "ANY",
        SqlFunctionCallScalarExpression.Identifiers.Any
      },
      {
        "ARRAY",
        SqlFunctionCallScalarExpression.Identifiers.Array
      },
      {
        "ARRAY_CONCAT",
        SqlFunctionCallScalarExpression.Identifiers.ArrayConcat
      },
      {
        "ARRAY_CONTAINS",
        SqlFunctionCallScalarExpression.Identifiers.ArrayContains
      },
      {
        "ARRAY_LENGTH",
        SqlFunctionCallScalarExpression.Identifiers.ArrayLength
      },
      {
        "ARRAY_SLICE",
        SqlFunctionCallScalarExpression.Identifiers.ArraySlice
      },
      {
        "ASIN",
        SqlFunctionCallScalarExpression.Identifiers.Asin
      },
      {
        "ATAN",
        SqlFunctionCallScalarExpression.Identifiers.Atan
      },
      {
        "ATN2",
        SqlFunctionCallScalarExpression.Identifiers.Atn2
      },
      {
        "AVG",
        SqlFunctionCallScalarExpression.Identifiers.Avg
      },
      {
        "CEILING",
        SqlFunctionCallScalarExpression.Identifiers.Ceiling
      },
      {
        "CONCAT",
        SqlFunctionCallScalarExpression.Identifiers.Concat
      },
      {
        "CONTAINS",
        SqlFunctionCallScalarExpression.Identifiers.Contains
      },
      {
        "COS",
        SqlFunctionCallScalarExpression.Identifiers.Cos
      },
      {
        "COT",
        SqlFunctionCallScalarExpression.Identifiers.Cot
      },
      {
        "COUNT",
        SqlFunctionCallScalarExpression.Identifiers.Count
      },
      {
        "DEGREES",
        SqlFunctionCallScalarExpression.Identifiers.Degrees
      },
      {
        "DOCUMENTID",
        SqlFunctionCallScalarExpression.Identifiers.Documentid
      },
      {
        "ENDSWITH",
        SqlFunctionCallScalarExpression.Identifiers.Endswith
      },
      {
        "EXP",
        SqlFunctionCallScalarExpression.Identifiers.Exp
      },
      {
        "FLOOR",
        SqlFunctionCallScalarExpression.Identifiers.Floor
      },
      {
        "INDEX_OF",
        SqlFunctionCallScalarExpression.Identifiers.IndexOf
      },
      {
        "IS_ARRAY",
        SqlFunctionCallScalarExpression.Identifiers.IsArray
      },
      {
        "IS_BOOL",
        SqlFunctionCallScalarExpression.Identifiers.IsBool
      },
      {
        "IS_DEFINED",
        SqlFunctionCallScalarExpression.Identifiers.IsDefined
      },
      {
        "IS_FINITE_NUMBER",
        SqlFunctionCallScalarExpression.Identifiers.IsFiniteNumber
      },
      {
        "IS_NULL",
        SqlFunctionCallScalarExpression.Identifiers.IsNull
      },
      {
        "IS_NUMBER",
        SqlFunctionCallScalarExpression.Identifiers.IsNumber
      },
      {
        "IS_OBJECT",
        SqlFunctionCallScalarExpression.Identifiers.IsObject
      },
      {
        "IS_PRIMITIVE",
        SqlFunctionCallScalarExpression.Identifiers.IsPrimitive
      },
      {
        "IS_STRING",
        SqlFunctionCallScalarExpression.Identifiers.IsString
      },
      {
        "LEFT",
        SqlFunctionCallScalarExpression.Identifiers.Left
      },
      {
        "LENGTH",
        SqlFunctionCallScalarExpression.Identifiers.Length
      },
      {
        "LIKE",
        SqlFunctionCallScalarExpression.Identifiers.Like
      },
      {
        "LOG",
        SqlFunctionCallScalarExpression.Identifiers.Log
      },
      {
        "LOG10",
        SqlFunctionCallScalarExpression.Identifiers.Log10
      },
      {
        "LOWER",
        SqlFunctionCallScalarExpression.Identifiers.Lower
      },
      {
        "LTRIM",
        SqlFunctionCallScalarExpression.Identifiers.Ltrim
      },
      {
        "MAX",
        SqlFunctionCallScalarExpression.Identifiers.Max
      },
      {
        "MIN",
        SqlFunctionCallScalarExpression.Identifiers.Min
      },
      {
        "PI",
        SqlFunctionCallScalarExpression.Identifiers.Pi
      },
      {
        "POWER",
        SqlFunctionCallScalarExpression.Identifiers.Power
      },
      {
        "RADIANS",
        SqlFunctionCallScalarExpression.Identifiers.Radians
      },
      {
        "RAND",
        SqlFunctionCallScalarExpression.Identifiers.Rand
      },
      {
        "REPLACE",
        SqlFunctionCallScalarExpression.Identifiers.Replace
      },
      {
        "REPLICATE",
        SqlFunctionCallScalarExpression.Identifiers.Replicate
      },
      {
        "REVERSE",
        SqlFunctionCallScalarExpression.Identifiers.Reverse
      },
      {
        "RIGHT",
        SqlFunctionCallScalarExpression.Identifiers.Right
      },
      {
        "ROUND",
        SqlFunctionCallScalarExpression.Identifiers.Round
      },
      {
        "RTRIM",
        SqlFunctionCallScalarExpression.Identifiers.Rtrim
      },
      {
        "SIGN",
        SqlFunctionCallScalarExpression.Identifiers.Sign
      },
      {
        "SIN",
        SqlFunctionCallScalarExpression.Identifiers.Sin
      },
      {
        "SQRT",
        SqlFunctionCallScalarExpression.Identifiers.Sqrt
      },
      {
        "SQUARE",
        SqlFunctionCallScalarExpression.Identifiers.Square
      },
      {
        "STARTSWITH",
        SqlFunctionCallScalarExpression.Identifiers.Startswith
      },
      {
        "ST_DISTANCE",
        SqlFunctionCallScalarExpression.Identifiers.StDistance
      },
      {
        "ST_INTERSECTS",
        SqlFunctionCallScalarExpression.Identifiers.StIntersects
      },
      {
        "ST_ISVALID",
        SqlFunctionCallScalarExpression.Identifiers.StIsvalid
      },
      {
        "ST_ISVALIDDETAILED",
        SqlFunctionCallScalarExpression.Identifiers.StIsvaliddetailed
      },
      {
        "ST_WITHIN",
        SqlFunctionCallScalarExpression.Identifiers.StWithin
      },
      {
        "SUBSTRING",
        SqlFunctionCallScalarExpression.Identifiers.Substring
      },
      {
        "SUM",
        SqlFunctionCallScalarExpression.Identifiers.Sum
      },
      {
        "TAN",
        SqlFunctionCallScalarExpression.Identifiers.Tan
      },
      {
        "TRUNC",
        SqlFunctionCallScalarExpression.Identifiers.Trunc
      },
      {
        "UPPER",
        SqlFunctionCallScalarExpression.Identifiers.Upper
      }
    };

    private SqlFunctionCallScalarExpression(
      SqlIdentifier name,
      bool isUdf,
      IReadOnlyList<SqlScalarExpression> arguments)
      : base(SqlObjectKind.FunctionCallScalarExpression)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (arguments == null)
        throw new ArgumentNullException("arguments must not be null.");
      foreach (SqlScalarExpression scalarExpression in (IEnumerable<SqlScalarExpression>) arguments)
      {
        if (scalarExpression == null)
          throw new ArgumentNullException("arguments must not have null items.");
      }
      this.Arguments = (IReadOnlyList<SqlScalarExpression>) new List<SqlScalarExpression>((IEnumerable<SqlScalarExpression>) arguments);
      this.Name = name;
      this.IsUdf = isUdf;
    }

    public SqlIdentifier Name { get; }

    public IReadOnlyList<SqlScalarExpression> Arguments { get; }

    public bool IsUdf { get; }

    public static SqlFunctionCallScalarExpression Create(
      SqlIdentifier name,
      bool isUdf,
      params SqlScalarExpression[] arguments)
    {
      return new SqlFunctionCallScalarExpression(name, isUdf, (IReadOnlyList<SqlScalarExpression>) arguments);
    }

    public static SqlFunctionCallScalarExpression Create(
      SqlIdentifier name,
      bool isUdf,
      IReadOnlyList<SqlScalarExpression> arguments)
    {
      return new SqlFunctionCallScalarExpression(name, isUdf, arguments);
    }

    public static SqlFunctionCallScalarExpression Create(
      string name,
      bool isUdf,
      params SqlScalarExpression[] arguments)
    {
      SqlIdentifier name1;
      if (!SqlFunctionCallScalarExpression.FunctionIdentifiers.TryGetValue(name, out name1))
        name1 = SqlIdentifier.Create(name);
      return SqlFunctionCallScalarExpression.Create(name1, isUdf, arguments);
    }

    public static SqlFunctionCallScalarExpression Create(
      string name,
      bool isUdf,
      IReadOnlyList<SqlScalarExpression> arguments)
    {
      SqlIdentifier name1;
      if (!SqlFunctionCallScalarExpression.FunctionIdentifiers.TryGetValue(name, out name1))
        name1 = SqlIdentifier.Create(name);
      return SqlFunctionCallScalarExpression.Create(name1, isUdf, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      string name,
      IReadOnlyList<SqlScalarExpression> arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, false, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      string name,
      params SqlScalarExpression[] arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, false, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      SqlIdentifier name,
      IReadOnlyList<SqlScalarExpression> arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, false, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      SqlIdentifier name,
      params SqlScalarExpression[] arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, false, arguments);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlScalarExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlScalarExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(
      SqlScalarExpressionVisitor<T, TResult> visitor,
      T input)
    {
      return visitor.Visit(this, input);
    }

    public static class Names
    {
      public const string InternalCompareBsonBinaryData = "_COMPARE_BSON_BINARYDATA";
      public const string InternalCompareObjects = "_COMPARE_OBJECTS";
      public const string InternalObjectToArray = "_ObjectToArray";
      public const string InternalProxyProjection = "_PROXY_PROJECTION";
      public const string InternalRegexMatch = "_REGEX_MATCH";
      public const string InternalStDistance = "_ST_DISTANCE";
      public const string InternalStIntersects = "_ST_INTERSECTS";
      public const string InternalStWithin = "_ST_WITHIN";
      public const string InternalTryArrayContains = "_TRY_ARRAY_CONTAINS";
      public const string Abs = "ABS";
      public const string Acos = "ACOS";
      public const string All = "ALL";
      public const string Any = "ANY";
      public const string Array = "ARRAY";
      public const string ArrayConcat = "ARRAY_CONCAT";
      public const string ArrayContains = "ARRAY_CONTAINS";
      public const string ArrayLength = "ARRAY_LENGTH";
      public const string ArraySlice = "ARRAY_SLICE";
      public const string Asin = "ASIN";
      public const string Atan = "ATAN";
      public const string Atn2 = "ATN2";
      public const string Avg = "AVG";
      public const string Ceiling = "CEILING";
      public const string Concat = "CONCAT";
      public const string Contains = "CONTAINS";
      public const string Cos = "COS";
      public const string Cot = "COT";
      public const string Count = "COUNT";
      public const string Degrees = "DEGREES";
      public const string Documentid = "DOCUMENTID";
      public const string Endswith = "ENDSWITH";
      public const string Exp = "EXP";
      public const string Floor = "FLOOR";
      public const string IndexOf = "INDEX_OF";
      public const string IsArray = "IS_ARRAY";
      public const string IsBool = "IS_BOOL";
      public const string IsDefined = "IS_DEFINED";
      public const string IsFiniteNumber = "IS_FINITE_NUMBER";
      public const string IsNull = "IS_NULL";
      public const string IsNumber = "IS_NUMBER";
      public const string IsObject = "IS_OBJECT";
      public const string IsPrimitive = "IS_PRIMITIVE";
      public const string IsString = "IS_STRING";
      public const string Left = "LEFT";
      public const string Length = "LENGTH";
      public const string Like = "LIKE";
      public const string Log = "LOG";
      public const string Log10 = "LOG10";
      public const string Lower = "LOWER";
      public const string Ltrim = "LTRIM";
      public const string Max = "MAX";
      public const string Min = "MIN";
      public const string Pi = "PI";
      public const string Power = "POWER";
      public const string Radians = "RADIANS";
      public const string Rand = "RAND";
      public const string Replace = "REPLACE";
      public const string Replicate = "REPLICATE";
      public const string Reverse = "REVERSE";
      public const string Right = "RIGHT";
      public const string Round = "ROUND";
      public const string Rtrim = "RTRIM";
      public const string Sign = "SIGN";
      public const string Sin = "SIN";
      public const string Sqrt = "SQRT";
      public const string Square = "SQUARE";
      public const string Startswith = "STARTSWITH";
      public const string StDistance = "ST_DISTANCE";
      public const string StIntersects = "ST_INTERSECTS";
      public const string StIsvalid = "ST_ISVALID";
      public const string StIsvaliddetailed = "ST_ISVALIDDETAILED";
      public const string StWithin = "ST_WITHIN";
      public const string Substring = "SUBSTRING";
      public const string Sum = "SUM";
      public const string Tan = "TAN";
      public const string Trunc = "TRUNC";
      public const string Upper = "UPPER";
    }

    public static class Identifiers
    {
      public static readonly SqlIdentifier InternalCompareBsonBinaryData = SqlIdentifier.Create("_COMPARE_BSON_BINARYDATA");
      public static readonly SqlIdentifier InternalCompareObjects = SqlIdentifier.Create("_COMPARE_OBJECTS");
      public static readonly SqlIdentifier InternalObjectToArray = SqlIdentifier.Create("_ObjectToArray");
      public static readonly SqlIdentifier InternalProxyProjection = SqlIdentifier.Create("_PROXY_PROJECTION");
      public static readonly SqlIdentifier InternalRegexMatch = SqlIdentifier.Create("_REGEX_MATCH");
      public static readonly SqlIdentifier InternalStDistance = SqlIdentifier.Create("_ST_DISTANCE");
      public static readonly SqlIdentifier InternalStIntersects = SqlIdentifier.Create("_ST_INTERSECTS");
      public static readonly SqlIdentifier InternalStWithin = SqlIdentifier.Create("_ST_WITHIN");
      public static readonly SqlIdentifier InternalTryArrayContains = SqlIdentifier.Create("_TRY_ARRAY_CONTAINS");
      public static readonly SqlIdentifier Abs = SqlIdentifier.Create("ABS");
      public static readonly SqlIdentifier Acos = SqlIdentifier.Create("ACOS");
      public static readonly SqlIdentifier All = SqlIdentifier.Create("ALL");
      public static readonly SqlIdentifier Any = SqlIdentifier.Create("ANY");
      public static readonly SqlIdentifier Array = SqlIdentifier.Create("ARRAY");
      public static readonly SqlIdentifier ArrayConcat = SqlIdentifier.Create("ARRAY_CONCAT");
      public static readonly SqlIdentifier ArrayContains = SqlIdentifier.Create("ARRAY_CONTAINS");
      public static readonly SqlIdentifier ArrayLength = SqlIdentifier.Create("ARRAY_LENGTH");
      public static readonly SqlIdentifier ArraySlice = SqlIdentifier.Create("ARRAY_SLICE");
      public static readonly SqlIdentifier Asin = SqlIdentifier.Create("ASIN");
      public static readonly SqlIdentifier Atan = SqlIdentifier.Create("ATAN");
      public static readonly SqlIdentifier Atn2 = SqlIdentifier.Create("ATN2");
      public static readonly SqlIdentifier Avg = SqlIdentifier.Create("AVG");
      public static readonly SqlIdentifier Ceiling = SqlIdentifier.Create("CEILING");
      public static readonly SqlIdentifier Concat = SqlIdentifier.Create("CONCAT");
      public static readonly SqlIdentifier Contains = SqlIdentifier.Create("CONTAINS");
      public static readonly SqlIdentifier Cos = SqlIdentifier.Create("COS");
      public static readonly SqlIdentifier Cot = SqlIdentifier.Create("COT");
      public static readonly SqlIdentifier Count = SqlIdentifier.Create("COUNT");
      public static readonly SqlIdentifier Degrees = SqlIdentifier.Create("DEGREES");
      public static readonly SqlIdentifier Documentid = SqlIdentifier.Create("DOCUMENTID");
      public static readonly SqlIdentifier Endswith = SqlIdentifier.Create("ENDSWITH");
      public static readonly SqlIdentifier Exp = SqlIdentifier.Create("EXP");
      public static readonly SqlIdentifier Floor = SqlIdentifier.Create("FLOOR");
      public static readonly SqlIdentifier IndexOf = SqlIdentifier.Create("INDEX_OF");
      public static readonly SqlIdentifier IsArray = SqlIdentifier.Create("IS_ARRAY");
      public static readonly SqlIdentifier IsBool = SqlIdentifier.Create("IS_BOOL");
      public static readonly SqlIdentifier IsDefined = SqlIdentifier.Create("IS_DEFINED");
      public static readonly SqlIdentifier IsFiniteNumber = SqlIdentifier.Create("IS_FINITE_NUMBER");
      public static readonly SqlIdentifier IsNull = SqlIdentifier.Create("IS_NULL");
      public static readonly SqlIdentifier IsNumber = SqlIdentifier.Create("IS_NUMBER");
      public static readonly SqlIdentifier IsObject = SqlIdentifier.Create("IS_OBJECT");
      public static readonly SqlIdentifier IsPrimitive = SqlIdentifier.Create("IS_PRIMITIVE");
      public static readonly SqlIdentifier IsString = SqlIdentifier.Create("IS_STRING");
      public static readonly SqlIdentifier Left = SqlIdentifier.Create("LEFT");
      public static readonly SqlIdentifier Length = SqlIdentifier.Create("LENGTH");
      public static readonly SqlIdentifier Like = SqlIdentifier.Create("LIKE");
      public static readonly SqlIdentifier Log = SqlIdentifier.Create("LOG");
      public static readonly SqlIdentifier Log10 = SqlIdentifier.Create("LOG10");
      public static readonly SqlIdentifier Lower = SqlIdentifier.Create("LOWER");
      public static readonly SqlIdentifier Ltrim = SqlIdentifier.Create("LTRIM");
      public static readonly SqlIdentifier Max = SqlIdentifier.Create("MAX");
      public static readonly SqlIdentifier Min = SqlIdentifier.Create("MIN");
      public static readonly SqlIdentifier Pi = SqlIdentifier.Create("PI");
      public static readonly SqlIdentifier Power = SqlIdentifier.Create("POWER");
      public static readonly SqlIdentifier Radians = SqlIdentifier.Create("RADIANS");
      public static readonly SqlIdentifier Rand = SqlIdentifier.Create("RAND");
      public static readonly SqlIdentifier Replace = SqlIdentifier.Create("REPLACE");
      public static readonly SqlIdentifier Replicate = SqlIdentifier.Create("REPLICATE");
      public static readonly SqlIdentifier Reverse = SqlIdentifier.Create("REVERSE");
      public static readonly SqlIdentifier Right = SqlIdentifier.Create("RIGHT");
      public static readonly SqlIdentifier Round = SqlIdentifier.Create("ROUND");
      public static readonly SqlIdentifier Rtrim = SqlIdentifier.Create("RTRIM");
      public static readonly SqlIdentifier Sign = SqlIdentifier.Create("SIGN");
      public static readonly SqlIdentifier Sin = SqlIdentifier.Create("SIN");
      public static readonly SqlIdentifier Sqrt = SqlIdentifier.Create("SQRT");
      public static readonly SqlIdentifier Square = SqlIdentifier.Create("SQUARE");
      public static readonly SqlIdentifier Startswith = SqlIdentifier.Create("STARTSWITH");
      public static readonly SqlIdentifier StDistance = SqlIdentifier.Create("ST_DISTANCE");
      public static readonly SqlIdentifier StIntersects = SqlIdentifier.Create("ST_INTERSECTS");
      public static readonly SqlIdentifier StIsvalid = SqlIdentifier.Create("ST_ISVALID");
      public static readonly SqlIdentifier StIsvaliddetailed = SqlIdentifier.Create("ST_ISVALIDDETAILED");
      public static readonly SqlIdentifier StWithin = SqlIdentifier.Create("ST_WITHIN");
      public static readonly SqlIdentifier Substring = SqlIdentifier.Create("SUBSTRING");
      public static readonly SqlIdentifier Sum = SqlIdentifier.Create("SUM");
      public static readonly SqlIdentifier Tan = SqlIdentifier.Create("TAN");
      public static readonly SqlIdentifier Trunc = SqlIdentifier.Create("TRUNC");
      public static readonly SqlIdentifier Upper = SqlIdentifier.Create("UPPER");
    }
  }
}
