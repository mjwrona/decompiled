// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent11
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent11 : ApplicationLicensingComponent10
  {
    public override IList<UserLicenseCount> GetUserLicenseUsage(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032271, nameof (GetUserLicenseUsage));
        this.PrepareStoredProcedure("prc_GetUserLicenseUsage");
        this.BindGuid("@scopeId", scopeId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<UserLicenseCount>((ObjectBinder<UserLicenseCount>) new Binders.UserLicenseCountBinder());
        return (IList<UserLicenseCount>) (resultCollection.GetCurrent<UserLicenseCount>().Items ?? new List<UserLicenseCount>());
      }
      catch (Exception ex)
      {
        this.TraceException(1032278, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032279, nameof (GetUserLicenseUsage));
      }
    }
  }
}
