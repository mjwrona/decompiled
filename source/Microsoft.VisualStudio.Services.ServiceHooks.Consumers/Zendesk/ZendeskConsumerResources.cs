// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zendesk.ZendeskConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zendesk
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ZendeskConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ZendeskConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ZendeskConsumerResources.resourceMan == null)
          ZendeskConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Zendesk.ZendeskConsumerResources", typeof (ZendeskConsumerResources).Assembly);
        return ZendeskConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ZendeskConsumerResources.resourceCulture;
      set => ZendeskConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => ZendeskConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), ZendeskConsumerResources.resourceCulture);

    internal static string ConsumerName => ZendeskConsumerResources.ResourceManager.GetString(nameof (ConsumerName), ZendeskConsumerResources.resourceCulture);

    internal static string CreatePrivateCommentActionDescription => ZendeskConsumerResources.ResourceManager.GetString(nameof (CreatePrivateCommentActionDescription), ZendeskConsumerResources.resourceCulture);

    internal static string CreatePrivateCommentActionName => ZendeskConsumerResources.ResourceManager.GetString(nameof (CreatePrivateCommentActionName), ZendeskConsumerResources.resourceCulture);

    internal static string NoopAction_NoZendeskComment => ZendeskConsumerResources.ResourceManager.GetString(nameof (NoopAction_NoZendeskComment), ZendeskConsumerResources.resourceCulture);

    internal static string NoopAction_NoZendeskTicket => ZendeskConsumerResources.ResourceManager.GetString(nameof (NoopAction_NoZendeskTicket), ZendeskConsumerResources.resourceCulture);

    internal static string ZendeskConsumer_AccountNameInputDescription => ZendeskConsumerResources.ResourceManager.GetString(nameof (ZendeskConsumer_AccountNameInputDescription), ZendeskConsumerResources.resourceCulture);

    internal static string ZendeskConsumer_AccountNameInputName => ZendeskConsumerResources.ResourceManager.GetString(nameof (ZendeskConsumer_AccountNameInputName), ZendeskConsumerResources.resourceCulture);

    internal static string ZendeskConsumer_ApiTokenInputDescription => ZendeskConsumerResources.ResourceManager.GetString(nameof (ZendeskConsumer_ApiTokenInputDescription), ZendeskConsumerResources.resourceCulture);

    internal static string ZendeskConsumer_ApiTokenInputName => ZendeskConsumerResources.ResourceManager.GetString(nameof (ZendeskConsumer_ApiTokenInputName), ZendeskConsumerResources.resourceCulture);

    internal static string ZendeskConsumer_UsernameInputDescription => ZendeskConsumerResources.ResourceManager.GetString(nameof (ZendeskConsumer_UsernameInputDescription), ZendeskConsumerResources.resourceCulture);

    internal static string ZendeskConsumer_UsernameInputName => ZendeskConsumerResources.ResourceManager.GetString(nameof (ZendeskConsumer_UsernameInputName), ZendeskConsumerResources.resourceCulture);
  }
}
