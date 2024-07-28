// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcInputValuesQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "InputValuesQuery")]
  public class HooksSvcInputValuesQueryController : ServiceHooksSvcControllerBase
  {
    [HttpPost]
    public SubscriptionInputValuesQuery QueryInputValues(
      SubscriptionInputValuesQuery inputValuesQuery)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      ArgumentUtility.CheckForNull<SubscriptionInputValuesQuery>(inputValuesQuery, nameof (inputValuesQuery));
      ArgumentUtility.CheckForNull<IList<InputValues>>(inputValuesQuery.InputValues, "inputValuesQuery.InputValues");
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(inputValuesQuery.Subscription, "inputValuesQuery.Subscription");
      if (inputValuesQuery.Scope == SubscriptionInputScope.Publisher)
        throw new ArgumentException(ServiceHooksResources.Error_PublisherInputsNotQueryable);
      ServiceHooksService service = this.TfsRequestContext.GetService<ServiceHooksService>();
      inputValuesQuery.InputValues = service.GetConsumerInputValues(this.TfsRequestContext, inputValuesQuery.Subscription.ConsumerId, inputValuesQuery.Subscription.ConsumerActionId, inputValuesQuery.Subscription.ConsumerInputs, inputValuesQuery.InputValues.Select<InputValues, string>((Func<InputValues, string>) (i => i.InputId)), inputValuesQuery.Subscription.Id == Guid.Empty ? new Guid?() : new Guid?(inputValuesQuery.Subscription.Id));
      return inputValuesQuery;
    }
  }
}
