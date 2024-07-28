// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.Strings
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

namespace Microsoft.Spatial
{
  internal static class Strings
  {
    internal static string SpatialImplementation_NoRegisteredOperations => TextRes.GetString(nameof (SpatialImplementation_NoRegisteredOperations));

    internal static string InvalidPointCoordinate(object p0, object p1) => TextRes.GetString(nameof (InvalidPointCoordinate), p0, p1);

    internal static string Point_AccessCoordinateWhenEmpty => TextRes.GetString(nameof (Point_AccessCoordinateWhenEmpty));

    internal static string SpatialBuilder_CannotCreateBeforeDrawn => TextRes.GetString(nameof (SpatialBuilder_CannotCreateBeforeDrawn));

    internal static string GmlReader_UnexpectedElement(object p0) => TextRes.GetString(nameof (GmlReader_UnexpectedElement), p0);

    internal static string GmlReader_ExpectReaderAtElement => TextRes.GetString(nameof (GmlReader_ExpectReaderAtElement));

    internal static string GmlReader_InvalidSpatialType(object p0) => TextRes.GetString(nameof (GmlReader_InvalidSpatialType), p0);

    internal static string GmlReader_EmptyRingsNotAllowed => TextRes.GetString(nameof (GmlReader_EmptyRingsNotAllowed));

    internal static string GmlReader_PosNeedTwoNumbers => TextRes.GetString(nameof (GmlReader_PosNeedTwoNumbers));

    internal static string GmlReader_PosListNeedsEvenCount => TextRes.GetString(nameof (GmlReader_PosListNeedsEvenCount));

    internal static string GmlReader_InvalidSrsName(object p0) => TextRes.GetString(nameof (GmlReader_InvalidSrsName), p0);

    internal static string GmlReader_InvalidAttribute(object p0, object p1) => TextRes.GetString(nameof (GmlReader_InvalidAttribute), p0, p1);

    internal static string WellKnownText_UnexpectedToken(object p0, object p1, object p2) => TextRes.GetString(nameof (WellKnownText_UnexpectedToken), p0, p1, p2);

    internal static string WellKnownText_UnexpectedCharacter(object p0) => TextRes.GetString(nameof (WellKnownText_UnexpectedCharacter), p0);

    internal static string WellKnownText_UnknownTaggedText(object p0) => TextRes.GetString(nameof (WellKnownText_UnknownTaggedText), p0);

    internal static string WellKnownText_TooManyDimensions => TextRes.GetString(nameof (WellKnownText_TooManyDimensions));

    internal static string Validator_SridMismatch => TextRes.GetString(nameof (Validator_SridMismatch));

    internal static string Validator_InvalidType(object p0) => TextRes.GetString(nameof (Validator_InvalidType), p0);

    internal static string Validator_FullGlobeInCollection => TextRes.GetString(nameof (Validator_FullGlobeInCollection));

    internal static string Validator_LineStringNeedsTwoPoints => TextRes.GetString(nameof (Validator_LineStringNeedsTwoPoints));

    internal static string Validator_FullGlobeCannotHaveElements => TextRes.GetString(nameof (Validator_FullGlobeCannotHaveElements));

    internal static string Validator_NestingOverflow(object p0) => TextRes.GetString(nameof (Validator_NestingOverflow), p0);

    internal static string Validator_InvalidPointCoordinate(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (Validator_InvalidPointCoordinate), p0, p1, p2, p3);
    }

    internal static string Validator_UnexpectedCall(object p0, object p1) => TextRes.GetString(nameof (Validator_UnexpectedCall), p0, p1);

    internal static string Validator_UnexpectedCall2(object p0, object p1, object p2) => TextRes.GetString(nameof (Validator_UnexpectedCall2), p0, p1, p2);

    internal static string Validator_InvalidPolygonPoints => TextRes.GetString(nameof (Validator_InvalidPolygonPoints));

    internal static string Validator_InvalidLatitudeCoordinate(object p0) => TextRes.GetString(nameof (Validator_InvalidLatitudeCoordinate), p0);

    internal static string Validator_InvalidLongitudeCoordinate(object p0) => TextRes.GetString(nameof (Validator_InvalidLongitudeCoordinate), p0);

    internal static string Validator_UnexpectedGeography => TextRes.GetString(nameof (Validator_UnexpectedGeography));

    internal static string Validator_UnexpectedGeometry => TextRes.GetString(nameof (Validator_UnexpectedGeometry));

    internal static string GeoJsonReader_MissingRequiredMember(object p0) => TextRes.GetString(nameof (GeoJsonReader_MissingRequiredMember), p0);

    internal static string GeoJsonReader_InvalidPosition => TextRes.GetString(nameof (GeoJsonReader_InvalidPosition));

    internal static string GeoJsonReader_InvalidTypeName(object p0) => TextRes.GetString(nameof (GeoJsonReader_InvalidTypeName), p0);

    internal static string GeoJsonReader_InvalidNullElement => TextRes.GetString(nameof (GeoJsonReader_InvalidNullElement));

    internal static string GeoJsonReader_ExpectedNumeric => TextRes.GetString(nameof (GeoJsonReader_ExpectedNumeric));

    internal static string GeoJsonReader_ExpectedArray => TextRes.GetString(nameof (GeoJsonReader_ExpectedArray));

    internal static string GeoJsonReader_InvalidCrsType(object p0) => TextRes.GetString(nameof (GeoJsonReader_InvalidCrsType), p0);

    internal static string GeoJsonReader_InvalidCrsName(object p0) => TextRes.GetString(nameof (GeoJsonReader_InvalidCrsName), p0);

    internal static string JsonReaderExtensions_CannotReadPropertyValueAsString(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (JsonReaderExtensions_CannotReadPropertyValueAsString), p0, p1);
    }

    internal static string JsonReaderExtensions_CannotReadValueAsJsonObject(object p0) => TextRes.GetString(nameof (JsonReaderExtensions_CannotReadValueAsJsonObject), p0);

    internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0) => TextRes.GetString(nameof (PlatformHelper_DateTimeOffsetMustContainTimeZone), p0);
  }
}
