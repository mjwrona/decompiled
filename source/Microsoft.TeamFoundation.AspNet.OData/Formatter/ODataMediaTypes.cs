// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataMediaTypes
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Formatter
{
  internal static class ODataMediaTypes
  {
    public static readonly string ApplicationJson = "application/json";
    public static readonly string ApplicationJsonODataFullMetadata = "application/json;odata.metadata=full";
    public static readonly string ApplicationJsonODataFullMetadataStreamingFalse = "application/json;odata.metadata=full;odata.streaming=false";
    public static readonly string ApplicationJsonODataFullMetadataStreamingTrue = "application/json;odata.metadata=full;odata.streaming=true";
    public static readonly string ApplicationJsonODataMinimalMetadata = "application/json;odata.metadata=minimal";
    public static readonly string ApplicationJsonODataMinimalMetadataStreamingFalse = "application/json;odata.metadata=minimal;odata.streaming=false";
    public static readonly string ApplicationJsonODataMinimalMetadataStreamingTrue = "application/json;odata.metadata=minimal;odata.streaming=true";
    public static readonly string ApplicationJsonODataNoMetadata = "application/json;odata.metadata=none";
    public static readonly string ApplicationJsonODataNoMetadataStreamingFalse = "application/json;odata.metadata=none;odata.streaming=false";
    public static readonly string ApplicationJsonODataNoMetadataStreamingTrue = "application/json;odata.metadata=none;odata.streaming=true";
    public static readonly string ApplicationJsonStreamingFalse = "application/json;odata.streaming=false";
    public static readonly string ApplicationJsonStreamingTrue = "application/json;odata.streaming=true";
    public static readonly string ApplicationXml = "application/xml";

    public static ODataMetadataLevel GetMetadataLevel(
      string mediaType,
      IEnumerable<KeyValuePair<string, string>> parameters)
    {
      if (mediaType == null || !string.Equals(ODataMediaTypes.ApplicationJson, mediaType, StringComparison.Ordinal))
        return ODataMetadataLevel.MinimalMetadata;
      KeyValuePair<string, string> keyValuePair = parameters.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => string.Equals("odata.metadata", p.Key, StringComparison.OrdinalIgnoreCase)));
      if (!keyValuePair.Equals((object) new KeyValuePair<string, string>()))
      {
        if (string.Equals("full", keyValuePair.Value, StringComparison.OrdinalIgnoreCase))
          return ODataMetadataLevel.FullMetadata;
        if (string.Equals("none", keyValuePair.Value, StringComparison.OrdinalIgnoreCase))
          return ODataMetadataLevel.NoMetadata;
      }
      return ODataMetadataLevel.MinimalMetadata;
    }
  }
}
