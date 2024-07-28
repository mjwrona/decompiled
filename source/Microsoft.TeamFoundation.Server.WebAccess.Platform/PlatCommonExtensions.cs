// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PlatCommonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class PlatCommonExtensions
  {
    public static TimeZoneInfo GetTimeZone(this IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<IUserPreferencesService>().GetUserPreferences(requestContext).TimeZone ?? requestContext.GetCollectionTimeZone();
    }

    public static TimeZoneInfo GetCollectionTimeZone(this IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      TimeZoneInfo timeZoneInfo = (TimeZoneInfo) null;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        timeZoneInfo = requestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(requestContext);
      return timeZoneInfo ?? TimeZoneInfo.Local;
    }
  }
}
