// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthTokenRequest
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;

namespace Microsoft.VisualStudio.Services.OAuth
{
  public class VssOAuthTokenRequest
  {
    private VssOAuthTokenParameters m_tokenParameters;
    private readonly VssOAuthGrant m_grant;
    private readonly VssOAuthClientCredential m_clientCredential;

    public VssOAuthTokenRequest(VssOAuthGrant grant, VssOAuthClientCredential clientCredential)
      : this(grant, clientCredential, (VssOAuthTokenParameters) null)
    {
    }

    public VssOAuthTokenRequest(
      VssOAuthGrant grant,
      VssOAuthClientCredential clientCredential,
      VssOAuthTokenParameters tokenParameters)
    {
      ArgumentUtility.CheckForNull<VssOAuthGrant>(grant, nameof (grant));
      this.m_grant = grant;
      this.m_clientCredential = clientCredential;
      this.m_tokenParameters = tokenParameters;
    }

    public VssOAuthGrant Grant => this.m_grant;

    public VssOAuthClientCredential ClientCredential => this.m_clientCredential;

    public VssOAuthTokenParameters Parameters
    {
      get
      {
        if (this.m_tokenParameters == null)
          this.m_tokenParameters = new VssOAuthTokenParameters();
        return this.m_tokenParameters;
      }
    }

    public static VssOAuthTokenRequest FromFormInput(FormDataCollection form)
    {
      HashSet<string> parsedParameters = new HashSet<string>();
      VssOAuthGrant grantFromFormInput = VssOAuthTokenRequest.CreateGrantFromFormInput(form, (ISet<string>) parsedParameters);
      VssOAuthClientCredential credentialFromFormInput = VssOAuthTokenRequest.CreateClientCredentialFromFormInput(form, (ISet<string>) parsedParameters);
      VssOAuthTokenParameters tokenParameters = new VssOAuthTokenParameters();
      foreach (KeyValuePair<string, string> keyValuePair in form)
      {
        if (parsedParameters.Add(keyValuePair.Key))
          tokenParameters.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return new VssOAuthTokenRequest(grantFromFormInput, credentialFromFormInput, tokenParameters);
    }

    private static VssOAuthGrant CreateGrantFromFormInput(
      FormDataCollection form,
      ISet<string> parsedParameters)
    {
      ArgumentUtility.CheckForNull<FormDataCollection>(form, nameof (form));
      string requiredValue1 = VssOAuthTokenRequest.GetRequiredValue(form, "grant_type", VssOAuthErrorCodes.InvalidRequest);
      switch (requiredValue1)
      {
        case "authorization_code":
          string requiredValue2 = VssOAuthTokenRequest.GetRequiredValue(form, "code", VssOAuthErrorCodes.InvalidRequest);
          parsedParameters.Add("code");
          return (VssOAuthGrant) new VssOAuthCodeGrant(requiredValue2);
        case "client_credentials":
          return (VssOAuthGrant) VssOAuthGrant.ClientCredentials;
        case "urn:ietf:params:oauth:grant-type:jwt-bearer":
          string requiredValue3 = VssOAuthTokenRequest.GetRequiredValue(form, "assertion", VssOAuthErrorCodes.InvalidRequest);
          parsedParameters.Add("assertion");
          return (VssOAuthGrant) new VssOAuthJwtBearerGrant(new VssOAuthJwtBearerAssertion(JsonWebToken.Create(requiredValue3)));
        case "refresh_token":
          string requiredValue4 = VssOAuthTokenRequest.GetRequiredValue(form, "refresh_token", VssOAuthErrorCodes.InvalidRequest);
          parsedParameters.Add("refresh_token");
          return (VssOAuthGrant) new VssOAuthRefreshTokenGrant(requiredValue4);
        default:
          throw new VssOAuthTokenRequestException("grant_type " + requiredValue1 + " is not supported")
          {
            Error = VssOAuthErrorCodes.UnsupportedGrantType
          };
      }
    }

    private static VssOAuthClientCredential CreateClientCredentialFromFormInput(
      FormDataCollection form,
      ISet<string> parsedParameters)
    {
      string subject = form["client_id"];
      if (form["client_assertion_type"] == "urn:ietf:params:oauth:client-assertion-type:jwt-bearer")
      {
        string requiredValue = VssOAuthTokenRequest.GetRequiredValue(form, "client_assertion", VssOAuthErrorCodes.InvalidClient);
        JsonWebToken bearerToken;
        try
        {
          bearerToken = JsonWebToken.Create(requiredValue);
        }
        catch (JsonWebTokenDeserializationException ex)
        {
          throw new VssOAuthTokenRequestException("client_assertion is not in the correct format", (Exception) ex)
          {
            Error = VssOAuthErrorCodes.InvalidClient
          };
        }
        if (!string.IsNullOrEmpty(subject))
        {
          if (subject.Equals(bearerToken.Subject, StringComparison.Ordinal))
            parsedParameters.Add("client_id");
          else
            throw new VssOAuthTokenRequestException("client_id " + subject + " does not match client_assertion subject " + bearerToken.Subject)
            {
              Error = VssOAuthErrorCodes.InvalidClient
            };
        }
        else
          subject = bearerToken.Subject;
        parsedParameters.Add("client_assertion");
        parsedParameters.Add("client_assertion_type");
        return (VssOAuthClientCredential) new VssOAuthJwtBearerClientCredential(subject, new VssOAuthJwtBearerAssertion(bearerToken));
      }
      if (!string.IsNullOrEmpty(subject))
      {
        parsedParameters.Add("client_id");
        string clientSecret = form["client_secret"];
        if (!string.IsNullOrEmpty(clientSecret))
        {
          parsedParameters.Add("client_secret");
          return (VssOAuthClientCredential) new VssOAuthPasswordClientCredential(subject, clientSecret);
        }
      }
      return (VssOAuthClientCredential) null;
    }

    private static string GetRequiredValue(
      FormDataCollection form,
      string parameterName,
      string error)
    {
      string str = form[parameterName];
      return !string.IsNullOrEmpty(str) ? str : throw new VssOAuthTokenRequestException(parameterName + " is required")
      {
        Error = error
      };
    }
  }
}
