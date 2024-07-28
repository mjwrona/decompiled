// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePropertiesColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabasePropertiesColumns : ObjectBinder<AzureDatabaseProperties>
  {
    private SqlColumnBinder m_objectiveColumn = new SqlColumnBinder("Objective");
    private SqlColumnBinder m_editionColumn = new SqlColumnBinder("Edition");
    private SqlColumnBinder m_maxSizeColumn = new SqlColumnBinder("MaxSizeInMB");
    private SqlColumnBinder m_sizeColumn = new SqlColumnBinder("SizeInMB");

    protected override AzureDatabaseProperties Bind()
    {
      string str = this.m_objectiveColumn.GetString((IDataReader) this.Reader, true);
      int int64_1 = (int) this.m_maxSizeColumn.GetInt64((IDataReader) this.Reader, -1L);
      int int64_2 = (int) this.m_sizeColumn.GetInt64((IDataReader) this.Reader, -1L);
      DatabaseServiceObjective result = DatabaseServiceObjective.NotAzure;
      if (!string.IsNullOrEmpty(str))
        Enum.TryParse<DatabaseServiceObjective>(str, out result);
      return new AzureDatabaseProperties(result)
      {
        CurrentMaxSizeInMB = int64_1,
        SizeInMB = int64_2
      };
    }
  }
}
