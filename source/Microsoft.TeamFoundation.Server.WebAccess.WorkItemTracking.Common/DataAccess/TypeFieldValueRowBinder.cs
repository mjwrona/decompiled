// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TypeFieldValueRowBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TypeFieldValueRowBinder : ObjectBinder<TypeFieldValueRow>
  {
    private SqlColumnBinder FieldTypeColumn = new SqlColumnBinder("TypeFieldType");
    private SqlColumnBinder TypeColumn = new SqlColumnBinder("TypeFieldValueType");
    private SqlColumnBinder ValueColumn = new SqlColumnBinder("TypeFieldValueValue");

    protected override TypeFieldValueRow Bind() => new TypeFieldValueRow()
    {
      FieldType = (FieldTypeEnum) this.FieldTypeColumn.GetByte((IDataReader) this.Reader),
      Type = this.TypeColumn.GetString((IDataReader) this.Reader, false),
      Value = this.ValueColumn.GetString((IDataReader) this.Reader, false)
    };
  }
}
