// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcConsumersController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "Consumers")]
  public class HooksSvcConsumersController : ServiceHooksSvcControllerBase
  {
    public IEnumerable<Consumer> GetConsumers()
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      IEnumerable<Consumer> consumers = this.TfsRequestContext.GetService<ServiceHooksService>().GetConsumers(this.TfsRequestContext);
      consumers.SetConsumerUrl(this.Url, this.TfsRequestContext);
      return consumers;
    }

    public Consumer GetConsumer(string consumerId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(consumerId, nameof (consumerId));
      this.CheckPermission(this.TfsRequestContext, 1);
      Consumer consumer = this.TfsRequestContext.GetService<ServiceHooksService>().GetConsumer(this.TfsRequestContext, consumerId);
      consumer.SetConsumerUrl(this.Url, this.TfsRequestContext);
      return consumer;
    }
  }
}
