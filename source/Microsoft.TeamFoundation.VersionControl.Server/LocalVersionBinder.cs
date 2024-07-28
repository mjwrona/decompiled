// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LocalVersionBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LocalVersionBinder : VersionControlObjectBinder<LocalVersion>
  {
    private SqlColumnBinder localItem = new SqlColumnBinder("LocalItem");
    private SqlColumnBinder versionFrom = new SqlColumnBinder("VersionFrom");

    public LocalVersionBinder()
    {
    }

    public LocalVersionBinder(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override LocalVersion Bind() => new LocalVersion()
    {
      Item = this.localItem.GetLocalItem(this.Reader, false),
      Version = this.versionFrom.GetInt32((IDataReader) this.Reader)
    };
  }
}
