// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.MockMsdnEntitlement
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  internal class MockMsdnEntitlement
  {
    private static readonly Dictionary<string, List<MsdnEntitlement>> s_userEntitlementsMap = MockMsdnEntitlement.CreateUserEntitlementsMap();
    private const string s_area = "VisualStudio.Services.LicensingService.MsdnLicensingAdapter";
    private const string s_layer = "BusinessLogic";

    public List<MsdnEntitlement> GetEntitlementsForIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1031201, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", nameof (GetEntitlementsForIdentity));
      return MockMsdnEntitlement.GetMockedMsdnEntitlementForUser(requestContext, userIdentity);
    }

    private static List<MsdnEntitlement> GetMockedMsdnEntitlementForUser(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      string property = userIdentity.GetProperty<string>("Account", string.Empty);
      if (string.IsNullOrEmpty(property))
      {
        requestContext.Trace(1031204, TraceLevel.Warning, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "Unexpected: (user identity) accountName is null.");
        return new List<MsdnEntitlement>();
      }
      string licenseTypeForUser = MockMsdnEntitlement.GetMsdnLicenseTypeForUser(property);
      List<MsdnEntitlement> var;
      if (!MockMsdnEntitlement.s_userEntitlementsMap.TryGetValue(licenseTypeForUser, out var))
      {
        requestContext.Trace(1031205, TraceLevel.Error, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", "Could not find any entitlements for User : {0}", (object) property);
        return (List<MsdnEntitlement>) null;
      }
      ArgumentUtility.CheckForNull<List<MsdnEntitlement>>(var, "msdnEntitlements");
      requestContext.Trace(1031206, TraceLevel.Warning, "VisualStudio.Services.LicensingService.MsdnLicensingAdapter", "BusinessLogic", string.Format("Created {0} MsdnEntitlements using MockMsdnEntitlement with license for User : {1}", (object) var.Count, (object) licenseTypeForUser.ToUpper()));
      return var;
    }

    private static List<MsdnEntitlement> CreateMsdnEntitlements(
      MsdnLicenseType msdnLicense,
      string subscriptionLevelCode,
      string subscriptionLevelName)
    {
      return new List<MsdnEntitlement>()
      {
        MockMsdnEntitlement.GetDefaultEntitlement(msdnLicense, LicensingRequestType.Client, subscriptionLevelCode, subscriptionLevelName),
        MockMsdnEntitlement.GetDefaultEntitlement(msdnLicense, LicensingRequestType.Service, subscriptionLevelCode, subscriptionLevelName)
      };
    }

    private static List<MsdnEntitlement> CreateMsdnEntitlementsForExtension(
      string entitlementCode,
      string subscriptionLevelCode,
      string subscriptionLevelName)
    {
      return new List<MsdnEntitlement>()
      {
        MockMsdnEntitlement.GetDefaultExtensionEntitlement(entitlementCode, LicensingRequestType.Client, subscriptionLevelCode, subscriptionLevelName),
        MockMsdnEntitlement.GetDefaultExtensionEntitlement(entitlementCode, LicensingRequestType.Service, subscriptionLevelCode, subscriptionLevelName)
      };
    }

    private static string GetMsdnLicenseTypeForUser(string emailAddress)
    {
      MockMsdnEntitlement.MockMsdnLicenseType result = MockMsdnEntitlement.MockMsdnLicenseType.None;
      if (emailAddress.IndexOf("msdn", StringComparison.OrdinalIgnoreCase) < 0 && emailAddress.IndexOf("vsenterprise", StringComparison.OrdinalIgnoreCase) < 0 && emailAddress.IndexOf("vsprofessional", StringComparison.OrdinalIgnoreCase) < 0 && emailAddress.IndexOf("extension", StringComparison.OrdinalIgnoreCase) < 0)
        return string.Empty;
      foreach (string str in (IEnumerable<string>) ((IEnumerable<string>) Enum.GetNames(typeof (MockMsdnEntitlement.MockMsdnLicenseType))).OrderBy<string, string>((Func<string, string>) (o => o)))
      {
        if (emailAddress.IndexOf(str, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          if (Enum.TryParse<MockMsdnEntitlement.MockMsdnLicenseType>(str, true, out result))
            break;
        }
      }
      return result == MockMsdnEntitlement.MockMsdnLicenseType.None ? string.Empty : result.ToString().ToUpper();
    }

    private static MsdnEntitlement GetDefaultExtensionEntitlement(
      string EntitlementCode,
      LicensingRequestType requestType,
      string subscriptionLevelCode = "X-RTL-000036",
      string subscriptionLevelName = null)
    {
      return new MsdnEntitlement()
      {
        EntitlementCode = EntitlementCode,
        EntitlementName = subscriptionLevelName,
        EntitlementType = "VSTSExtension",
        IsEntitlementAvailable = true,
        SubscriptionExpirationDate = DateTimeOffset.UtcNow.AddYears(1),
        SubscriptionId = "555555",
        SubscriptionLevelCode = subscriptionLevelCode,
        SubscriptionLevelName = subscriptionLevelName,
        SubscriptionStatus = "Active",
        SubscriptionChannel = "FTE"
      };
    }

    private static MsdnEntitlement GetDefaultEntitlement(
      MsdnLicenseType msdnLicenseType,
      LicensingRequestType requestType,
      string subscriptionLevelCode = "X-RTL-000036",
      string subscriptionLevelName = null)
    {
      if (msdnLicenseType == MsdnLicenseType.Ultimate || msdnLicenseType == MsdnLicenseType.Premium)
        msdnLicenseType = MsdnLicenseType.Enterprise;
      if (requestType == LicensingRequestType.Client && msdnLicenseType == MsdnLicenseType.Platforms)
        msdnLicenseType = MsdnLicenseType.TestProfessional;
      string str = msdnLicenseType.ToString().ToUpper().Replace("PROFESSIONAL", "PRO");
      return new MsdnEntitlement()
      {
        EntitlementCode = requestType == LicensingRequestType.Client ? string.Format("IDE-{0}", (object) str) : (msdnLicenseType == MsdnLicenseType.Professional ? "VSO-STD" : "VSO-ADVP"),
        EntitlementName = requestType == LicensingRequestType.Client ? string.Format("Visual Studio {0} IDE", (object) str) : "Azure DevOps Services Advanced",
        EntitlementType = requestType == LicensingRequestType.Client ? "LicensingIde" : "LicensingVso",
        IsEntitlementAvailable = true,
        SubscriptionExpirationDate = DateTimeOffset.UtcNow.AddYears(1),
        SubscriptionId = "555555",
        SubscriptionLevelCode = subscriptionLevelCode,
        SubscriptionLevelName = subscriptionLevelName,
        SubscriptionStatus = "Active",
        SubscriptionChannel = "FTE"
      };
    }

    private static Dictionary<string, List<MsdnEntitlement>> CreateUserEntitlementsMap()
    {
      Dictionary<string, List<MsdnEntitlement>> dictionary = new Dictionary<string, List<MsdnEntitlement>>();
      dictionary.Add("MSDNULTIMATE", MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Ultimate, "X-RTL-000036", string.Format("VS {0} with MSDN (Retail)", (object) "Ultimate")));
      dictionary.Add("MSDNPROFESSIONAL", MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Professional, "X-RTL-000036", string.Format("VS {0} with MSDN (Retail)", (object) "Professional")));
      dictionary.Add("MSDNTESTPROFESSIONAL", MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.TestProfessional, "X-RTL-000036", string.Format("VS {0} with MSDN (Retail)", (object) "TestProfessional")));
      dictionary.Add("MSDNPREMIUM", MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Premium, "X-RTL-000036", string.Format("VS {0} with MSDN (Retail)", (object) "Premium")));
      List<MsdnEntitlement> msdnEntitlements = MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Enterprise, "X-RTL-000036", string.Format("VS {0} with MSDN (Retail)", (object) "Enterprise"));
      msdnEntitlements.AddRange((IEnumerable<MsdnEntitlement>) MockMsdnEntitlement.CreateMsdnEntitlementsForExtension("EXT-PKM-00001", "VSE-MON-0001", "PKG Management"));
      dictionary.Add("MSDNENTERPRISE", msdnEntitlements);
      dictionary.Add("MSDNPLATFORMS", MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Platforms, "MDN-SDG-000001", string.Format("VS {0} with MSDN (Retail)", (object) "Platforms")));
      dictionary.Add("VSENTERPRISEANNUAL", MockMsdnEntitlement.CreateVsEnterpriseEntitlements("VSE-ANU-0001", "VS Cloud Enterprise(Annual)"));
      dictionary.Add("VSENTERPRISEMONTHLY", MockMsdnEntitlement.CreateVsEnterpriseEntitlements("VSE-MON-0001", "VS Cloud Enterprise (Monthly)"));
      dictionary.Add("VSPROFESSIONALANNUAL", MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Professional, "VSP-ANU-0001", "VS Cloud Professional (Annual)"));
      dictionary.Add("VSPROFESSIONALMONTHLY", MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Professional, "VSP-MON-0001", "VS Cloud Professional (Monthly)"));
      dictionary.Add("TESTMANAGEREXTENSION", MockMsdnEntitlement.CreateMsdnEntitlementsForExtension("EXT-MTM-0001", "VSE-MON-0001", "Test Manager Extension"));
      return dictionary.AddRange<KeyValuePair<string, List<MsdnEntitlement>>, Dictionary<string, List<MsdnEntitlement>>>((IEnumerable<KeyValuePair<string, List<MsdnEntitlement>>>) dictionary.Where<KeyValuePair<string, List<MsdnEntitlement>>>((Func<KeyValuePair<string, List<MsdnEntitlement>>, bool>) (m => m.Key.StartsWith("MSDN"))).Select<KeyValuePair<string, List<MsdnEntitlement>>, KeyValuePair<string, List<MsdnEntitlement>>>((Func<KeyValuePair<string, List<MsdnEntitlement>>, KeyValuePair<string, List<MsdnEntitlement>>>) (ent => new KeyValuePair<string, List<MsdnEntitlement>>("Grace".ToUpper() + ent.Key, ent.Value.Select<MsdnEntitlement, MsdnEntitlement>((Func<MsdnEntitlement, MsdnEntitlement>) (e =>
      {
        MsdnEntitlement userEntitlementsMap = e.Clone();
        userEntitlementsMap.SubscriptionStatus = "Grace";
        return userEntitlementsMap;
      })).ToList<MsdnEntitlement>()))).ToList<KeyValuePair<string, List<MsdnEntitlement>>>());
    }

    private static List<MsdnEntitlement> CreateVsEnterpriseEntitlements(
      string subscriptionLevelCode,
      string subscriptionLevelName)
    {
      List<MsdnEntitlement> msdnEntitlements = MockMsdnEntitlement.CreateMsdnEntitlements(MsdnLicenseType.Enterprise, subscriptionLevelCode, subscriptionLevelName);
      msdnEntitlements.AddRange((IEnumerable<MsdnEntitlement>) MockMsdnEntitlement.CreateMsdnEntitlementsForExtension("EXT-MTM-0001", "VSE-MON-0001", "Test Manager Extension"));
      return msdnEntitlements;
    }

    private enum MockMsdnLicenseType
    {
      None = 0,
      MsdnProfessional = 2,
      MsdnPlatforms = 3,
      MsdnTestProfessional = 4,
      MsdnPremium = 5,
      MsdnUltimate = 6,
      MsdnEnterprise = 7,
      VsEnterpriseAnnual = 8,
      VsProfessionalAnnual = 9,
      VsEnterpriseMonthly = 10, // 0x0000000A
      VsProfessionalMonthly = 11, // 0x0000000B
      TestManagerExtension = 12, // 0x0000000C
      GraceMsdnProfessional = 13, // 0x0000000D
      GraceMsdnPlatforms = 14, // 0x0000000E
      GraceMsdnTestProfessional = 15, // 0x0000000F
      GraceMsdnPremium = 16, // 0x00000010
      GraceMsdnUltimate = 17, // 0x00000011
      GraceMsdnEnterprise = 18, // 0x00000012
    }
  }
}
