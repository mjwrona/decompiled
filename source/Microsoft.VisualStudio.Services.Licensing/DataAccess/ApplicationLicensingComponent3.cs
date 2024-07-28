// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent3
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
  internal class ApplicationLicensingComponent3 : ApplicationLicensingComponent2
  {
    public override UserLicense GetUserLicense(Guid scopeId, Guid userId)
    {
      try
      {
        this.TraceEnter(1032201, nameof (GetUserLicense));
        this.PrepareStoredProcedure("prc_GetUserLicense");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc, scopeId);
        return rc.GetCurrent<UserLicense>().Items.FirstOrDefault<UserLicense>();
      }
      catch (Exception ex)
      {
        this.TraceException(1032208, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032209, nameof (GetUserLicense));
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

    public override void DeleteUserLicense(
      Guid scopeId,
      Guid userId,
      ILicensingEvent licensingEvent)
    {
      try
      {
        this.TraceEnter(1032221, nameof (DeleteUserLicense));
        this.PrepareStoredProcedure("prc_DeleteUserLicense");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032228, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032229, nameof (DeleteUserLicense));
      }
    }

    public override IList<UserLicense> GetUserLicenses(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032231, nameof (GetUserLicenses));
        this.PrepareStoredProcedure("prc_GetUserLicenses");
        this.BindGuid("@scopeId", scopeId);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc, scopeId);
        return (IList<UserLicense>) rc.GetCurrent<UserLicense>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032238, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032239, nameof (GetUserLicenses));
      }
    }

    public override IList<AccountLicenseCount> GetUserLicensesDistribution(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032271, nameof (GetUserLicensesDistribution));
        this.PrepareStoredProcedure("prc_GetUserLicensesDistribution");
        this.BindGuid("@scopeId", scopeId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<AccountLicenseCount>((ObjectBinder<AccountLicenseCount>) new Binders.AccountLicenseCountBinder());
        return (IList<AccountLicenseCount>) (resultCollection.GetCurrent<AccountLicenseCount>().Items ?? new List<AccountLicenseCount>());
      }
      catch (Exception ex)
      {
        this.TraceException(1032278, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032279, nameof (GetUserLicensesDistribution));
      }
    }

    public override void UpdateUserLastAccessed(
      Guid scopeId,
      Guid userId,
      DateTimeOffset lastAccessedDate)
    {
      try
      {
        this.TraceEnter(1032281, nameof (UpdateUserLastAccessed));
        this.PrepareStoredProcedure("prc_UpdateUserLastAccessed");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindDateTime("@lastAccessedDate", lastAccessedDate.UtcDateTime);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032288, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032289, nameof (UpdateUserLastAccessed));
      }
    }

    public override void CreateScope(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032291, nameof (CreateScope));
        this.PrepareStoredProcedure("prc_CreateLicenseScope");
        this.BindGuid("@scopeId", scopeId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032298, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032299, nameof (CreateScope));
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

    public override void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      ILicensingEvent licensingEvent)
    {
      try
      {
        this.TraceEnter(1032151, nameof (UpdateUserStatus));
        this.PrepareStoredProcedure("prc_UpdateUserStatus");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindByte("@status", (byte) status);
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032158, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032159, nameof (UpdateUserStatus));
      }
    }
  }
}
