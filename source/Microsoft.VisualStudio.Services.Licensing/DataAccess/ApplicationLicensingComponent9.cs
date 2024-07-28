// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent9
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent9 : ApplicationLicensingComponent8
  {
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
        this.BindUserLicenseTable("@previousUserLicenses", (IEnumerable<UserLicense>) previousUserLicenses);
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

    internal override List<UserLicense> GetPreviousUserLicenses(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032261, nameof (GetPreviousUserLicenses));
        this.PrepareStoredProcedure("prc_GetPreviousUserLicenses");
        this.BindGuid("@scopeId", scopeId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<UserLicense>((ObjectBinder<UserLicense>) new Binders.PreviousUserLicenseRowBinder());
        return resultCollection.GetCurrent<UserLicense>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032268, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032269, nameof (GetPreviousUserLicenses));
      }
    }

    internal override SqlParameter BindUserLicenseTable(
      string parameterName,
      IEnumerable<UserLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserLicense>();
      System.Func<UserLicense, SqlDataRecord> selector = (System.Func<UserLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BaseLicenseComponent.typ_UserLicenseTableV2);
        sqlDataRecord.SetGuid(0, license.UserId);
        sqlDataRecord.SetByte(1, (byte) license.Status);
        sqlDataRecord.SetByte(2, (byte) license.Source);
        sqlDataRecord.SetByte(3, (byte) license.License);
        sqlDataRecord.SetByte(4, (byte) license.AssignmentSource);
        sqlDataRecord.SetDateTime(5, license.AssignmentDate.DateTime);
        sqlDataRecord.SetDateTime(6, license.DateCreated.DateTime);
        sqlDataRecord.SetDateTime(7, license.LastUpdated.DateTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserLicense_v2", rows.Select<UserLicense, SqlDataRecord>(selector));
    }

    internal override SqlParameter BindUserExtensionLicenseTable(
      string parameterName,
      IEnumerable<UserExtensionLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserExtensionLicense>();
      System.Func<UserExtensionLicense, SqlDataRecord> selector = (System.Func<UserExtensionLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BaseLicenseComponent.typ_UserExtensionLicenseTableV2);
        sqlDataRecord.SetInt32(0, license.InternalScopeId);
        sqlDataRecord.SetGuid(1, license.UserId);
        sqlDataRecord.SetString(2, license.ExtensionId);
        sqlDataRecord.SetByte(3, (byte) license.Source);
        sqlDataRecord.SetByte(4, (byte) license.Status);
        sqlDataRecord.SetByte(5, (byte) license.AssignmentSource);
        sqlDataRecord.SetGuid(6, license.CollectionId);
        sqlDataRecord.SetDateTime(7, license.AssignmentDate.DateTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserExtensionLicense_v2", rows.Select<UserExtensionLicense, SqlDataRecord>(selector));
    }
  }
}
