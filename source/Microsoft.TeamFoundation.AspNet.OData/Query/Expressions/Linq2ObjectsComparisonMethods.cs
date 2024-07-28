// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.Linq2ObjectsComparisonMethods
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System.Reflection;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal static class Linq2ObjectsComparisonMethods
  {
    public static readonly MethodInfo AreByteArraysEqualMethodInfo = typeof (Linq2ObjectsComparisonMethods).GetMethod("AreByteArraysEqual");
    public static readonly MethodInfo AreByteArraysNotEqualMethodInfo = typeof (Linq2ObjectsComparisonMethods).GetMethod("AreByteArraysNotEqual");

    public static bool AreByteArraysEqual(byte[] left, byte[] right)
    {
      if (left == right)
        return true;
      if (left == null || right == null || left.Length != right.Length)
        return false;
      for (int index = 0; index < left.Length; ++index)
      {
        if ((int) left[index] != (int) right[index])
          return false;
      }
      return true;
    }

    public static bool AreByteArraysNotEqual(byte[] left, byte[] right) => !Linq2ObjectsComparisonMethods.AreByteArraysEqual(left, right);
  }
}
