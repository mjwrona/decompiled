// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.UserIdentityHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ResourceArea("970AA69F-E316-4D78-B7B0-B7137E47A22C")]
  internal class UserIdentityHttpClient : VssHttpClientBase
  {
    public UserIdentityHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentity(
      SubjectDescriptor subjectDescriptor,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentity(subjectDescriptor.ToString(), cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentity(
      IdentityDescriptor identityDescriptor,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentity(UserHelper.CreatePseudoSubjectDescriptor(identityDescriptor).ToString(), cancellationToken);
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentity(
      Guid identityId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReadIdentity(identityId.ToString("D"), cancellationToken);
    }

    private async Task<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentity(
      string userDescriptor,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserIdentityHttpClient identityHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("28010c54-d0c0-4c89-a5b0-1c9e188b9fb7");
      object routeValues = (object) new
      {
        userDescriptor = userDescriptor
      };
      try
      {
        return await identityHttpClient.SendAsync<Microsoft.VisualStudio.Services.Identity.Identity>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, new ApiResourceVersion("5.0-preview.2"), cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex) when (ex is UserDoesNotExistException || ex is IdentityNotFoundException)
      {
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
    }

    public Task<IList<Microsoft.VisualStudio.Services.Identity.Identity>> ReadIdentities(
      IList<Guid> identityIds,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("299e50df-fe45-4d3a-8b5b-a5836fac74dc");
      HttpContent httpContent = (HttpContent) new ObjectContent<IList<Guid>>(identityIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("5.1-preview.1");
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, version: version, content: content, cancellationToken: cancellationToken2);
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> CreateIdentity(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("28010c54-d0c0-4c89-a5b0-1c9e188b9fb7");
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Identity.Identity>(identity, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.2");
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Identity.Identity>(method, locationId, version: version, content: content, cancellationToken: cancellationToken2);
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentity(
      SubjectDescriptor userDescriptor,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("28010c54-d0c0-4c89-a5b0-1c9e188b9fb7");
      object obj = (object) new
      {
        userDescriptor = userDescriptor
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Identity.Identity>(identity, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.2");
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Identity.Identity>(method, locationId, routeValues, version, content, cancellationToken: cancellationToken2);
    }

    public Task<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentity(
      Guid identityId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("28010c54-d0c0-4c89-a5b0-1c9e188b9fb7");
      object obj = (object) new
      {
        userDescriptor = identityId.ToString("D")
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Identity.Identity>(identity, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.2");
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Identity.Identity>(method, locationId, routeValues, version, content, cancellationToken: cancellationToken2);
    }
  }
}
