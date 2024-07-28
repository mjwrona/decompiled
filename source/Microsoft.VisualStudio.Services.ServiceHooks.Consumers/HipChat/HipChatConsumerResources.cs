// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HipChat.HipChatConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HipChat
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class HipChatConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal HipChatConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (HipChatConsumerResources.resourceMan == null)
          HipChatConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.HipChat.HipChatConsumerResources", typeof (HipChatConsumerResources).Assembly);
        return HipChatConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => HipChatConsumerResources.resourceCulture;
      set => HipChatConsumerResources.resourceCulture = value;
    }

    public static string ConsumerDescription => HipChatConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), HipChatConsumerResources.resourceCulture);

    public static string ConsumerName => HipChatConsumerResources.ResourceManager.GetString(nameof (ConsumerName), HipChatConsumerResources.resourceCulture);

    public static string HipChatConsumer_AuthTokenInputDescription => HipChatConsumerResources.ResourceManager.GetString(nameof (HipChatConsumer_AuthTokenInputDescription), HipChatConsumerResources.resourceCulture);

    public static string HipChatConsumer_AuthTokenInputName => HipChatConsumerResources.ResourceManager.GetString(nameof (HipChatConsumer_AuthTokenInputName), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorEmpty => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorEmpty), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorGray => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorGray), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorGreen => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorGreen), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorInputDescription => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorInputDescription), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorInputName => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorInputName), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorPurple => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorPurple), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorRandom => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorRandom), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorRed => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorRed), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_BgColorYellow => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_BgColorYellow), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_NotifyRoomInputDescription => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_NotifyRoomInputDescription), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_NotifyRoomInputName => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_NotifyRoomInputName), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_QueryError_ExceptionFormat => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_QueryError_ExceptionFormat), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_QueryError_ResponseFailureFormat => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_QueryError_ResponseFailureFormat), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_QueryError_SuppliedTokenNotAuthorized => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_QueryError_SuppliedTokenNotAuthorized), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_RoomNameInputDescription => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_RoomNameInputDescription), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_RoomNameInputName => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_RoomNameInputName), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_ShowDetailsInputDescription => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_ShowDetailsInputDescription), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomAction_ShowDetailsInputName => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_ShowDetailsInputName), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomActionDescription => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomActionDescription), HipChatConsumerResources.resourceCulture);

    public static string PostMessageToRoomActionName => HipChatConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomActionName), HipChatConsumerResources.resourceCulture);
  }
}
