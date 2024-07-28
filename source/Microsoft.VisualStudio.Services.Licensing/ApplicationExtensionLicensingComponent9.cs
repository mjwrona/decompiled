// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent9
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent9 : ApplicationExtensionLicensingComponent8
  {
    internal override void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid CollectionId)
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
        this.BindAssignmentSource(assignmentSource);
        this.BindGuid("@CollectionId", CollectionId);
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

    public override int UpdateExtensionsAssignedToUserBatchWithCount(
      Guid scopeId,
      Guid userId,
      IEnumerable<string> extensionIds,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      try
      {
        this.TraceEnter(1034181, nameof (UpdateExtensionsAssignedToUserBatchWithCount));
        this.PrepareStoredProcedure("prc_UpdateExtensionsAssignedToUserBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindStringTable("@extensionIds", extensionIds);
        this.BindByte("@source", (byte) source);
        this.BindAssignmentSource(assignmentSource);
        this.BindGuid("@collectionId", collectionId);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        this.TraceException(1034188, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034189, nameof (UpdateExtensionsAssignedToUserBatchWithCount));
      }
    }

    public override void UpdateUserStatusBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid CollectionId)
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
        this.BindAssignmentSource(assignmentSource);
        this.BindGuid("@CollectionId", CollectionId);
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

    internal override void AssignExtensionLicenseToUser(
      Guid scopeId,
      Guid userId,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      try
      {
        this.TraceEnter(1034111, nameof (AssignExtensionLicenseToUser));
        this.PrepareStoredProcedure("prc_UpsertUserExtensionLicense");
        this.BindGuid("@userId", userId);
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@source", (byte) source);
        this.BindAssignmentSource(assignmentSource);
        this.BindGuid("@collectionId", collectionId);
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

    public override void AssignExtensionLicenseToUserBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      try
      {
        this.TraceEnter(1034141, nameof (AssignExtensionLicenseToUserBatch));
        this.PrepareStoredProcedure("prc_UpsertUserExtensionLicenseBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable(nameof (userIds), userIds);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@source", (byte) source);
        this.BindAssignmentSource(assignmentSource);
        this.BindGuid("@collectionId", collectionId);
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

    internal override void AddUserExtensionLicenseRowBinder(ResultCollection rc, Guid scopeId) => rc.AddBinder<UserExtensionLicense>((ObjectBinder<UserExtensionLicense>) new ExtensionBinders.UserExtensionLicenseRowBinderV2(scopeId));

    internal override void AddUserExtensionsRowBinder(ResultCollection rc) => rc.AddBinder<KeyValuePair<Guid, ExtensionSource>>((ObjectBinder<KeyValuePair<Guid, ExtensionSource>>) new ExtensionBinders.UserExtensionsRowBinderV2());
  }
}
