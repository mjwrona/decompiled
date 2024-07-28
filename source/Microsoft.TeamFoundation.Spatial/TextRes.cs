// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.TextRes
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace Microsoft.Spatial
{
  internal sealed class TextRes
  {
    internal const string SpatialImplementation_NoRegisteredOperations = "SpatialImplementation_NoRegisteredOperations";
    internal const string InvalidPointCoordinate = "InvalidPointCoordinate";
    internal const string Point_AccessCoordinateWhenEmpty = "Point_AccessCoordinateWhenEmpty";
    internal const string SpatialBuilder_CannotCreateBeforeDrawn = "SpatialBuilder_CannotCreateBeforeDrawn";
    internal const string GmlReader_UnexpectedElement = "GmlReader_UnexpectedElement";
    internal const string GmlReader_ExpectReaderAtElement = "GmlReader_ExpectReaderAtElement";
    internal const string GmlReader_InvalidSpatialType = "GmlReader_InvalidSpatialType";
    internal const string GmlReader_EmptyRingsNotAllowed = "GmlReader_EmptyRingsNotAllowed";
    internal const string GmlReader_PosNeedTwoNumbers = "GmlReader_PosNeedTwoNumbers";
    internal const string GmlReader_PosListNeedsEvenCount = "GmlReader_PosListNeedsEvenCount";
    internal const string GmlReader_InvalidSrsName = "GmlReader_InvalidSrsName";
    internal const string GmlReader_InvalidAttribute = "GmlReader_InvalidAttribute";
    internal const string WellKnownText_UnexpectedToken = "WellKnownText_UnexpectedToken";
    internal const string WellKnownText_UnexpectedCharacter = "WellKnownText_UnexpectedCharacter";
    internal const string WellKnownText_UnknownTaggedText = "WellKnownText_UnknownTaggedText";
    internal const string WellKnownText_TooManyDimensions = "WellKnownText_TooManyDimensions";
    internal const string Validator_SridMismatch = "Validator_SridMismatch";
    internal const string Validator_InvalidType = "Validator_InvalidType";
    internal const string Validator_FullGlobeInCollection = "Validator_FullGlobeInCollection";
    internal const string Validator_LineStringNeedsTwoPoints = "Validator_LineStringNeedsTwoPoints";
    internal const string Validator_FullGlobeCannotHaveElements = "Validator_FullGlobeCannotHaveElements";
    internal const string Validator_NestingOverflow = "Validator_NestingOverflow";
    internal const string Validator_InvalidPointCoordinate = "Validator_InvalidPointCoordinate";
    internal const string Validator_UnexpectedCall = "Validator_UnexpectedCall";
    internal const string Validator_UnexpectedCall2 = "Validator_UnexpectedCall2";
    internal const string Validator_InvalidPolygonPoints = "Validator_InvalidPolygonPoints";
    internal const string Validator_InvalidLatitudeCoordinate = "Validator_InvalidLatitudeCoordinate";
    internal const string Validator_InvalidLongitudeCoordinate = "Validator_InvalidLongitudeCoordinate";
    internal const string Validator_UnexpectedGeography = "Validator_UnexpectedGeography";
    internal const string Validator_UnexpectedGeometry = "Validator_UnexpectedGeometry";
    internal const string GeoJsonReader_MissingRequiredMember = "GeoJsonReader_MissingRequiredMember";
    internal const string GeoJsonReader_InvalidPosition = "GeoJsonReader_InvalidPosition";
    internal const string GeoJsonReader_InvalidTypeName = "GeoJsonReader_InvalidTypeName";
    internal const string GeoJsonReader_InvalidNullElement = "GeoJsonReader_InvalidNullElement";
    internal const string GeoJsonReader_ExpectedNumeric = "GeoJsonReader_ExpectedNumeric";
    internal const string GeoJsonReader_ExpectedArray = "GeoJsonReader_ExpectedArray";
    internal const string GeoJsonReader_InvalidCrsType = "GeoJsonReader_InvalidCrsType";
    internal const string GeoJsonReader_InvalidCrsName = "GeoJsonReader_InvalidCrsName";
    internal const string JsonReaderExtensions_CannotReadPropertyValueAsString = "JsonReaderExtensions_CannotReadPropertyValueAsString";
    internal const string JsonReaderExtensions_CannotReadValueAsJsonObject = "JsonReaderExtensions_CannotReadValueAsJsonObject";
    internal const string PlatformHelper_DateTimeOffsetMustContainTimeZone = "PlatformHelper_DateTimeOffsetMustContainTimeZone";
    private static TextRes loader;
    private ResourceManager resources;

    internal TextRes() => this.resources = new ResourceManager("Microsoft.Spatial", this.GetType().Assembly);

    private static TextRes GetLoader()
    {
      if (TextRes.loader == null)
      {
        TextRes textRes = new TextRes();
        Interlocked.CompareExchange<TextRes>(ref TextRes.loader, textRes, (TextRes) null);
      }
      return TextRes.loader;
    }

    private static CultureInfo Culture => (CultureInfo) null;

    public static ResourceManager Resources => TextRes.GetLoader().resources;

    public static string GetString(string name, params object[] args)
    {
      TextRes loader = TextRes.GetLoader();
      if (loader == null)
        return (string) null;
      string format = loader.resources.GetString(name, TextRes.Culture);
      if (args == null || args.Length == 0)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is string str && str.Length > 1024)
          args[index] = (object) (str.Substring(0, 1021) + "...");
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string GetString(string name) => TextRes.GetLoader()?.resources.GetString(name, TextRes.Culture);

    public static string GetString(string name, out bool usedFallback)
    {
      usedFallback = false;
      return TextRes.GetString(name);
    }

    public static object GetObject(string name) => TextRes.GetLoader()?.resources.GetObject(name, TextRes.Culture);
  }
}
