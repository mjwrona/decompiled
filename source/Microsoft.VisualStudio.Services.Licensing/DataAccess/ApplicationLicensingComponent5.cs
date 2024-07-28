// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent5
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent5 : ApplicationLicensingComponent4
  {
    public override IList<UserLicense> GetUserLicenses(Guid scopeId, IList<Guid> userIds)
    {
      try
      {
        this.TraceEnter(1032191, nameof (GetUserLicenses));
        this.PrepareStoredProcedure("prc_GetUserLicensesBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable("@userIds", (IEnumerable<Guid>) userIds);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc, scopeId);
        return (IList<UserLicense>) rc.GetCurrent<UserLicense>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032198, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032199, nameof (GetUserLicenses));
      }
    }

    public override IList<UserLicense> GetUserLicenses(Guid scopeId, int top, int skip)
    {
      try
      {
        this.TraceEnter(1032181, nameof (GetUserLicenses));
        this.PrepareStoredProcedure("prc_GetUserLicensesPaged");
        this.BindGuid("@scopeId", scopeId);
        this.BindInt("@top", top);
        this.BindInt("@skip", skip);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc, scopeId);
        return (IList<UserLicense>) rc.GetCurrent<UserLicense>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032188, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032189, nameof (GetUserLicenses));
      }
    }

    public override void TransferUserLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      try
      {
        this.TraceEnter(1032241, nameof (TransferUserLicenses));
        this.PrepareStoredProcedure("prc_TransferAccountUserLicenses");
        this.BindGuid("@accountId", scopeId);
        this.BindKeyValuePairGuidGuidTable("@identities", userIdTransferMap);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032248, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032249, nameof (TransferUserLicenses));
      }
    }
  }
}
