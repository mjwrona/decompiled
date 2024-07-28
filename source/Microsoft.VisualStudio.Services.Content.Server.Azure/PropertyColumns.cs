// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.PropertyColumns
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  internal class PropertyColumns : ObjectBinder<SqlProperty>
  {
    protected SqlColumnBinder IdxColumn = new SqlColumnBinder("Idx");
    protected SqlColumnBinder PropertyNameColumn = new SqlColumnBinder("PropertyName");
    protected SqlColumnBinder PropertyTypeColumn = new SqlColumnBinder("PropertyType");
    protected SqlColumnBinder PropertyValueColumn = new SqlColumnBinder("PropertyValue");

    protected override SqlProperty Bind() => new SqlProperty()
    {
      Idx = this.IdxColumn.GetInt32((IDataReader) this.Reader),
      PropertyName = this.PropertyNameColumn.GetString((IDataReader) this.Reader, false),
      PropertyType = this.PropertyTypeColumn.GetInt16((IDataReader) this.Reader),
      PropertyValue = this.PropertyValueColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
