// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent6
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
  internal class ApplicationLicensingComponent6 : ApplicationLicensingComponent5
  {
    public override IList<Guid> GetScopes()
    {
      try
      {
        this.TraceEnter(1032381, nameof (GetScopes));
        this.PrepareStoredProcedure("prc_GetLicenseScopes");
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new Binders.LicensingScope());
        return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032388, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032389, nameof (GetScopes));
      }
    }

    internal override void CreateScopeAlias(Guid originalScopeId, Guid aliasScopeId)
    {
      try
      {
        this.TraceEnter(1032391, nameof (CreateScopeAlias));
        this.PrepareStoredProcedure("prc_CreateLicenseScopeAlias");
        this.BindGuid("@originalScopeId", originalScopeId);
        this.BindGuid("@aliasScopeId", aliasScopeId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032398, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032399, nameof (CreateScopeAlias));
      }
    }

    public override UserLicense GetUserLicense(Guid scopeId, Guid userId)
    {
      try
      {
        this.TraceEnter(1032201, nameof (GetUserLicense));
        this.PrepareStoredProcedure("prc_GetUserLicense");
        this.BindGuid("@userId", userId);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc);
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

    public override IList<UserLicense> GetUserLicenses(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032231, nameof (GetUserLicenses));
        this.PrepareStoredProcedure("prc_GetUserLicenses");
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc);
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

    public override IList<UserLicense> GetUserLicenses(Guid scopeId, IList<Guid> userIds)
    {
      try
      {
        this.TraceEnter(1032191, nameof (GetUserLicenses));
        this.PrepareStoredProcedure("prc_GetUserLicensesBatch");
        this.BindGuidTable("@userIds", (IEnumerable<Guid>) userIds);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc);
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
        this.BindInt("@top", top);
        this.BindInt("@skip", skip);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc);
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

    public override void UpdateUserLastAccessed(
      Guid scopeId,
      Guid userId,
      DateTimeOffset lastAccessedDate)
    {
      try
      {
        this.TraceEnter(1032281, nameof (UpdateUserLastAccessed));
        this.PrepareStoredProcedure("prc_UpdateUserLastAccessed");
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

    public override void DeleteUserLicense(
      Guid scopeId,
      Guid userId,
      ILicensingEvent licensingEvent)
    {
      try
      {
        this.TraceEnter(1032221, nameof (DeleteUserLicense));
        this.PrepareStoredProcedure("prc_DeleteUserLicense");
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

    public override void TransferUserLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      try
      {
        this.TraceEnter(1032241, nameof (TransferUserLicenses));
        this.PrepareStoredProcedure("prc_TransferUserLicenses");
        this.BindGuid("@scopeId", scopeId);
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

    protected virtual void AddUserLicenseRowBinder(ResultCollection rc) => rc.AddBinder<UserLicense>((ObjectBinder<UserLicense>) new Binders.UserLicenseRowBinderV3());
  }
}
