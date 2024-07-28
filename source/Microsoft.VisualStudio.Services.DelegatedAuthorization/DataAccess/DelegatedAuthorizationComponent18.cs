// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent18
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent18 : DelegatedAuthorizationComponent17
  {
    public override AccessTokenKeyPage ListAccessTokensByPage(
      Guid userId,
      TokenPageRequest tokenPageRequest,
      bool isPublic = false,
      bool includePublicData = false)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForNull<TokenPageRequest>(tokenPageRequest, nameof (tokenPageRequest));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(tokenPageRequest.PageRequestTimeStamp, "PageRequestTimeStamp");
      AccessTokenKeyPage accessTokenKeyPage = new AccessTokenKeyPage();
      try
      {
        this.TraceEnter(1048537, nameof (ListAccessTokensByPage));
        DateTime exact = DateTime.ParseExact(tokenPageRequest.PageRequestTimeStamp, "R", (IFormatProvider) CultureInfo.InvariantCulture);
        this.PrepareStoredProcedure("prc_GetAccessTokensByUserIdByPage");
        this.BindGuid("@userId", userId);
        this.BindBoolean("@includePublicData", includePublicData);
        this.BindBoolean("@isPublic", isPublic);
        this.BindInt("@displayFilterOption", (int) tokenPageRequest.DisplayFilterOption);
        this.BindInt("@createdByOption", (int) tokenPageRequest.CreatedByOption);
        this.BindInt("@sortByOption", (int) tokenPageRequest.SortByOption);
        this.BindBoolean("@isSortAscending", tokenPageRequest.IsSortAscending);
        this.BindInt("@startRowNumber", tokenPageRequest.StartRowNumber);
        this.BindInt("@pageSize", tokenPageRequest.PageSize);
        this.BindDateTime("@pageRequestTimestamp", exact);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder6());
          resultCollection.AddBinder<int>((ObjectBinder<int>) new NextRowNumberBinder());
          List<AccessTokenKey> items = resultCollection.GetCurrent<AccessTokenKey>().Items;
          accessTokenKeyPage.AccessTokenKeyList = (IList<AccessTokenKey>) items;
          resultCollection.NextResult();
          accessTokenKeyPage.NextRowNumber = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048538, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048539, nameof (ListAccessTokensByPage));
      }
      return accessTokenKeyPage;
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

    public override List<string> RevokeAllAccessTokenKeysForUser(Guid identityId)
    {
      try
      {
        this.TraceEnter(1048555, nameof (RevokeAllAccessTokenKeysForUser));
        this.PrepareStoredProcedure("prc_RevokeAllAccessTokenKeysForUser");
        this.BindGuid("@identityId", identityId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<string>((ObjectBinder<string>) new AccessTokenHashStringBinder());
          return resultCollection.GetCurrent<string>().Items;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048556, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048557, nameof (RevokeAllAccessTokenKeysForUser));
      }
    }
  }
}
