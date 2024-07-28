// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.JsonSharedUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.Json
{
  internal static class JsonSharedUtils
  {
    internal static bool IsDoubleValueSerializedAsString(double value) => double.IsInfinity(value) || double.IsNaN(value);

    internal static bool ValueTypeMatchesJsonType(
      ODataPrimitiveValue primitiveValue,
      IEdmPrimitiveTypeReference valueTypeReference)
    {
      return JsonSharedUtils.ValueTypeMatchesJsonType(primitiveValue, ExtensionMethods.PrimitiveKind(valueTypeReference));
    }

    internal static bool ValueTypeMatchesJsonType(
      ODataPrimitiveValue primitiveValue,
      EdmPrimitiveTypeKind primitiveTypeKind)
    {
      switch (primitiveTypeKind)
      {
        case EdmPrimitiveTypeKind.Boolean:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.String:
          return true;
        case EdmPrimitiveTypeKind.Double:
          return !JsonSharedUtils.IsDoubleValueSerializedAsString((double) primitiveValue.Value);
        default:
          return false;
      }
    }
  }
}
