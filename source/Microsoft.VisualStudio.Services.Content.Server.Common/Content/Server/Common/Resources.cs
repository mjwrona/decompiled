// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceMan == null)
          Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.Content.Server.Common.Resources", typeof (Microsoft.VisualStudio.Services.Content.Server.Common.Resources).Assembly);
        return Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceCulture;
      set => Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceCulture = value;
    }

    internal static string ForecastedCpuOverMaxThreshold => Microsoft.VisualStudio.Services.Content.Server.Common.Resources.ResourceManager.GetString(nameof (ForecastedCpuOverMaxThreshold), Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceCulture);

    internal static string ForecastedCpuOverMinThresholdAndRandomlySelected => Microsoft.VisualStudio.Services.Content.Server.Common.Resources.ResourceManager.GetString(nameof (ForecastedCpuOverMinThresholdAndRandomlySelected), Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceCulture);

    internal static string ThrottlingResponseMessage => Microsoft.VisualStudio.Services.Content.Server.Common.Resources.ResourceManager.GetString(nameof (ThrottlingResponseMessage), Microsoft.VisualStudio.Services.Content.Server.Common.Resources.resourceCulture);
  }
}
