// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlFunctionCallScalarExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlFunctionCallScalarExpression : SqlScalarExpression
  {
    private static readonly ImmutableDictionary<string, SqlIdentifier> FunctionIdentifiers = new Dictionary<string, SqlIdentifier>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
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
        "_M_EVAL_EQ",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalEq
      },
      {
        "_M_EVAL_GT",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalGt
      },
      {
        "_M_EVAL_GTE",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalGte
      },
      {
        "_M_EVAL_IN",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalIn
      },
      {
        "_M_EVAL_LT",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalLt
      },
      {
        "_M_EVAL_LTE",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalLte
      },
      {
        "_M_EVAL_NEQ",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalNeq
      },
      {
        "_M_EVAL_NIN",
        SqlFunctionCallScalarExpression.Identifiers.InternalEvalNin
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
        "C_BINARY",
        SqlFunctionCallScalarExpression.Identifiers.Binary
      },
      {
        "C_FLOAT32",
        SqlFunctionCallScalarExpression.Identifiers.Float32
      },
      {
        "C_FLOAT64",
        SqlFunctionCallScalarExpression.Identifiers.Float64
      },
      {
        "C_GUID",
        SqlFunctionCallScalarExpression.Identifiers.Guid
      },
      {
        "C_INT16",
        SqlFunctionCallScalarExpression.Identifiers.Int16
      },
      {
        "C_INT32",
        SqlFunctionCallScalarExpression.Identifiers.Int32
      },
      {
        "C_INT64",
        SqlFunctionCallScalarExpression.Identifiers.Int64
      },
      {
        "C_INT8",
        SqlFunctionCallScalarExpression.Identifiers.Int8
      },
      {
        "C_LIST",
        SqlFunctionCallScalarExpression.Identifiers.List
      },
      {
        "C_LISTCONTAINS",
        SqlFunctionCallScalarExpression.Identifiers.ListContains
      },
      {
        "C_MAP",
        SqlFunctionCallScalarExpression.Identifiers.Map
      },
      {
        "C_MAPCONTAINS",
        SqlFunctionCallScalarExpression.Identifiers.MapContains
      },
      {
        "C_MAPCONTAINSKEY",
        SqlFunctionCallScalarExpression.Identifiers.MapContainsKey
      },
      {
        "C_MAPCONTAINSVALUE",
        SqlFunctionCallScalarExpression.Identifiers.MapContainsValue
      },
      {
        "C_SET",
        SqlFunctionCallScalarExpression.Identifiers.Set
      },
      {
        "C_SETCONTAINS",
        SqlFunctionCallScalarExpression.Identifiers.SetContains
      },
      {
        "C_TUPLE",
        SqlFunctionCallScalarExpression.Identifiers.Tuple
      },
      {
        "C_UDT",
        SqlFunctionCallScalarExpression.Identifiers.Udt
      },
      {
        "C_UINT32",
        SqlFunctionCallScalarExpression.Identifiers.UInt32
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
        "DateTimeAdd",
        SqlFunctionCallScalarExpression.Identifiers.DateTimeAdd
      },
      {
        "DateTimeDiff",
        SqlFunctionCallScalarExpression.Identifiers.DateTimeDiff
      },
      {
        "DateTimeFromParts",
        SqlFunctionCallScalarExpression.Identifiers.DateTimeFromParts
      },
      {
        "DateTimePart",
        SqlFunctionCallScalarExpression.Identifiers.DateTimePart
      },
      {
        "DateTimeToTicks",
        SqlFunctionCallScalarExpression.Identifiers.DateTimeToTicks
      },
      {
        "DateTimeToTimestamp",
        SqlFunctionCallScalarExpression.Identifiers.DateTimeToTimestamp
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
        "GetCurrentDateTime",
        SqlFunctionCallScalarExpression.Identifiers.GetCurrentDateTime
      },
      {
        "GetCurrentTicks",
        SqlFunctionCallScalarExpression.Identifiers.GetCurrentTicks
      },
      {
        "GetCurrentTimestamp",
        SqlFunctionCallScalarExpression.Identifiers.GetCurrentTimestamp
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
        "ObjectToArray",
        SqlFunctionCallScalarExpression.Identifiers.ObjectToArray
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
        "StringEquals",
        SqlFunctionCallScalarExpression.Identifiers.StringEquals
      },
      {
        "StringToArray",
        SqlFunctionCallScalarExpression.Identifiers.StringToArray
      },
      {
        "StringToBoolean",
        SqlFunctionCallScalarExpression.Identifiers.StringToBoolean
      },
      {
        "StringToNull",
        SqlFunctionCallScalarExpression.Identifiers.StringToNull
      },
      {
        "StringToNumber",
        SqlFunctionCallScalarExpression.Identifiers.StringToNumber
      },
      {
        "StringToObject",
        SqlFunctionCallScalarExpression.Identifiers.StringToObject
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
        "TicksToDateTime",
        SqlFunctionCallScalarExpression.Identifiers.TicksToDateTime
      },
      {
        "TimestampToDateTime",
        SqlFunctionCallScalarExpression.Identifiers.TimestampToDateTime
      },
      {
        "ToString",
        SqlFunctionCallScalarExpression.Identifiers.ToString
      },
      {
        "TRIM",
        SqlFunctionCallScalarExpression.Identifiers.Trim
      },
      {
        "TRUNC",
        SqlFunctionCallScalarExpression.Identifiers.Trunc
      },
      {
        "UPPER",
        SqlFunctionCallScalarExpression.Identifiers.Upper
      }
    }.ToImmutableDictionary<string, SqlIdentifier>();

    private SqlFunctionCallScalarExpression(
      SqlIdentifier name,
      bool isUdf,
      ImmutableArray<SqlScalarExpression> arguments)
    {
      foreach (SqlObject sqlObject in arguments)
      {
        if (sqlObject == (SqlObject) null)
          throw new ArgumentNullException("arguments must not have null items.");
      }
      this.Arguments = arguments;
      this.Name = name ?? throw new ArgumentNullException(nameof (name));
      this.IsUdf = isUdf;
    }

    public SqlIdentifier Name { get; }

    public ImmutableArray<SqlScalarExpression> Arguments { get; }

    public bool IsUdf { get; }

    public static SqlFunctionCallScalarExpression Create(
      SqlIdentifier name,
      bool isUdf,
      params SqlScalarExpression[] arguments)
    {
      return new SqlFunctionCallScalarExpression(name, isUdf, ((IEnumerable<SqlScalarExpression>) arguments).ToImmutableArray<SqlScalarExpression>());
    }

    public static SqlFunctionCallScalarExpression Create(
      SqlIdentifier name,
      bool isUdf,
      ImmutableArray<SqlScalarExpression> arguments)
    {
      return new SqlFunctionCallScalarExpression(name, isUdf, arguments);
    }

    public static SqlFunctionCallScalarExpression Create(
      string name,
      bool isUdf,
      params SqlScalarExpression[] arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, isUdf, ((IEnumerable<SqlScalarExpression>) arguments).ToImmutableArray<SqlScalarExpression>());
    }

    public static SqlFunctionCallScalarExpression Create(
      string name,
      bool isUdf,
      ImmutableArray<SqlScalarExpression> arguments)
    {
      SqlIdentifier name1;
      if (!SqlFunctionCallScalarExpression.FunctionIdentifiers.TryGetValue(name, out name1))
        name1 = SqlIdentifier.Create(name);
      return SqlFunctionCallScalarExpression.Create(name1, isUdf, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      string name,
      params SqlScalarExpression[] arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, false, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      string name,
      ImmutableArray<SqlScalarExpression> arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, false, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      SqlIdentifier name,
      params SqlScalarExpression[] arguments)
    {
      return SqlFunctionCallScalarExpression.Create(name, false, arguments);
    }

    public static SqlFunctionCallScalarExpression CreateBuiltin(
      SqlIdentifier name,
      ImmutableArray<SqlScalarExpression> arguments)
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
      public const string InternalEvalEq = "_M_EVAL_EQ";
      public const string InternalEvalGt = "_M_EVAL_GT";
      public const string InternalEvalGte = "_M_EVAL_GTE";
      public const string InternalEvalIn = "_M_EVAL_IN";
      public const string InternalEvalLt = "_M_EVAL_LT";
      public const string InternalEvalLte = "_M_EVAL_LTE";
      public const string InternalEvalNeq = "_M_EVAL_NEQ";
      public const string InternalEvalNin = "_M_EVAL_NIN";
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
      public const string Binary = "C_BINARY";
      public const string Float32 = "C_FLOAT32";
      public const string Float64 = "C_FLOAT64";
      public const string Guid = "C_GUID";
      public const string Int16 = "C_INT16";
      public const string Int32 = "C_INT32";
      public const string Int64 = "C_INT64";
      public const string Int8 = "C_INT8";
      public const string List = "C_LIST";
      public const string ListContains = "C_LISTCONTAINS";
      public const string Map = "C_MAP";
      public const string MapContains = "C_MAPCONTAINS";
      public const string MapContainsKey = "C_MAPCONTAINSKEY";
      public const string MapContainsValue = "C_MAPCONTAINSVALUE";
      public const string Set = "C_SET";
      public const string SetContains = "C_SETCONTAINS";
      public const string Tuple = "C_TUPLE";
      public const string Udt = "C_UDT";
      public const string UInt32 = "C_UINT32";
      public const string Ceiling = "CEILING";
      public const string Concat = "CONCAT";
      public const string Contains = "CONTAINS";
      public const string Cos = "COS";
      public const string Cot = "COT";
      public const string Count = "COUNT";
      public const string DateTimeAdd = "DateTimeAdd";
      public const string DateTimeDiff = "DateTimeDiff";
      public const string DateTimeFromParts = "DateTimeFromParts";
      public const string DateTimePart = "DateTimePart";
      public const string DateTimeToTicks = "DateTimeToTicks";
      public const string DateTimeToTimestamp = "DateTimeToTimestamp";
      public const string Degrees = "DEGREES";
      public const string Documentid = "DOCUMENTID";
      public const string Endswith = "ENDSWITH";
      public const string Exp = "EXP";
      public const string Floor = "FLOOR";
      public const string GetCurrentDateTime = "GetCurrentDateTime";
      public const string GetCurrentTicks = "GetCurrentTicks";
      public const string GetCurrentTimestamp = "GetCurrentTimestamp";
      public const string IndexOf = "INDEX_OF";
      public const string IntAdd = "IntAdd";
      public const string IntBitwiseAnd = "IntBitwiseAnd";
      public const string IntBitwiseLeftShift = "IntBitwiseLeftShift";
      public const string IntBitwiseNot = "IntBitwiseNot";
      public const string IntBitwiseOr = "IntBitwiseOr";
      public const string IntBitwiseRightShift = "IntBitwiseRightShift";
      public const string IntBitwiseXor = "IntBitwiseXor";
      public const string IntDiv = "IntDiv";
      public const string IntMod = "IntMod";
      public const string IntMul = "IntMul";
      public const string IntSub = "IntSub";
      public const string IsArray = "IS_ARRAY";
      public const string IsBool = "IS_BOOL";
      public const string IsDefined = "IS_DEFINED";
      public const string IsFiniteNumber = "IS_FINITE_NUMBER";
      public const string IsInteger = "IS_INTEGER";
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
      public const string ObjectToArray = "ObjectToArray";
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
      public const string StringEquals = "StringEquals";
      public const string StringToArray = "StringToArray";
      public const string StringToBoolean = "StringToBoolean";
      public const string StringToNull = "StringToNull";
      public const string StringToNumber = "StringToNumber";
      public const string StringToObject = "StringToObject";
      public const string Substring = "SUBSTRING";
      public const string Sum = "SUM";
      public const string Tan = "TAN";
      public const string TicksToDateTime = "TicksToDateTime";
      public const string TimestampToDateTime = "TimestampToDateTime";
      public const string ToString = "ToString";
      public const string Trim = "TRIM";
      public const string Trunc = "TRUNC";
      public const string Upper = "UPPER";
    }

    public static class Identifiers
    {
      public static readonly SqlIdentifier InternalCompareBsonBinaryData = SqlIdentifier.Create("_COMPARE_BSON_BINARYDATA");
      public static readonly SqlIdentifier InternalCompareObjects = SqlIdentifier.Create("_COMPARE_OBJECTS");
      public static readonly SqlIdentifier InternalEvalEq = SqlIdentifier.Create("_M_EVAL_EQ");
      public static readonly SqlIdentifier InternalEvalGt = SqlIdentifier.Create("_M_EVAL_GT");
      public static readonly SqlIdentifier InternalEvalGte = SqlIdentifier.Create("_M_EVAL_GTE");
      public static readonly SqlIdentifier InternalEvalIn = SqlIdentifier.Create("_M_EVAL_IN");
      public static readonly SqlIdentifier InternalEvalLt = SqlIdentifier.Create("_M_EVAL_LT");
      public static readonly SqlIdentifier InternalEvalLte = SqlIdentifier.Create("_M_EVAL_LTE");
      public static readonly SqlIdentifier InternalEvalNeq = SqlIdentifier.Create("_M_EVAL_NEQ");
      public static readonly SqlIdentifier InternalEvalNin = SqlIdentifier.Create("_M_EVAL_NIN");
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
      public static readonly SqlIdentifier Binary = SqlIdentifier.Create("C_BINARY");
      public static readonly SqlIdentifier Float32 = SqlIdentifier.Create("C_FLOAT32");
      public static readonly SqlIdentifier Float64 = SqlIdentifier.Create("C_FLOAT64");
      public static readonly SqlIdentifier Guid = SqlIdentifier.Create("C_GUID");
      public static readonly SqlIdentifier Int16 = SqlIdentifier.Create("C_INT16");
      public static readonly SqlIdentifier Int32 = SqlIdentifier.Create("C_INT32");
      public static readonly SqlIdentifier Int64 = SqlIdentifier.Create("C_INT64");
      public static readonly SqlIdentifier Int8 = SqlIdentifier.Create("C_INT8");
      public static readonly SqlIdentifier List = SqlIdentifier.Create("C_LIST");
      public static readonly SqlIdentifier ListContains = SqlIdentifier.Create("C_LISTCONTAINS");
      public static readonly SqlIdentifier Map = SqlIdentifier.Create("C_MAP");
      public static readonly SqlIdentifier MapContains = SqlIdentifier.Create("C_MAPCONTAINS");
      public static readonly SqlIdentifier MapContainsKey = SqlIdentifier.Create("C_MAPCONTAINSKEY");
      public static readonly SqlIdentifier MapContainsValue = SqlIdentifier.Create("C_MAPCONTAINSVALUE");
      public static readonly SqlIdentifier Set = SqlIdentifier.Create("C_SET");
      public static readonly SqlIdentifier SetContains = SqlIdentifier.Create("C_SETCONTAINS");
      public static readonly SqlIdentifier Tuple = SqlIdentifier.Create("C_TUPLE");
      public static readonly SqlIdentifier Udt = SqlIdentifier.Create("C_UDT");
      public static readonly SqlIdentifier UInt32 = SqlIdentifier.Create("C_UINT32");
      public static readonly SqlIdentifier Ceiling = SqlIdentifier.Create("CEILING");
      public static readonly SqlIdentifier Concat = SqlIdentifier.Create("CONCAT");
      public static readonly SqlIdentifier Contains = SqlIdentifier.Create("CONTAINS");
      public static readonly SqlIdentifier Cos = SqlIdentifier.Create("COS");
      public static readonly SqlIdentifier Cot = SqlIdentifier.Create("COT");
      public static readonly SqlIdentifier Count = SqlIdentifier.Create("COUNT");
      public static readonly SqlIdentifier DateTimeAdd = SqlIdentifier.Create(nameof (DateTimeAdd));
      public static readonly SqlIdentifier DateTimeDiff = SqlIdentifier.Create(nameof (DateTimeDiff));
      public static readonly SqlIdentifier DateTimeFromParts = SqlIdentifier.Create(nameof (DateTimeFromParts));
      public static readonly SqlIdentifier DateTimePart = SqlIdentifier.Create(nameof (DateTimePart));
      public static readonly SqlIdentifier DateTimeToTicks = SqlIdentifier.Create(nameof (DateTimeToTicks));
      public static readonly SqlIdentifier DateTimeToTimestamp = SqlIdentifier.Create(nameof (DateTimeToTimestamp));
      public static readonly SqlIdentifier Degrees = SqlIdentifier.Create("DEGREES");
      public static readonly SqlIdentifier Documentid = SqlIdentifier.Create("DOCUMENTID");
      public static readonly SqlIdentifier Endswith = SqlIdentifier.Create("ENDSWITH");
      public static readonly SqlIdentifier Exp = SqlIdentifier.Create("EXP");
      public static readonly SqlIdentifier Floor = SqlIdentifier.Create("FLOOR");
      public static readonly SqlIdentifier GetCurrentDateTime = SqlIdentifier.Create(nameof (GetCurrentDateTime));
      public static readonly SqlIdentifier GetCurrentTicks = SqlIdentifier.Create(nameof (GetCurrentTicks));
      public static readonly SqlIdentifier GetCurrentTimestamp = SqlIdentifier.Create(nameof (GetCurrentTimestamp));
      public static readonly SqlIdentifier IndexOf = SqlIdentifier.Create("INDEX_OF");
      public static readonly SqlIdentifier IntAdd = SqlIdentifier.Create(nameof (IntAdd));
      public static readonly SqlIdentifier IntBitwiseAnd = SqlIdentifier.Create(nameof (IntBitwiseAnd));
      public static readonly SqlIdentifier IntBitwiseLeftShift = SqlIdentifier.Create(nameof (IntBitwiseLeftShift));
      public static readonly SqlIdentifier IntBitwiseNot = SqlIdentifier.Create(nameof (IntBitwiseNot));
      public static readonly SqlIdentifier IntBitwiseOr = SqlIdentifier.Create(nameof (IntBitwiseOr));
      public static readonly SqlIdentifier IntBitwiseRightShift = SqlIdentifier.Create(nameof (IntBitwiseRightShift));
      public static readonly SqlIdentifier IntBitwiseXor = SqlIdentifier.Create(nameof (IntBitwiseXor));
      public static readonly SqlIdentifier IntDiv = SqlIdentifier.Create(nameof (IntDiv));
      public static readonly SqlIdentifier IntMod = SqlIdentifier.Create(nameof (IntMod));
      public static readonly SqlIdentifier IntMul = SqlIdentifier.Create(nameof (IntMul));
      public static readonly SqlIdentifier IntSub = SqlIdentifier.Create(nameof (IntSub));
      public static readonly SqlIdentifier IsArray = SqlIdentifier.Create("IS_ARRAY");
      public static readonly SqlIdentifier IsBool = SqlIdentifier.Create("IS_BOOL");
      public static readonly SqlIdentifier IsDefined = SqlIdentifier.Create("IS_DEFINED");
      public static readonly SqlIdentifier IsFiniteNumber = SqlIdentifier.Create("IS_FINITE_NUMBER");
      public static readonly SqlIdentifier IsInteger = SqlIdentifier.Create("IS_INTEGER");
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
      public static readonly SqlIdentifier ObjectToArray = SqlIdentifier.Create(nameof (ObjectToArray));
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
      public static readonly SqlIdentifier StringEquals = SqlIdentifier.Create(nameof (StringEquals));
      public static readonly SqlIdentifier StringToArray = SqlIdentifier.Create(nameof (StringToArray));
      public static readonly SqlIdentifier StringToBoolean = SqlIdentifier.Create(nameof (StringToBoolean));
      public static readonly SqlIdentifier StringToNull = SqlIdentifier.Create(nameof (StringToNull));
      public static readonly SqlIdentifier StringToNumber = SqlIdentifier.Create(nameof (StringToNumber));
      public static readonly SqlIdentifier StringToObject = SqlIdentifier.Create(nameof (StringToObject));
      public static readonly SqlIdentifier Substring = SqlIdentifier.Create("SUBSTRING");
      public static readonly SqlIdentifier Sum = SqlIdentifier.Create("SUM");
      public static readonly SqlIdentifier Tan = SqlIdentifier.Create("TAN");
      public static readonly SqlIdentifier TicksToDateTime = SqlIdentifier.Create(nameof (TicksToDateTime));
      public static readonly SqlIdentifier TimestampToDateTime = SqlIdentifier.Create(nameof (TimestampToDateTime));
      public static readonly SqlIdentifier ToString = SqlIdentifier.Create(nameof (ToString));
      public static readonly SqlIdentifier Trim = SqlIdentifier.Create("TRIM");
      public static readonly SqlIdentifier Trunc = SqlIdentifier.Create("TRUNC");
      public static readonly SqlIdentifier Upper = SqlIdentifier.Create("UPPER");
    }
  }
}
