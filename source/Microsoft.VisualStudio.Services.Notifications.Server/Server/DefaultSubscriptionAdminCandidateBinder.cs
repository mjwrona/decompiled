// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DefaultSubscriptionAdminCandidateBinder
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class DefaultSubscriptionAdminCandidateBinder : 
    ObjectBinder<DefaultSubscriptionAdminCandidate>
  {
    private SqlColumnBinder SubscriptionNameColumn = new SqlColumnBinder("SubscriptionName");
    private SqlColumnBinder BlockUserDisableColumn = new SqlColumnBinder("BlockUserDisable");
    private SqlColumnBinder IsDisabledColumn = new SqlColumnBinder("IsDisabled");

    protected override DefaultSubscriptionAdminCandidate Bind() => new DefaultSubscriptionAdminCandidate()
    {
      SubscriptionName = this.SubscriptionNameColumn.GetString((IDataReader) this.Reader, false),
      IsDisabled = this.IsDisabledColumn.ColumnExists((IDataReader) this.Reader) && this.IsDisabledColumn.GetBoolean((IDataReader) this.Reader, false),
      BlockUserDisable = this.BlockUserDisableColumn.ColumnExists((IDataReader) this.Reader) && this.BlockUserDisableColumn.GetBoolean((IDataReader) this.Reader, false)
    };
  }
}
