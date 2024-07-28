// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FollowsMatcherTemplateInputProvider
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Server.Follows;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class FollowsMatcherTemplateInputProvider : 
    IMatcherBasedTemplateInputProvider,
    ITemplateInputProvider
  {
    private readonly ServiceFactory<IDisposableReadOnlyList<IFollowsEmailMetadataProvider>> m_emailProvidersFactory;

    public string[] SupportedMatcherTypes => new string[1]
    {
      "FollowsMatcher"
    };

    internal FollowsMatcherTemplateInputProvider(
      IDisposableReadOnlyList<IFollowsEmailMetadataProvider> emailProviders)
    {
      this.m_emailProvidersFactory = (ServiceFactory<IDisposableReadOnlyList<IFollowsEmailMetadataProvider>>) (context => emailProviders);
    }

    public FollowsMatcherTemplateInputProvider() => this.m_emailProvidersFactory = (ServiceFactory<IDisposableReadOnlyList<IFollowsEmailMetadataProvider>>) (context => context.GetExtensions<IFollowsEmailMetadataProvider>());

    public void UpdateSystemInputs(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext)
    {
      using (IDisposableReadOnlyList<IFollowsEmailMetadataProvider> source = this.m_emailProvidersFactory(requestContext))
      {
        IFollowsEmailMetadataProvider metadataProvider = source != null ? source.FirstOrDefault<IFollowsEmailMetadataProvider>((Func<IFollowsEmailMetadataProvider, bool>) (p => p.SupportedEventType == transformContext.EventType)) : (IFollowsEmailMetadataProvider) null;
        if (metadataProvider == null)
          return;
        try
        {
          transformContext.SystemInputs["SubscriptionUnsubscribeAction"] = CoreRes.ArtifactUnfollowActionName();
          string unfollowUrl = metadataProvider.GetUnfollowUrl(requestContext, transformContext);
          transformContext.SystemInputs["SubscriptionUnsubscribeUrl"] = unfollowUrl;
          string artifactType = metadataProvider.GetArtifactType(requestContext, transformContext);
          transformContext.SystemInputs["ArtifactType"] = artifactType;
          transformContext.SystemInputs["SubscriptionReason"] = CoreRes.SubscriptionReasonFollows((object) artifactType);
          transformContext.SystemInputs.Remove("SubscriptionUrl");
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15093000, "Follows", "FollowsController", ex);
          throw;
        }
      }
    }
  }
}
