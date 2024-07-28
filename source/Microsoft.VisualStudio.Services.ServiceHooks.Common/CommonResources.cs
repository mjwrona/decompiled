// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.CommonResources
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class CommonResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CommonResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (CommonResources.resourceMan == null)
          CommonResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ServiceHooks.Common.CommonResources", typeof (CommonResources).Assembly);
        return CommonResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => CommonResources.resourceCulture;
      set => CommonResources.resourceCulture = value;
    }

    internal static string InvalidUrlLoopback => CommonResources.ResourceManager.GetString(nameof (InvalidUrlLoopback), CommonResources.resourceCulture);

    internal static string InvalidUrlSpecialPurposeFormat => CommonResources.ResourceManager.GetString(nameof (InvalidUrlSpecialPurposeFormat), CommonResources.resourceCulture);

    internal static string NoCustomActionDescription => CommonResources.ResourceManager.GetString(nameof (NoCustomActionDescription), CommonResources.resourceCulture);

    internal static string SessionTokenIdentityNotFound => CommonResources.ResourceManager.GetString(nameof (SessionTokenIdentityNotFound), CommonResources.resourceCulture);

    internal static string TaskCanceledTimeout => CommonResources.ResourceManager.GetString(nameof (TaskCanceledTimeout), CommonResources.resourceCulture);
  }
}
