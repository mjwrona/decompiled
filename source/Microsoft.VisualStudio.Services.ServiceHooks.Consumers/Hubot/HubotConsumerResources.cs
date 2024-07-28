// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot.HubotConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class HubotConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal HubotConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (HubotConsumerResources.resourceMan == null)
          HubotConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Hubot.HubotConsumerResources", typeof (HubotConsumerResources).Assembly);
        return HubotConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => HubotConsumerResources.resourceCulture;
      set => HubotConsumerResources.resourceCulture = value;
    }

    public static string ConsumerDescription => HubotConsumerResources.ResourceManager.GetString(nameof (ConsumerDescription), HubotConsumerResources.resourceCulture);

    public static string ConsumerName => HubotConsumerResources.ResourceManager.GetString(nameof (ConsumerName), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_ConnectionStringInputDescription => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_ConnectionStringInputDescription), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_ConnectionStringInputName => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_ConnectionStringInputName), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_QueryError_ExceptionFormat => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_QueryError_ExceptionFormat), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_QueryError_MessagingExceptionFormat => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_QueryError_MessagingExceptionFormat), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_QueryError_SuppliedConnectionStringNotAuthorized => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_QueryError_SuppliedConnectionStringNotAuthorized), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_QueryError_SuppliedConnectionStringNotWellFormed => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_QueryError_SuppliedConnectionStringNotWellFormed), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_QueueNameInputDescription => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_QueueNameInputDescription), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotAction_QueueNameInputName => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotAction_QueueNameInputName), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotActionDescription => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotActionDescription), HubotConsumerResources.resourceCulture);

    public static string EnqueueTeamRoomMessageToHubotActionName => HubotConsumerResources.ResourceManager.GetString(nameof (EnqueueTeamRoomMessageToHubotActionName), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotAction_PasswordInputDescription => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotAction_PasswordInputDescription), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotAction_PasswordInputName => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotAction_PasswordInputName), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotAction_UrlInputDescription => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotAction_UrlInputDescription), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotAction_UrlInputName => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotAction_UrlInputName), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotAction_UsernameInputDescription => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotAction_UsernameInputDescription), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotAction_UsernameInputName => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotAction_UsernameInputName), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotActionDescription => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotActionDescription), HubotConsumerResources.resourceCulture);

    public static string PostTeamRoomMessageToHubotActionName => HubotConsumerResources.ResourceManager.GetString(nameof (PostTeamRoomMessageToHubotActionName), HubotConsumerResources.resourceCulture);
  }
}
