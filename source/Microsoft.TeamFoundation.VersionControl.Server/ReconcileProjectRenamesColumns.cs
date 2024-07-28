// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ReconcileProjectRenamesColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ReconcileProjectRenamesColumns : VersionControlObjectBinder<ReconcileProjectRename>
  {
    private SqlColumnBinder oldProjectName = new SqlColumnBinder("OldProjectName");
    private SqlColumnBinder newProjectName = new SqlColumnBinder("NewProjectName");

    protected override ReconcileProjectRename Bind() => new ReconcileProjectRename(this.oldProjectName.GetString((IDataReader) this.Reader, false), this.newProjectName.GetString((IDataReader) this.Reader, false));
  }
}
