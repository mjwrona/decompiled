// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepGroupDataBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingStepGroupDataBinder : ObjectBinder<ServicingStepGroupData>
  {
    private SqlColumnBinder m_groupNameColumn = new SqlColumnBinder("GroupName");
    private SqlColumnBinder m_handlersColumn = new SqlColumnBinder("Handlers");

    protected override ServicingStepGroupData Bind() => new ServicingStepGroupData()
    {
      GroupName = this.m_groupNameColumn.GetString((IDataReader) this.Reader, false),
      Handlers = string.Intern(this.m_handlersColumn.GetString((IDataReader) this.Reader, true)) ?? ""
    };
  }
}
