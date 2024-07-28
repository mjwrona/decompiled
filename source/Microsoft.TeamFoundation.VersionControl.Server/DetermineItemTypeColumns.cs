// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DetermineItemTypeColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class DetermineItemTypeColumns : VersionControlObjectBinder<DeterminedItem>
  {
    internal SqlColumnBinder queryPath = new SqlColumnBinder("QueryPath");
    internal SqlColumnBinder filePattern = new SqlColumnBinder("FilePattern");

    public DetermineItemTypeColumns()
    {
    }

    public DetermineItemTypeColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override DeterminedItem Bind() => new DeterminedItem()
    {
      QueryPath = this.queryPath.GetServerItem(this.Reader, true),
      FilePattern = this.filePattern.GetServerItem(this.Reader, true)
    };
  }
}
