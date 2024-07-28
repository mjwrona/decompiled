// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent8
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent8 : ApplicationExtensionLicensingComponent7
  {
    public override IList<UserExtensionLicense> GetUserExtensionLicenses(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032301, nameof (GetUserExtensionLicenses));
        this.PrepareStoredProcedure("prc_GetUserExtensionLicenses");
        this.BindGuid("@scopeId", scopeId);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserExtensionLicenseRowBinder(rc, scopeId);
        return (IList<UserExtensionLicense>) rc.GetCurrent<UserExtensionLicense>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032308, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032309, nameof (GetUserExtensionLicenses));
      }
    }

    public override void TransferUserExtensionLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      try
      {
        this.TraceEnter(1032310, nameof (TransferUserExtensionLicenses));
        this.PrepareStoredProcedure("prc_TransferUserExtensionLicenses");
        this.BindGuid("@scopeId", scopeId);
        this.BindKeyValuePairGuidGuidTable("@identities", userIdTransferMap);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032311, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032312, nameof (TransferUserExtensionLicenses));
      }
    }
  }
}
