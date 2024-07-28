// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.IODataJsonLightWriterResourceState
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.JsonLight
{
  internal interface IODataJsonLightWriterResourceState
  {
    ODataResourceBase Resource { get; }

    IEdmStructuredType ResourceType { get; }

    IEdmStructuredType ResourceTypeFromMetadata { get; }

    ODataResourceSerializationInfo SerializationInfo { get; }

    bool IsUndeclared { get; }

    bool EditLinkWritten { get; set; }

    bool ReadLinkWritten { get; set; }

    bool MediaEditLinkWritten { get; set; }

    bool MediaReadLinkWritten { get; set; }

    bool MediaContentTypeWritten { get; set; }

    bool MediaETagWritten { get; set; }

    ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse);
  }
}
