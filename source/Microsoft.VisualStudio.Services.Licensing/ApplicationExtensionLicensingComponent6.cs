// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent6
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent6 : ApplicationExtensionLicensingComponent5
  {
    public override IList<AccountExtensionCount> GetAccountExtensionCount(
      Guid scopeId,
      UserExtensionLicenseStatus status)
    {
      try
      {
        this.TraceEnter(1034200, nameof (GetAccountExtensionCount));
        this.PrepareStoredProcedure("prc_GetAccountExtensions");
        this.BindGuid("@scopeId", scopeId);
        this.BindByte("@status", (byte) status);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          this.AddAccountExtensionsAssignedRowBinder(rc);
          return (IList<AccountExtensionCount>) rc.GetCurrent<AccountExtensionCount>().Items;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1034202, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034201, nameof (GetAccountExtensionCount));
      }
    }

    public override IDictionary<Guid, IList<ExtensionSource>> GetExtensionsForUsersBatch(
      Guid scopeId,
      IList<Guid> userIds)
    {
      try
      {
        this.TraceEnter(1034191, nameof (GetExtensionsForUsersBatch));
        this.PrepareStoredProcedure("prc_GetExtensionsForUsersBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable("@userIds", (IEnumerable<Guid>) userIds);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          this.AddUserExtensionsRowBinder(rc);
          return (IDictionary<Guid, IList<ExtensionSource>>) rc.GetCurrent<KeyValuePair<Guid, ExtensionSource>>().Items.GroupBy<KeyValuePair<Guid, ExtensionSource>, Guid, ExtensionSource, KeyValuePair<Guid, IList<ExtensionSource>>>((System.Func<KeyValuePair<Guid, ExtensionSource>, Guid>) (item => item.Key), (System.Func<KeyValuePair<Guid, ExtensionSource>, ExtensionSource>) (item => item.Value), (Func<Guid, IEnumerable<ExtensionSource>, KeyValuePair<Guid, IList<ExtensionSource>>>) ((key, values) => new KeyValuePair<Guid, IList<ExtensionSource>>(key, (IList<ExtensionSource>) values.ToList<ExtensionSource>()))).ToDictionary<KeyValuePair<Guid, IList<ExtensionSource>>, Guid, IList<ExtensionSource>>((System.Func<KeyValuePair<Guid, IList<ExtensionSource>>, Guid>) (item => item.Key), (System.Func<KeyValuePair<Guid, IList<ExtensionSource>>, IList<ExtensionSource>>) (item => item.Value));
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1034198, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034199, nameof (GetExtensionsForUsersBatch));
      }
    }

    internal void CopyUserExtensionLicense(
      IEnumerable<UserExtensionLicense> userExtensionLicenses)
    {
      this.PrepareStoredProcedure("prc_CopyUserExtensionLicense");
      this.BindUserExtensionLicenseTable("@entries", userExtensionLicenses);
      this.ExecuteNonQuery();
    }
  }
}
