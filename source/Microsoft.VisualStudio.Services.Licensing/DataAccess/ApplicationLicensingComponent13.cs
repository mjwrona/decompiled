// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent13
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
  internal class ApplicationLicensingComponent13 : ApplicationLicensingComponent12
  {
    internal override SqlParameter BindUserExtensionLicenseTable(
      string parameterName,
      IEnumerable<UserExtensionLicense> rows)
    {
      rows = rows ?? Enumerable.Empty<UserExtensionLicense>();
      Func<UserExtensionLicense, SqlDataRecord> selector = (Func<UserExtensionLicense, SqlDataRecord>) (license =>
      {
        SqlDataRecord record = new SqlDataRecord(BaseLicenseComponent.typ_UserExtensionLicenseTableV3);
        record.SetInt32(0, license.InternalScopeId);
        record.SetGuid(1, license.UserId);
        record.SetString(2, license.ExtensionId);
        record.SetByte(3, (byte) license.Source);
        record.SetByte(4, (byte) license.Status);
        record.SetByte(5, (byte) license.AssignmentSource);
        record.SetGuid(6, license.CollectionId);
        record.SetDateTime(7, license.AssignmentDate.DateTime);
        record.SetNullableDateTime(8, license.LastUpdated.GetValueOrDefault());
        return record;
      });
      return this.BindTable(parameterName, "Licensing.typ_UserExtensionLicense_v3", rows.Select<UserExtensionLicense, SqlDataRecord>(selector));
    }
  }
}
