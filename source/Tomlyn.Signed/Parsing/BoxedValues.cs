// Decompiled with JetBrains decompiler
// Type: Tomlyn.Parsing.BoxedValues
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Parsing
{
  internal static class BoxedValues
  {
    public static readonly object True = (object) true;
    public static readonly object False = (object) false;
    public static readonly object IntegerZero = (object) 0L;
    public static readonly object IntegerOne = (object) 1L;
    public static readonly object FloatZero = (object) 0.0;
    public static readonly object FloatOne = (object) 1.0;
    public static readonly object FloatPositiveInfinity = (object) double.PositiveInfinity;
    public static readonly object FloatNegativeInfinity = (object) double.NegativeInfinity;
    public static readonly object FloatNan = (object) BitConverter.Int64BitsToDouble(-2251799813685248L);
    public static readonly object FloatPositiveNaN = (object) BitConverter.Int64BitsToDouble(9221120237041090560L);
    public static readonly object FloatNegativeNaN = BoxedValues.FloatNan;
  }
}
