// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceHostHistoryEntryBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServiceHostHistoryEntryBinder2 : ServiceHostHistoryEntryBinder
  {
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");

    protected override ServiceHostHistoryEntry Bind()
    {
      ServiceHostHistoryEntry hostHistoryEntry = base.Bind();
      hostHistoryEntry.Name = this.NameColumn.GetString((IDataReader) this.Reader, true);
      return hostHistoryEntry;
    }
  }
}
