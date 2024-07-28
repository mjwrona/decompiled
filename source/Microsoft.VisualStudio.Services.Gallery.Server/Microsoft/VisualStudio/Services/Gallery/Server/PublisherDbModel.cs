// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherDbModel
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class PublisherDbModel : PublisherBase
  {
    private int logoFileId = -1;
    private readonly ReferenceLinks m_Links;
    private PublisherMetadata metadata;
    public string Domain;
    public bool IsDomainVerified;
    public bool IsDnsTokenVerified;

    public int LogoFileId
    {
      get
      {
        object obj;
        if (this.logoFileId == -1 && this.Metadata.PublisherDetailsLinks?.Links != null && this.Metadata.PublisherDetailsLinks.Links.TryGetValue("logo", out obj))
          this.logoFileId = int.Parse(((IEnumerable<string>) (obj as ReferenceLink).Href.Split('/')).Last<string>());
        return this.logoFileId;
      }
      set => this.logoFileId = value;
    }

    public PublisherMetadata Metadata
    {
      get
      {
        if (this.metadata == null)
          this.metadata = new PublisherMetadata()
          {
            PublisherDetailsLinks = PublisherLinksHelper.GetValidatedPublisherDetailsLinks(this.m_Links)
          };
        return this.metadata;
      }
      set => this.metadata = value;
    }

    public PublisherDbModel()
    {
    }

    public PublisherDbModel(Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      this.PublisherId = publisher.PublisherId;
      this.PublisherName = publisher.PublisherName;
      this.DisplayName = publisher.DisplayName;
      this.EmailAddress = publisher.EmailAddress;
      this.Extensions = publisher.Extensions;
      this.Flags = publisher.Flags;
      this.LastUpdated = publisher.LastUpdated;
      this.LongDescription = publisher.LongDescription;
      this.ShortDescription = publisher.ShortDescription;
      this.m_Links = publisher.Links;
      this.Domain = publisher.Domain?.ToString();
      this.IsDomainVerified = publisher.IsDomainVerified;
      this.IsDnsTokenVerified = publisher.IsDnsTokenVerified;
    }

    public Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher GetPublisher(
      IVssRequestContext requestContext)
    {
      ReferenceLinks referenceLinks = (ReferenceLinks) null;
      if (this.Metadata.PublisherDetailsLinks != null)
        referenceLinks = this.Metadata.PublisherDetailsLinks.Clone();
      if (this.LogoFileId > -1)
      {
        if (referenceLinks == null)
          referenceLinks = new ReferenceLinks();
        referenceLinks.AddLink("logo", PublisherLinksHelper.GetPublisherLogoCdnUrlString(requestContext, this.PublisherName, this.LogoFileId.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        referenceLinks.AddLink("fallbackLogo", PublisherLinksHelper.GetPublisherLogoEndpoint(requestContext, this.PublisherName, this.LogoFileId.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = new Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher();
      publisher.PublisherId = this.PublisherId;
      publisher.PublisherName = this.PublisherName;
      publisher.DisplayName = this.DisplayName;
      publisher.EmailAddress = this.EmailAddress;
      publisher.Extensions = this.Extensions;
      publisher.Flags = this.Flags;
      publisher.LastUpdated = this.LastUpdated;
      publisher.LongDescription = this.LongDescription;
      publisher.ShortDescription = this.ShortDescription;
      publisher.Links = referenceLinks;
      publisher.State = this.State;
      publisher.Domain = this.Domain;
      publisher.IsDomainVerified = this.IsDomainVerified;
      publisher.IsDnsTokenVerified = this.IsDnsTokenVerified;
      return publisher;
    }
  }
}
