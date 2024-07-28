// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Customized.Authentication.ChallengeCacheHandler
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System.Net;
using System.Net.Http;
using System.Threading;

namespace Microsoft.Azure.KeyVault.Customized.Authentication
{
  public class ChallengeCacheHandler : MessageProcessingHandler
  {
    protected override HttpRequestMessage ProcessRequest(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      return request;
    }

    protected override HttpResponseMessage ProcessResponse(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      if (response.StatusCode == HttpStatusCode.Unauthorized)
      {
        HttpBearerChallenge challengeFromResponse = HttpBearerChallenge.GetBearerChallengeFromResponse(response);
        if (challengeFromResponse != null)
          HttpBearerChallengeCache.GetInstance().SetChallengeForURL(response.RequestMessage.RequestUri, challengeFromResponse);
      }
      return response;
    }
  }
}
