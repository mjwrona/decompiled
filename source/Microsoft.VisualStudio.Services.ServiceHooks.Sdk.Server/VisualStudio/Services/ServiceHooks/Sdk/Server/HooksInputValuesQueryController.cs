// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksInputValuesQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "InputValuesQuery")]
  [ClientInclude(RestClientLanguages.TypeScriptWebPlatform)]
  public class HooksInputValuesQueryController : ServiceHooksPublisherControllerBase
  {
    [ClientSuppressWarning(ClientWarnings.NamingGuidelines)]
    public SubscriptionInputValuesQuery PostInputValuesQuery(
      SubscriptionInputValuesQuery inputValuesQuery)
    {
      ArgumentUtility.CheckForNull<SubscriptionInputValuesQuery>(inputValuesQuery, nameof (inputValuesQuery));
      ArgumentUtility.CheckForNull<IList<InputValues>>(inputValuesQuery.InputValues, "inputValuesQuery.InputValues");
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(inputValuesQuery.Subscription, "inputValuesQuery.Subscription");
      if (inputValuesQuery.Scope == SubscriptionInputScope.Publisher)
      {
        ServiceHooksPublisher publisher = this.FindPublisher(inputValuesQuery.Subscription.PublisherId);
        inputValuesQuery.InputValues = publisher.GetSubscriptionInputValues(this.TfsRequestContext, inputValuesQuery.Subscription.EventType, inputValuesQuery.Subscription.PublisherInputs, inputValuesQuery.InputValues.Select<InputValues, string>((Func<InputValues, string>) (i => i.InputId)));
      }
      else
        inputValuesQuery.InputValues = this.GetHooksService().GetConsumerInputValues(this.TfsRequestContext, inputValuesQuery.Subscription.ConsumerId, inputValuesQuery.Subscription.ConsumerActionId, inputValuesQuery.Subscription.ConsumerInputs, inputValuesQuery.InputValues.Select<InputValues, string>((Func<InputValues, string>) (i => i.InputId)), inputValuesQuery.Subscription.Id == Guid.Empty ? new Guid?() : new Guid?(inputValuesQuery.Subscription.Id));
      return inputValuesQuery;
    }
  }
}
