// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdTranslationTableValuedParameterExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityIdTranslationTableValuedParameterExtensions
  {
    private static readonly SqlMetaData[] typ_IdentityIdTranslationTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("MasterId", SqlDbType.UniqueIdentifier)
    };

    public static SqlParameter BindIdentityIdTranslationTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IdentityIdTranslation> rows)
    {
      return component.Bind<IdentityIdTranslation>(parameterName, rows, IdentityIdTranslationTableValuedParameterExtensions.typ_IdentityIdTranslationTable, "typ_IdentityIdTranslationTable", (Action<SqlDataRecord, IdentityIdTranslation>) ((record, identityIdTranslation) =>
      {
        record.SetGuid(0, identityIdTranslation.Id);
        record.SetGuid(1, identityIdTranslation.MasterId);
      }));
    }
  }
}
