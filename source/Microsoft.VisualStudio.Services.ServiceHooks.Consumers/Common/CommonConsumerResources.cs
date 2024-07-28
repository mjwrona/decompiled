// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.CommonConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class CommonConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CommonConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (CommonConsumerResources.resourceMan == null)
          CommonConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.CommonConsumerResources", typeof (CommonConsumerResources).Assembly);
        return CommonConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => CommonConsumerResources.resourceCulture;
      set => CommonConsumerResources.resourceCulture = value;
    }

    public static string Error_WorkItemFieldDoNotExist => CommonConsumerResources.ResourceManager.GetString(nameof (Error_WorkItemFieldDoNotExist), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_DetailedMessagesToSendInputDescription => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_DetailedMessagesToSendInputDescription), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_DetailedMessagesToSendInputName => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_DetailedMessagesToSendInputName), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventMessages_All => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventMessages_All), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventMessages_Html => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventMessages_Html), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventMessages_Markdown => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventMessages_Markdown), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventMessages_None => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventMessages_None), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventMessages_Text => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventMessages_Text), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventResourceDetails_All => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventResourceDetails_All), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventResourceDetails_Minimal => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventResourceDetails_Minimal), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_EventResourceDetails_None => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_EventResourceDetails_None), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_MessagesToSendInputDescription => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_MessagesToSendInputDescription), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_MessagesToSendInputName => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_MessagesToSendInputName), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_MinimalPayloadInputDescription => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_MinimalPayloadInputDescription), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_MinimalPayloadInputName => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_MinimalPayloadInputName), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_ResourceDetailsToSendInputDescription => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_ResourceDetailsToSendInputDescription), CommonConsumerResources.resourceCulture);

    public static string EventTransformerConsumerAction_ResourceDetailsToSendInputName => CommonConsumerResources.ResourceManager.GetString(nameof (EventTransformerConsumerAction_ResourceDetailsToSendInputName), CommonConsumerResources.resourceCulture);

    public static string InputDescriptor_AdvancedGroupName => CommonConsumerResources.ResourceManager.GetString(nameof (InputDescriptor_AdvancedGroupName), CommonConsumerResources.resourceCulture);

    public static string InputDescriptor_SupportsTemplatesIndicationLabel => CommonConsumerResources.ResourceManager.GetString(nameof (InputDescriptor_SupportsTemplatesIndicationLabel), CommonConsumerResources.resourceCulture);

    public static string ReleaseHelper_ErrorWhileExtractingJToken => CommonConsumerResources.ResourceManager.GetString(nameof (ReleaseHelper_ErrorWhileExtractingJToken), CommonConsumerResources.resourceCulture);

    public static string ReleaseHelper_TextLinkFormat => CommonConsumerResources.ResourceManager.GetString(nameof (ReleaseHelper_TextLinkFormat), CommonConsumerResources.resourceCulture);
  }
}
