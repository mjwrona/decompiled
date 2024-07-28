// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent3
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent3 : DelegatedAuthorizationComponent2
  {
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
        this.TraceException(1048538, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048539, nameof (IssueAccessToken));
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
          resultCollection.AddBinder<Authorization>((ObjectBinder<Authorization>) new AuthorizationBinder());
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

    public override void CreateAccessTokenKey(
      Guid accessId,
      string tokenHash,
      string displayName,
      Guid identityId,
      bool isPublic = false,
      string publicData = null)
    {
      try
      {
        this.TraceEnter(1048540, nameof (CreateAccessTokenKey));
        this.PrepareStoredProcedure("prc_CreatetDelegatedAuthorizationAccessKey");
        this.BindGuid("@accessId", accessId);
        this.BindString("@accessHash", tokenHash, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@displayName", displayName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(1048541, nameof (CreateAccessTokenKey));
      }
    }
  }
}
