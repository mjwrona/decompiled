// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcConsumerActionsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "Actions")]
  public class HooksSvcConsumerActionsController : ServiceHooksSvcControllerBase
  {
    public IEnumerable<ConsumerAction> GetConsumerActions(string consumerId)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      IList<ConsumerAction> actions = this.TfsRequestContext.GetService<ServiceHooksService>().GetConsumer(this.TfsRequestContext, consumerId).Actions;
      actions.SetConsumerActionUrl(this.Url, this.TfsRequestContext);
      return (IEnumerable<ConsumerAction>) actions;
    }

    public ConsumerAction GetConsumerAction(string consumerId, string consumerActionId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(consumerId, nameof (consumerId));
      this.CheckPermission(this.TfsRequestContext, 1);
      ConsumerAction consumerAction = this.TfsRequestContext.GetService<ServiceHooksService>().GetConsumer(this.TfsRequestContext, consumerId).Actions.FirstOrDefault<ConsumerAction>((Func<ConsumerAction, bool>) (a => string.Equals(consumerActionId, a.Id)));
      if (consumerAction == null)
        throw new ConsumerActionNotFoundException(consumerId, consumerActionId);
      consumerAction.SetConsumerActionUrl(this.Url, this.TfsRequestContext);
      return consumerAction;
    }
  }
}
