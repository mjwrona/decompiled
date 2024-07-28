// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ServerItemLocalVersionUpdateColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ServerItemLocalVersionUpdateColumns15 : ServerItemLocalVersionUpdateColumns
  {
    public ServerItemLocalVersionUpdateColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ServerItemLocalVersionUpdate Bind()
    {
      ServerItemLocalVersionUpdate localVersionUpdate = new ServerItemLocalVersionUpdate();
      localVersionUpdate.SourceItemPathPair = this.GetItemPathPair(this.sourceServerItem.GetServerItem(this.Reader, false));
      localVersionUpdate.ItemId = this.itemId.GetInt32((IDataReader) this.Reader);
      localVersionUpdate.LocalVersion = this.versionLocal.GetInt32((IDataReader) this.Reader);
      localVersionUpdate.TargetLocalItem = this.targetLocalItem.GetLocalItem(this.Reader, true);
      return localVersionUpdate;
    }
  }
}
