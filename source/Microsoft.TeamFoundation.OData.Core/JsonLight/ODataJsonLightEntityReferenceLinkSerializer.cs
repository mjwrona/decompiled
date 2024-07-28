// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightEntityReferenceLinkSerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightEntityReferenceLinkSerializer : ODataJsonLightSerializer
  {
    internal ODataJsonLightEntityReferenceLinkSerializer(
      ODataJsonLightOutputContext jsonLightOutputContext)
      : base(jsonLightOutputContext, true)
    {
    }

    internal void WriteEntityReferenceLink(ODataEntityReferenceLink link) => this.WriteTopLevelPayload((Action) (() => this.WriteEntityReferenceLinkImplementation(link, true)));

    internal void WriteEntityReferenceLinks(ODataEntityReferenceLinks entityReferenceLinks) => this.WriteTopLevelPayload((Action) (() => this.WriteEntityReferenceLinksImplementation(entityReferenceLinks)));

    private void WriteEntityReferenceLinkImplementation(
      ODataEntityReferenceLink entityReferenceLink,
      bool isTopLevel)
    {
      WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);
      this.JsonWriter.StartObjectScope();
      if (isTopLevel)
        this.WriteContextUriProperty(ODataPayloadKind.EntityReferenceLink);
      this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.id");
      this.JsonWriter.WriteValue(this.UriToString(entityReferenceLink.Url));
      this.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) entityReferenceLink.InstanceAnnotations);
      this.JsonWriter.EndObjectScope();
    }

    private void WriteEntityReferenceLinksImplementation(
      ODataEntityReferenceLinks entityReferenceLinks)
    {
      bool flag = false;
      this.JsonWriter.StartObjectScope();
      this.WriteContextUriProperty(ODataPayloadKind.EntityReferenceLinks);
      if (entityReferenceLinks.Count.HasValue)
        this.WriteCountAnnotation(entityReferenceLinks.Count.Value);
      if (entityReferenceLinks.NextPageLink != (Uri) null)
      {
        flag = true;
        this.WriteNextLinkAnnotation(entityReferenceLinks.NextPageLink);
      }
      this.InstanceAnnotationWriter.WriteInstanceAnnotations((IEnumerable<ODataInstanceAnnotation>) entityReferenceLinks.InstanceAnnotations);
      this.JsonWriter.WriteValuePropertyName();
      this.JsonWriter.StartArrayScope();
      IEnumerable<ODataEntityReferenceLink> links = entityReferenceLinks.Links;
      if (links != null)
      {
        foreach (ODataEntityReferenceLink entityReferenceLink in links)
        {
          WriterValidationUtils.ValidateEntityReferenceLinkNotNull(entityReferenceLink);
          this.WriteEntityReferenceLinkImplementation(entityReferenceLink, false);
        }
      }
      this.JsonWriter.EndArrayScope();
      if (!flag && entityReferenceLinks.NextPageLink != (Uri) null)
        this.WriteNextLinkAnnotation(entityReferenceLinks.NextPageLink);
      this.JsonWriter.EndObjectScope();
    }

    private void WriteNextLinkAnnotation(Uri nextPageLink)
    {
      this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.nextLink");
      this.JsonWriter.WriteValue(this.UriToString(nextPageLink));
    }

    private void WriteCountAnnotation(long countValue)
    {
      this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.count");
      this.JsonWriter.WriteValue(countValue);
    }
  }
}
