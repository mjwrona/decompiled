// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent2
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent2 : DelegatedAuthorizationComponent
  {
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
  }
}
