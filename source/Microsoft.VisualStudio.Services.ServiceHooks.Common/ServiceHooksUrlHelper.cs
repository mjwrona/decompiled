// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ServiceHooksUrlHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class ServiceHooksUrlHelper
  {
    public static string GenerateServiceHooksSubscriptionUrl(
      IVssRequestContext requestContext,
      Guid scopeId,
      Guid subscriptionId)
    {
      if (scopeId == NotificationClientConstants.CollectionScope)
        return (string) null;
      string hooksSubscriptionUrl = (string) null;
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, requestContext.ExecutionEnvironment.IsHostedDeployment ? ServiceInstanceTypes.TFS : LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
      if (!string.IsNullOrEmpty(locationServiceUrl))
        hooksSubscriptionUrl = UriUtility.CombinePath(UriUtility.CombinePath(locationServiceUrl, scopeId.ToString()), "_apps/hub/ms.vss-servicehooks-web.manageServiceHooks-project") + "?" + nameof (subscriptionId) + "=" + (object) subscriptionId;
      return hooksSubscriptionUrl;
    }
  }
}
