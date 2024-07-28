// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent2
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent2 : ApplicationExtensionLicensingComponent1
  {
    internal override int UpdateExtensionsAssignedToUserBatchWithCount(
      Guid scopeId,
      Guid userId,
      IEnumerable<string> extensionIds,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      try
      {
        this.TraceEnter(1034181, nameof (UpdateExtensionsAssignedToUserBatchWithCount));
        this.PrepareStoredProcedure("prc_UpdateExtensionsAssignedToUserBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindStringTable("@extensionIds", extensionIds);
        this.BindByte("@source", (byte) source);
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
  }
}
