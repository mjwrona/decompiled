// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.NoOpResourceMetadataBuilder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Evaluation
{
  internal sealed class NoOpResourceMetadataBuilder : ODataResourceMetadataBuilder
  {
    private readonly ODataResourceBase resource;

    internal NoOpResourceMetadataBuilder(ODataResourceBase resource) => this.resource = resource;

    internal override Uri GetEditLink() => this.resource.NonComputedEditLink;

    internal override Uri GetReadLink() => this.resource.NonComputedReadLink;

    internal override Uri GetId() => !this.resource.IsTransient ? this.resource.NonComputedId : (Uri) null;

    internal override string GetETag() => this.resource.NonComputedETag;

    internal override ODataStreamReferenceValue GetMediaResource() => this.resource.NonComputedMediaResource;

    internal override IEnumerable<ODataProperty> GetProperties(
      IEnumerable<ODataProperty> nonComputedProperties)
    {
      return nonComputedProperties;
    }

    internal override IEnumerable<ODataAction> GetActions() => this.resource.NonComputedActions;

    internal override IEnumerable<ODataFunction> GetFunctions() => this.resource.NonComputedFunctions;

    internal override Uri GetNavigationLinkUri(
      string navigationPropertyName,
      Uri navigationLinkUrl,
      bool hasNestedResourceInfoUrl)
    {
      return navigationLinkUrl;
    }

    internal override Uri GetAssociationLinkUri(
      string navigationPropertyName,
      Uri associationLinkUrl,
      bool hasAssociationLinkUrl)
    {
      return associationLinkUrl;
    }

    internal override bool TryGetIdForSerialization(out Uri id)
    {
      if (this.resource.IsTransient)
      {
        id = (Uri) null;
        return true;
      }
      id = this.GetId();
      return id != (Uri) null;
    }
  }
}
