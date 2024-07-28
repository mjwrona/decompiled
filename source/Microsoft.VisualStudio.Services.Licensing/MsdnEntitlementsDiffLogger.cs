// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.MsdnEntitlementsDiffLogger
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class MsdnEntitlementsDiffLogger : EntityDiffLogger
  {
    private readonly IList<Tuple<MsdnEntitlement, MsdnEntitlement, IList<string>>> m_diffs;
    private const string c_area = "Licensing";
    private const string c_layer = "MsdnEntitlementsDiffLogger";
    private static readonly TimeSpan s_tenMinutes = TimeSpan.FromMinutes(10.0);

    public MsdnEntitlementsDiffLogger(IList<MsdnEntitlement> source, IList<MsdnEntitlement> target) => this.m_diffs = (IList<Tuple<MsdnEntitlement, MsdnEntitlement, IList<string>>>) EntityDiffLogger.CompareCollections<MsdnEntitlement, string>((IEnumerable<MsdnEntitlement>) source, (IEnumerable<MsdnEntitlement>) target, (Func<MsdnEntitlement, string>) (right => this.GetIdentifier(right)), new Func<MsdnEntitlement, MsdnEntitlement, Tuple<bool, IList<string>>>(this.IsMatch)).ToList<Tuple<MsdnEntitlement, MsdnEntitlement, IList<string>>>();

    public bool HasDiff() => !this.m_diffs.IsNullOrEmpty<Tuple<MsdnEntitlement, MsdnEntitlement, IList<string>>>();

    internal override void LogDiff(IVssRequestContext requestContext)
    {
      requestContext.Trace(203011, TraceLevel.Info, "Licensing", nameof (MsdnEntitlementsDiffLogger), "Comparing entitlements...");
      if (!this.m_diffs.IsNullOrEmpty<Tuple<MsdnEntitlement, MsdnEntitlement, IList<string>>>())
      {
        requestContext.Trace(203013, TraceLevel.Warning, "Licensing", nameof (MsdnEntitlementsDiffLogger), "Found mismatches in entitlements");
        EntityDiffLogger.Log<MsdnEntitlement>(requestContext, (IEnumerable<Tuple<MsdnEntitlement, MsdnEntitlement, IList<string>>>) this.m_diffs, "Licensing", nameof (MsdnEntitlementsDiffLogger));
      }
      else
        requestContext.Trace(203014, TraceLevel.Info, "Licensing", nameof (MsdnEntitlementsDiffLogger), "Entitlements look good");
    }

    private Tuple<bool, IList<string>> IsMatch(MsdnEntitlement source, MsdnEntitlement target)
    {
      string identifier1 = this.GetIdentifier(source);
      string identifier2 = this.GetIdentifier(target);
      IList<string> enumerable = (IList<string>) new List<string>();
      string str = identifier2;
      if (identifier1 != str)
        enumerable.Add("Mismatch in identifier");
      if (!MsdnEntitlementsDiffLogger.StringEquals(source.EntitlementName, target.EntitlementName))
        enumerable.Add("Mismatch in EntitlementName");
      if (!MsdnEntitlementsDiffLogger.StringEquals(source.EntitlementType, target.EntitlementType))
        enumerable.Add("Mismatch in EntitlementType");
      if (!MsdnEntitlementsDiffLogger.StringEquals(source.SubscriptionChannel, target.SubscriptionChannel))
        enumerable.Add("Mismatch in SubscriptionChannel");
      if (!MsdnEntitlementsDiffLogger.StringEquals(source.SubscriptionLevelCode, target.SubscriptionLevelCode))
        enumerable.Add("Mismatch in SubscriptionLevelCode");
      if (!MsdnEntitlementsDiffLogger.StringEquals(source.SubscriptionLevelName, target.SubscriptionLevelName))
        enumerable.Add("Mismatch in SubscriptionLevelName");
      if (!MsdnEntitlementsDiffLogger.StringEquals(source.SubscriptionStatus, target.SubscriptionStatus))
        enumerable.Add("Mismatch in SubscriptionStatus");
      bool? isActivated1 = source.IsActivated;
      bool? isActivated2 = target.IsActivated;
      if (!(isActivated1.GetValueOrDefault() == isActivated2.GetValueOrDefault() & isActivated1.HasValue == isActivated2.HasValue))
        enumerable.Add("Mismatch in IsActivated");
      if (source.IsEntitlementAvailable != target.IsEntitlementAvailable)
        enumerable.Add("Mismatch in IsEntitlementAvailable");
      if (TimeSpan.FromTicks(Math.Abs(source.SubscriptionExpirationDate.UtcTicks - target.SubscriptionExpirationDate.UtcTicks)) > MsdnEntitlementsDiffLogger.s_tenMinutes)
        enumerable.Add("Mismatch in SubscriptionExpirationDate");
      return new Tuple<bool, IList<string>>(enumerable.IsNullOrEmpty<string>(), enumerable);
    }

    private string GetIdentifier(MsdnEntitlement source) => (source.EntitlementCode + "-" + source.SubscriptionId).ToLowerInvariant();

    private static bool StringEquals(string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
  }
}
