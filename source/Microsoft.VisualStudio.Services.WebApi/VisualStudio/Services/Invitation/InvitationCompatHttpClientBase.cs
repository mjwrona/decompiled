// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Invitation.InvitationCompatHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Invitation
{
  [ResourceArea("287A6D53-7DC8-4618-8D57-6945B848A4AD")]
  public class InvitationCompatHttpClientBase : VssHttpClientBase
  {
    public InvitationCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InvitationCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InvitationCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InvitationCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public InvitationCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public async Task SendAccountInvitationAsync(
      InvitationData invitationData,
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InvitationCompatHttpClientBase compatHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("bc7ca053-e204-435b-a143-6240ba8a93bf");
      object obj1 = (object) new{ userId = userId };
      HttpContent httpContent = (HttpContent) new ObjectContent<InvitationData>(invitationData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      InvitationCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await compatHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public async Task SendOrganizationInvitationAsync(
      InvitationData invitationData,
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InvitationCompatHttpClientBase compatHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("bc7ca053-e204-435b-a143-6240ba8a93bf");
      object obj1 = (object) new{ userId = userId };
      HttpContent httpContent = (HttpContent) new ObjectContent<InvitationData>(invitationData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      InvitationCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.1, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await compatHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
