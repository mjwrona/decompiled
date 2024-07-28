// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNoteColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CheckinNoteColumns : VersionControlObjectBinder<CheckinNoteFieldValue>
  {
    private SqlColumnBinder checkinNoteId = new SqlColumnBinder("CheckInNoteId");
    private SqlColumnBinder fieldName = new SqlColumnBinder("FieldName");
    private SqlColumnBinder baseValue = new SqlColumnBinder("BaseValue");

    protected override CheckinNoteFieldValue Bind() => new CheckinNoteFieldValue()
    {
      checkinNoteId = this.checkinNoteId.GetInt32((IDataReader) this.Reader),
      Name = this.fieldName.GetString((IDataReader) this.Reader, false),
      Value = this.baseValue.GetString((IDataReader) this.Reader, false)
    };
  }
}
