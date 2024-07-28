// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks.WebHooksConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class WebHooksConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal WebHooksConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (WebHooksConsumerResources.resourceMan == null)
          WebHooksConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.WebHooks.WebHooksConsumerResources", typeof (WebHooksConsumerResources).Assembly);
        return WebHooksConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => WebHooksConsumerResources.resourceCulture;
      set => WebHooksConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => WebHooksConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), WebHooksConsumerResources.resourceCulture);

    internal static string ConsumerName => WebHooksConsumerResources.ResourceManager.GetString(nameof (ConsumerName), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_AcceptUntrustedCertsInputDescription => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_AcceptUntrustedCertsInputDescription), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_AcceptUntrustedCertsInputName => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_AcceptUntrustedCertsInputName), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_BasicAuthPasswordDescription => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_BasicAuthPasswordDescription), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_BasicAuthPasswordName => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_BasicAuthPasswordName), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_BasicAuthUsernameDescription => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_BasicAuthUsernameDescription), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_BasicAuthUsernameName => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_BasicAuthUsernameName), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_DescriptionFormat => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_DescriptionFormat), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_HeadersInputDescription => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_HeadersInputDescription), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_HeadersInputName => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_HeadersInputName), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_UrlInputDescription => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_UrlInputDescription), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestAction_UrlInputName => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestAction_UrlInputName), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestActionDescription => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestActionDescription), WebHooksConsumerResources.resourceCulture);

    internal static string HttpRequestActionName => WebHooksConsumerResources.ResourceManager.GetString(nameof (HttpRequestActionName), WebHooksConsumerResources.resourceCulture);

    internal static string InvalidInputs_UriSchemeMustBeHttpsWhenConfidentialInputIncluded => WebHooksConsumerResources.ResourceManager.GetString(nameof (InvalidInputs_UriSchemeMustBeHttpsWhenConfidentialInputIncluded), WebHooksConsumerResources.resourceCulture);
  }
}
