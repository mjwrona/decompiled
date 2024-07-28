// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent10
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent10 : ApplicationLicensingComponent9
  {
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
        this.BindByte("@originIfAbsent", (byte) originIfAbsent);
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
        this.BindByte("@origin", (byte) origin);
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

    internal override SqlParameter BindUserLicenseTable(
      string parameterName,
      IEnumerable<UserLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserLicense>();
      System.Func<UserLicense, SqlDataRecord> selector = (System.Func<UserLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BaseLicenseComponent.typ_UserLicenseTableV3);
        sqlDataRecord.SetGuid(0, license.UserId);
        sqlDataRecord.SetByte(1, (byte) license.Status);
        sqlDataRecord.SetByte(2, (byte) license.Source);
        sqlDataRecord.SetByte(3, (byte) license.License);
        sqlDataRecord.SetByte(4, (byte) license.Origin);
        sqlDataRecord.SetByte(5, (byte) license.AssignmentSource);
        sqlDataRecord.SetDateTime(6, license.AssignmentDate.DateTime);
        sqlDataRecord.SetDateTime(7, license.DateCreated.DateTime);
        sqlDataRecord.SetDateTime(8, license.LastUpdated.DateTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserLicense_v3", rows.Select<UserLicense, SqlDataRecord>(selector));
    }

    internal override SqlParameter BindUserExtensionLicenseTable(
      string parameterName,
      IEnumerable<UserExtensionLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserExtensionLicense>();
      System.Func<UserExtensionLicense, SqlDataRecord> selector = (System.Func<UserExtensionLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BaseLicenseComponent.typ_UserExtensionLicenseTableV2);
        sqlDataRecord.SetInt32(0, license.InternalScopeId);
        sqlDataRecord.SetGuid(1, license.UserId);
        sqlDataRecord.SetString(2, license.ExtensionId);
        sqlDataRecord.SetByte(3, (byte) license.Source);
        sqlDataRecord.SetByte(4, (byte) license.Status);
        sqlDataRecord.SetByte(5, (byte) license.AssignmentSource);
        sqlDataRecord.SetGuid(6, license.CollectionId);
        sqlDataRecord.SetDateTime(7, license.AssignmentDate.DateTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserExtensionLicense_v2", rows.Select<UserExtensionLicense, SqlDataRecord>(selector));
    }

    protected override void AddUserLicenseRowBinder(ResultCollection rc) => rc.AddBinder<UserLicense>((ObjectBinder<UserLicense>) new Binders.UserLicenseRowBinderV5());

    protected override void AddUserLicenseRowBinder(ResultCollection rc, Guid accountId) => this.AddUserLicenseRowBinder(rc);
  }
}
