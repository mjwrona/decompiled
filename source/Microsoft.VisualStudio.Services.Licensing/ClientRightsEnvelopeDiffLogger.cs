// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ClientRightsEnvelopeDiffLogger
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
  internal class ClientRightsEnvelopeDiffLogger : EntityDiffLogger
  {
    private readonly IList<Tuple<IClientRight, IClientRight, IList<string>>> m_diffs;
    private const string c_area = "Licensing";
    private const string c_layer = "ClientRightsEnvelopeDiffLogger";
    private static readonly TimeSpan s_tenMinutes = TimeSpan.FromMinutes(10.0);

    public ClientRightsEnvelopeDiffLogger(
      IClientRightsEnvelope source,
      IClientRightsEnvelope target)
    {
      this.m_diffs = (IList<Tuple<IClientRight, IClientRight, IList<string>>>) EntityDiffLogger.CompareCollections<IClientRight, string>((IEnumerable<IClientRight>) source.Rights, (IEnumerable<IClientRight>) target.Rights, (Func<IClientRight, string>) (right => this.GetIdentifier(right)), new Func<IClientRight, IClientRight, Tuple<bool, IList<string>>>(this.IsMatch)).ToList<Tuple<IClientRight, IClientRight, IList<string>>>();
    }

    internal override void LogDiff(IVssRequestContext requestContext)
    {
      requestContext.Trace(202011, TraceLevel.Info, "Licensing", nameof (ClientRightsEnvelopeDiffLogger), "Comparing client rights...");
      if (!this.m_diffs.IsNullOrEmpty<Tuple<IClientRight, IClientRight, IList<string>>>())
      {
        requestContext.Trace(202013, TraceLevel.Warning, "Licensing", nameof (ClientRightsEnvelopeDiffLogger), "Found mismatches in client rights");
        EntityDiffLogger.Log<IClientRight>(requestContext, (IEnumerable<Tuple<IClientRight, IClientRight, IList<string>>>) this.m_diffs, "Licensing", nameof (ClientRightsEnvelopeDiffLogger));
      }
      else
        requestContext.Trace(202014, TraceLevel.Info, "Licensing", nameof (ClientRightsEnvelopeDiffLogger), "Client rights look good");
    }

    public bool HasDiff() => !this.m_diffs.IsNullOrEmpty<Tuple<IClientRight, IClientRight, IList<string>>>();

    private Tuple<bool, IList<string>> IsMatch(IClientRight source, IClientRight target)
    {
      if (source == null || target == null)
        return new Tuple<bool, IList<string>>(false, (IList<string>) new string[1]
        {
          "The source or target was null"
        });
      string identifier1 = this.GetIdentifier(source);
      string identifier2 = this.GetIdentifier(target);
      IList<string> enumerable = (IList<string>) new List<string>();
      string str = identifier2;
      if (identifier1 != str)
        enumerable.Add("Mismatch in identifier");
      if (!ClientRightsEnvelopeDiffLogger.StringEquals(source.AuthorizedVSEdition, target.AuthorizedVSEdition))
        enumerable.Add("Mismatch in AuthorizedVSEdition");
      if (!ClientRightsEnvelopeDiffLogger.StringEquals(source.LicenseDescriptionId, target.LicenseDescriptionId))
        enumerable.Add("Mismatch in LicenseDescriptionId");
      if (!ClientRightsEnvelopeDiffLogger.StringEquals(source.LicenseFallbackDescription, target.LicenseFallbackDescription))
        enumerable.Add("Mismatch in LicenseFallbackDescription");
      if (!ClientRightsEnvelopeDiffLogger.StringEquals(source.LicenseSourceName, target.LicenseSourceName))
        enumerable.Add("Mismatch in LicenseSourceName");
      if (!ClientRightsEnvelopeDiffLogger.StringEquals(source.LicenseUrl, target.LicenseUrl))
        enumerable.Add("Mismatch in LicenseUrl");
      if (source.Version != target.Version)
        enumerable.Add("Mismatch in Version");
      if (source.ClientVersion != target.ClientVersion)
        enumerable.Add("Mismatch in ClientVersion");
      DateTimeOffset expirationDate = source.ExpirationDate;
      long utcTicks1 = expirationDate.UtcTicks;
      expirationDate = target.ExpirationDate;
      long utcTicks2 = expirationDate.UtcTicks;
      if (TimeSpan.FromTicks(Math.Abs(utcTicks1 - utcTicks2)) > ClientRightsEnvelopeDiffLogger.s_tenMinutes)
        enumerable.Add("Mismatch in ExpirationDate");
      foreach (string key in new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        "IsTrialLicense",
        "SubscriptionChannel",
        "SubscriptionLevel"
      })
      {
        object obj1 = (object) null;
        object obj2 = (object) null;
        source.Attributes?.TryGetValue(key, out obj1);
        target.Attributes?.TryGetValue(key, out obj2);
        if (obj1 != null || obj2 != null)
        {
          if (obj1 == null || obj2 == null)
            enumerable.Add("Mismatch in attribute " + key);
          else if (!ClientRightsEnvelopeDiffLogger.StringEquals(obj1.ToString(), obj2.ToString()))
            enumerable.Add("Mismatch in attribute " + key);
        }
      }
      return new Tuple<bool, IList<string>>(enumerable.IsNullOrEmpty<string>(), enumerable);
    }

    private string GetIdentifier(IClientRight right) => right.Name.ToLowerInvariant();

    private static bool StringEquals(string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
  }
}
