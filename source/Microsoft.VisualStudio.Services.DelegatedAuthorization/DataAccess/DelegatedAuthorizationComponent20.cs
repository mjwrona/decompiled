// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent20
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent20 : DelegatedAuthorizationComponent19
  {
    public override void UpdateAudienceAndAccessHashForSSHKeyUsingAccessId(
      Guid authorizationId,
      Guid accessId,
      string audience,
      string accessHash)
    {
      try
      {
        this.TraceEnter(1048567, nameof (UpdateAudienceAndAccessHashForSSHKeyUsingAccessId));
        this.PrepareStoredProcedure("prc_UpdateAudienceAndAccessHashForSSHKeyUsingAccessId");
        this.BindGuid("@authorizationId", authorizationId);
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@accessHash", accessHash, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@accessId", accessId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1048568, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048569, nameof (UpdateAudienceAndAccessHashForSSHKeyUsingAccessId));
      }
    }

    internal override IDictionary<int, string> GetRedirectUriForRegistration(Guid registrationId)
    {
      IDictionary<int, string> uriForRegistration = (IDictionary<int, string>) new Dictionary<int, string>();
      string sqlStatement = "SELECT [Id], [Location] FROM [dbo].[tbl_DelegatedAuthorizationRegistrationRedirectLocation] with(nolock) WHERE [PartitionId] = 1 AND [RegistrationId] = @registrationId";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement, 1);
      this.BindGuid("@registrationId", registrationId);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        if (sqlDataReader.HasRows)
        {
          while (sqlDataReader.Read())
            uriForRegistration[this.ConvertValue<int>(sqlDataReader["Id"])] = this.ConvertValue<string>(sqlDataReader["Location"]).ToLower();
        }
        sqlDataReader.Close();
      }
      return uriForRegistration;
    }

    private TType ConvertValue<TType>(object value)
    {
      Type nullableType = typeof (TType);
      Type type1 = Nullable.GetUnderlyingType(nullableType);
      if ((object) type1 == null)
        type1 = nullableType;
      Type type2 = type1;
      return (TType) (value == null || value == DBNull.Value ? (type2 == typeof (string) ? (object) string.Empty : Activator.CreateInstance(type2)) : Convert.ChangeType(value, type2));
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder5());
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
        this.BindString("@scope", scopes, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder5());
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
  }
}
