// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Kato.KatoConsumerResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Kato
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class KatoConsumerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal KatoConsumerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (KatoConsumerResources.resourceMan == null)
          KatoConsumerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Kato.KatoConsumerResources", typeof (KatoConsumerResources).Assembly);
        return KatoConsumerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => KatoConsumerResources.resourceCulture;
      set => KatoConsumerResources.resourceCulture = value;
    }

    internal static string Kato_ConsumerDescription => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_ConsumerDescription), KatoConsumerResources.resourceCulture);

    internal static string Kato_ConsumerName => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_ConsumerName), KatoConsumerResources.resourceCulture);

    internal static string Kato_PostEventToRoom_ActionDescription => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_PostEventToRoom_ActionDescription), KatoConsumerResources.resourceCulture);

    internal static string Kato_PostEventToRoom_ActionName => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_PostEventToRoom_ActionName), KatoConsumerResources.resourceCulture);

    internal static string Kato_RoomName_InputDescription => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_RoomName_InputDescription), KatoConsumerResources.resourceCulture);

    internal static string Kato_RoomName_InputName => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_RoomName_InputName), KatoConsumerResources.resourceCulture);

    internal static string Kato_RoomToken_InputDescription => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_RoomToken_InputDescription), KatoConsumerResources.resourceCulture);

    internal static string Kato_RoomToken_InputName => KatoConsumerResources.ResourceManager.GetString(nameof (Kato_RoomToken_InputName), KatoConsumerResources.resourceCulture);
  }
}
