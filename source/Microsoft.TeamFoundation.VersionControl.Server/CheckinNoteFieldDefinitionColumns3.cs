// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNoteFieldDefinitionColumns3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CheckinNoteFieldDefinitionColumns3 : CheckinNoteFieldDefinitionColumns
  {
    public CheckinNoteFieldDefinitionColumns3(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override CheckinNoteFieldDefinition Bind() => new CheckinNoteFieldDefinition()
    {
      ItemPathPair = this.GetItemPathPair(this.associatedItem.GetServerItem(this.Reader, false)),
      Name = this.fieldName.GetString((IDataReader) this.Reader, false),
      Required = this.required.GetBoolean((IDataReader) this.Reader),
      DisplayOrder = this.displayOrder.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
