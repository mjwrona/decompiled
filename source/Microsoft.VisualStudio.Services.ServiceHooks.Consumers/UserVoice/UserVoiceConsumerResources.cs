// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.UserVoice.UserVoiceConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.UserVoice
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class UserVoiceConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal UserVoiceConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (UserVoiceConsumerResources.resourceMan == null)
          UserVoiceConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.UserVoice.UserVoiceConsumerResources", typeof (UserVoiceConsumerResources).Assembly);
        return UserVoiceConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => UserVoiceConsumerResources.resourceCulture;
      set => UserVoiceConsumerResources.resourceCulture = value;
    }

    internal static string ConsumerDescription => UserVoiceConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), UserVoiceConsumerResources.resourceCulture);

    internal static string ConsumerName => UserVoiceConsumerResources.ResourceManager.GetString(nameof (ConsumerName), UserVoiceConsumerResources.resourceCulture);

    internal static string NoopAction_NoUserVoiceArtifact => UserVoiceConsumerResources.ResourceManager.GetString(nameof (NoopAction_NoUserVoiceArtifact), UserVoiceConsumerResources.resourceCulture);

    internal static string SendLinkedWorkItemEventAction_AuthTokenInputDescription => UserVoiceConsumerResources.ResourceManager.GetString(nameof (SendLinkedWorkItemEventAction_AuthTokenInputDescription), UserVoiceConsumerResources.resourceCulture);

    internal static string SendLinkedWorkItemEventAction_AuthTokenInputName => UserVoiceConsumerResources.ResourceManager.GetString(nameof (SendLinkedWorkItemEventAction_AuthTokenInputName), UserVoiceConsumerResources.resourceCulture);

    internal static string SendLinkedWorkItemEventAction_DetailedDescriptionFormat => UserVoiceConsumerResources.ResourceManager.GetString(nameof (SendLinkedWorkItemEventAction_DetailedDescriptionFormat), UserVoiceConsumerResources.resourceCulture);

    internal static string SendLinkedWorkItemEventAction_UrlInputDescription => UserVoiceConsumerResources.ResourceManager.GetString(nameof (SendLinkedWorkItemEventAction_UrlInputDescription), UserVoiceConsumerResources.resourceCulture);

    internal static string SendLinkedWorkItemEventAction_UrlInputName => UserVoiceConsumerResources.ResourceManager.GetString(nameof (SendLinkedWorkItemEventAction_UrlInputName), UserVoiceConsumerResources.resourceCulture);

    internal static string SendLinkedWorkItemEventActionDescription => UserVoiceConsumerResources.ResourceManager.GetString(nameof (SendLinkedWorkItemEventActionDescription), UserVoiceConsumerResources.resourceCulture);

    internal static string SendLinkedWorkItemEventActionName => UserVoiceConsumerResources.ResourceManager.GetString(nameof (SendLinkedWorkItemEventActionName), UserVoiceConsumerResources.resourceCulture);
  }
}
