// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azuqua.AzuquaConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azuqua
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class AzuquaConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AzuquaConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (AzuquaConsumerResources.resourceMan == null)
          AzuquaConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azuqua.AzuquaConsumerResources", typeof (AzuquaConsumerResources).Assembly);
        return AzuquaConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => AzuquaConsumerResources.resourceCulture;
      set => AzuquaConsumerResources.resourceCulture = value;
    }

    public static string Azuqua_ConsumerDescription => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_ConsumerDescription), AzuquaConsumerResources.resourceCulture);

    public static string Azuqua_ConsumerName => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_ConsumerName), AzuquaConsumerResources.resourceCulture);

    public static string Azuqua_PostEventToFlo_ActionDescription => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_PostEventToFlo_ActionDescription), AzuquaConsumerResources.resourceCulture);

    public static string Azuqua_PostEventToFlo_ActionName => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_PostEventToFlo_ActionName), AzuquaConsumerResources.resourceCulture);

    public static string Azuqua_PostEventToFlo_ClientToken_InputDescription => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_PostEventToFlo_ClientToken_InputDescription), AzuquaConsumerResources.resourceCulture);

    public static string Azuqua_PostEventToFlo_ClientToken_InputName => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_PostEventToFlo_ClientToken_InputName), AzuquaConsumerResources.resourceCulture);

    public static string Azuqua_PostEventToFlo_WebhookId_InputDescription => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_PostEventToFlo_WebhookId_InputDescription), AzuquaConsumerResources.resourceCulture);

    public static string Azuqua_PostEventToFlo_WebhookId_InputName => AzuquaConsumerResources.ResourceManager.GetString(nameof (Azuqua_PostEventToFlo_WebhookId_InputName), AzuquaConsumerResources.resourceCulture);
  }
}
