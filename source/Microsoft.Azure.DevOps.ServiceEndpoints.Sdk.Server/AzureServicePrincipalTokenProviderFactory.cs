// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureServicePrincipalTokenProviderFactory
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal static class AzureServicePrincipalTokenProviderFactory
  {
    public static IAuthorizationTokenProvider CreateProvider(
      ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      AzureServicePrincipalTokenProvider aadTokenProvider = new AzureServicePrincipalTokenProvider(serviceEndpoint, requestContext);
      AuthenticateChallengeProvider authenticateOptionsProvider = new AuthenticateChallengeProvider((IWwwAuthenticateHeaderParser) new WwwAuthenticateHeaderParser((IAuthenticateOptionsParser) new AuthenticateOptionsParser()));
      AcrRequestEvaluator acrRequestEvaluator1 = new AcrRequestEvaluator();
      GeneralHttpRequesterFactory requesterFactory1 = new GeneralHttpRequesterFactory(requestContext, "AzureContainerRegistryClient");
      AadToAcrRefreshTokenProvider refreshTokenProvider = new AadToAcrRefreshTokenProvider((IAuthorizationTokenProvider) aadTokenProvider, serviceEndpoint, (IExternalProviderHttpRequesterFactory) requesterFactory1, (IRequestEvaluator) acrRequestEvaluator1);
      GeneralHttpRequesterFactory requesterFactory2 = requesterFactory1;
      AcrRequestEvaluator acrRequestEvaluator2 = acrRequestEvaluator1;
      return (IAuthorizationTokenProvider) new CompositeAuthorizationTokenProvider(new IAuthorizationTokenProvider[2]
      {
        (IAuthorizationTokenProvider) new AcrAccessTokenProvider((IAuthenticateChallengeProvider) authenticateOptionsProvider, (IAuthorizationTokenProvider) refreshTokenProvider, (IExternalProviderHttpRequesterFactory) requesterFactory2, (IRequestEvaluator) acrRequestEvaluator2),
        (IAuthorizationTokenProvider) aadTokenProvider
      });
    }
  }
}
