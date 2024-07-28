// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.CollectionDateTimeService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class CollectionDateTimeService : ICollectionDateTimeService, IVssFrameworkService
  {
    private const string c_DateTimeRequestContextKey = "DateTimeRequestContextKey";
    private const string c_TimeZoneRequestContextKey = "TimeZoneRequestContextKey";

    public DateTime GetCollectionCurrentDateTime(IVssRequestContext requestContext) => TimeZoneInfo.ConvertTime(DateTime.Now, this.GetCollectionTimeZone(requestContext));

    public TimeZoneInfo GetCollectionTimeZone(IVssRequestContext requestContext)
    {
      if (!requestContext.Items.ContainsKey("TimeZoneRequestContextKey"))
      {
        TimeZoneInfo collectionTimeZone;
        using (AnalyticsMetadataComponent component = requestContext.CreateComponent<AnalyticsMetadataComponent>())
          collectionTimeZone = component.GetCollectionTimeZoneWithZoneId().CollectionTimeZone;
        requestContext.Items["TimeZoneRequestContextKey"] = (object) collectionTimeZone;
      }
      return (TimeZoneInfo) requestContext.Items["TimeZoneRequestContextKey"];
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
