// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent8
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
  internal class DelegatedAuthorizationComponent8 : DelegatedAuthorizationComponent7
  {
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder4());
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
        this.BindNullableUrl("@registrationLocation", registration.RegistrationLocation, 500);
        this.BindNullableUrl("@registrationLogoSecureLocation", registration.RegistrationLogoSecureLocation, 500);
        this.BindNullableUrl("@registrationTermsOfServiceLocation", registration.RegistrationTermsOfServiceLocation, 500);
        this.BindNullableUrl("@registrationPrivacyPolicyLocation", registration.RegistrationPrivacyStatementLocation, 500);
        this.BindString("@responseTypes", registration.ResponseTypes, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@scopes", registration.Scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@secretVersionId", registration.SecretVersionId);
        this.BindBoolean("@isValid", true);
        this.BindByte("@clientType", (byte) registration.ClientType);
        this.BindBoolean("@isWellKnown", registration.IsWellKnown);
        this.BindDateTime("@SecretValidTo", registration.SecretValidTo.Value.DateTime);
        this.BindString("@PublicKey", registration.PublicKey, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder4());
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
        this.BindNullableGuid("@identityId", registration.IdentityId);
        this.BindString("@organizationName", registration.OrganizationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@organizationLocation", registration.OrganizationLocation, 500);
        this.BindString("@registrationName", registration.RegistrationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationDescription", registration.RegistrationDescription, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@registrationLocation", registration.RegistrationLocation, 500);
        this.BindNullableUrl("@registrationLogoSecureLocation", registration.RegistrationLogoSecureLocation, 500);
        this.BindNullableUrl("@registrationTermsOfServiceLocation", registration.RegistrationTermsOfServiceLocation, 500);
        this.BindNullableUrl("@registrationPrivacyPolicyLocation", registration.RegistrationPrivacyStatementLocation, 500);
        this.BindString("@responseTypes", registration.ResponseTypes, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@scopes", registration.Scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableGuid("@secretVersionId", registration.SecretVersionId);
        this.BindNullableBoolean("@isValid", new bool?(registration.IsValid));
        this.BindByte("@clientType", (byte) registration.ClientType);
        this.BindNullableBoolean("@isWellKnown", new bool?(registration.IsWellKnown));
        this.BindString("@PublicKey", registration.PublicKey, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        if (registration.SecretValidTo.HasValue)
          this.BindNullableDateTime("@SecretValidTo", new DateTime?(registration.SecretValidTo.Value.DateTime));
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder4());
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

    public override AccessTokenKey GetAccessTokenKey(Guid authorizationId, bool isPublic = false)
    {
      try
      {
        this.TraceEnter(1048549, nameof (GetAccessTokenKey));
        this.PrepareStoredProcedure("prc_GetAccessTokenKey");
        this.BindGuid("@authorizationId", authorizationId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder4());
          return resultCollection.GetCurrent<AccessTokenKey>().Items.FirstOrDefault<AccessTokenKey>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048550, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048551, nameof (GetAccessTokenKey));
      }
    }

    public override List<AccessTokenKey> ListAccessTokenKeys(
      Guid userId,
      bool isPublic = false,
      bool includePublicData = false)
    {
      if (userId == Guid.Empty)
        throw new ArgumentNullException();
      List<AccessTokenKey> accessTokenKeyList = new List<AccessTokenKey>();
      try
      {
        this.TraceEnter(1048537, nameof (ListAccessTokenKeys));
        this.PrepareStoredProcedure("prc_GetAccessTokensByUserId");
        this.BindGuid("@userId", userId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder4());
          List<AccessTokenKey> items = resultCollection.GetCurrent<AccessTokenKey>().Items;
          accessTokenKeyList.AddRange((IEnumerable<AccessTokenKey>) items);
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048538, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048539, nameof (ListAccessTokenKeys));
      }
      return accessTokenKeyList;
    }

    public override AccessTokenKey UpdateAccessToken(
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      string audience = null,
      bool isPublic = false)
    {
      try
      {
        this.TraceEnter(1048552, nameof (UpdateAccessToken));
        this.PrepareStoredProcedure("prc_UpdateAccessToken");
        this.BindGuid("@authorizationId", authorizationId);
        this.BindString("@displayName", displayName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@scope", scope, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        if (validTo.HasValue && validTo.Value > DateTime.MinValue)
          this.BindNullableDateTime("@validTo", new DateTime?(validTo.Value));
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder4());
          return resultCollection.GetCurrent<AccessTokenKey>().Items.FirstOrDefault<AccessTokenKey>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048553, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048554, nameof (UpdateAccessToken));
      }
    }

    public override AccessTokenData IssueTokenPair(
      Guid userId,
      Guid clientId,
      int redirectLocationId,
      string scope,
      DateTime validTo,
      string audience = null,
      Guid? accessIdToRefresh = null)
    {
      AccessTokenData accessTokenData = new AccessTokenData();
      try
      {
        this.TraceEnter(1048558, nameof (IssueTokenPair));
        this.PrepareStoredProcedure("prc_IssueTokenPair");
        this.BindGuid("@identityId", userId);
        this.BindGuid("@registrationId", clientId);
        this.BindInt("@redirectLocationId", redirectLocationId);
        this.BindString("@scope", scope, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindDateTime("@validTo", validTo);
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        if (accessIdToRefresh.HasValue)
          this.BindNullableGuid("@accessIdToRefresh", accessIdToRefresh.Value);
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
        this.TraceException(1048559, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048560, nameof (IssueTokenPair));
      }
      return accessTokenData;
    }
  }
}
