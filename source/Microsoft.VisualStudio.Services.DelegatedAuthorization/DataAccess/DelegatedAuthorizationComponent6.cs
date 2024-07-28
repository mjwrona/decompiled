// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent6
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
  internal class DelegatedAuthorizationComponent6 : DelegatedAuthorizationComponent5
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
        this.BindDateTime("@SecretValidTo", registration.SecretValidTo.Value.DateTime);
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder3());
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder3());
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder3());
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
          resultCollection.AddBinder<Registration>((ObjectBinder<Registration>) new RegistrationBinder3());
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
  }
}
