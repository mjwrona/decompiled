// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MergeSourceBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class MergeSourceBinder : VersionControlObjectBinder<MergeSource>
  {
    protected SqlColumnBinder serverItem = new SqlColumnBinder("ServerItem");
    protected SqlColumnBinder versionFrom = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder versionTo = new SqlColumnBinder("VersionTo");
    protected SqlColumnBinder isRename = new SqlColumnBinder("IsRename");
    protected SqlColumnBinder sequenceId = new SqlColumnBinder("SequenceId");

    public MergeSourceBinder()
    {
    }

    public MergeSourceBinder(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override MergeSource Bind() => new MergeSource()
    {
      ItemPathPair = ItemPathPair.FromServerItem(DBPath.DatabaseToServerPath(this.serverItem.GetString((IDataReader) this.Reader, false))),
      VersionFrom = this.versionFrom.GetInt32((IDataReader) this.Reader),
      VersionTo = this.versionTo.GetInt32((IDataReader) this.Reader),
      IsRename = this.isRename.GetBoolean((IDataReader) this.Reader),
      SequenceId = this.sequenceId.GetInt32((IDataReader) this.Reader)
    };
  }
}
