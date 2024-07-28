// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[20]
    {
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent>(1),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent2>(2),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent3>(3),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent4>(4),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent5>(5),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent6>(6),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent7>(7),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent8>(8),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent9>(9),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent10>(10),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent11>(11),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent12>(12),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent13>(13),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent14>(14),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent15>(15),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent16>(16),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent17>(17),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent18>(18),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent19>(19),
      (IComponentCreator) new ComponentCreator<DelegatedAuthorizationComponent20>(20)
    }, "DelegatedAuthorization");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = DelegatedAuthorizationComponent.CreateSqlExceptionFactories();

    public DelegatedAuthorizationComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DelegatedAuthorizationComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "DelegatedAuthorization";

    private static Dictionary<int, SqlExceptionFactory> CreateSqlExceptionFactories() => new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1080001,
        new SqlExceptionFactory(typeof (RegistrationNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new RegistrationNotFoundException("RegistrationNotFound")))
      },
      {
        1080002,
        new SqlExceptionFactory(typeof (ResponseTypeNotSupportedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ResponseTypeNotSupportedException("ResponseTypeNotSupportedException")))
      },
      {
        1080003,
        new SqlExceptionFactory(typeof (InvalidRedirectUriException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidRedirectUriException("InvalidRedirectUriException")))
      },
      {
        1080004,
        new SqlExceptionFactory(typeof (InvalidRegistrationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidRegistrationException("InvalidRegistrationException")))
      },
      {
        1080005,
        new SqlExceptionFactory(typeof (AuthorizationNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AuthorizationNotFoundException("AuthorizationNotFoundException")))
      },
      {
        1080006,
        new SqlExceptionFactory(typeof (InvalidAuthorizationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAuthorizationException("InvalidAuthorizationException")))
      },
      {
        1080007,
        new SqlExceptionFactory(typeof (AccessAlreadyIssuedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AccessAlreadyIssuedException("AccessAlreadyIssuedException")))
      },
      {
        1080008,
        new SqlExceptionFactory(typeof (InvalidAuthorizationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAuthorizationException("InvalidAuthorizationException")))
      },
      {
        1080009,
        new SqlExceptionFactory(typeof (AccessTokenNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AccessTokenNotFoundException("AccessTokenNotFoundException")))
      },
      {
        1080010,
        new SqlExceptionFactory(typeof (InvalidAccessTokenException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAccessTokenException("InvalidAccessTokenException")))
      },
      {
        1080011,
        new SqlExceptionFactory(typeof (AccessTokenAlreadyRefreshedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AccessTokenAlreadyRefreshedException("AccessTokenAlreadyRefreshedException")))
      },
      {
        1080012,
        new SqlExceptionFactory(typeof (InvalidClientSecretException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidClientSecretException("InvalidClientSecretException")))
      },
      {
        1080013,
        new SqlExceptionFactory(typeof (RegistrationAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new RegistrationAlreadyExistsException("RegistrationAlreadyExistsException")))
      },
      {
        1080014,
        new SqlExceptionFactory(typeof (UnableToDeleteRegistrationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new UnableToDeleteRegistrationException("UnableToDeleteRegistrationException")))
      },
      {
        1080015,
        new SqlExceptionFactory(typeof (UpdateRegistrationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new UpdateRegistrationFailException("UpdateRegistrationFailException")))
      },
      {
        1080016,
        new SqlExceptionFactory(typeof (InvalidAuthorizationScopeException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAuthorizationScopeException("InvalidAuthorizationScopeException")))
      },
      {
        1080017,
        new SqlExceptionFactory(typeof (RedirectUriAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new RedirectUriAlreadyExistsException("RedirectUriAlreadyExistsException")))
      },
      {
        1080018,
        new SqlExceptionFactory(typeof (AccessTokenCreationFailedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AccessTokenCreationFailedException("AccessTokenCreationFailedException")))
      },
      {
        1080019,
        new SqlExceptionFactory(typeof (InvalidAccessTokenKeyException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAccessTokenKeyException("InvalidAccessTokenKeyException")))
      },
      {
        1080020,
        new SqlExceptionFactory(typeof (InvalidAccessIdException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidAccessIdException("InvalidAccessIdException")))
      },
      {
        1080021,
        new SqlExceptionFactory(typeof (InvalidClientTypeException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidClientTypeException("InvalidClientTypeException")))
      },
      {
        1080022,
        new SqlExceptionFactory(typeof (UpdateDelegatedAuthorizationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new UpdateDelegatedAuthorizationFailException("UpdateDelegatedAuthorizationFailException")))
      },
      {
        1080023,
        new SqlExceptionFactory(typeof (HostAuthorizationNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new HostAuthorizationNotFoundException("HostAuthorizationNotFoundException")))
      }
    };

    public virtual Registration InitiateAuthorization(
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder());
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

    public virtual Authorization Authorize(
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
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder());
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

    public virtual void Revoke(Guid identityId, Guid authorizationId)
    {
      try
      {
        this.TraceEnter(1048516, nameof (Revoke));
        this.PrepareStoredProcedure("prc_RevokeAuthorization");
        this.BindGuid("@identityId", identityId);
        this.BindGuid("@authorizationId", authorizationId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1048517, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048518, nameof (Revoke));
      }
    }

    public virtual AccessTokenData CreateAccessToken(
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
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder());
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

    public virtual AccessTokenData RefreshAccessToken(
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
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder());
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

    public virtual AccessTokenData RefreshAccessToken(
      Guid accessId,
      Guid clientSecretVersionId,
      Uri redirectUri,
      DateTime validFrom,
      DateTime validTo,
      Guid? newAccessId = null)
    {
      return this.RefreshAccessToken(accessId, clientSecretVersionId, validFrom, validTo);
    }

    public virtual AccessTokenData IssueAccessToken(
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
      return (AccessTokenData) null;
    }

    public virtual AccessTokenData IssueTokenPair(
      Guid userId,
      Guid clientId,
      int redirectLocationId,
      string scope,
      DateTime validTo,
      string audience = null,
      Guid? accessIdToRefresh = null)
    {
      return (AccessTokenData) null;
    }

    public virtual AccessTokenData GetAccessTokenByKey(string accessTokenHash, bool isPublic = false) => (AccessTokenData) null;

    public virtual void CreateAccessTokenKey(
      Guid accessId,
      string tokenHash,
      string displayName,
      Guid identityId,
      bool isPublic = false,
      string publicData = null)
    {
    }

    public virtual HostAuthorization GetHostAuthorization(Guid clientId, Guid hostId) => (HostAuthorization) null;

    public virtual HostAuthorization UpdateHostAuthorization(
      Guid clientId,
      Guid hostId,
      Guid? newId = null)
    {
      return (HostAuthorization) null;
    }

    public virtual HostAuthorization InsertDelegatedHostAuthorization(
      Guid id,
      Guid clientId,
      Guid hostId)
    {
      return (HostAuthorization) null;
    }

    public virtual IList<string> RevokeHostAuthorization(Guid clientId, Guid hostId) => (IList<string>) null;

    public virtual AccessTokenKey UpdateAccessToken(
      Guid authorizationId,
      string displayName = null,
      string scope = null,
      DateTime? validTo = null,
      string audience = null,
      bool isPublic = false)
    {
      return (AccessTokenKey) null;
    }

    public virtual Authorization GetAuthorization(Guid authorizationId) => (Authorization) null;

    public virtual IEnumerable<AuthorizationDetails> GetAuthorizations(Guid userId)
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder());
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

    public virtual Registration CreateRegistration(Registration registration)
    {
      try
      {
        this.TraceEnter(1048519, nameof (CreateRegistration));
        this.PrepareStoredProcedure("prc_CreateDelegatedAuthorizationRegistration");
        this.BindGuid("@registrationId", registration.RegistrationId);
        this.BindGuid("@identityId", registration.IdentityId);
        this.BindString("@organizationName", registration.OrganizationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@organizationLocation", registration.OrganizationLocation, 256);
        this.BindString("@registrationName", registration.RegistrationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationDescription", registration.RegistrationDescription, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationLocation", registration.RegistrationLocation.ToString(), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@registrationLogoSecureLocation", registration.RegistrationLogoSecureLocation, 256);
        this.BindNullableUrl("@registrationTermsOfServiceLocation", registration.RegistrationTermsOfServiceLocation, 256);
        this.BindNullableUrl("@registrationPrivacyPolicyLocation", registration.RegistrationPrivacyStatementLocation, 256);
        this.BindString("@responseTypes", registration.ResponseTypes, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@scopes", registration.Scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@secretVersionId", registration.SecretVersionId);
        this.BindBoolean("@isValid", registration.IsValid);
        List<RegistrationRedirectLocation> rows = new List<RegistrationRedirectLocation>();
        foreach (Uri redirectUri in (IEnumerable<Uri>) registration.RedirectUris)
          rows.Add(new RegistrationRedirectLocation()
          {
            RegistrationId = registration.RegistrationId,
            Location = redirectUri
          });
        this.BindRegistrationRedirectLocationTable("@redirectUris", (IEnumerable<RegistrationRedirectLocation>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder());
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

    internal virtual IDictionary<int, string> GetRedirectUriForRegistration(Guid registrationId) => (IDictionary<int, string>) null;

    public virtual IList<Registration> ListRegistrations(Guid identityId)
    {
      List<Registration> registrationList = new List<Registration>();
      try
      {
        this.TraceEnter(1048531, nameof (ListRegistrations));
        this.PrepareStoredProcedure("prc_ListDelegatedAuthorizationRegistrations");
        this.BindGuid("@identityId", identityId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder());
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

    public virtual Registration GetRegistration(Guid identityId, Guid registrationId)
    {
      try
      {
        this.TraceEnter(1048528, nameof (GetRegistration));
        this.PrepareStoredProcedure("prc_GetDelegatedAuthorizationRegistration");
        this.BindGuid("@identityId", identityId);
        this.BindGuid("@registrationId", registrationId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder());
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

    public virtual Registration UpdateRegistration(Registration registration)
    {
      try
      {
        this.TraceEnter(1048534, nameof (UpdateRegistration));
        Uri redirectUri = registration.RedirectUris[0];
        this.PrepareStoredProcedure("prc_UpdateDelegatedAuthorizationRegistration");
        this.BindGuid("@registrationId", registration.RegistrationId);
        this.BindGuid("@identityId", registration.IdentityId);
        this.BindString("@organizationName", registration.OrganizationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@organizationLocation", registration.OrganizationLocation, 256);
        this.BindString("@registrationName", registration.RegistrationName, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationDescription", registration.RegistrationDescription, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@registrationLocation", registration.RegistrationLocation.ToString(), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableUrl("@registrationLogoSecureLocation", registration.RegistrationLogoSecureLocation, 256);
        this.BindNullableUrl("@registrationTermsOfServiceLocation", registration.RegistrationTermsOfServiceLocation, 256);
        this.BindNullableUrl("@registrationPrivacyPolicyLocation", registration.RegistrationPrivacyStatementLocation, 256);
        this.BindString("@responseTypes", registration.ResponseTypes, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@scopes", registration.Scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@secretVersionId", registration.SecretVersionId);
        this.BindBoolean("@isValid", registration.IsValid);
        this.BindString("@redirectUri", redirectUri.ToString(), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder());
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

    public virtual void DeleteRegistration(Guid registrationId)
    {
      try
      {
        this.TraceEnter(1048522, nameof (DeleteRegistration));
        this.PrepareStoredProcedure("prc_PurgeDelegatedAuthRegistration");
        this.BindGuid("@registrationId", registrationId);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(1048524, nameof (DeleteRegistration));
      }
    }

    public virtual List<AccessTokenKey> ListAccessTokenKeys(
      Guid userId,
      Guid accessId,
      bool isPublic = false)
    {
      return (List<AccessTokenKey>) null;
    }

    public virtual List<AccessTokenKey> ListAccessTokenKeys(
      Guid userId,
      bool isPublic = false,
      bool includePublicData = false)
    {
      return (List<AccessTokenKey>) null;
    }

    public virtual AccessTokenKeyPage ListAccessTokensByPage(
      Guid userId,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false)
    {
      return (AccessTokenKeyPage) null;
    }

    public virtual AccessTokenKey GetAccessTokenKey(Guid authorizationId, bool isPublic = false) => (AccessTokenKey) null;

    public virtual string RevokeAccessTokenKey(Guid authorizationId) => (string) null;

    public virtual string RemoveAccessTokenKey(Guid authorizationId) => (string) null;

    public virtual List<string> RevokeAllAccessTokenKeysForUser(Guid identityId) => (List<string>) null;

    public virtual IList<HostAuthorization> GetHostAuthorizations(Guid hostId) => (IList<HostAuthorization>) null;

    public virtual IList<Guid> GetAuthorizationIdsByPublicData(string publicData) => (IList<Guid>) null;

    public virtual void UpdateAudienceAndAccessHashForSSHKey(
      Guid authorizationId,
      string audience = null,
      string accessHash = null)
    {
    }

    public virtual void UpdateAudienceForPATsAndHostAuth(
      Guid orgHostId,
      Guid collectionHostId,
      string oldAudience = null,
      string newAudience = null)
    {
    }

    public virtual void UpdateAudienceAndAccessHashForSSHKeyUsingAccessId(
      Guid authorizationId,
      Guid accessId,
      string audience,
      string accessHash)
    {
    }

    public virtual List<AccessTokenKey> GetSSHKeysForCollection(string audience = null) => (List<AccessTokenKey>) null;

    protected void BindNullableUrl(string parameterName, Uri parameterValue, int maxLength)
    {
      string parameterValue1 = (string) null;
      if (parameterValue != (Uri) null)
        parameterValue1 = parameterValue.ToString();
      this.BindString(parameterName, parameterValue1, maxLength, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
    }

    protected new void TraceEnter(int tracepoint, [CallerMemberName] string methodName = null) => base.TraceEnter(tracepoint, methodName);

    protected new void TraceLeave(int tracePoint, [CallerMemberName] string methodName = null) => base.TraceLeave(tracePoint, methodName);
  }
}
