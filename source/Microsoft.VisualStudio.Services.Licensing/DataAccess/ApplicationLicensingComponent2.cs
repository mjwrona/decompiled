// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent2
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent2 : ApplicationLicensingComponent
  {
    protected override void AddUserLicenseRowBinder(ResultCollection rc, Guid accountId) => rc.AddBinder<UserLicense>((ObjectBinder<UserLicense>) new Binders.UserLicenseRowBinderV2(accountId));

    public override void UpdateUserLastAccessed(
      Guid scopeId,
      Guid userId,
      DateTimeOffset lastAccessedDate)
    {
      try
      {
        this.TraceEnter(1032141, nameof (UpdateUserLastAccessed));
        this.PrepareStoredProcedure("prc_UpdateUserLastAccessed");
        this.BindGuid("@userId", userId);
        this.BindDateTime("@lastAccessedDate", lastAccessedDate.UtcDateTime);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032148, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032149, nameof (UpdateUserLastAccessed));
      }
    }
  }
}
