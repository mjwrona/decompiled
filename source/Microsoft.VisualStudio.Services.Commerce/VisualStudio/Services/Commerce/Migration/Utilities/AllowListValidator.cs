// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.Utilities.AllowListValidator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce.Migration.Utilities
{
  internal static class AllowListValidator
  {
    private static readonly HashSet<int> m_allowListMeterIds = new HashSet<int>()
    {
      CommerceAllowListMeterIds.BasicsMeterId,
      CommerceAllowListMeterIds.PremiumBuildAgentMeterId,
      CommerceAllowListMeterIds.TestManagerMeterId,
      CommerceAllowListMeterIds.ArtifactsMeterId,
      CommerceAllowListMeterIds.PrivateBuildAgentMeterId
    };
    private static readonly HashSet<int> m_allowListDevfabricMeterIds = new HashSet<int>()
    {
      CommerceDevfabricMeterIds.BasicsMeterId,
      CommerceDevfabricMeterIds.PremiumBuildAgentMeterId,
      CommerceDevfabricMeterIds.TestManagerMeterId,
      CommerceDevfabricMeterIds.ArtifactsMeterId,
      CommerceDevfabricMeterIds.PrivateBuildAgentMeterId
    };

    public static IEnumerable<int> RetrieveAllowedMeterIds(
      IVssRequestContext requestContext,
      IEnumerable<int> meterIds)
    {
      Enumerable.Empty<int>();
      return !requestContext.ServiceHost.IsProduction ? (IEnumerable<int>) meterIds.Where<int>((Func<int, bool>) (id => AllowListValidator.m_allowListDevfabricMeterIds.Contains(id))).ToList<int>() : (IEnumerable<int>) meterIds.Where<int>((Func<int, bool>) (id => AllowListValidator.m_allowListMeterIds.Contains(id))).ToList<int>();
    }

    public static bool IsAllowed(IVssRequestContext requestContext, int meterId) => requestContext.ServiceHost.IsProduction ? AllowListValidator.m_allowListMeterIds.Contains(meterId) : AllowListValidator.m_allowListDevfabricMeterIds.Contains(meterId);
  }
}
