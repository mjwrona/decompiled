// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ViewExtension
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class ViewExtension
  {
    public static void AddLinks(
      this AnalyticsView view,
      IVssRequestContext requestContext,
      AnalyticsViewScope viewScope)
    {
      if (view.Links == null)
        view.Links = new ReferenceLinks();
      string viewUrl = AnalyticsViewsUriBuilder.GetViewUrl(requestContext, viewScope, view.Id);
      view.Links.AddLink("self", viewUrl);
      view.Url = viewUrl;
    }

    public static void AdjustToAccountTime(
      this AnalyticsView view,
      IVssRequestContext requestContext)
    {
      ICollectionDateTimeService service = requestContext.GetService<ICollectionDateTimeService>();
      if (view.CreatedDate.Kind == DateTimeKind.Utc)
        view.CreatedDate = TimeZoneInfo.ConvertTime(view.CreatedDate, service.GetCollectionTimeZone(requestContext));
      if (view.LastModifiedDate.Kind != DateTimeKind.Utc)
        return;
      view.LastModifiedDate = TimeZoneInfo.ConvertTime(view.LastModifiedDate, service.GetCollectionTimeZone(requestContext));
    }

    public static void SetLastUpdatedBy(this AnalyticsView view, IVssRequestContext requestContext)
    {
      if (view.CreatedBy != null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = IdentityHelper.FindIdentity(requestContext, Guid.Parse(view.CreatedBy.Id), false);
        view.CreatedBy = identity.ToIdentityRef(requestContext);
      }
      if (view.LastModifiedBy == null)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = IdentityHelper.FindIdentity(requestContext, Guid.Parse(view.LastModifiedBy.Id), false);
      view.LastModifiedBy = identity1.ToIdentityRef(requestContext);
    }
  }
}
