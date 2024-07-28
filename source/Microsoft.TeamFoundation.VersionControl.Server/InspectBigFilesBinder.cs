// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InspectBigFilesBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class InspectBigFilesBinder : VersionControlObjectBinder<InspectBigFilesInfo>
  {
    private SqlColumnBinder bytes = new SqlColumnBinder("Bytes");
    private SqlColumnBinder fileId = new SqlColumnBinder("FileId");
    private SqlColumnBinder isDeleted = new SqlColumnBinder("IsDeleted");
    private SqlColumnBinder serverItem = new SqlColumnBinder("ServerItem");

    public InspectBigFilesBinder(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override InspectBigFilesInfo Bind()
    {
      string serverPath = DBPath.DatabaseToServerPath(this.serverItem.GetString((IDataReader) this.Reader, false));
      return new InspectBigFilesInfo()
      {
        Bytes = this.bytes.GetInt64((IDataReader) this.Reader),
        FileId = this.fileId.GetInt32((IDataReader) this.Reader),
        IsDeleted = this.isDeleted.GetBoolean((IDataReader) this.Reader),
        ServerItem = this.m_component.ConvertToPathWithProjectName(serverPath)
      };
    }
  }
}
