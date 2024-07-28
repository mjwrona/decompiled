// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightWriterUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;

namespace Microsoft.OData.JsonLight
{
  internal static class ODataJsonLightWriterUtils
  {
    internal static void WriteValuePropertyName(this IJsonWriter jsonWriter) => jsonWriter.WriteName("value");

    internal static void WritePropertyAnnotationName(
      this IJsonWriter jsonWriter,
      string propertyName,
      string annotationName)
    {
      jsonWriter.WriteName(propertyName + "@" + annotationName);
    }

    internal static void WriteInstanceAnnotationName(
      this IJsonWriter jsonWriter,
      string annotationName)
    {
      jsonWriter.WriteName("@" + annotationName);
    }
  }
}
