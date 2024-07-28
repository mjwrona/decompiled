// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherLinksHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class PublisherLinksHelper
  {
    public static string GetPublisherLogoCdnUrlString(
      IVssRequestContext requestContext,
      string publisherName,
      string logoFileId)
    {
      ICDNService service = requestContext.GetService<ICDNService>();
      string valueFromPath = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherAsset/*").GetValueFromPath<string>("/Configuration/Service/Gallery/PublisherAsset/CDNHost", (string) null);
      return new UriBuilder(Uri.UriSchemeHttps, publisherName + valueFromPath)
      {
        Path = service.GetPublisherAssetCDNPath(publisherName, logoFileId.ToString((IFormatProvider) CultureInfo.InvariantCulture))
      }.Uri.ToString();
    }

    public static string GetPublisherLogoEndpoint(
      IVssRequestContext requestContext,
      string publisherName,
      string logoFileId)
    {
      return requestContext.RequestUri() == (Uri) null ? (string) null : new Uri(new Uri(requestContext.RequestUri().GetLeftPart(UriPartial.Authority)), "_apis/gallery/publishers/" + publisherName + "/publisherasset?assetType=logo").ToString();
    }

    public static ReferenceLinks GetValidatedPublisherDetailsLinks(ReferenceLinks links)
    {
      ReferenceLinks publisherDetailsLinks = (ReferenceLinks) null;
      if (links != null)
      {
        foreach (string metadataInputLink in (IEnumerable<string>) WellKnownPublisherMetadataLinks.PublisherMetadataInputLinks)
        {
          object obj;
          if (links.Links.TryGetValue(metadataInputLink, out obj))
          {
            ReferenceLink referenceLink = obj as ReferenceLink;
            if (!string.IsNullOrWhiteSpace(referenceLink.Href))
            {
              if (!GalleryServerUtil.IsValidUrl(referenceLink.Href))
              {
                if (metadataInputLink != "support" || !PublisherLinksHelper.IsValidEmail(referenceLink.Href))
                  throw new ArgumentException(metadataInputLink + " URL provided is not correct");
              }
              else
              {
                if (metadataInputLink == "twitter" && !referenceLink.Href.StartsWith("https://twitter.com") && !referenceLink.Href.StartsWith("https://www.twitter.com") && !referenceLink.Href.StartsWith("https://mobile.twitter.com"))
                  throw new ArgumentException("Twitter URL provided is not correct");
                if (metadataInputLink == "linkedIn" && !referenceLink.Href.StartsWith("https://linkedin.com") && !referenceLink.Href.StartsWith("https://www.linkedin.com"))
                  throw new ArgumentException("LinkedIn URL provided is not correct");
              }
              if (publisherDetailsLinks == null)
                publisherDetailsLinks = new ReferenceLinks();
              publisherDetailsLinks.AddLink(metadataInputLink, referenceLink.Href);
            }
          }
        }
      }
      return publisherDetailsLinks;
    }

    private static bool IsValidEmail(string emailString)
    {
      try
      {
        MailAddress mailAddress = new MailAddress(emailString);
        return true;
      }
      catch (FormatException ex)
      {
        return false;
      }
    }
  }
}
