// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent8
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent8 : ApplicationLicensingComponent7
  {
    public override UserLicense SetUserLicense(
      Guid scopeId,
      Guid userId,
      LicensingSource source,
      int license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      AccountUserStatus statusIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity)
    {
      try
      {
        this.TraceEnter(1032211, nameof (SetUserLicense));
        this.PrepareStoredProcedure("prc_UpsertUserLicense");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindByte("@source", (byte) source);
        this.BindByte("@license", (byte) license);
        this.BindAssignmentSource(assignmentSource);
        this.BindByte("@statusIfAbsent", (byte) statusIfAbsent);
        this.BindEventData(licensingEvent);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          rc.AddBinder<LicenseEventRow>((ObjectBinder<LicenseEventRow>) new Binders.LicenseEventRowBinder());
          rc.NextResult();
          this.AddUserLicenseRowBinder(rc, scopeId);
          return rc.GetCurrent<UserLicense>().Items.FirstOrDefault<UserLicense>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1032218, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032219, nameof (SetUserLicense));
      }
    }

    public override void AddUser(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      License licenseIfAbsent,
      AssignmentSource assignmentSourceIfAbsent,
      LicensingOrigin originIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity)
    {
      try
      {
        this.TraceEnter(1032141, nameof (AddUser));
        this.PrepareStoredProcedure("prc_UpsertUserStatus");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindByte("@status", (byte) status);
        this.BindByte("@sourceIfAbsent", (byte) licenseIfAbsent.Source);
        this.BindByte("@licenseIfAbsent", (byte) licenseIfAbsent.GetLicenseAsInt32());
        this.BindByte("@assignmentSourceIfAbsent", (byte) assignmentSourceIfAbsent);
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032148, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032149, nameof (AddUser));
      }
    }

    public override List<UserLicense> GetPreviousUserLicenses(Guid scopeId, IList<Guid> userIds)
    {
      try
      {
        this.TraceEnter(1032161, nameof (GetPreviousUserLicenses));
        this.PrepareStoredProcedure("prc_GetPreviousUserLicensesBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable("@userIds", (IEnumerable<Guid>) userIds);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<UserLicense>((ObjectBinder<UserLicense>) new Binders.PreviousUserLicenseRowBinder());
        return resultCollection.GetCurrent<UserLicense>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032168, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032169, nameof (GetPreviousUserLicenses));
      }
    }

    protected override void AddUserLicenseRowBinder(ResultCollection rc) => rc.AddBinder<UserLicense>((ObjectBinder<UserLicense>) new Binders.UserLicenseRowBinderV4());

    protected override void AddUserLicenseRowBinder(ResultCollection rc, Guid accountId) => this.AddUserLicenseRowBinder(rc);
  }
}
