// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangesetChangeTypeSummaryColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ChangesetChangeTypeSummaryColumns : 
    VersionControlObjectBinder<ChangesetChangeTypeSummary>
  {
    private SqlColumnBinder addedCount = new SqlColumnBinder("Added");
    private SqlColumnBinder editedCount = new SqlColumnBinder("Edited");
    private SqlColumnBinder deletedCount = new SqlColumnBinder("Deleted");
    private readonly int changeset;

    public ChangesetChangeTypeSummaryColumns(int changeset) => this.changeset = changeset;

    protected override ChangesetChangeTypeSummary Bind() => new ChangesetChangeTypeSummary()
    {
      ChangeSetId = this.changeset,
      AddedCount = this.addedCount.GetInt32((IDataReader) this.Reader, 0),
      EditedCount = this.editedCount.GetInt32((IDataReader) this.Reader, 0),
      DeletedCount = this.deletedCount.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
