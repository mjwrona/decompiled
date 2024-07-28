// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.IServiceHooksPublisherEventRegistration
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public interface IServiceHooksPublisherEventRegistration
  {
    string PublisherId { get; }

    string PublisherEventType { get; }

    IEnumerable<EventTypeDescriptor> GetDescriptors(
      IVssRequestContext requestContext,
      IDictionary<string, string> publisherInputs = null);

    string GetEventDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> currentPublisherInputValues);

    ServiceHooksPublisherEventData GetPayloadFromEvent(
      IVssRequestContext requestContext,
      object eventData,
      ServiceHooksChannelMetadata metadata,
      IDictionary<string, string> contextData = null);

    Event GetSampleEvent(
      IVssRequestContext requestContext,
      IDictionary<string, string> publisherInputs,
      string eventType,
      string resourceVersion);

    InputValues GetSubscriptionInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentPublisherInputValues);

    string ProductClaim { get; }
  }
}
