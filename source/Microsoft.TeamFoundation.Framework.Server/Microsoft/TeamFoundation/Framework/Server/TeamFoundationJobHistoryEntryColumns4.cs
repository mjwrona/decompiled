// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobHistoryEntryColumns4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationJobHistoryEntryColumns4 : TeamFoundationJobHistoryEntryColumns3
  {
    private SqlColumnBinder QueueFlagsColumn = new SqlColumnBinder("QueueFlags");

    protected override TeamFoundationJobHistoryEntry Bind()
    {
      TeamFoundationJobHistoryEntry foundationJobHistoryEntry = base.Bind();
      if (foundationJobHistoryEntry != null)
        foundationJobHistoryEntry.QueueFlagsValue = this.QueueFlagsColumn.GetInt32((IDataReader) this.Reader);
      return foundationJobHistoryEntry;
    }
  }
}
