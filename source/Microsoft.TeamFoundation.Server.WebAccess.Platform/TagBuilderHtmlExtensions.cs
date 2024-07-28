// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TagBuilderHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class TagBuilderHtmlExtensions
  {
    public static TagBuilder Data(this TagBuilder tagBuilder, string key, object value)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      string str = !(value is string) ? new JavaScriptSerializer().Serialize(value) : (string) value;
      tagBuilder.MergeAttribute("data-" + key, str);
      return tagBuilder;
    }

    public static TagBuilder Attribute(
      this TagBuilder tagBuilder,
      string attribute,
      string attributeValue)
    {
      return tagBuilder.Attribute(attribute, attributeValue, true);
    }

    public static TagBuilder Attribute(
      this TagBuilder tagBuilder,
      string attribute,
      string attributeValue,
      bool replaceExisting)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      ArgumentUtility.CheckStringForNullOrEmpty(attribute, nameof (attribute));
      tagBuilder.MergeAttribute(attribute, attributeValue, replaceExisting);
      return tagBuilder;
    }

    public static TagBuilder Attribute(this TagBuilder tagBuilder, object attributes) => tagBuilder.Attribute(attributes, true);

    public static TagBuilder Attribute(
      this TagBuilder tagBuilder,
      object attributes,
      bool replaceExisting)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      if (attributes != null)
      {
        RouteValueDictionary attributes1 = new RouteValueDictionary(attributes);
        if (attributes1.ContainsKey("addClass"))
        {
          tagBuilder.AddClass(attributes1["addClass"] as string);
          attributes1.Remove("addClass");
        }
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) attributes1, replaceExisting);
      }
      return tagBuilder;
    }

    public static TagBuilder AddClass(this TagBuilder tagBuilder, string cssClass)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      tagBuilder.AddCssClass(cssClass);
      return tagBuilder;
    }

    public static TagBuilder Append(this TagBuilder tagBuilder, params object[] children)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      if (children.Length != 0)
      {
        StringBuilder stringBuilder = new StringBuilder(tagBuilder.InnerHtml ?? "");
        foreach (object child in children)
        {
          if (child != null)
            stringBuilder.Append(child.ToString());
        }
        tagBuilder.InnerHtml = stringBuilder.ToString();
      }
      return tagBuilder;
    }

    public static TagBuilder AppendTag(
      this TagBuilder tagBuilder,
      string tagName,
      Action<TagBuilder> tagAction)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      TagBuilder tagBuilder1 = new TagBuilder(tagName);
      if (tagAction != null)
        tagAction(tagBuilder1);
      tagBuilder.Append((object) tagBuilder1);
      return tagBuilder;
    }

    public static TagBuilder Text(this TagBuilder tagBuilder, string text)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      tagBuilder.SetInnerText(text);
      return tagBuilder;
    }

    public static TagBuilder Html(this TagBuilder tagBuilder, string html)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      tagBuilder.InnerHtml = html;
      return tagBuilder;
    }

    public static MvcHtmlString ToHtmlString(this TagBuilder tagBuilder) => tagBuilder.ToHtmlString(TagRenderMode.Normal);

    public static MvcHtmlString ToHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      return MvcHtmlString.Create(tagBuilder.ToString(renderMode));
    }

    public static TagBuilder Scope(this TagBuilder tagBuilder, Action<TagBuilder> scopeAction)
    {
      ArgumentUtility.CheckForNull<TagBuilder>(tagBuilder, nameof (tagBuilder));
      if (scopeAction != null)
        scopeAction(tagBuilder);
      return tagBuilder;
    }

    public static TagBuilder AddNonceAttribute(
      this TagBuilder tagBuilder,
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      if (requestContext != null && (requestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyFeatureFlag) || requestContext.IsFeatureEnabled(ContentSecurityPolicyFeatureFlags.ContentSecurityPolicyReportOnlyFeatureFlag)))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string nonceValue = vssRequestContext.GetService<IContentSecurityPolicyNonceManagementService>().GetNonceValue(vssRequestContext, httpContext);
        if (!string.IsNullOrEmpty(nonceValue))
          tagBuilder.MergeAttribute("nonce", nonceValue);
      }
      return tagBuilder;
    }
  }
}
