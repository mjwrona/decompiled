// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.ServiceHooksConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ServiceHooksConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ServiceHooksConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ServiceHooksConsumerResources.resourceMan == null)
          ServiceHooksConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.ServiceHooksConsumerResources", typeof (ServiceHooksConsumerResources).Assembly);
        return ServiceHooksConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ServiceHooksConsumerResources.resourceCulture;
      set => ServiceHooksConsumerResources.resourceCulture = value;
    }

    internal static string Error_CannotIssueSessionTokensForTestNotifications_Reason_TesterIsNotTheSubscriber => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_CannotIssueSessionTokensForTestNotifications_Reason_TesterIsNotTheSubscriber), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_CouldNotIssueSessionTokenForConsumerAction => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_CouldNotIssueSessionTokenForConsumerAction), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_DuplicateConsumerIdentifier => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_DuplicateConsumerIdentifier), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_DuplicateConsumerInputDescriptorIdTemplate => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_DuplicateConsumerInputDescriptorIdTemplate), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_EventHandlerReturnedNullActionTaskTemplate => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_EventHandlerReturnedNullActionTaskTemplate), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_HttpConsumersMustUseHttpsTemplate => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_HttpConsumersMustUseHttpsTemplate), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_MatchingEventHandlerMethodNotFoundTemplate => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_MatchingEventHandlerMethodNotFoundTemplate), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_NoActionsForConsumerId => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_NoActionsForConsumerId), ServiceHooksConsumerResources.resourceCulture);

    internal static string Error_NoMatchForConsumerIdOnActionTemplate => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (Error_NoMatchForConsumerIdOnActionTemplate), ServiceHooksConsumerResources.resourceCulture);

    internal static string SessionTokenNameFormat => ServiceHooksConsumerResources.ResourceManager.GetString(nameof (SessionTokenNameFormat), ServiceHooksConsumerResources.resourceCulture);
  }
}
