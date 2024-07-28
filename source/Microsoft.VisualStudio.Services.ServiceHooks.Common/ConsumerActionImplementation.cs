// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ConsumerActionImplementation
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public abstract class ConsumerActionImplementation
  {
    public abstract string Id { get; }

    public abstract string ConsumerId { get; }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract string[] SupportedEventTypes { get; }

    public abstract IDictionary<string, string[]> SupportedResourceVersions { get; }

    public abstract bool AllowResourceVersionOverride { get; }

    public abstract IList<InputDescriptor> InputDescriptors { get; }

    public virtual InputValues GetInputValues(
      IVssRequestContext requestContext,
      string inputId,
      IDictionary<string, string> currentConsumerInputValues)
    {
      return (InputValues) null;
    }

    public virtual string GetActionDescription(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputValues)
    {
      return (string) null;
    }

    public virtual SessionTokenConfigurationDescriptor GetSessionTokenConfigurationDescriptor() => (SessionTokenConfigurationDescriptor) null;

    public virtual void ValidateConsumerInputs(
      IVssRequestContext requestContext,
      IDictionary<string, string> consumerInputs)
    {
    }

    internal Func<HttpClient> GetHttpClientFunc { set; private get; }

    protected HttpClient GetHttpClient(IVssRequestContext requestContext)
    {
      if (this.GetHttpClientFunc != null)
        return this.GetHttpClientFunc();
      return new HttpClient((HttpMessageHandler) new HttpClientHandler()
      {
        AllowAutoRedirect = !requestContext.IsFeatureEnabled("ServiceHooks.Notification.DisableRedirects")
      });
    }
  }
}
