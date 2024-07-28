// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataMediaTypeFormatters
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.AspNet.OData.Formatter
{
  public static class ODataMediaTypeFormatters
  {
    private const string DollarFormat = "$format";
    private const string JsonFormat = "json";
    private const string XmlFormat = "xml";

    public static IList<ODataMediaTypeFormatter> Create() => (IList<ODataMediaTypeFormatter>) new List<ODataMediaTypeFormatter>()
    {
      ODataMediaTypeFormatters.CreateApplicationJson(),
      ODataMediaTypeFormatters.CreateApplicationXml(),
      ODataMediaTypeFormatters.CreateRawValue()
    };

    private static void AddSupportedEncodings(MediaTypeFormatter formatter)
    {
      formatter.SupportedEncodings.Add((Encoding) new UTF8Encoding(false, true));
      formatter.SupportedEncodings.Add((Encoding) new UnicodeEncoding(false, true, true));
    }

    private static ODataMediaTypeFormatter CreateRawValue()
    {
      ODataMediaTypeFormatter withoutMediaTypes = ODataMediaTypeFormatters.CreateFormatterWithoutMediaTypes(ODataPayloadKind.Value);
      withoutMediaTypes.MediaTypeMappings.Add((MediaTypeMapping) new ODataPrimitiveValueMediaTypeMapping());
      withoutMediaTypes.MediaTypeMappings.Add((MediaTypeMapping) new ODataEnumValueMediaTypeMapping());
      withoutMediaTypes.MediaTypeMappings.Add((MediaTypeMapping) new ODataBinaryValueMediaTypeMapping());
      withoutMediaTypes.MediaTypeMappings.Add((MediaTypeMapping) new ODataCountMediaTypeMapping());
      return withoutMediaTypes;
    }

    private static ODataMediaTypeFormatter CreateApplicationJson()
    {
      ODataMediaTypeFormatter withoutMediaTypes = ODataMediaTypeFormatters.CreateFormatterWithoutMediaTypes(ODataPayloadKind.ResourceSet, ODataPayloadKind.Resource, ODataPayloadKind.Property, ODataPayloadKind.EntityReferenceLink, ODataPayloadKind.EntityReferenceLinks, ODataPayloadKind.Collection, ODataPayloadKind.ServiceDocument, ODataPayloadKind.Error, ODataPayloadKind.Parameter, ODataPayloadKind.Delta);
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingTrue));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataMinimalMetadataStreamingFalse));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataMinimalMetadata));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingTrue));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataFullMetadataStreamingFalse));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataFullMetadata));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingTrue));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataNoMetadataStreamingFalse));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonODataNoMetadata));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonStreamingTrue));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJsonStreamingFalse));
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationJson));
      withoutMediaTypes.AddDollarFormatQueryStringMappings();
      withoutMediaTypes.AddQueryStringMapping("$format", "json", ODataMediaTypes.ApplicationJson);
      return withoutMediaTypes;
    }

    private static ODataMediaTypeFormatter CreateApplicationXml()
    {
      ODataMediaTypeFormatter withoutMediaTypes = ODataMediaTypeFormatters.CreateFormatterWithoutMediaTypes(ODataPayloadKind.MetadataDocument);
      withoutMediaTypes.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(ODataMediaTypes.ApplicationXml));
      withoutMediaTypes.AddDollarFormatQueryStringMappings();
      withoutMediaTypes.AddQueryStringMapping("$format", "xml", ODataMediaTypes.ApplicationXml);
      return withoutMediaTypes;
    }

    private static ODataMediaTypeFormatter CreateFormatterWithoutMediaTypes(
      params ODataPayloadKind[] payloadKinds)
    {
      ODataMediaTypeFormatter formatter = new ODataMediaTypeFormatter((IEnumerable<ODataPayloadKind>) payloadKinds);
      ODataMediaTypeFormatters.AddSupportedEncodings((MediaTypeFormatter) formatter);
      return formatter;
    }

    private static void AddDollarFormatQueryStringMappings(this ODataMediaTypeFormatter formatter)
    {
      foreach (MediaTypeHeaderValue supportedMediaType in (IEnumerable<MediaTypeHeaderValue>) formatter.SupportedMediaTypes)
      {
        QueryStringMediaTypeMapping mediaTypeMapping = new QueryStringMediaTypeMapping("$format", supportedMediaType);
        formatter.MediaTypeMappings.Add((MediaTypeMapping) mediaTypeMapping);
      }
    }
  }
}
