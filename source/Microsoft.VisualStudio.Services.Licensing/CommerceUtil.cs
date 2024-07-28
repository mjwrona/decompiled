// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CommerceUtil
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AzComm.SharedContracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class CommerceUtil
  {
    private static readonly IReadOnlyDictionary<AccountLicenseType, Guid> _accountLicenseMeterIdMap = (IReadOnlyDictionary<AccountLicenseType, Guid>) new Dictionary<AccountLicenseType, Guid>()
    {
      {
        AccountLicenseType.Express,
        AzCommMeterIds.BasicsMeterId
      },
      {
        AccountLicenseType.Advanced,
        AzCommMeterIds.TestManagerMeterId
      }
    };
    private static readonly string _isAzCommV2ApiEnabledFeatureFlag = "AzureDevOps.Services.Licensing.UseCommerceV2Apis";

    public static bool IsAzCommV2ApiEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(CommerceUtil._isAzCommV2ApiEnabledFeatureFlag);

    public static IReadOnlyDictionary<AccountLicenseType, Guid> GetAccountLicenseTypeToMeterIdMap() => CommerceUtil._accountLicenseMeterIdMap;

    public static IReadOnlyDictionary<Guid, AccountLicenseType> GetMeterIdToAccountLicenseTypeMap() => (IReadOnlyDictionary<Guid, AccountLicenseType>) CommerceUtil._accountLicenseMeterIdMap.ToDictionary<KeyValuePair<AccountLicenseType, Guid>, Guid, AccountLicenseType>((Func<KeyValuePair<AccountLicenseType, Guid>, Guid>) (i => i.Value), (Func<KeyValuePair<AccountLicenseType, Guid>, AccountLicenseType>) (i => i.Key));
  }
}
