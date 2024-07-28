// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNoteFieldDefinitionColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CheckinNoteFieldDefinitionColumns : 
    VersionControlObjectBinder<CheckinNoteFieldDefinition>
  {
    protected SqlColumnBinder associatedItem = new SqlColumnBinder("AssociatedItem");
    protected SqlColumnBinder fieldName = new SqlColumnBinder("FieldName");
    protected SqlColumnBinder required = new SqlColumnBinder("Required");
    protected SqlColumnBinder displayOrder = new SqlColumnBinder("DisplayOrder");

    public CheckinNoteFieldDefinitionColumns()
    {
    }

    public CheckinNoteFieldDefinitionColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override CheckinNoteFieldDefinition Bind() => new CheckinNoteFieldDefinition()
    {
      ItemPathPair = this.GetPreDataspaceItemPathPair(this.associatedItem.GetServerItem(this.Reader, false)),
      Name = this.fieldName.GetString((IDataReader) this.Reader, false),
      Required = this.required.GetBoolean((IDataReader) this.Reader),
      DisplayOrder = this.displayOrder.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
