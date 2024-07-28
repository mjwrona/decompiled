// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.JsonLightODataAnnotationWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;

namespace Microsoft.OData.JsonLight
{
  internal sealed class JsonLightODataAnnotationWriter
  {
    private static readonly int ODataAnnotationPrefixLength = "odata.".Length;
    private readonly IJsonWriter jsonWriter;
    private readonly bool enableWritingODataAnnotationWithoutPrefix;
    private readonly ODataVersion odataVersion;

    public JsonLightODataAnnotationWriter(
      IJsonWriter jsonWriter,
      bool enableWritingODataAnnotationWithoutPrefix,
      ODataVersion? odataVersion)
    {
      this.jsonWriter = jsonWriter;
      this.enableWritingODataAnnotationWithoutPrefix = enableWritingODataAnnotationWithoutPrefix;
      this.odataVersion = (ODataVersion) ((int) odataVersion ?? 0);
    }

    public void WriteODataTypeInstanceAnnotation(string typeName, bool writeRawValue = false)
    {
      this.WriteInstanceAnnotationName("odata.type");
      if (writeRawValue)
        this.jsonWriter.WriteValue(typeName);
      else
        this.jsonWriter.WriteValue(WriterUtils.PrefixTypeNameForWriting(typeName, this.odataVersion));
    }

    public void WriteODataTypePropertyAnnotation(string propertyName, string typeName)
    {
      this.WritePropertyAnnotationName(propertyName, "odata.type");
      this.jsonWriter.WriteValue(WriterUtils.PrefixTypeNameForWriting(typeName, this.odataVersion));
    }

    public void WritePropertyAnnotationName(string propertyName, string annotationName) => this.jsonWriter.WritePropertyAnnotationName(propertyName, this.SimplifyODataAnnotationName(annotationName));

    public void WriteInstanceAnnotationName(string annotationName) => this.jsonWriter.WriteInstanceAnnotationName(this.SimplifyODataAnnotationName(annotationName));

    private string SimplifyODataAnnotationName(string annotationName) => !this.enableWritingODataAnnotationWithoutPrefix ? annotationName : annotationName.Substring(JsonLightODataAnnotationWriter.ODataAnnotationPrefixLength);
  }
}
