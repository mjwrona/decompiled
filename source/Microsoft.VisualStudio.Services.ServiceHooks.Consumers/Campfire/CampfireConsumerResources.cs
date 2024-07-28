// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Campfire.CampfireConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Campfire
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class CampfireConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CampfireConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (CampfireConsumerResources.resourceMan == null)
          CampfireConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Campfire.CampfireConsumerResources", typeof (CampfireConsumerResources).Assembly);
        return CampfireConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => CampfireConsumerResources.resourceCulture;
      set => CampfireConsumerResources.resourceCulture = value;
    }

    internal static string CampfireConsumer_AccountNameInputDescription => CampfireConsumerResources.ResourceManager.GetString(nameof (CampfireConsumer_AccountNameInputDescription), CampfireConsumerResources.resourceCulture);

    internal static string CampfireConsumer_AccountNameInputName => CampfireConsumerResources.ResourceManager.GetString(nameof (CampfireConsumer_AccountNameInputName), CampfireConsumerResources.resourceCulture);

    internal static string CampfireConsumer_AuthTokenInputDescription => CampfireConsumerResources.ResourceManager.GetString(nameof (CampfireConsumer_AuthTokenInputDescription), CampfireConsumerResources.resourceCulture);

    internal static string CampfireConsumer_AuthTokenInputName => CampfireConsumerResources.ResourceManager.GetString(nameof (CampfireConsumer_AuthTokenInputName), CampfireConsumerResources.resourceCulture);

    internal static string ConsumerDescription => CampfireConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), CampfireConsumerResources.resourceCulture);

    internal static string ConsumerName => CampfireConsumerResources.ResourceManager.GetString(nameof (ConsumerName), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomAction_QueryError_ExceptionFormat => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_QueryError_ExceptionFormat), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomAction_QueryError_ResponseFailureFormat => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_QueryError_ResponseFailureFormat), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomAction_QueryError_SuppliedCredentialsNotAuthorized => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_QueryError_SuppliedCredentialsNotAuthorized), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomAction_RoomIdInputDescription => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_RoomIdInputDescription), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomAction_RoomIdInputName => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_RoomIdInputName), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomAction_ShowDetailsInputDescription => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_ShowDetailsInputDescription), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomAction_ShowDetailsInputName => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomAction_ShowDetailsInputName), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomActionDescription => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomActionDescription), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomActionDetailFormat => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomActionDetailFormat), CampfireConsumerResources.resourceCulture);

    internal static string PostMessageToRoomActionName => CampfireConsumerResources.ResourceManager.GetString(nameof (PostMessageToRoomActionName), CampfireConsumerResources.resourceCulture);
  }
}
