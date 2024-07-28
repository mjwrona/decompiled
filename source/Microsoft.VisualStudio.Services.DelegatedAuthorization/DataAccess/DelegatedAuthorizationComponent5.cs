// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent5
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent5 : DelegatedAuthorizationComponent4
  {
    public override Registration CreateRegistration(Registration registration)
    {
      try
      {
        this.TraceEnter(1048519, nameof (CreateRegistration));
        this.PrepareStoredProcedure("prc_CreateDelegatedAuthorizationRegistration");
        this.BindGuid("@registrationId", registration.RegistrationId);
        this.BindGuid("@identityId", registration.IdentityId);
        this.BindString("@organizationName", registration.OrganizationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@organizationLocation", registration.OrganizationLocation, 500);
        this.BindString("@registrationName", registration.RegistrationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationDescription", registration.RegistrationDescription, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationLocation", registration.RegistrationLocation.ToString(), 500, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@registrationLogoSecureLocation", registration.RegistrationLogoSecureLocation, 500);
        this.BindNullableUrl("@registrationTermsOfServiceLocation", registration.RegistrationTermsOfServiceLocation, 500);
        this.BindNullableUrl("@registrationPrivacyPolicyLocation", registration.RegistrationPrivacyStatementLocation, 500);
        this.BindString("@responseTypes", registration.ResponseTypes, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@scopes", registration.Scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@secretVersionId", registration.SecretVersionId);
        this.BindBoolean("@isValid", registration.IsValid);
        this.BindByte("@clientType", (byte) registration.ClientType);
        this.BindBoolean("@isWellKnown", registration.IsWellKnown);
        List<RegistrationRedirectLocation> rows = new List<RegistrationRedirectLocation>();
        foreach (Uri redirectUri in (IEnumerable<Uri>) registration.RedirectUris)
          rows.Add(new RegistrationRedirectLocation()
          {
            RegistrationId = registration.RegistrationId,
            Location = redirectUri
          });
        this.BindRegistrationRedirectLocationTable2("@redirectUris", (IEnumerable<RegistrationRedirectLocation>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder2());
          resultCollection.AddBinder<RegistrationRedirectLocation>((ObjectBinder<RegistrationRedirectLocation>) new RegistrationRedirectLocationBinder());
          Registration registration1 = resultCollection.GetCurrent<Registration>().Items.FirstOrDefault<Registration>();
          if (registration1 != null)
          {
            resultCollection.NextResult();
            List<RegistrationRedirectLocation> items = resultCollection.GetCurrent<RegistrationRedirectLocation>().Items;
            if (items != null && items.Count > 0)
            {
              foreach (RegistrationRedirectLocation redirectLocation in items)
                registration1.RedirectUris.Add(redirectLocation.Location);
            }
          }
          return registration1;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048520, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048521, nameof (CreateRegistration));
      }
    }

    public override Registration UpdateRegistration(Registration registration)
    {
      try
      {
        this.TraceEnter(1048534, nameof (UpdateRegistration));
        this.PrepareStoredProcedure("prc_UpdateDelegatedAuthorizationRegistration");
        this.BindGuid("@registrationId", registration.RegistrationId);
        this.BindGuid("@identityId", registration.IdentityId);
        this.BindString("@organizationName", registration.OrganizationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@organizationLocation", registration.OrganizationLocation, 500);
        this.BindString("@registrationName", registration.RegistrationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationDescription", registration.RegistrationDescription, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationLocation", registration.RegistrationLocation.ToString(), 500, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@registrationLogoSecureLocation", registration.RegistrationLogoSecureLocation, 500);
        this.BindNullableUrl("@registrationTermsOfServiceLocation", registration.RegistrationTermsOfServiceLocation, 500);
        this.BindNullableUrl("@registrationPrivacyPolicyLocation", registration.RegistrationPrivacyStatementLocation, 500);
        this.BindString("@responseTypes", registration.ResponseTypes, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@scopes", registration.Scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@secretVersionId", registration.SecretVersionId);
        this.BindBoolean("@isValid", registration.IsValid);
        this.BindByte("@clientType", (byte) registration.ClientType);
        this.BindBoolean("@isWellKnown", registration.IsWellKnown);
        List<RegistrationRedirectLocation> rows = new List<RegistrationRedirectLocation>();
        foreach (Uri redirectUri in (IEnumerable<Uri>) registration.RedirectUris)
          rows.Add(new RegistrationRedirectLocation()
          {
            RegistrationId = registration.RegistrationId,
            Location = redirectUri
          });
        this.BindRegistrationRedirectLocationTable2("@redirectUris", (IEnumerable<RegistrationRedirectLocation>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder2());
          resultCollection.AddBinder<RegistrationRedirectLocation>((ObjectBinder<RegistrationRedirectLocation>) new RegistrationRedirectLocationBinder());
          Registration registration1 = resultCollection.GetCurrent<Registration>().Items.FirstOrDefault<Registration>();
          if (registration1 != null)
          {
            resultCollection.NextResult();
            List<RegistrationRedirectLocation> items = resultCollection.GetCurrent<RegistrationRedirectLocation>().Items;
            if (items != null && items.Count > 0)
            {
              foreach (RegistrationRedirectLocation redirectLocation in items)
                registration1.RedirectUris.Add(redirectLocation.Location);
            }
          }
          return registration1;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048535, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048536, nameof (UpdateRegistration));
      }
    }

    public override Registration GetRegistration(Guid identityId, Guid registrationId)
    {
      try
      {
        this.TraceEnter(1048528, nameof (GetRegistration));
        this.PrepareStoredProcedure("prc_GetDelegatedAuthorizationRegistration");
        this.BindGuid("@identityId", identityId);
        this.BindGuid("@registrationId", registrationId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder2());
          resultCollection.AddBinder<RegistrationRedirectLocation>((ObjectBinder<RegistrationRedirectLocation>) new RegistrationRedirectLocationBinder());
          Registration registration = resultCollection.GetCurrent<Registration>().Items.FirstOrDefault<Registration>();
          if (registration != null)
          {
            resultCollection.NextResult();
            List<RegistrationRedirectLocation> items = resultCollection.GetCurrent<RegistrationRedirectLocation>().Items;
            if (items != null && items.Count > 0)
            {
              foreach (RegistrationRedirectLocation redirectLocation in items)
                registration.RedirectUris.Add(redirectLocation.Location);
            }
          }
          return registration;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048508, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048530, nameof (GetRegistration));
      }
    }

    public override IList<Registration> ListRegistrations(Guid identityId)
    {
      List<Registration> registrationList = new List<Registration>();
      try
      {
        this.TraceEnter(1048531, nameof (ListRegistrations));
        this.PrepareStoredProcedure("prc_ListDelegatedAuthorizationRegistrations");
        this.BindGuid("@identityId", identityId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder2());
          registrationList.AddRange((IEnumerable<Registration>) resultCollection.GetCurrent<Registration>().Items);
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048532, ex);
        throw;
      }
      finally
      {
        this.TraceEnter(1048533, nameof (ListRegistrations));
      }
      return (IList<Registration>) registrationList;
    }

    public override IEnumerable<AuthorizationDetails> GetAuthorizations(Guid userId)
    {
      List<AuthorizationDetails> authorizations = new List<AuthorizationDetails>();
      try
      {
        this.TraceEnter(1048507, nameof (GetAuthorizations));
        this.PrepareStoredProcedure("prc_GetAuthorizations");
        this.BindGuid("@identityId", userId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder());
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder2());
          resultCollection.AddBinder<RegistrationRedirectLocation>((ObjectBinder<RegistrationRedirectLocation>) new RegistrationRedirectLocationBinder());
          List<Authorization> items1 = resultCollection.GetCurrent<Authorization>().Items;
          authorizations.AddRange(items1.Select<Authorization, AuthorizationDetails>((System.Func<Authorization, AuthorizationDetails>) (authorization => new AuthorizationDetails()
          {
            Authorization = authorization
          })));
          resultCollection.NextResult();
          List<Registration> items2 = resultCollection.GetCurrent<Registration>().Items;
          resultCollection.NextResult();
          List<RegistrationRedirectLocation> items3 = resultCollection.GetCurrent<RegistrationRedirectLocation>().Items;
          if (items2 != null)
          {
            if (items2.Count > 0)
            {
              if (items3 != null && items3.Count > 0)
              {
                foreach (RegistrationRedirectLocation redirectLocation in items3)
                {
                  RegistrationRedirectLocation registrationRedirectLocation = redirectLocation;
                  items2.FirstOrDefault<Registration>((System.Func<Registration, bool>) (r => r.RegistrationId == registrationRedirectLocation.RegistrationId))?.RedirectUris.Add(registrationRedirectLocation.Location);
                }
              }
              foreach (AuthorizationDetails authorizationDetails1 in authorizations)
              {
                AuthorizationDetails authorizationDetails = authorizationDetails1;
                authorizationDetails.ClientRegistration = items2.FirstOrDefault<Registration>((System.Func<Registration, bool>) (r => r.RegistrationId == authorizationDetails.Authorization.RegistrationId));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048508, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048509, nameof (GetAuthorizations));
      }
      return (IEnumerable<AuthorizationDetails>) authorizations;
    }

    public override Registration InitiateAuthorization(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes)
    {
      Registration registration;
      try
      {
        this.TraceEnter(1048501, nameof (InitiateAuthorization));
        this.PrepareStoredProcedure("prc_InitiateAuthorization");
        this.BindGuid("@registrationId", clientId);
        this.BindString("@redirectUri", redirectUri.ToString(), 524, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@responseType", responseType.ToString(), 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@scope", scopes, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder2());
          resultCollection.AddBinder<RegistrationRedirectLocation>((ObjectBinder<RegistrationRedirectLocation>) new RegistrationRedirectLocationBinder());
          registration = resultCollection.GetCurrent<Registration>().Items.FirstOrDefault<Registration>();
          if (registration != null)
          {
            resultCollection.NextResult();
            List<RegistrationRedirectLocation> items = resultCollection.GetCurrent<RegistrationRedirectLocation>().Items;
            if (items != null)
            {
              if (items.Count > 0)
              {
                foreach (RegistrationRedirectLocation redirectLocation in items)
                  registration.RedirectUris.Add(redirectLocation.Location);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048502, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048503, nameof (InitiateAuthorization));
      }
      return registration;
    }

    public override Authorization Authorize(
      Guid userId,
      ResponseType responseType,
      Guid clientId,
      Uri redirectUri,
      string scopes,
      DateTime validFrom,
      DateTime validTo,
      string audience = null,
      Guid? authorizationId = null)
    {
      try
      {
        this.TraceEnter(1048504, nameof (Authorize));
        this.PrepareStoredProcedure("prc_CreateAuthorization");
        this.BindGuid("@registrationId", clientId);
        this.BindGuid("@identityId", userId);
        this.BindString("@responseType", responseType.ToString(), 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@redirectUri", redirectUri.ToString(), 524, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@scope", scopes, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindDateTime("@validFrom", validFrom);
        this.BindDateTime("@validTo", validTo);
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder2());
          resultCollection.AddBinder<RegistrationRedirectLocation>((ObjectBinder<RegistrationRedirectLocation>) new RegistrationRedirectLocationBinder());
          return resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048505, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048506, nameof (Authorize));
      }
    }

    public override AccessTokenData CreateAccessToken(
      Guid authorizationId,
      Guid clientSecretVersionId,
      Uri redirectUri,
      DateTime validFrom,
      DateTime validTo,
      Guid? accessId = null)
    {
      AccessTokenData accessToken = new AccessTokenData();
      try
      {
        this.TraceEnter(1048510, nameof (CreateAccessToken));
        this.PrepareStoredProcedure("prc_CreateAccessToken");
        this.BindGuid("@authorizationId", authorizationId);
        this.BindGuid("@clientSecretVersionId", clientSecretVersionId);
        this.BindString("@redirectUri", redirectUri.ToString(), 524, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindDateTime("@validFrom", validFrom);
        this.BindDateTime("@validTo", validTo);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessToken>((ObjectBinder<AccessToken>) new AccessTokenBinder());
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder2());
          accessToken.AccessToken = resultCollection.GetCurrent<AccessToken>().Items.FirstOrDefault<AccessToken>();
          resultCollection.NextResult();
          accessToken.Authorization = resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048511, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048512, nameof (CreateAccessToken));
      }
      return accessToken;
    }

    public override AccessTokenData RefreshAccessToken(
      Guid accessId,
      Guid clientSecretVersionId,
      DateTime validFrom,
      DateTime validTo)
    {
      AccessTokenData accessTokenData = new AccessTokenData();
      try
      {
        this.TraceEnter(1048513, nameof (RefreshAccessToken));
        this.PrepareStoredProcedure("prc_RefreshAccessToken");
        this.BindGuid("@accessId", accessId);
        this.BindGuid("@clientSecretVersionId", clientSecretVersionId);
        this.BindDateTime("@validFrom", validFrom);
        this.BindDateTime("@validTo", validTo);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessToken>((ObjectBinder<AccessToken>) new AccessTokenBinder());
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder2());
          accessTokenData.AccessToken = resultCollection.GetCurrent<AccessToken>().Items.FirstOrDefault<AccessToken>();
          resultCollection.NextResult();
          accessTokenData.Authorization = resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048514, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048515, nameof (RefreshAccessToken));
      }
      return accessTokenData;
    }

    public override AccessTokenData RefreshAccessToken(
      Guid accessId,
      Guid clientSecretVersionId,
      Uri redirectUri,
      DateTime validFrom,
      DateTime validTo,
      Guid? newAccessId = null)
    {
      AccessTokenData accessTokenData = new AccessTokenData();
      try
      {
        this.TraceEnter(1048513, nameof (RefreshAccessToken));
        this.PrepareStoredProcedure("prc_RefreshAccessToken");
        this.BindGuid("@accessId", accessId);
        this.BindGuid("@clientSecretVersionId", clientSecretVersionId);
        this.BindString("@redirectUri", redirectUri.ToString(), 524, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindDateTime("@validFrom", validFrom);
        this.BindDateTime("@validTo", validTo);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessToken>((ObjectBinder<AccessToken>) new AccessTokenBinder());
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder2());
          accessTokenData.AccessToken = resultCollection.GetCurrent<AccessToken>().Items.FirstOrDefault<AccessToken>();
          resultCollection.NextResult();
          accessTokenData.Authorization = resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048514, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048515, nameof (RefreshAccessToken));
      }
      return accessTokenData;
    }

    public override AccessTokenData GetAccessTokenByKey(string accessTokenHash, bool isPublic = false)
    {
      AccessTokenData accessTokenByKey = new AccessTokenData();
      try
      {
        this.TraceEnter(1048542, nameof (GetAccessTokenByKey));
        this.PrepareStoredProcedure("prc_GetAccessTokenByKey");
        this.BindString("@accessHash", accessTokenHash, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessToken>((ObjectBinder<AccessToken>) new AccessTokenBinder());
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder2());
          accessTokenByKey.AccessToken = resultCollection.GetCurrent<AccessToken>().Items.FirstOrDefault<AccessToken>();
          resultCollection.NextResult();
          accessTokenByKey.Authorization = resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048543, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048544, nameof (GetAccessTokenByKey));
      }
      return accessTokenByKey;
    }

    public override AccessTokenData IssueAccessToken(
      Guid userId,
      Guid clientId,
      int redirectLocationId,
      string scope,
      DateTime validTo,
      string audience = null,
      string source = null,
      bool isRequestedByTfsPatWebUI = false,
      Guid? authorizationId = null,
      Guid? accessId = null)
    {
      AccessTokenData accessTokenData = new AccessTokenData();
      try
      {
        this.TraceEnter(1048537, nameof (IssueAccessToken));
        this.PrepareStoredProcedure("prc_IssueAccessToken");
        this.BindGuid("@identityId", userId);
        this.BindGuid("@registrationId", clientId);
        this.BindInt("@redirectLocationId", redirectLocationId);
        this.BindString("@scope", scope, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindDateTime("@validTo", validTo);
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessToken>((ObjectBinder<AccessToken>) new AccessTokenBinder());
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder2());
          accessTokenData.AccessToken = resultCollection.GetCurrent<AccessToken>().Items.FirstOrDefault<AccessToken>();
          resultCollection.NextResult();
          accessTokenData.Authorization = resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048538, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048539, nameof (IssueAccessToken));
      }
      return accessTokenData;
    }

    public override List<AccessTokenKey> ListAccessTokenKeys(
      Guid userId,
      Guid accessId,
      bool isPublic = false)
    {
      if (userId == Guid.Empty)
        throw new ArgumentNullException();
      List<AccessTokenKey> accessTokenKeyList = new List<AccessTokenKey>();
      try
      {
        this.TraceEnter(1048537, nameof (ListAccessTokenKeys));
        this.PrepareStoredProcedure("prc_GetAccessTokensByUserId");
        this.BindGuid("@userId", userId);
        this.BindGuid("@accessId", accessId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder2());
          List<AccessTokenKey> items = resultCollection.GetCurrent<AccessTokenKey>().Items;
          accessTokenKeyList.AddRange((IEnumerable<AccessTokenKey>) items);
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048532, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048533, nameof (ListAccessTokenKeys));
      }
      return accessTokenKeyList;
    }
  }
}
