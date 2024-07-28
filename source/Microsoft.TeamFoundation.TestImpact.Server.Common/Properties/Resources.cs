// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
        if (Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceMan == null)
          Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.TestImpact.Server.Properties.Resources", typeof (Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources).Assembly);
        return Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture;
      set => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture = value;
    }

    internal static string AccessDenied => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (AccessDenied), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string ArgumentNull => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (ArgumentNull), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string ArgumentNullOrEmpty => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (ArgumentNullOrEmpty), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string AutomatedTestCase => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (AutomatedTestCase), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string FromDateGreaterThanToDate => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (FromDateGreaterThanToDate), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string InvalidArgument => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (InvalidArgument), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string InvalidBuildId => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (InvalidBuildId), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string ManualTestCase => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (ManualTestCase), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);

    internal static string MaxCodeSignaturesLimitCrossedError => Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.ResourceManager.GetString(nameof (MaxCodeSignaturesLimitCrossedError), Microsoft.TeamFoundation.TestImpact.Server.Common.Properties.Resources.resourceCulture);
  }
}
