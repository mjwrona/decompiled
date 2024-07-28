// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.EmailInteractionUriBuilder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public class EmailInteractionUriBuilder
  {
    private const string RedirectionEndPoint = "_emailInteraction";
    private const string RedirectionParameter = "redirection";
    private const string ContextParameter = "context";

    public Uri Redirection { get; set; }

    public EmailInteraction Interaction { get; set; }

    protected IVssRequestContext RequestContext { get; private set; }

    public EmailInteractionUriBuilder(
      IVssRequestContext requestContext,
      Uri redirection,
      EmailInteraction interaction)
    {
      this.RequestContext = requestContext;
      this.Redirection = redirection;
      this.Interaction = interaction;
    }

    public Uri BuildEmailInteractionUri()
    {
      IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Deployment);
      Uri uri = new Uri(vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.ClientAccessMappingMoniker));
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Path = "_emailInteraction";
      NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
      queryString["redirection"] = this.Redirection?.AbsoluteUri;
      if (this.Interaction != null)
        queryString["context"] = EmailInteraction.ToBase64Context(this.Interaction);
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.Uri;
    }
  }
}
