// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionBinder2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class WorkItemTypeExtensionBinder2 : WorkItemTypeExtensionBinder
  {
    protected SqlColumnBinder m_rankColumn = new SqlColumnBinder("Rank");

    protected override WorkItemTypeletRecord Bind()
    {
      WorkItemTypeletRecord itemTypeletRecord = base.Bind();
      itemTypeletRecord.Rank = this.m_rankColumn.GetInt32(this.Reader);
      return itemTypeletRecord;
    }
  }
}
