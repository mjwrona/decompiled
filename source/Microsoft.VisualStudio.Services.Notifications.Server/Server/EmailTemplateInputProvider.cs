// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EmailTemplateInputProvider
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.EmailNotification;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EmailTemplateInputProvider : 
    IChannelBasedTemplateInputProvider,
    ITemplateInputProvider
  {
    private const string EMAIL_OPENED_EVENT_TRACKING_URL = "EmailOpenedEventTrackingUrl";

    public string[] SupportedChannelTypes => new string[3]
    {
      "User",
      "EmailHtml",
      "EmailPlaintext"
    };

    public void UpdateSystemInputs(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext)
    {
      string str = this.ShouldGetEmailOpenedEventTrackingUrl(requestContext) ? this.GetEmailOpenedEventTrackingUrl(requestContext, transformContext) : string.Empty;
      transformContext.SystemInputs["EmailOpenedEventTrackingUrl"] = str;
      if (!NotificationTransformUtils.ShouldUseV2EmailTemplates(requestContext))
        return;
      transformContext.SystemInputs["useV2"] = true.ToString();
    }

    private bool ShouldGetEmailOpenedEventTrackingUrl(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      return requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS || InstanceManagementHelper.RequiresSharedService(requestContext.ServiceInstanceType());
    }

    private string GetEmailOpenedEventTrackingUrl(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext)
    {
      EmailInteraction interaction = new EmailInteraction()
      {
        EmailId = Guid.NewGuid(),
        EmailType = transformContext.DeliveryDetails.Matcher.Equals("FollowsMatcher") ? "Follows" : "Alert",
        EmailVariation = transformContext.EventType,
        InteractionId = string.Empty,
        InteractionType = InteractionType.EmailOpened
      };
      return new EmailInteractionUriBuilder(requestContext, (Uri) null, interaction).BuildEmailInteractionUri().AbsoluteUri;
    }
  }
}
