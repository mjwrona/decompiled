// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent5
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent5 : ApplicationExtensionLicensingComponent4
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
  }
}
