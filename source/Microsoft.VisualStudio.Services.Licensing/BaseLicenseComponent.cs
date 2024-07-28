// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.BaseLicenseComponent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public abstract class BaseLicenseComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_UserLicenseTable = new SqlMetaData[7]
    {
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Source", SqlDbType.TinyInt),
      new SqlMetaData("License", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentDate", SqlDbType.DateTime),
      new SqlMetaData("DateCreated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime)
    };
    protected static readonly SqlMetaData[] typ_UserLicenseTableV2 = new SqlMetaData[8]
    {
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Source", SqlDbType.TinyInt),
      new SqlMetaData("License", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentSource", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentDate", SqlDbType.DateTime),
      new SqlMetaData("DateCreated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime)
    };
    protected static readonly SqlMetaData[] typ_UserLicenseTableV3 = new SqlMetaData[9]
    {
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Source", SqlDbType.TinyInt),
      new SqlMetaData("License", SqlDbType.TinyInt),
      new SqlMetaData("Origin", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentSource", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentDate", SqlDbType.DateTime),
      new SqlMetaData("DateCreated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime)
    };
    protected static readonly SqlMetaData[] typ_UserLicenseTableV4 = new SqlMetaData[10]
    {
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Source", SqlDbType.TinyInt),
      new SqlMetaData("License", SqlDbType.TinyInt),
      new SqlMetaData("Origin", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentSource", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentDate", SqlDbType.DateTime),
      new SqlMetaData("DateCreated", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime),
      new SqlMetaData("LastAccessed", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_UserExtensionLicenseTable = new SqlMetaData[7]
    {
      new SqlMetaData("InternalScopeId", SqlDbType.Int),
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ExtensionId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Source", SqlDbType.TinyInt),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("CollectionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignmentDate", SqlDbType.DateTime)
    };
    protected static readonly SqlMetaData[] typ_UserExtensionLicenseTableV2 = new SqlMetaData[8]
    {
      new SqlMetaData("InternalScopeId", SqlDbType.Int),
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ExtensionId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Source", SqlDbType.TinyInt),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentSource", SqlDbType.TinyInt),
      new SqlMetaData("CollectionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignmentDate", SqlDbType.DateTime)
    };
    protected static readonly SqlMetaData[] typ_UserExtensionLicenseTableV3 = new SqlMetaData[9]
    {
      new SqlMetaData("InternalScopeId", SqlDbType.Int),
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ExtensionId", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Source", SqlDbType.TinyInt),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("AssignmentSource", SqlDbType.TinyInt),
      new SqlMetaData("CollectionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("AssignmentDate", SqlDbType.DateTime),
      new SqlMetaData("LastUpdated", SqlDbType.DateTime)
    };

    [ExcludeFromCodeCoverage]
    public virtual IVssRequestContext ComponentRequestContext => this.RequestContext;

    protected override string TraceArea => "Licensing";

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure) => base.PrepareStoredProcedure("Licensing." + storedProcedure);

    protected void BindAssignmentSource(AssignmentSource assignmentSource)
    {
      if (assignmentSource == AssignmentSource.None)
        return;
      this.BindByte("@assignmentSource", (byte) assignmentSource);
    }

    protected void BindEventData(ILicensingEvent licensingEvent)
    {
      this.BindString("@eventTypeFamily", licensingEvent.EventTypeFamily, 100, false, SqlDbType.NVarChar);
      this.BindString("@eventTypeDescriptor", licensingEvent.EventTypeDescriptor, 300, false, SqlDbType.NVarChar);
      this.BindString("@eventData", licensingEvent.EventData.Serialize(), -1, false, SqlDbType.NVarChar);
    }

    internal virtual SqlParameter BindUserLicenseTable(
      string parameterName,
      IEnumerable<UserLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserLicense>();
      System.Func<UserLicense, SqlDataRecord> selector = (System.Func<UserLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BaseLicenseComponent.typ_UserLicenseTable);
        sqlDataRecord.SetGuid(0, license.UserId);
        sqlDataRecord.SetByte(1, (byte) license.Status);
        sqlDataRecord.SetByte(2, (byte) license.Source);
        sqlDataRecord.SetByte(3, (byte) license.License);
        sqlDataRecord.SetDateTime(4, license.AssignmentDate.DateTime);
        sqlDataRecord.SetDateTime(5, license.DateCreated.DateTime);
        sqlDataRecord.SetDateTime(6, license.LastUpdated.DateTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserLicense", rows.Select<UserLicense, SqlDataRecord>(selector));
    }

    internal virtual SqlParameter BindUserExtensionLicenseTable(
      string parameterName,
      IEnumerable<UserExtensionLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserExtensionLicense>();
      System.Func<UserExtensionLicense, SqlDataRecord> selector = (System.Func<UserExtensionLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BaseLicenseComponent.typ_UserExtensionLicenseTable);
        sqlDataRecord.SetInt32(0, license.InternalScopeId);
        sqlDataRecord.SetGuid(1, license.UserId);
        sqlDataRecord.SetString(2, license.ExtensionId);
        sqlDataRecord.SetByte(3, (byte) license.Source);
        sqlDataRecord.SetByte(4, (byte) license.Status);
        sqlDataRecord.SetGuid(5, license.CollectionId);
        sqlDataRecord.SetDateTime(6, license.AssignmentDate.DateTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserExtensionLicense", rows.Select<UserExtensionLicense, SqlDataRecord>(selector));
    }
  }
}
