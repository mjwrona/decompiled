// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PublicContributedHubRequestRestrictionsAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class PublicContributedHubRequestRestrictionsAttribute : 
    PublicProjectRequestRestrictionsAttribute
  {
    public PublicContributedHubRequestRestrictionsAttribute(string projectParameter = "project")
      : base(false, true, projectParameter, (string) null)
    {
    }

    public override AllowPublicAccessResult Allow(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      return PublicAccessHelpers.RouteSupportsPublicAccess(requestContext) ? base.Allow(requestContext, routeValues) : AllowPublicAccessResult.NotSupported;
    }
  }
}
