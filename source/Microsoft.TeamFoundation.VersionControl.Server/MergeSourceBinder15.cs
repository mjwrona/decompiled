// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MergeSourceBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class MergeSourceBinder15 : MergeSourceBinder
  {
    public MergeSourceBinder15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override MergeSource Bind() => new MergeSource()
    {
      ItemPathPair = this.GetItemPathPair(DBPath.DatabaseToServerPath(this.serverItem.GetString((IDataReader) this.Reader, false))),
      VersionFrom = this.versionFrom.GetInt32((IDataReader) this.Reader),
      VersionTo = this.versionTo.GetInt32((IDataReader) this.Reader),
      IsRename = this.isRename.GetBoolean((IDataReader) this.Reader),
      SequenceId = this.sequenceId.GetInt32((IDataReader) this.Reader)
    };
  }
}
