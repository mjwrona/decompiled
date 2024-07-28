// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.PolicyDataValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public static class PolicyDataValidator
  {
    private const int c_maxPolicyErrorMessageLength = 250;

    public static void ValidatePolicyNameAndData(string policyName, Policy policy)
    {
      PolicyDataValidator.ValidatePolicyName(policyName);
      PolicyDataValidator.ValidatePolicyData(policy);
      if (!policyName.Equals(policy.Name))
        throw new TenantPolicyBadRequestException(FrameworkResources.TenantPolicyNameDataDoNotMatch((object) policy.Name, (object) policyName));
    }

    public static void ValidatePolicyName(string policyName)
    {
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(policyName, nameof (policyName));
        ArgumentUtility.CheckStringForAnyWhiteSpace(policyName, nameof (policyName));
      }
      catch (Exception ex)
      {
        throw new TenantPolicyBadRequestException(ex.Message);
      }
      if (!PolicyNames.KnownTenantPolicyNames.Contains<string>(policyName))
        throw new TenantPolicyBadRequestException(FrameworkResources.TenantPolicyNotExist((object) policyName));
    }

    public static void ValidatePolicyData(Policy policy)
    {
      try
      {
        ArgumentUtility.CheckForNull<Policy>(policy, nameof (policy));
        PolicyDataValidator.ValidatePolicyName(policy.Name);
        PolicyDataValidator.CheckProperties(policy);
      }
      catch (Exception ex)
      {
        throw new TenantPolicyBadRequestException(ex.Message);
      }
    }

    public static void ValidatePolicyTenantId(Guid tenantId)
    {
      if (tenantId == Guid.Empty)
        throw new TenantPolicyBadRequestException(FrameworkResources.TenantPolicyInvalidTenantId((object) tenantId.ToString()));
    }

    private static void CheckProperties(Policy policy)
    {
      if (policy.Properties == null || policy.Properties.Count == 0)
        return;
      HashSet<string> hashSet = PolicyProperties.TenantPolicyProperties[policy.Name].ToHashSet<string>();
      foreach (KeyValuePair<string, string> property in (IEnumerable<KeyValuePair<string, string>>) policy.Properties)
      {
        string key = property.Key;
        string str = property.Value;
        if (!hashSet.Contains(key))
          throw new ArgumentException(FrameworkResources.TenantPolicyInvalidProperty((object) key));
        int result;
        if (key.Equals("MaxPatLifespanInDays") && (!int.TryParse(str, out result) || result <= 0))
          throw new ArgumentException(FrameworkResources.TenantPolicyInvalidMaxPatLifespanInDays());
        if (!string.IsNullOrEmpty(str))
        {
          ArgumentUtility.CheckStringForInvalidSqlEscapeCharacters(str, "propertyValue");
          if (key.Equals("ErrorMessage"))
            ArgumentUtility.CheckStringLength(str, "propertyValue", 250);
          if (key.Equals("AllowedUsersAndGroupObjectIds"))
            JsonUtilities.Deserialize<ISet<Guid>>(str);
        }
      }
    }
  }
}
