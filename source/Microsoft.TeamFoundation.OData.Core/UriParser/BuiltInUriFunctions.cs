// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BuiltInUriFunctions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class BuiltInUriFunctions
  {
    private static readonly Dictionary<string, FunctionSignatureWithReturnType[]> builtInFunctions = BuiltInUriFunctions.InitializeBuiltInFunctions();

    internal static bool TryGetBuiltInFunction(
      string name,
      out FunctionSignatureWithReturnType[] signatures)
    {
      return BuiltInUriFunctions.builtInFunctions.TryGetValue(name, out signatures);
    }

    internal static void CreateSpatialFunctions(
      IDictionary<string, FunctionSignatureWithReturnType[]> functions)
    {
      FunctionSignatureWithReturnType[] signatureWithReturnTypeArray1 = new FunctionSignatureWithReturnType[2]
      {
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDouble(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDouble(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true)
        })
      };
      functions.Add("geo.distance", signatureWithReturnTypeArray1);
      FunctionSignatureWithReturnType[] signatureWithReturnTypeArray2 = new FunctionSignatureWithReturnType[4]
      {
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, true)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true)
        })
      };
      functions.Add("geo.intersects", signatureWithReturnTypeArray2);
      FunctionSignatureWithReturnType[] signatureWithReturnTypeArray3 = new FunctionSignatureWithReturnType[2]
      {
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDouble(true), new IEdmTypeReference[1]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, true)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDouble(true), new IEdmTypeReference[1]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, true)
        })
      };
      functions.Add("geo.length", signatureWithReturnTypeArray3);
    }

    private static Dictionary<string, FunctionSignatureWithReturnType[]> InitializeBuiltInFunctions()
    {
      Dictionary<string, FunctionSignatureWithReturnType[]> functions = new Dictionary<string, FunctionSignatureWithReturnType[]>((IEqualityComparer<string>) StringComparer.Ordinal);
      BuiltInUriFunctions.CreateStringFunctions((IDictionary<string, FunctionSignatureWithReturnType[]>) functions);
      BuiltInUriFunctions.CreateSpatialFunctions((IDictionary<string, FunctionSignatureWithReturnType[]>) functions);
      BuiltInUriFunctions.CreateDateTimeFunctions((IDictionary<string, FunctionSignatureWithReturnType[]>) functions);
      BuiltInUriFunctions.CreateMathFunctions((IDictionary<string, FunctionSignatureWithReturnType[]>) functions);
      BuiltInUriFunctions.CreateLogicFunctions((IDictionary<string, FunctionSignatureWithReturnType[]>) functions);
      return functions;
    }

    private static void CreateStringFunctions(
      IDictionary<string, FunctionSignatureWithReturnType[]> functions)
    {
      FunctionSignatureWithReturnType signatureWithReturnType1 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(false), new IEdmTypeReference[2]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("endswith", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType1
      });
      FunctionSignatureWithReturnType signatureWithReturnType2 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[2]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("indexof", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType2
      });
      FunctionSignatureWithReturnType signatureWithReturnType3 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[3]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("replace", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType3
      });
      FunctionSignatureWithReturnType signatureWithReturnType4 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(false), new IEdmTypeReference[2]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("startswith", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType4
      });
      FunctionSignatureWithReturnType signatureWithReturnType5 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("tolower", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType5
      });
      FunctionSignatureWithReturnType signatureWithReturnType6 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("toupper", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType6
      });
      FunctionSignatureWithReturnType signatureWithReturnType7 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("trim", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType7
      });
      FunctionSignatureWithReturnType[] signatureWithReturnTypeArray = new FunctionSignatureWithReturnType[6]
      {
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[2]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(true)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[3]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[3]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[3]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(true)
        }),
        new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[3]
        {
          (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(true),
          (IEdmTypeReference) EdmCoreModel.Instance.GetInt32(true)
        })
      };
      functions.Add("substring", signatureWithReturnTypeArray);
      FunctionSignatureWithReturnType signatureWithReturnType8 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(false), new IEdmTypeReference[2]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("contains", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType8
      });
      FunctionSignatureWithReturnType signatureWithReturnType9 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetString(true), new IEdmTypeReference[2]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true),
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("concat", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType9
      });
      FunctionSignatureWithReturnType signatureWithReturnType10 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetString(true)
      });
      functions.Add("length", new FunctionSignatureWithReturnType[1]
      {
        signatureWithReturnType10
      });
    }

    private static void CreateDateTimeFunctions(
      IDictionary<string, FunctionSignatureWithReturnType[]> functions)
    {
      FunctionSignatureWithReturnType[] functionSignatureArray = BuiltInUriFunctions.CreateDateTimeFunctionSignatureArray();
      FunctionSignatureWithReturnType[] offsetReturnDate = BuiltInUriFunctions.CreateDateTimeOffsetReturnDate();
      FunctionSignatureWithReturnType[] offsetReturnDecimal = BuiltInUriFunctions.CreateDateTimeOffsetReturnDecimal();
      FunctionSignatureWithReturnType[] offsetReturnTimeOfDay = BuiltInUriFunctions.CreateDateTimeOffsetReturnTimeOfDay();
      FunctionSignatureWithReturnType[] array1 = ((IEnumerable<FunctionSignatureWithReturnType>) functionSignatureArray).Concat<FunctionSignatureWithReturnType>(BuiltInUriFunctions.CreateDurationFunctionSignatures()).ToArray<FunctionSignatureWithReturnType>();
      FunctionSignatureWithReturnType[] returnDateTimeOffset = BuiltInUriFunctions.CreateVoidReturnDateTimeOffset();
      FunctionSignatureWithReturnType[] durationReturnDecimal = BuiltInUriFunctions.CreateDurationReturnDecimal();
      FunctionSignatureWithReturnType[] dateReturnInt = BuiltInUriFunctions.CreateDateReturnInt();
      FunctionSignatureWithReturnType[] timeOfDayReturnInt = BuiltInUriFunctions.CreateTimeOfDayReturnInt();
      FunctionSignatureWithReturnType[] dayReturnDecimal = BuiltInUriFunctions.CreateTimeOfDayReturnDecimal();
      FunctionSignatureWithReturnType[] array2 = ((IEnumerable<FunctionSignatureWithReturnType>) functionSignatureArray).Concat<FunctionSignatureWithReturnType>((IEnumerable<FunctionSignatureWithReturnType>) dateReturnInt).ToArray<FunctionSignatureWithReturnType>();
      FunctionSignatureWithReturnType[] array3 = ((IEnumerable<FunctionSignatureWithReturnType>) array1).Concat<FunctionSignatureWithReturnType>((IEnumerable<FunctionSignatureWithReturnType>) timeOfDayReturnInt).ToArray<FunctionSignatureWithReturnType>();
      FunctionSignatureWithReturnType[] array4 = ((IEnumerable<FunctionSignatureWithReturnType>) offsetReturnDecimal).Concat<FunctionSignatureWithReturnType>((IEnumerable<FunctionSignatureWithReturnType>) dayReturnDecimal).ToArray<FunctionSignatureWithReturnType>();
      functions.Add("year", array2);
      functions.Add("month", array2);
      functions.Add("day", array2);
      functions.Add("hour", array3);
      functions.Add("minute", array3);
      functions.Add("second", array3);
      functions.Add("fractionalseconds", array4);
      functions.Add("totaloffsetminutes", functionSignatureArray);
      functions.Add("now", returnDateTimeOffset);
      functions.Add("maxdatetime", returnDateTimeOffset);
      functions.Add("mindatetime", returnDateTimeOffset);
      functions.Add("totalseconds", durationReturnDecimal);
      functions.Add("date", offsetReturnDate);
      functions.Add("time", offsetReturnTimeOfDay);
    }

    private static FunctionSignatureWithReturnType[] CreateDateTimeFunctionSignatureArray() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)
      })
    };

    private static IEnumerable<FunctionSignatureWithReturnType> CreateDurationFunctionSignatures()
    {
      yield return new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false)
      });
      yield return new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, true)
      });
    }

    private static FunctionSignatureWithReturnType[] CreateVoidReturnDateTimeOffset() => new FunctionSignatureWithReturnType[1]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false), new IEdmTypeReference[0])
    };

    private static FunctionSignatureWithReturnType[] CreateDateTimeOffsetReturnDate() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDate(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDate(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)
      })
    };

    private static FunctionSignatureWithReturnType[] CreateDateTimeOffsetReturnDecimal() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)
      })
    };

    private static FunctionSignatureWithReturnType[] CreateDateTimeOffsetReturnTimeOfDay() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)
      })
    };

    private static FunctionSignatureWithReturnType[] CreateDurationReturnDecimal() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, true)
      })
    };

    private static FunctionSignatureWithReturnType[] CreateDateReturnInt() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetDate(false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetDate(true)
      })
    };

    private static FunctionSignatureWithReturnType[] CreateTimeOfDayReturnInt() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetInt32(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, true)
      })
    };

    private static FunctionSignatureWithReturnType[] CreateTimeOfDayReturnDecimal() => new FunctionSignatureWithReturnType[2]
    {
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false)
      }),
      new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, true)
      })
    };

    private static void CreateMathFunctions(
      IDictionary<string, FunctionSignatureWithReturnType[]> functions)
    {
      functions.Add("round", BuiltInUriFunctions.CreateMathFunctionSignatureArray());
      functions.Add("floor", BuiltInUriFunctions.CreateMathFunctionSignatureArray());
      functions.Add("ceiling", BuiltInUriFunctions.CreateMathFunctionSignatureArray());
    }

    private static FunctionSignatureWithReturnType[] CreateMathFunctionSignatureArray()
    {
      FunctionSignatureWithReturnType signatureWithReturnType1 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDouble(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetDouble(false)
      });
      FunctionSignatureWithReturnType signatureWithReturnType2 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDouble(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetDouble(true)
      });
      FunctionSignatureWithReturnType signatureWithReturnType3 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false)
      });
      FunctionSignatureWithReturnType signatureWithReturnType4 = new FunctionSignatureWithReturnType((IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(false), new IEdmTypeReference[1]
      {
        (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(true)
      });
      return new FunctionSignatureWithReturnType[4]
      {
        signatureWithReturnType1,
        signatureWithReturnType3,
        signatureWithReturnType2,
        signatureWithReturnType4
      };
    }

    private static void CreateLogicFunctions(
      IDictionary<string, FunctionSignatureWithReturnType[]> functions)
    {
      functions.Add("iif", BuiltInUriFunctions.CreateLogicFunctionSignatureArray());
    }

    private static FunctionSignatureWithReturnType[] CreateLogicFunctionSignatureArray()
    {
      IEnumerable<EdmPrimitiveTypeKind> primitiveTypeKinds = Enum.GetValues(typeof (EdmPrimitiveTypeKind)).Cast<EdmPrimitiveTypeKind>().Where<EdmPrimitiveTypeKind>((Func<EdmPrimitiveTypeKind, bool>) (k => k != EdmPrimitiveTypeKind.None && k != EdmPrimitiveTypeKind.Stream));
      List<FunctionSignatureWithReturnType> signatureWithReturnTypeList = new List<FunctionSignatureWithReturnType>();
      foreach (EdmPrimitiveTypeKind kind in primitiveTypeKinds)
      {
        IEdmTypeReference primitiveTypeReference1 = BuiltInUriFunctions.GetPrimitiveTypeReference(kind, false);
        IEdmTypeReference primitiveTypeReference2 = BuiltInUriFunctions.GetPrimitiveTypeReference(kind, true);
        IEdmPrimitiveTypeReference boolean1 = EdmCoreModel.Instance.GetBoolean(false);
        IEdmPrimitiveTypeReference boolean2 = EdmCoreModel.Instance.GetBoolean(true);
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference1, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean1,
          primitiveTypeReference1,
          primitiveTypeReference1
        }));
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference2, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean1,
          primitiveTypeReference1,
          primitiveTypeReference2
        }));
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference2, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean1,
          primitiveTypeReference2,
          primitiveTypeReference1
        }));
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference2, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean1,
          primitiveTypeReference2,
          primitiveTypeReference2
        }));
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference1, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean2,
          primitiveTypeReference1,
          primitiveTypeReference1
        }));
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference2, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean2,
          primitiveTypeReference1,
          primitiveTypeReference2
        }));
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference2, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean2,
          primitiveTypeReference2,
          primitiveTypeReference1
        }));
        signatureWithReturnTypeList.Add(new FunctionSignatureWithReturnType(primitiveTypeReference2, new IEdmTypeReference[3]
        {
          (IEdmTypeReference) boolean2,
          primitiveTypeReference2,
          primitiveTypeReference2
        }));
      }
      return signatureWithReturnTypeList.ToArray();
    }

    private static IEdmTypeReference GetPrimitiveTypeReference(
      EdmPrimitiveTypeKind kind,
      bool isNullable)
    {
      return kind != EdmPrimitiveTypeKind.Decimal ? (IEdmTypeReference) new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(kind), isNullable) : (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(isNullable);
    }
  }
}
