// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PolicyArgumentValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class PolicyArgumentValidator
  {
    private const int c_maxPolicyNameLength = 100;
    private static readonly IDictionary<Type, PolicyArgumentValidator.PolicyValueType> s_SupportedTypes = (IDictionary<Type, PolicyArgumentValidator.PolicyValueType>) new Dictionary<Type, PolicyArgumentValidator.PolicyValueType>()
    {
      {
        typeof (object),
        PolicyArgumentValidator.PolicyValueType.Unknown
      },
      {
        typeof (bool),
        PolicyArgumentValidator.PolicyValueType.Boolean
      },
      {
        typeof (int),
        PolicyArgumentValidator.PolicyValueType.Integer
      },
      {
        typeof (string),
        PolicyArgumentValidator.PolicyValueType.String
      },
      {
        typeof (double),
        PolicyArgumentValidator.PolicyValueType.Double
      },
      {
        typeof (DateTime),
        PolicyArgumentValidator.PolicyValueType.DateTime
      },
      {
        typeof (long),
        PolicyArgumentValidator.PolicyValueType.Long
      }
    };

    public static void ValidatePolicyName(string policyName)
    {
      try
      {
        ArgumentUtility.CheckForNull<string>(policyName, nameof (policyName));
        ArgumentUtility.CheckStringForNullOrEmpty(policyName, nameof (policyName));
        ArgumentUtility.CheckStringForAnyWhiteSpace(policyName, nameof (policyName));
      }
      catch (Exception ex)
      {
        throw new OrganizationBadRequestException(ex.Message);
      }
    }

    public static void ValidatePolicyValue<T>(T value)
    {
      try
      {
        PolicyArgumentValidator.ValidatePolicyValueType<T>();
        ArgumentUtility.CheckGenericForNull((object) value, nameof (value));
        RegistryUtility.ToString<T>(value);
      }
      catch (Exception ex)
      {
        throw new OrganizationBadRequestException(ex.Message);
      }
    }

    public static void ValidatePolicyValueType<T>()
    {
      try
      {
        Type key = typeof (T);
        if (!PolicyArgumentValidator.s_SupportedTypes.ContainsKey(key))
          throw new ArgumentException(string.Format("Setting of value type {0} is not supported.", (object) key));
      }
      catch (Exception ex)
      {
        throw new OrganizationBadRequestException(ex.Message);
      }
    }

    public static void ValidatePolicyNameSize(string policyName)
    {
      Exception validationException;
      if (!PolicyArgumentValidator.TryValidatePolicyNameSize(policyName, out validationException))
        throw validationException;
    }

    public static bool TryValidatePolicyNameSize(
      string policyName,
      out Exception validationException)
    {
      if (policyName.Length < 1 || policyName.Length > 100)
      {
        validationException = (Exception) new OrganizationBadRequestException(string.Format("Policy name must not be more than {0} characters", (object) 100));
        return false;
      }
      validationException = (Exception) null;
      return true;
    }

    private enum PolicyValueType
    {
      Unknown,
      Boolean,
      Integer,
      String,
      Double,
      DateTime,
      Long,
    }
  }
}
