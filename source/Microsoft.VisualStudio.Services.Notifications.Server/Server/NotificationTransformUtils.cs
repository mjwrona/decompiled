// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTransformUtils
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotificationTransformUtils
  {
    public static bool ShouldUseV2EmailTemplates(IVssRequestContext requestContext)
    {
      ContributedFeatureState featureState = requestContext.GetService<IContributedFeatureService>().GetFeatureState(requestContext, "ms.vss-notifications.use-email-templates-v2-feature");
      return featureState != null ? featureState.State == ContributedFeatureEnabledValue.Enabled : requestContext.IsFeatureEnabled("Notifications.UseEmailTemplatesV2");
    }
  }
}
