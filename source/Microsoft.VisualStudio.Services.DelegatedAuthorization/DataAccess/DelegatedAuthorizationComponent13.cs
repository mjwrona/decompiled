// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent13
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
  internal class DelegatedAuthorizationComponent13 : DelegatedAuthorizationComponent12
  {
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
        this.BindString("@scope", scope, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@isPublic", isPublic);
        if (validTo.HasValue && validTo.Value > DateTime.MinValue)
          this.BindNullableDateTime("@validTo", new DateTime?(validTo.Value));
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder6());
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
        this.BindString("@scope", scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
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
        this.BindString("@scope", scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
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
  }
}
