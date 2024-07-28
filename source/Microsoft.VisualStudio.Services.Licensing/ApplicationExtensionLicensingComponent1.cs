// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent1
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent1 : ApplicationExtensionLicensingComponent
  {
    public override IList<UserExtensionLicense> GetUserExtensionLicenses(
      Guid scopeId,
      Guid userId,
      UserExtensionLicenseStatus status)
    {
      try
      {
        this.TraceEnter(1034101, nameof (GetUserExtensionLicenses));
        this.PrepareStoredProcedure("prc_GetUserExtensions");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindByte("@status", (byte) status);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          this.AddUserExtensionLicenseRowBinder(rc);
          return (IList<UserExtensionLicense>) rc.GetCurrent<UserExtensionLicense>().Items;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1034108, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034109, nameof (GetUserExtensionLicenses));
      }
    }

    internal override void AssignExtensionLicenseToUser(
      Guid scopeId,
      Guid userId,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      try
      {
        this.TraceEnter(1034111, nameof (AssignExtensionLicenseToUser));
        this.PrepareStoredProcedure("prc_UpsertUserExtensionLicense");
        this.BindGuid("@userId", userId);
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@source", (byte) source);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1034118, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034119, nameof (AssignExtensionLicenseToUser));
      }
    }

    internal override void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      try
      {
        this.TraceEnter(1034121, nameof (UpdateUserStatus));
        this.PrepareStoredProcedure("prc_UpdateUserExtensionLicenseStatus");
        this.BindGuid("@userId", userId);
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@status", (byte) status);
        this.BindByte("@source", (byte) source);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1034128, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034129, nameof (UpdateUserStatus));
      }
    }

    public override int GetExtensionUsageCountInAccount(Guid scopeId, string extensionId)
    {
      try
      {
        this.TraceEnter(1034131, nameof (GetExtensionUsageCountInAccount));
        this.PrepareStoredProcedure("prc_GetExtensionUsageCount");
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@source", (byte) 1);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        this.TraceException(1034138, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034139, nameof (GetExtensionUsageCountInAccount));
      }
    }

    internal override void AssignExtensionLicenseToUserBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      try
      {
        this.TraceEnter(1034141, nameof (AssignExtensionLicenseToUserBatch));
        this.PrepareStoredProcedure("prc_UpsertUserExtensionLicenseBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable(nameof (userIds), userIds);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@source", (byte) source);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1034148, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034149, nameof (AssignExtensionLicenseToUserBatch));
      }
    }

    internal override void UpdateUserStatusBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      try
      {
        this.TraceEnter(1034151, nameof (UpdateUserStatusBatch));
        this.PrepareStoredProcedure("prc_UpdateUserExtensionLicenseStatusBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable(nameof (userIds), userIds);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@status", (byte) status);
        this.BindByte("@source", (byte) source);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1034158, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034159, nameof (UpdateUserStatusBatch));
      }
    }

    internal override void UpdateExtensionsAssignedToUserBatch(
      Guid scopeId,
      Guid userIds,
      IEnumerable<string> extensionIds,
      LicensingSource source)
    {
      try
      {
        this.TraceEnter(1034161, nameof (UpdateExtensionsAssignedToUserBatch));
        this.PrepareStoredProcedure("prc_UpdateExtensionsAssignedToUserBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userIds);
        this.BindStringTable("@extensionIds", extensionIds);
        this.BindByte("@source", (byte) source);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1034168, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034169, nameof (UpdateExtensionsAssignedToUserBatch));
      }
    }

    public override IList<UserExtensionLicense> FilterUsersWithExtensionBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId)
    {
      try
      {
        this.TraceEnter(1034171, nameof (FilterUsersWithExtensionBatch));
        this.PrepareStoredProcedure("prc_GetUsersForExtensionBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable("@userIds", userIds);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          this.AddUserExtensionLicenseRowBinder(rc);
          return (IList<UserExtensionLicense>) rc.GetCurrent<UserExtensionLicense>().Items;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1034178, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034179, nameof (FilterUsersWithExtensionBatch));
      }
    }

    internal void AddUserExtensionLicenseRowBinder(ResultCollection rc) => this.AddUserExtensionLicenseRowBinder(rc, this.RequestContext.ServiceHost.InstanceId);

    internal virtual void AddUserExtensionLicenseRowBinder(ResultCollection rc, Guid scopeId) => rc.AddBinder<UserExtensionLicense>((ObjectBinder<UserExtensionLicense>) new ExtensionBinders.UserExtensionLicenseRowBinder(scopeId));

    internal void AddAccountExtensionsAssignedRowBinder(ResultCollection rc) => rc.AddBinder<AccountExtensionCount>((ObjectBinder<AccountExtensionCount>) new ExtensionBinders.AccountExtensionsAssignedRowBinder());

    internal virtual void AddUserExtensionsRowBinder(ResultCollection rc) => rc.AddBinder<KeyValuePair<Guid, ExtensionSource>>((ObjectBinder<KeyValuePair<Guid, ExtensionSource>>) new ExtensionBinders.UserExtensionsRowBinder());
  }
}
