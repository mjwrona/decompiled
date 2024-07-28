// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeletRecordBinder5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemTypeletRecordBinder5 : WorkItemTypeletRecordBinder4
  {
    protected SqlColumnBinder m_iconColumn = new SqlColumnBinder("Icon");

    protected override WorkItemTypeletRecord Bind()
    {
      WorkItemTypeletRecord itemTypeletRecord = base.Bind();
      itemTypeletRecord.Icon = this.m_iconColumn.GetString(this.Reader, true);
      return itemTypeletRecord;
    }
  }
}
