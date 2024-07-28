// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.NotificationBinder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class NotificationBinder : ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>
  {
    private SqlColumnBinder NotificationId = new SqlColumnBinder(nameof (NotificationId));
    private SqlColumnBinder SubscriptionId = new SqlColumnBinder(nameof (SubscriptionId));
    private SqlColumnBinder EventId = new SqlColumnBinder(nameof (EventId));
    private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
    private SqlColumnBinder Result = new SqlColumnBinder(nameof (Result));
    private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
    private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));

    protected override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification Bind() => new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification()
    {
      Id = this.NotificationId.GetInt32((IDataReader) this.Reader),
      SubscriptionId = this.SubscriptionId.GetGuid((IDataReader) this.Reader),
      EventId = this.EventId.GetGuid((IDataReader) this.Reader),
      Status = (NotificationStatus) this.Status.GetInt16((IDataReader) this.Reader),
      Result = (NotificationResult) this.Result.GetInt16((IDataReader) this.Reader),
      CreatedDate = this.CreatedDate.GetDateTime((IDataReader) this.Reader),
      ModifiedDate = this.ModifiedDate.GetDateTime((IDataReader) this.Reader)
    };
  }
}
