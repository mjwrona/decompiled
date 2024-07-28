// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightContextUriParseResult
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightContextUriParseResult
  {
    private readonly Uri contextUriFromPayload;

    internal ODataJsonLightContextUriParseResult(Uri contextUriFromPayload) => this.contextUriFromPayload = contextUriFromPayload;

    internal Uri ContextUri => this.contextUriFromPayload;

    internal Uri MetadataDocumentUri { get; set; }

    internal string Fragment { get; set; }

    internal string SelectQueryOption { get; set; }

    internal IEdmNavigationSource NavigationSource { get; set; }

    internal IEdmType EdmType { get; set; }

    internal IEnumerable<ODataPayloadKind> DetectedPayloadKinds { get; set; }

    internal ODataPath Path { get; set; }

    internal ODataDeltaKind DeltaKind { get; set; }
  }
}
