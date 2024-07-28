// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent7
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent7 : ApplicationLicensingComponent6
  {
    public override UserLicense GetUserLicense(Guid scopeId, Guid userId)
    {
      try
      {
        this.TraceEnter(1032201, nameof (GetUserLicense));
        this.PrepareStoredProcedure("prc_GetUserLicense");
        this.BindGuid("@scopeId", scopeId);
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
        this.BindGuid("@scopeId", scopeId);
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
        this.BindGuid("@scopeId", scopeId);
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
        this.BindGuid("@scopeId", scopeId);
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

    public override IPagedList<UserLicense> GetUserLicenses(Guid scopeId, string continuationToken)
    {
      int parameterValue = string.IsNullOrEmpty(continuationToken) ? 100 : 1000;
      int result;
      if (!int.TryParse(continuationToken, out result))
        throw new ArgumentException("Invalid continuation token, must be an integer", nameof (continuationToken));
      try
      {
        this.TraceEnter(1032181, nameof (GetUserLicenses));
        this.PrepareStoredProcedure("prc_GetUserLicensesPaged");
        this.BindGuid("@scopeId", scopeId);
        this.BindInt("@top", parameterValue);
        this.BindInt("@skip", result);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc);
        List<UserLicense> items = rc.GetCurrent<UserLicense>().Items;
        string continuationToken1 = items.Count == parameterValue ? (result + items.Count).ToString() : (string) null;
        return (IPagedList<UserLicense>) new PagedList<UserLicense>((IEnumerable<UserLicense>) items, continuationToken1);
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
        this.BindGuid("@scopeId", scopeId);
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
        this.BindGuid("@scopeId", scopeId);
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
        this.BindGuid("@scopeId", scopeId);
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

    public override void ImportScope(
      Guid scopeId,
      List<UserLicense> userLicenses,
      List<UserLicense> previousUserLicenses,
      List<UserExtensionLicense> userExtensionLicenses,
      ILicensingEvent licensingEvent)
    {
      try
      {
        this.TraceEnter(1032251, nameof (ImportScope));
        this.PrepareStoredProcedure("prc_ImportScope");
        this.BindGuid("@scopeId", scopeId);
        this.BindUserLicenseTable("@userLicenses", (IEnumerable<UserLicense>) userLicenses);
        this.BindUserExtensionLicenseTable("@userExtensionLicenses", (IEnumerable<UserExtensionLicense>) userExtensionLicenses);
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032258, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032259, nameof (ImportScope));
      }
    }

    public override void DeleteScope(Guid scopeId, ILicensingEvent licensingEvent)
    {
      try
      {
        this.TraceEnter(1032351, nameof (DeleteScope));
        this.PrepareStoredProcedure("prc_DeleteScope");
        this.BindGuid("@scopeId", scopeId);
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032358, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032359, nameof (DeleteScope));
      }
    }
  }
}
