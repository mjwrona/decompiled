// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.NoOpODataQueryExpanderFactory
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class NoOpODataQueryExpanderFactory : IODataQueryFactory
  {
    public string ExpandQuery(
      IVssRequestContext requestContext,
      AnalyticsView view,
      AnalyticsViewScope viewScope,
      AnalyticsViewQuery query,
      bool preview)
    {
      return query.ODataTemplate;
    }
  }
}
