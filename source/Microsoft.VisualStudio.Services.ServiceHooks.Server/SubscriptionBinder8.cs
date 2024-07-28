// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.SubscriptionBinder8
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class SubscriptionBinder8 : SubscriptionBinder7
  {
    private SqlColumnBinder LastProbationRetryDate = new SqlColumnBinder(nameof (LastProbationRetryDate));

    protected override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription Bind()
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = base.Bind();
      subscription.LastProbationRetryDate = this.LastProbationRetryDate.GetDateTime((IDataReader) this.Reader);
      return subscription;
    }
  }
}
