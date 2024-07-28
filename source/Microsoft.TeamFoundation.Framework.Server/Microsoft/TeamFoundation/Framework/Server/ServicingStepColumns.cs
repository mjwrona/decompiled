// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingStepColumns : ObjectBinder<ServicingStep>
  {
    private SqlColumnBinder m_groupNameColumn = new SqlColumnBinder("GroupName");
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("StepName");
    private SqlColumnBinder m_performerColumn = new SqlColumnBinder("StepPerformer");
    private SqlColumnBinder m_stepTypeColumn = new SqlColumnBinder("StepType");
    private SqlColumnBinder m_optionsColumn = new SqlColumnBinder("Options");
    private SqlColumnBinder m_dataColumn = new SqlColumnBinder("StepData");

    protected override ServicingStep Bind()
    {
      ServicingStep.StepOptions int32 = (ServicingStep.StepOptions) this.m_optionsColumn.GetInt32((IDataReader) this.Reader);
      return new ServicingStep(this.m_groupNameColumn.GetString((IDataReader) this.Reader, false), this.m_nameColumn.GetString((IDataReader) this.Reader, false), this.m_performerColumn.GetString((IDataReader) this.Reader, false), this.m_stepTypeColumn.GetString((IDataReader) this.Reader, false), this.m_dataColumn.GetString((IDataReader) this.Reader, true))
      {
        Options = int32
      };
    }
  }
}
