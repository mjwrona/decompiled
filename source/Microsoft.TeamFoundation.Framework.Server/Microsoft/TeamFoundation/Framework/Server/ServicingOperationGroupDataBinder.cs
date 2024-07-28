// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationGroupDataBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingOperationGroupDataBinder : ObjectBinder<ServicingOperationGroupData>
  {
    private SqlColumnBinder m_servicingOperationColumn = new SqlColumnBinder("ServicingOperation");
    private SqlColumnBinder m_groupNameColumn = new SqlColumnBinder("GroupName");
    private SqlColumnBinder m_groupOrderNumberColumn = new SqlColumnBinder("GroupOrderNumber");

    protected override ServicingOperationGroupData Bind() => new ServicingOperationGroupData()
    {
      ServicingOperation = string.Intern(this.m_servicingOperationColumn.GetString((IDataReader) this.Reader, false)),
      GroupName = this.m_groupNameColumn.GetString((IDataReader) this.Reader, false),
      GroupOrderNumber = this.m_groupOrderNumberColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
