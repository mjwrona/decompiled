// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialSdkEngagementProviderService
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SocialEngagement.Server;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  public class SocialSdkEngagementProviderService : 
    ISocialSdkEngagementProviderService,
    IVssFrameworkService
  {
    private List<ISocialSdkSocialEngagementProvider> m_SocialEngagementProviders = new List<ISocialSdkSocialEngagementProvider>();

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      using (IDisposableReadOnlyList<ISocialSdkSocialEngagementProvider> extensions = requestContext.Elevate().GetExtensions<ISocialSdkSocialEngagementProvider>(ExtensionLifetime.Service))
      {
        if (extensions == null)
          return;
        this.m_SocialEngagementProviders.Clear();
        this.m_SocialEngagementProviders.AddRange((IEnumerable<ISocialSdkSocialEngagementProvider>) extensions.ToList<ISocialSdkSocialEngagementProvider>());
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<ISocialSdkSocialEngagementProvider> GetSocialSdkSocialEngagementProviders(
      IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      return this.m_SocialEngagementProviders;
    }
  }
}
