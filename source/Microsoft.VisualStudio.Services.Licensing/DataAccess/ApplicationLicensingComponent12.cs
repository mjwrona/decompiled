// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent12
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent12 : ApplicationLicensingComponent11
  {
    internal override SqlParameter BindUserLicenseTable(
      string parameterName,
      IEnumerable<UserLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserLicense>();
      Func<UserLicense, SqlDataRecord> selector = (Func<UserLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord record = new SqlDataRecord(BaseLicenseComponent.typ_UserLicenseTableV4);
        record.SetGuid(0, license.UserId);
        record.SetByte(1, (byte) license.Status);
        record.SetByte(2, (byte) license.Source);
        record.SetByte(3, (byte) license.License);
        record.SetByte(4, (byte) license.Origin);
        record.SetByte(5, (byte) license.AssignmentSource);
        record.SetDateTime(6, license.AssignmentDate.DateTime);
        record.SetDateTime(7, license.DateCreated.DateTime);
        record.SetDateTime(8, license.LastUpdated.DateTime);
        record.SetNullableDateTime(9, license.LastAccessed > DateTimeOffset.MinValue ? new DateTime?(license.LastAccessed.UtcDateTime) : new DateTime?());
        return record;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserLicense_v4", rows.Select<UserLicense, SqlDataRecord>(selector));
    }
  }
}
