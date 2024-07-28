// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.SubscriptionBinder3
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class SubscriptionBinder3 : SubscriptionBinder2
  {
    private SqlColumnBinder EventDescription = new SqlColumnBinder(nameof (EventDescription));
    private SqlColumnBinder ActionDescription = new SqlColumnBinder(nameof (ActionDescription));

    protected override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription Bind()
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = base.Bind();
      subscription.EventDescription = this.EventDescription.GetString((IDataReader) this.Reader, false);
      subscription.ActionDescription = this.ActionDescription.GetString((IDataReader) this.Reader, true);
      return subscription;
    }
  }
}
