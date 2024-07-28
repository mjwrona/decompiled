// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent11
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent11 : DelegatedAuthorizationComponent10
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
        this.BindString("@scope", scope, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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
        this.BindString("@scope", scope, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
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

    public override AccessTokenKey GetAccessTokenKey(Guid authorizationId, bool isPublic = false)
    {
      try
      {
        this.TraceEnter(1048549, nameof (GetAccessTokenKey));
        this.PrepareStoredProcedure("prc_GetAccessTokenKey");
        this.BindGuid("@authorizationId", authorizationId);
        this.BindBoolean("@isPublic", isPublic);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder6());
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
        this.BindBoolean("@isPublic", isPublic);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder6());
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
        this.BindBoolean("@isPublic", isPublic);
        this.BindBoolean("@includePublicData", includePublicData);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder6());
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

    public override AccessTokenData GetAccessTokenByKey(string accessTokenHash, bool isPublic = false)
    {
      AccessTokenData accessTokenByKey = new AccessTokenData();
      try
      {
        this.TraceEnter(1048542, nameof (GetAccessTokenByKey));
        this.PrepareStoredProcedure("prc_GetAccessTokenByKey");
        this.BindString("@accessHash", accessTokenHash, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindBoolean("@isPublic", isPublic);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessToken>((ObjectBinder<AccessToken>) new AccessTokenBinder());
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder3());
          accessTokenByKey.AccessToken = resultCollection.GetCurrent<AccessToken>().Items.FirstOrDefault<AccessToken>();
          resultCollection.NextResult();
          accessTokenByKey.Authorization = resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (InvalidAccessTokenKeyException ex)
      {
        if (isPublic)
          this.TraceException(1048543, (Exception) ex, TraceLevel.Info);
        else
          this.TraceException(1048543, (Exception) ex);
        throw;
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

    public override Authorization GetAuthorization(Guid authorizationId)
    {
      try
      {
        this.TraceEnter(1048561, nameof (GetAuthorization));
        this.PrepareStoredProcedure("prc_GetAuthorization");
        this.BindGuid("@authorizationId", authorizationId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder3());
          return resultCollection.GetCurrent<Authorization>().Items.FirstOrDefault<Authorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048562, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048563, nameof (GetAuthorization));
      }
    }
  }
}
