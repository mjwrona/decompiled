// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExpandedChangeBinder15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ExpandedChangeBinder15 : ExpandedChangeBinder
  {
    public ExpandedChangeBinder15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override ExpandedChange Bind() => new ExpandedChange()
    {
      itemId = this.itemId.GetInt32((IDataReader) this.Reader),
      localItem = this.localItem.GetLocalItem(this.Reader, true),
      requestIndex = this.requestIndex.GetInt32((IDataReader) this.Reader),
      requiredLockLevel = (LockLevel) this.lockLevel.GetByte((IDataReader) this.Reader),
      itemPathPair = this.GetItemPathPair(this.serverItem.GetServerItem(this.Reader, false)),
      targetItemPathPair = this.GetItemPathPair(this.serverItem.GetServerItem(this.Reader, true))
    };
  }
}
