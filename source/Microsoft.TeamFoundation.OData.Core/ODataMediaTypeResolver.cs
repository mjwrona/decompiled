// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMediaTypeResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  public class ODataMediaTypeResolver
  {
    private static readonly HashSet<ODataPayloadKind> JsonPayloadKindSet = new HashSet<ODataPayloadKind>()
    {
      ODataPayloadKind.ResourceSet,
      ODataPayloadKind.Resource,
      ODataPayloadKind.Property,
      ODataPayloadKind.EntityReferenceLink,
      ODataPayloadKind.EntityReferenceLinks,
      ODataPayloadKind.Collection,
      ODataPayloadKind.ServiceDocument,
      ODataPayloadKind.Error,
      ODataPayloadKind.Parameter,
      ODataPayloadKind.Delta,
      ODataPayloadKind.IndividualProperty
    };
    private static readonly ODataMediaTypeResolver MediaTypeResolver = new ODataMediaTypeResolver();
    private static IDictionary<ODataPayloadKind, IEnumerable<ODataMediaTypeFormat>> SpecialMediaTypeFormat = (IDictionary<ODataPayloadKind, IEnumerable<ODataMediaTypeFormat>>) new Dictionary<ODataPayloadKind, IEnumerable<ODataMediaTypeFormat>>()
    {
      {
        ODataPayloadKind.Batch,
        (IEnumerable<ODataMediaTypeFormat>) new ODataMediaTypeFormat[1]
        {
          new ODataMediaTypeFormat(new ODataMediaType("multipart", "mixed"), ODataFormat.Batch)
        }
      },
      {
        ODataPayloadKind.Value,
        (IEnumerable<ODataMediaTypeFormat>) new ODataMediaTypeFormat[1]
        {
          new ODataMediaTypeFormat(new ODataMediaType("text", "plain"), ODataFormat.RawValue)
        }
      },
      {
        ODataPayloadKind.BinaryValue,
        (IEnumerable<ODataMediaTypeFormat>) new ODataMediaTypeFormat[1]
        {
          new ODataMediaTypeFormat(new ODataMediaType("application", "octet-stream"), ODataFormat.RawValue)
        }
      },
      {
        ODataPayloadKind.MetadataDocument,
        (IEnumerable<ODataMediaTypeFormat>) new ODataMediaTypeFormat[1]
        {
          new ODataMediaTypeFormat(new ODataMediaType("application", "xml"), ODataFormat.Metadata)
        }
      },
      {
        ODataPayloadKind.Asynchronous,
        (IEnumerable<ODataMediaTypeFormat>) new ODataMediaTypeFormat[1]
        {
          new ODataMediaTypeFormat(new ODataMediaType("application", "http"), ODataFormat.RawValue)
        }
      }
    };
    private static IEnumerable<ODataMediaTypeFormat> JsonMediaTypeFormats = ODataMediaTypeResolver.SetJsonLightMediaTypes();

    public virtual IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(
      ODataPayloadKind payloadKind)
    {
      if (ODataMediaTypeResolver.JsonPayloadKindSet.Contains(payloadKind))
        return ODataMediaTypeResolver.JsonMediaTypeFormats;
      return payloadKind == ODataPayloadKind.Batch ? ODataMediaTypeResolver.SpecialMediaTypeFormat[payloadKind].Concat<ODataMediaTypeFormat>(ODataMediaTypeResolver.JsonMediaTypeFormats) : ODataMediaTypeResolver.SpecialMediaTypeFormat[payloadKind];
    }

    internal static ODataMediaTypeResolver GetMediaTypeResolver(IServiceProvider container) => container == null ? ODataMediaTypeResolver.MediaTypeResolver : container.GetRequiredService<ODataMediaTypeResolver>();

    private static IEnumerable<ODataMediaTypeFormat> SetJsonLightMediaTypes()
    {
      KeyValuePair<string, string> keyValuePair1 = new KeyValuePair<string, string>("odata.metadata", "minimal");
      KeyValuePair<string, string> keyValuePair2 = new KeyValuePair<string, string>("odata.metadata", "full");
      KeyValuePair<string, string> keyValuePair3 = new KeyValuePair<string, string>("odata.metadata", "none");
      KeyValuePair<string, string> keyValuePair4 = new KeyValuePair<string, string>("odata.streaming", "true");
      KeyValuePair<string, string> keyValuePair5 = new KeyValuePair<string, string>("odata.streaming", "false");
      KeyValuePair<string, string> keyValuePair6 = new KeyValuePair<string, string>("IEEE754Compatible", "false");
      KeyValuePair<string, string> keyValuePair7 = new KeyValuePair<string, string>("IEEE754Compatible", "true");
      return (IEnumerable<ODataMediaTypeFormat>) new List<ODataMediaTypeFormat>()
      {
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair1,
          keyValuePair4,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair1,
          keyValuePair4,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair1,
          keyValuePair4
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair1,
          keyValuePair5,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair1,
          keyValuePair5,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair1,
          keyValuePair5
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair1,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair1,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          keyValuePair1
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair2,
          keyValuePair4,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair2,
          keyValuePair4,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair2,
          keyValuePair4
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair2,
          keyValuePair5,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair2,
          keyValuePair5,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair2,
          keyValuePair5
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair2,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair2,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          keyValuePair2
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair3,
          keyValuePair4,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair3,
          keyValuePair4,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair3,
          keyValuePair4
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair3,
          keyValuePair5,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[3]
        {
          keyValuePair3,
          keyValuePair5,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair3,
          keyValuePair5
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair3,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair3,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          keyValuePair3
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair4,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair4,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          keyValuePair4
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair5,
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[2]
        {
          keyValuePair5,
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          keyValuePair5
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          keyValuePair6
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
        {
          keyValuePair7
        }), ODataFormat.Json),
        new ODataMediaTypeFormat(new ODataMediaType("application", "json"), ODataFormat.Json)
      };
    }
  }
}
