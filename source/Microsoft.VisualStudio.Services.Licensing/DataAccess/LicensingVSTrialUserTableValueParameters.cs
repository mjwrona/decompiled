// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.LicensingVSTrialUserTableValueParameters
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal static class LicensingVSTrialUserTableValueParameters
  {
    private static readonly SqlMetaData[] typ_LicensingVSTrialUser = new SqlMetaData[5]
    {
      new SqlMetaData("UserId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("MajorVersion", SqlDbType.Int),
      new SqlMetaData("ProductFamilyId", SqlDbType.Int),
      new SqlMetaData("ProductEditionId", SqlDbType.Int),
      new SqlMetaData("IsMigrated", SqlDbType.Bit)
    };

    public static SqlParameter BindLicensingVSTrialUserTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<VisualStudioTrialUserInfo> rows)
    {
      return component.Bind<VisualStudioTrialUserInfo>(parameterName, rows, LicensingVSTrialUserTableValueParameters.typ_LicensingVSTrialUser, "typ_LicensingVSTrialUser", (Action<SqlDataRecord, VisualStudioTrialUserInfo>) ((record, visualStudioTrialUserInfo) =>
      {
        record.SetGuid(0, visualStudioTrialUserInfo.UserId);
        record.SetInt32(1, visualStudioTrialUserInfo.MajorVersion);
        record.SetInt32(2, visualStudioTrialUserInfo.ProductFamilyId);
        record.SetInt32(3, visualStudioTrialUserInfo.ProductEditionId);
        record.SetBoolean(4, visualStudioTrialUserInfo.IsMigrated);
      }));
    }
  }
}
