// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.IODataJsonLightReaderResourceState
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal interface IODataJsonLightReaderResourceState
  {
    ODataResourceBase Resource { get; }

    IEdmStructuredType ResourceType { get; }

    IEdmStructuredType ResourceTypeFromMetadata { get; set; }

    IEdmNavigationSource NavigationSource { get; }

    ODataResourceMetadataBuilder MetadataBuilder { get; set; }

    bool AnyPropertyFound { get; set; }

    ODataJsonLightReaderNestedInfo FirstNestedInfo { get; set; }

    PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; }

    SelectedPropertiesNode SelectedProperties { get; }

    List<string> NavigationPropertiesRead { get; }

    bool ProcessingMissingProjectedNestedResourceInfos { get; set; }
  }
}
