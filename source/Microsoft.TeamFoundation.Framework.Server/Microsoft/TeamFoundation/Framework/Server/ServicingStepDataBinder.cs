// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepDataBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingStepDataBinder : ObjectBinder<ServicingStepData>
  {
    private SqlColumnBinder m_stepNameColumn = new SqlColumnBinder("StepName");
    private SqlColumnBinder m_groupNameColumn = new SqlColumnBinder("GroupName");
    private SqlColumnBinder m_orderNumberColumn = new SqlColumnBinder("OrderNumber");
    private SqlColumnBinder m_stepPerformerColumn = new SqlColumnBinder("StepPerformer");
    private SqlColumnBinder m_stepTypeColumn = new SqlColumnBinder("StepType");
    private SqlColumnBinder m_optionsColumn = new SqlColumnBinder("Options");
    private SqlColumnBinder m_stepDataColumn = new SqlColumnBinder("StepData");

    protected override ServicingStepData Bind() => new ServicingStepData()
    {
      StepName = this.m_stepNameColumn.GetString((IDataReader) this.Reader, false),
      GroupName = string.Intern(this.m_groupNameColumn.GetString((IDataReader) this.Reader, false)),
      OrderNumber = this.m_orderNumberColumn.GetInt32((IDataReader) this.Reader),
      Options = (ServicingStep.StepOptions) this.m_optionsColumn.GetInt32((IDataReader) this.Reader),
      StepPerformer = string.Intern(this.m_stepPerformerColumn.GetString((IDataReader) this.Reader, false)),
      StepType = string.Intern(this.m_stepTypeColumn.GetString((IDataReader) this.Reader, false)),
      StepData = this.m_stepDataColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
