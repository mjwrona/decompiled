// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Web.AnalyticsWebModule
// Assembly: Microsoft.VisualStudio.Services.Analytics.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 455612C1-A616-4BB6-B9F5-E94C097DFD14
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Analytics.Web
{
  public class AnalyticsWebModule : WebPlatformModule
  {
    private static readonly Guid AnalyticsServiceId = new Guid("0000003C-0000-8888-8000-000000000000");

    public override ContributedServiceContext CreateContributedServiceContext(
      RequestContext requestContext)
    {
      ContributedServiceContext contributedServiceContext = base.CreateContributedServiceContext(requestContext);
      contributedServiceContext.CssModulePrefixes = new List<string>()
      {
        "Analytics"
      };
      return contributedServiceContext;
    }
  }
}
