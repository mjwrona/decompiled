// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkingFolderColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkingFolderColumns15 : WorkingFolderColumns
  {
    public WorkingFolderColumns15(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override WorkingFolder Bind()
    {
      WorkingFolder workingFolder = new WorkingFolder();
      workingFolder.workspaceId = this.workspaceID.GetInt32((IDataReader) this.Reader);
      workingFolder.ItemPathPair = this.GetItemPathPair(this.fullPath.GetServerItem(this.Reader, false));
      workingFolder.LocalItem = this.localPath.GetLocalItem(this.Reader, true);
      workingFolder.Type = this.mappingType.GetBoolean((IDataReader) this.Reader) ? WorkingFolderType.Map : WorkingFolderType.Cloak;
      workingFolder.Depth = (int) this.depth.GetByte((IDataReader) this.Reader);
      return workingFolder;
    }
  }
}
