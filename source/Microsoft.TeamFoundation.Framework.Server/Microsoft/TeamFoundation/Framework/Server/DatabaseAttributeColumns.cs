// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseAttributeColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseAttributeColumns : ObjectBinder<DatabaseAttribute>
  {
    private SqlColumnBinder nameColumn = new SqlColumnBinder("name");
    private SqlColumnBinder valueColumn = new SqlColumnBinder("value");

    public DatabaseAttributeColumns()
    {
    }

    public DatabaseAttributeColumns(SqlDataReader dataReader, string storedProcedureName)
      : base(dataReader, storedProcedureName)
    {
    }

    protected override DatabaseAttribute Bind()
    {
      string attributeName = this.nameColumn.GetString((IDataReader) this.Reader, false);
      object obj = this.valueColumn.GetObject((IDataReader) this.Reader);
      string attributeValue = obj != null ? (!(obj is Guid guid) ? obj.ToString() : guid.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture)) : (string) null;
      return new DatabaseAttribute(attributeName, attributeValue);
    }
  }
}
