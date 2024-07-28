// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BasicAuthHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ResourceArea("8A3D49B8-91F0-46EF-B33D-DDA338C25DB3")]
  public class BasicAuthHttpClient : VssHttpClientBase
  {
    private static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      ["IdentityNotFoundException"] = typeof (IdentityNotFoundException),
      ["InvalidAccessException"] = typeof (InvalidAccessException),
      ["BasicAuthPasswordInvalidException"] = typeof (BasicAuthPasswordInvalidException)
    };
    private static readonly ApiResourceVersion s_currentApiVersion = new ApiResourceVersion(1.0);

    public BasicAuthHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BasicAuthHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BasicAuthHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<BasicAuthCredential> GetCredentialAsync(Guid identityId, object userState = null) => this.SendAsync<BasicAuthCredential>(HttpMethod.Get, BasicAuthResourceIds.BasicAuth, (object) new
    {
      identityId = identityId.ToString("D")
    }, BasicAuthHttpClient.s_currentApiVersion, userState: userState);

    public virtual Task<HttpResponseMessage> DeleteCredentialAsync(
      Guid identityId,
      object userState = null)
    {
      return this.SendAsync(HttpMethod.Delete, BasicAuthResourceIds.BasicAuth, (object) new
      {
        identityId = identityId.ToString("D")
      }, BasicAuthHttpClient.s_currentApiVersion, userState: userState);
    }

    public virtual Task<HttpResponseMessage> CreateCredentialAsync(
      Guid identityId,
      BasicAuthCredential credential,
      object userState = null)
    {
      return this.PutAsync<BasicAuthCredential>(credential, BasicAuthResourceIds.BasicAuth, (object) new
      {
        identityId = identityId.ToString("D")
      }, BasicAuthHttpClient.s_currentApiVersion, userState: userState);
    }

    public virtual Task<HttpResponseMessage> UpdateCredentialAsync(
      Guid identityId,
      BasicAuthCredential credential,
      object userState = null)
    {
      return this.PatchAsync<BasicAuthCredential>(credential, BasicAuthResourceIds.BasicAuth, (object) new
      {
        identityId = identityId.ToString("D")
      }, BasicAuthHttpClient.s_currentApiVersion, userState: userState);
    }

    public virtual async Task<bool> CheckCredentialAsync(
      Guid identityId,
      string password,
      object userState = null)
    {
      BasicAuthHttpClient basicAuthHttpClient = this;
      try
      {
        return (await basicAuthHttpClient.PostAsync(new
        {
          password = password
        }, BasicAuthResourceIds.BasicAuth, (object) new
        {
          identityId = identityId.ToString("D")
        }, BasicAuthHttpClient.s_currentApiVersion, userState: userState).ConfigureAwait(false)).IsSuccessStatusCode;
      }
      catch (BasicAuthWrongPasswordException ex)
      {
        return false;
      }
      catch (BasicAuthPasswordInvalidException ex)
      {
        return false;
      }
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) BasicAuthHttpClient.s_translatedExceptions;
  }
}
