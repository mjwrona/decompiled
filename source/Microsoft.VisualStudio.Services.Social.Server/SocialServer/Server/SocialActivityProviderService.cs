// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.SocialActivityProviderService
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server
{
  internal class SocialActivityProviderService : ISocialActivityProviderService, IVssFrameworkService
  {
    private Dictionary<string, List<ISocialSdkSocialActivityProvider>> m_ActivityProviders = new Dictionary<string, List<ISocialSdkSocialActivityProvider>>();

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      using (IDisposableReadOnlyList<ISocialSdkSocialActivityProvider> extensions = requestContext.GetExtensions<ISocialSdkSocialActivityProvider>(ExtensionLifetime.Service))
      {
        this.m_ActivityProviders.Clear();
        if (extensions == null)
          return;
        extensions.ToList<ISocialSdkSocialActivityProvider>().ForEach((Action<ISocialSdkSocialActivityProvider>) (provider =>
        {
          string supportedActivityType = provider.GetSupportedActivityType();
          if (!this.m_ActivityProviders.ContainsKey(supportedActivityType))
            this.m_ActivityProviders.Add(supportedActivityType, new List<ISocialSdkSocialActivityProvider>());
          this.m_ActivityProviders[supportedActivityType].Add(provider);
        }));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ISocialSdkSocialActivityProvider GetSocialSdkSocialActivityProvider(
      IVssRequestContext requestContext,
      string activityType)
    {
      requestContext.CheckDeploymentRequestContext();
      List<ISocialSdkSocialActivityProvider> source = this.m_ActivityProviders.ContainsKey(activityType) ? this.m_ActivityProviders[activityType] : throw new ActivityTypeProviderNotFoundException();
      return source.Count <= 1 ? source.First<ISocialSdkSocialActivityProvider>() : throw new ProviderAlreadyPresentException(source.Select<ISocialSdkSocialActivityProvider, string>((Func<ISocialSdkSocialActivityProvider, string>) (provider => provider.GetType().FullName)), activityType);
    }
  }
}
