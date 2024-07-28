// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ConfigurationResourceComponent3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ConfigurationResourceComponent3 : ConfigurationResourceComponent2
  {
    protected override SqlParameter BindFileTypeExtensionTable(
      string parameterName,
      IEnumerable<FileTypeExtension> rows)
    {
      rows = rows ?? Enumerable.Empty<FileTypeExtension>();
      Func<FileTypeExtension, SqlDataRecord> selector = (Func<FileTypeExtension, SqlDataRecord>) (extension =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ConfigurationResourceComponent.typ_Extension);
        sqlDataRecord.SetString(0, extension.Name);
        sqlDataRecord.SetString(1, extension.Extension);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Extension2", rows.Select<FileTypeExtension, SqlDataRecord>(selector));
    }
  }
}
