// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityKeyMapTableValuedParameters
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityKeyMapTableValuedParameters
  {
    private static readonly SqlMetaData[] typ_IdentityKeyMap = new SqlMetaData[3]
    {
      new SqlMetaData("Cuid", SqlDbType.UniqueIdentifier),
      new SqlMetaData("StorageKey", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TypeId", SqlDbType.TinyInt)
    };

    public static SqlParameter BindIdentityKeyMapTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IdentityKeyMap> rows)
    {
      return component.Bind<IdentityKeyMap>(parameterName, rows, IdentityKeyMapTableValuedParameters.typ_IdentityKeyMap, "typ_IdentityKeyMap", (Action<SqlDataRecord, IdentityKeyMap>) ((record, identityeKeyMap) =>
      {
        record.SetGuid(0, identityeKeyMap.Cuid);
        record.SetNullableGuid(1, identityeKeyMap.StorageKey);
        record.SetByte(2, SubjectTypeMapper.Instance.GetTypeIdFromName(identityeKeyMap.SubjectType));
      }));
    }
  }
}
