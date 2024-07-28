// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.TypeFieldBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class TypeFieldBinder : ObjectBinder<TypeField>
  {
    private SqlColumnBinder FieldRefNameColumn = new SqlColumnBinder("FieldRefName");
    private SqlColumnBinder FieldTypeColumn = new SqlColumnBinder("FieldType");
    private SqlColumnBinder FormatColumn = new SqlColumnBinder("Format");

    protected override TypeField Bind()
    {
      TypeField typeField = new TypeField();
      typeField.Name = this.FieldRefNameColumn.GetString((IDataReader) this.Reader, false);
      typeField.Type = (FieldTypeEnum) this.FieldTypeColumn.GetByte((IDataReader) this.Reader);
      typeField.Format = this.FormatColumn.GetString((IDataReader) this.Reader, true);
      return typeField;
    }
  }
}
