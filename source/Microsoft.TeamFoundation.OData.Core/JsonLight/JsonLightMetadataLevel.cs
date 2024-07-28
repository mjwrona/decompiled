// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.JsonLightMetadataLevel
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal abstract class JsonLightMetadataLevel
  {
    internal static JsonLightMetadataLevel Create(
      ODataMediaType mediaType,
      Uri metadataDocumentUri,
      IEdmModel model,
      bool writingResponse)
    {
      if (writingResponse && mediaType.Parameters != null)
      {
        foreach (KeyValuePair<string, string> parameter in mediaType.Parameters)
        {
          if (HttpUtils.IsMetadataParameter(parameter.Key))
          {
            if (string.Compare(parameter.Value, "minimal", StringComparison.OrdinalIgnoreCase) == 0)
              return (JsonLightMetadataLevel) new JsonMinimalMetadataLevel();
            if (string.Compare(parameter.Value, "full", StringComparison.OrdinalIgnoreCase) == 0)
              return (JsonLightMetadataLevel) new JsonFullMetadataLevel(metadataDocumentUri, model);
            if (string.Compare(parameter.Value, "none", StringComparison.OrdinalIgnoreCase) == 0)
              return (JsonLightMetadataLevel) new JsonNoMetadataLevel();
          }
        }
      }
      return (JsonLightMetadataLevel) new JsonMinimalMetadataLevel();
    }

    internal abstract JsonLightTypeNameOracle GetTypeNameOracle();

    internal abstract ODataResourceMetadataBuilder CreateResourceMetadataBuilder(
      ODataResourceBase resource,
      IODataResourceTypeContext typeContext,
      ODataResourceSerializationInfo serializationInfo,
      IEdmStructuredType actualResourceType,
      SelectedPropertiesNode selectedProperties,
      bool isResponse,
      bool keyAsSegment,
      ODataUri odataUri,
      ODataMessageWriterSettings settings);

    internal virtual void InjectMetadataBuilder(
      ODataResourceBase resource,
      ODataResourceMetadataBuilder builder)
    {
      resource.MetadataBuilder = builder;
    }
  }
}
