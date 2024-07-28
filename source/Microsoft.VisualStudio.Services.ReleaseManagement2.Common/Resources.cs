// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceMan == null)
          Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources", typeof (Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources).Assembly);
        return Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceCulture;
      set => Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceCulture = value;
    }

    internal static string CannotFindSecurityPermission => Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.ResourceManager.GetString(nameof (CannotFindSecurityPermission), Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceCulture);

    internal static string CannotFindSecurityToken => Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.ResourceManager.GetString(nameof (CannotFindSecurityToken), Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceCulture);

    internal static string InvalidValuesInMultiplierInput => Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.ResourceManager.GetString(nameof (InvalidValuesInMultiplierInput), Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceCulture);

    internal static string Machine => Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.ResourceManager.GetString(nameof (Machine), Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceCulture);

    internal static string Multiplier => Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.ResourceManager.GetString(nameof (Multiplier), Microsoft.VisualStudio.Services.ReleaseManagement.Common.Resources.resourceCulture);
  }
}
