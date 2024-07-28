// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Licensing.LicensingResources
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server.Licensing
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class LicensingResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal LicensingResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (LicensingResources.resourceMan == null)
          LicensingResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Framework.Server.Licensing.LicensingResources", typeof (LicensingResources).Assembly);
        return LicensingResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => LicensingResources.resourceCulture;
      set => LicensingResources.resourceCulture = value;
    }

    public static string LicenseNameAccountAdvanced => LicensingResources.ResourceManager.GetString(nameof (LicenseNameAccountAdvanced), LicensingResources.resourceCulture);

    public static string LicenseNameAccountBasicPlusTestPlans => LicensingResources.ResourceManager.GetString(nameof (LicenseNameAccountBasicPlusTestPlans), LicensingResources.resourceCulture);

    public static string LicenseNameAccountEarlyAdopter => LicensingResources.ResourceManager.GetString(nameof (LicenseNameAccountEarlyAdopter), LicensingResources.resourceCulture);

    public static string LicenseNameAccountExpress => LicensingResources.ResourceManager.GetString(nameof (LicenseNameAccountExpress), LicensingResources.resourceCulture);

    public static string LicenseNameAccountProfessional => LicensingResources.ResourceManager.GetString(nameof (LicenseNameAccountProfessional), LicensingResources.resourceCulture);

    public static string LicenseNameAccountStakeholder => LicensingResources.ResourceManager.GetString(nameof (LicenseNameAccountStakeholder), LicensingResources.resourceCulture);

    public static string LicenseNameMsdnEligible => LicensingResources.ResourceManager.GetString(nameof (LicenseNameMsdnEligible), LicensingResources.resourceCulture);

    public static string LicenseNameMsdnEnterprise => LicensingResources.ResourceManager.GetString(nameof (LicenseNameMsdnEnterprise), LicensingResources.resourceCulture);

    public static string LicenseNameMsdnPlatforms => LicensingResources.ResourceManager.GetString(nameof (LicenseNameMsdnPlatforms), LicensingResources.resourceCulture);

    public static string LicenseNameMsdnPremium => LicensingResources.ResourceManager.GetString(nameof (LicenseNameMsdnPremium), LicensingResources.resourceCulture);

    public static string LicenseNameMsdnProfessional => LicensingResources.ResourceManager.GetString(nameof (LicenseNameMsdnProfessional), LicensingResources.resourceCulture);

    public static string LicenseNameMsdnTestProfessional => LicensingResources.ResourceManager.GetString(nameof (LicenseNameMsdnTestProfessional), LicensingResources.resourceCulture);

    public static string LicenseNameMsdnUltimate => LicensingResources.ResourceManager.GetString(nameof (LicenseNameMsdnUltimate), LicensingResources.resourceCulture);

    public static string UsersAlreadyInProjectErrorMessage => LicensingResources.ResourceManager.GetString(nameof (UsersAlreadyInProjectErrorMessage), LicensingResources.resourceCulture);

    public static string UserStatusExpired => LicensingResources.ResourceManager.GetString(nameof (UserStatusExpired), LicensingResources.resourceCulture);

    public static string UserStatusMsdnDisabledEligible => LicensingResources.ResourceManager.GetString(nameof (UserStatusMsdnDisabledEligible), LicensingResources.resourceCulture);

    public static string UserStatusMsdnExpired => LicensingResources.ResourceManager.GetString(nameof (UserStatusMsdnExpired), LicensingResources.resourceCulture);

    public static string UserStatusMsdnNotValidated => LicensingResources.ResourceManager.GetString(nameof (UserStatusMsdnNotValidated), LicensingResources.resourceCulture);

    public static string UserStatusMsdnPending => LicensingResources.ResourceManager.GetString(nameof (UserStatusMsdnPending), LicensingResources.resourceCulture);

    public static string UserWithNoGroupMembership => LicensingResources.ResourceManager.GetString(nameof (UserWithNoGroupMembership), LicensingResources.resourceCulture);
  }
}
