// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent19
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent19 : DelegatedAuthorizationComponent18
  {
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
        if (accessId.HasValue && accessId.Value != Guid.Empty)
          this.BindGuid("@accessId", accessId.Value);
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
        if (newAccessId.HasValue && newAccessId.Value != Guid.Empty)
          this.BindGuid("@newAccessId", newAccessId.Value);
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

    public override HostAuthorization UpdateHostAuthorization(
      Guid clientId,
      Guid hostId,
      Guid? newId = null)
    {
      try
      {
        this.TraceEnter(1048543, nameof (UpdateHostAuthorization));
        this.PrepareStoredProcedure("prc_UpdateHostAuthorization");
        this.BindGuid("@registrationId", clientId);
        this.BindGuid("@hostId", hostId);
        if (newId.HasValue && newId.Value != Guid.Empty)
          this.BindGuid("@newId", newId.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<HostAuthorization>((ObjectBinder<HostAuthorization>) new HostAuthorizationBinder());
          return resultCollection.GetCurrent<HostAuthorization>().Items.FirstOrDefault<HostAuthorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048544, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048545, nameof (UpdateHostAuthorization));
      }
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
        this.BindString("@scope", scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindDateTime("@validFrom", validFrom);
        this.BindDateTime("@validTo", validTo);
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        if (authorizationId.HasValue)
          this.BindGuid("@authorizationId", authorizationId.Value);
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
        this.BindString("@scope", scope, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindDateTime("@validTo", validTo);
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@source", source, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindBoolean("@setAppScopes", !isRequestedByTfsPatWebUI);
        if (authorizationId.HasValue)
          this.BindGuid("@authorizationId", authorizationId.Value);
        if (accessId.HasValue)
          this.BindGuid("@accessId", accessId.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessToken>((ObjectBinder<AccessToken>) new AccessTokenBinder());
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder3());
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
  }
}
