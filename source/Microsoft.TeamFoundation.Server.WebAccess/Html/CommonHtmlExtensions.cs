// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.CommonHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class CommonHtmlExtensions
  {
    public static TagBuilder IdentityImageTag(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity)
    {
      return htmlHelper.IdentityImageTag(identity.TeamFoundationId, (object) null, (object) null);
    }

    public static TagBuilder IdentityImageTag(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity,
      object htmlAttributes)
    {
      return htmlHelper.IdentityImageTag(identity.TeamFoundationId, htmlAttributes, (object) null);
    }

    public static TagBuilder IdentityImageTag(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity,
      object htmlAttributes,
      object routeValues)
    {
      return htmlHelper.IdentityImageTag(identity.TeamFoundationId, htmlAttributes, routeValues);
    }

    public static TagBuilder IdentityImageTag(this HtmlHelper htmlHelper, Guid identityId) => htmlHelper.IdentityImageTag(identityId, (object) null, (object) null);

    public static TagBuilder IdentityImageTag(
      this HtmlHelper htmlHelper,
      Guid identityId,
      object htmlAttributes)
    {
      return htmlHelper.IdentityImageTag(identityId, htmlAttributes, (object) null);
    }

    public static TagBuilder IdentityImageTag(
      this HtmlHelper htmlHelper,
      Guid identityId,
      object htmlAttributes,
      object routeValues)
    {
      TfsWebContext webContext = htmlHelper.ViewContext.TfsWebContext();
      return new TagBuilder("img").AddClass("identity-picture").AddClass("identity-" + identityId.ToString()).Attribute("src", webContext.IdentityImage(identityId, routeValues)).Attribute("alt", "").Attribute(htmlAttributes);
    }

    public static MvcHtmlString IdentityImage(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity)
    {
      return htmlHelper.IdentityImage(identity.TeamFoundationId, (object) null, (object) null);
    }

    public static MvcHtmlString IdentityImage(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity,
      object htmlAttributes)
    {
      return htmlHelper.IdentityImage(identity.TeamFoundationId, htmlAttributes, (object) null);
    }

    public static MvcHtmlString IdentityImage(
      this HtmlHelper htmlHelper,
      TeamFoundationIdentity identity,
      object htmlAttributes,
      object routeValues)
    {
      return htmlHelper.IdentityImage(identity.TeamFoundationId, htmlAttributes, routeValues);
    }

    public static MvcHtmlString IdentityImage(this HtmlHelper htmlHelper, Guid identityId) => htmlHelper.IdentityImage(identityId, (object) null, (object) null);

    public static MvcHtmlString IdentityImage(
      this HtmlHelper htmlHelper,
      Guid identityId,
      object htmlAttributes)
    {
      return htmlHelper.IdentityImage(identityId, htmlAttributes, (object) null);
    }

    public static MvcHtmlString IdentityImage(
      this HtmlHelper htmlHelper,
      Guid identityId,
      object htmlAttributes,
      object routeValues)
    {
      return htmlHelper.IdentityImageTag(identityId, htmlAttributes, routeValues).ToHtmlString();
    }

    public static bool ShouldShowStakeholderWarningMessage(this HtmlHelper htmlHelper)
    {
      IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext;
      Guid currentProjectGuid = htmlHelper.ViewContext.TfsWebContext().CurrentProjectGuid;
      return !(currentProjectGuid == Guid.Empty) && tfsRequestContext.GetService<IProjectService>().GetProject(tfsRequestContext, currentProjectGuid).Visibility == ProjectVisibility.Private;
    }
  }
}
