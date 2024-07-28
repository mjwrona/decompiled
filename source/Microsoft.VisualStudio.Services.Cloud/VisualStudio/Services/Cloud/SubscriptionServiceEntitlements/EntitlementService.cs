// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements.EntitlementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements
{
  public class EntitlementService : IEntitlementService, IVssFrameworkService
  {
    private const string c_area = "SubscriptionServiceEntitlements";
    private const string c_layer = "EntitlementService";
    private readonly IEntitlementsHttpClientProvider entitlementsHttpClientProvider;
    private readonly IEv4EntitlementSettingsManager ev4EntitlementSettingsManager;
    private readonly IEntitlementsAccessTokenProvider entitlementsAccessTokenProvider;
    private Ev4EntitlementsSettings m_Ev4Settings;
    private AuthenticationResult m_authenticationResult;

    public EntitlementService()
      : this((IEntitlementsHttpClientProvider) new EntitlementsHttpClientProvider(), (IEv4EntitlementSettingsManager) new Ev4EntitlementSettingsManager(), (IEntitlementsAccessTokenProvider) new EntitlementsAccessTokenProvider())
    {
    }

    public EntitlementService(
      IEntitlementsHttpClientProvider entitlementsHttpClientProvider,
      IEv4EntitlementSettingsManager ev4EntitlementSettingsManager,
      IEntitlementsAccessTokenProvider entitlementsAccessTokenProvider)
    {
      this.entitlementsHttpClientProvider = entitlementsHttpClientProvider;
      this.ev4EntitlementSettingsManager = ev4EntitlementSettingsManager;
      this.entitlementsAccessTokenProvider = entitlementsAccessTokenProvider;
    }

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_Ev4Settings = this.ev4EntitlementSettingsManager.GetEv4Settings(context);
      try
      {
        ServicePointManager.FindServicePoint(new Uri(this.m_Ev4Settings.Uri)).ConnectionLeaseTimeout = (int) TimeSpan.FromSeconds(60.0).TotalMilliseconds;
      }
      catch (Exception ex)
      {
        context.TraceException(1031067, "SubscriptionServiceEntitlements", nameof (EntitlementService), ex);
      }
    }

    public void ServiceEnd(IVssRequestContext context) => this.entitlementsHttpClientProvider.Dispose();

    public async Task<List<Ev4Entitlement>> GetMyEntitlementsAsync(
      IVssRequestContext requestContext,
      string site,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (userIdentity == null)
      {
        requestContext.TraceAlways(12500011, TraceLevel.Warning, "SubscriptionServiceEntitlements", nameof (EntitlementService), "Unexpected: identity is null.");
        return (List<Ev4Entitlement>) null;
      }
      string upn = this.GetUpn(requestContext, userIdentity);
      try
      {
        if (string.IsNullOrEmpty(upn))
        {
          requestContext.TraceAlways(12500012, TraceLevel.Warning, "SubscriptionServiceEntitlements", nameof (EntitlementService), "Unexpected: Identity AccountName attribute is empty. Identity: " + userIdentity.Id.ToString());
          return (List<Ev4Entitlement>) null;
        }
        requestContext.TraceAlways(1031065, TraceLevel.Info, "SubscriptionServiceEntitlements", nameof (EntitlementService), "User {0} has requested for an entitlement", (object) userIdentity.Id.ToString());
        string requestBody;
        if (userIdentity.IsMsa())
        {
          requestBody = JsonConvert.SerializeObject((object) new EntitlementsContract()
          {
            Upn = upn,
            Site = site,
            EntitlementBI = new EntitlementBI()
            {
              PUID = this.GetUserPuid(requestContext, userIdentity),
              VSID = userIdentity?.Id.ToString(),
              CUID = userIdentity.GetProperty<string>("CUID", string.Empty)
            }
          });
        }
        else
        {
          EntitlementsContract entitlementsContract = new EntitlementsContract();
          entitlementsContract.Upn = upn;
          entitlementsContract.Site = site;
          EntitlementBI entitlementBi = new EntitlementBI();
          entitlementBi.ObjectId = userIdentity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", string.Empty);
          Guid guid = requestContext.GetTenantId();
          entitlementBi.TenantId = guid.ToString();
          Microsoft.VisualStudio.Services.Identity.Identity identity = userIdentity;
          string str;
          if (identity == null)
          {
            str = (string) null;
          }
          else
          {
            guid = identity.Id;
            str = guid.ToString();
          }
          entitlementBi.VSID = str;
          entitlementBi.CUID = userIdentity.GetProperty<string>("CUID", string.Empty);
          entitlementsContract.EntitlementBI = entitlementBi;
          requestBody = JsonConvert.SerializeObject((object) entitlementsContract);
        }
        return await this.GetEv4EntitlementsAsync(requestBody, requestContext, cancellationToken);
      }
      catch (JsonException ex)
      {
        requestContext.TraceException(1031066, TraceLevel.Error, "SubscriptionServiceEntitlements", nameof (EntitlementService), (Exception) ex, "Json Exception has occurred for the user {0}", (object) userIdentity.Id.ToString());
        return new List<Ev4Entitlement>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1031065, "SubscriptionServiceEntitlements", nameof (EntitlementService), ex);
        throw;
      }
    }

    private async Task<List<Ev4Entitlement>> GetEv4EntitlementsAsync(
      string requestBody,
      IVssRequestContext context,
      CancellationToken cancellationToken)
    {
      string str = (string) null;
      string parameter = this.AcquireAccessToken(context);
      HttpContent httpContent = (HttpContent) new StringContent(requestBody, Encoding.UTF8, "application/json");
      HttpRequestMessage request = new HttpRequestMessage()
      {
        Method = HttpMethod.Post,
        RequestUri = new Uri(this.m_Ev4Settings.Uri),
        Content = httpContent
      };
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", parameter);
      using (HttpClient client = this.entitlementsHttpClientProvider.GetEv4EntitlementsHttpClient(context, this.m_Ev4Settings.LogServiceName))
      {
        HttpResponseMessage httpResponseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!httpResponseMessage.IsSuccessStatusCode)
          throw new VssServiceResponseException(httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, (Exception) null);
        str = await httpResponseMessage.Content.ReadAsStringAsync();
      }
      return JsonConvert.DeserializeObject<List<Ev4Entitlement>>(str);
    }

    private string AcquireAccessToken(IVssRequestContext context)
    {
      if (this.m_authenticationResult == null || this.m_authenticationResult.ExpiresOn.UtcDateTime <= DateTime.UtcNow.AddMinutes(15.0))
        this.m_authenticationResult = this.entitlementsAccessTokenProvider.AuthenticateApplication(context, this.m_Ev4Settings);
      return this.m_authenticationResult.AccessToken;
    }

    private string GetUpn(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      string upn = (string) null;
      if (userIdentity.HasUpn())
        upn = !userIdentity.HasEmaillessUpn() ? userIdentity.GetProperty<string>("Account", string.Empty) : IdentityHelper.GetPreferredEmailAddress(requestContext, userIdentity.Id) ?? string.Empty;
      return upn;
    }

    private string GetUserPuid(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      string userPuidFromSid = this.GetUserPuidFromSid(requestContext, userIdentity);
      if (this.IsMsaPuid(userPuidFromSid))
        return userPuidFromSid;
      string property = userIdentity.GetProperty<string>("PUID", string.Empty);
      return this.IsMsaPuid(property) ? property : (string) null;
    }

    private bool IsMsaPuid(string userPuid) => !string.IsNullOrEmpty(userPuid) && userPuid.All<char>((Func<char, bool>) (c => char.IsLetterOrDigit(c)));

    private string GetUserPuidFromSid(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      if (userIdentity.Descriptor == (IdentityDescriptor) null)
      {
        requestContext.Trace(1031031, TraceLevel.Warning, "SubscriptionServiceEntitlements", nameof (EntitlementService), "Unexpected: userIdentity.Descriptor is null.");
        return (string) null;
      }
      if (string.IsNullOrEmpty(userIdentity.Descriptor.Identifier))
      {
        requestContext.Trace(1031032, TraceLevel.Warning, "SubscriptionServiceEntitlements", nameof (EntitlementService), "Unexpected: userIdentity.Descriptor.Identifier is empty.");
        return (string) null;
      }
      string[] strArray = userIdentity.Descriptor.Identifier.Split(new char[1]
      {
        '@'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 2)
        return (string) null;
      string str = strArray[0];
      return !string.Equals(strArray[1], "live.com", StringComparison.OrdinalIgnoreCase) ? (string) null : str;
    }
  }
}
