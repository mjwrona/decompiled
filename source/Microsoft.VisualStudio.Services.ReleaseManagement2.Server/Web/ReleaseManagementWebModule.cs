// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Web.ReleaseManagementWebModule
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Web
{
  public class ReleaseManagementWebModule : WebPlatformModule
  {
    public override WebContext CreateWebContext(RequestContext requestContext) => (WebContext) new ReleaseManagementWebContext(requestContext);

    public override ContributedServiceContext CreateContributedServiceContext(
      RequestContext requestContext)
    {
      ContributedServiceContext contributedServiceContext = base.CreateContributedServiceContext(requestContext);
      contributedServiceContext.CssModulePrefixes = new List<string>()
      {
        "RM"
      };
      return contributedServiceContext;
    }
  }
}
