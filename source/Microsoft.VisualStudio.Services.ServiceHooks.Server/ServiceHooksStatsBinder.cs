// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksStatsBinder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class ServiceHooksStatsBinder : ObjectBinder<ServiceHooksStats>
  {
    private SqlColumnBinder EnabledSubscriptionCount = new SqlColumnBinder(nameof (EnabledSubscriptionCount));
    private SqlColumnBinder DisabledSubscriptionCount = new SqlColumnBinder(nameof (DisabledSubscriptionCount));

    protected override ServiceHooksStats Bind()
    {
      int int32_1 = this.EnabledSubscriptionCount.GetInt32((IDataReader) this.Reader);
      int int32_2 = this.DisabledSubscriptionCount.GetInt32((IDataReader) this.Reader);
      return new ServiceHooksStats()
      {
        EnabledSubscriptionCount = int32_1,
        DisabledSubscriptionCount = int32_2
      };
    }
  }
}
