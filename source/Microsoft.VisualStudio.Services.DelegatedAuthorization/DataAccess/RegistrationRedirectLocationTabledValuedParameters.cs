// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.RegistrationRedirectLocationTabledValuedParameters
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal static class RegistrationRedirectLocationTabledValuedParameters
  {
    private static readonly SqlMetaData[] typ_DelegatedAuthorizationRegistrationRedirectLocation = new SqlMetaData[2]
    {
      new SqlMetaData("RegistrationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Location", SqlDbType.NVarChar, 256L)
    };
    private static readonly SqlMetaData[] typ_DelegatedAuthorizationRegistrationRedirectLocation2 = new SqlMetaData[2]
    {
      new SqlMetaData("RegistrationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Location", SqlDbType.NVarChar, 500L)
    };

    public static SqlParameter BindRegistrationRedirectLocationTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<RegistrationRedirectLocation> rows)
    {
      return component.Bind<RegistrationRedirectLocation>(parameterName, rows, RegistrationRedirectLocationTabledValuedParameters.typ_DelegatedAuthorizationRegistrationRedirectLocation, "typ_DelegatedAuthorizationRegistrationRedirectLocation", (Action<SqlDataRecord, RegistrationRedirectLocation>) ((record, redirectLocation) =>
      {
        record.SetGuid(0, redirectLocation.RegistrationId);
        record.SetString(1, redirectLocation.Location.ToString());
      }));
    }

    public static SqlParameter BindRegistrationRedirectLocationTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<RegistrationRedirectLocation> rows)
    {
      return component.Bind<RegistrationRedirectLocation>(parameterName, rows, RegistrationRedirectLocationTabledValuedParameters.typ_DelegatedAuthorizationRegistrationRedirectLocation2, "typ_DelegatedAuthorizationRegistrationRedirectLocation2", (Action<SqlDataRecord, RegistrationRedirectLocation>) ((record, redirectLocation) =>
      {
        record.SetGuid(0, redirectLocation.RegistrationId);
        record.SetString(1, redirectLocation.Location.ToString());
      }));
    }
  }
}
