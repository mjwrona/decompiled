// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeletRecordBinder3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemTypeletRecordBinder3 : WorkItemTypeletRecordBinder2
  {
    private SqlColumnBinder m_disabledColumn = new SqlColumnBinder("Disabled");
    private SqlColumnBinder m_rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder m_abstract = new SqlColumnBinder("Abstract");
    private SqlColumnBinder m_overridden = new SqlColumnBinder("Overridden");

    protected override WorkItemTypeletRecord Bind()
    {
      WorkItemTypeletRecord itemTypeletRecord = base.Bind();
      itemTypeletRecord.Disabled = this.m_disabledColumn.GetBoolean(this.Reader, false);
      itemTypeletRecord.IsAbstract = this.m_abstract.GetBoolean(this.Reader, false, false);
      itemTypeletRecord.Rank = this.m_rank.GetInt32(this.Reader, 0, 0);
      itemTypeletRecord.Overridden = this.m_overridden.GetBoolean(this.Reader, false, false);
      return itemTypeletRecord;
    }
  }
}
