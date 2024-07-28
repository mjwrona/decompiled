// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityPropertyKpis
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityPropertyKpis
  {
    private static ConcurrentDictionary<string, IdentityPropertyKpis.IdentityProperty> m_propertyCounts = new ConcurrentDictionary<string, IdentityPropertyKpis.IdentityProperty>();
    private const string c_DisableIdentityPropertyKpiPublishFeatureFlag = "Microsoft.VisualStudio.Services.Identity.Telemetry.DisablePublishPropertyKpis";

    internal static void IncrementReadsForProperty(string propertyName) => IdentityPropertyKpis.m_propertyCounts.GetOrAdd(propertyName, new IdentityPropertyKpis.IdentityProperty()).IncrementReads();

    internal static void IncrementWritesForProperty(string propertyName) => IdentityPropertyKpis.m_propertyCounts.GetOrAdd(propertyName, new IdentityPropertyKpis.IdentityProperty()).IncrementWrites();

    internal static void PublishKpis(IVssRequestContext requestContext, object taskArgs)
    {
      ConcurrentDictionary<string, IdentityPropertyKpis.IdentityProperty> concurrentDictionary1 = new ConcurrentDictionary<string, IdentityPropertyKpis.IdentityProperty>();
      ConcurrentDictionary<string, IdentityPropertyKpis.IdentityProperty> concurrentDictionary2 = Interlocked.Exchange<ConcurrentDictionary<string, IdentityPropertyKpis.IdentityProperty>>(ref IdentityPropertyKpis.m_propertyCounts, concurrentDictionary1);
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.Telemetry.DisablePublishPropertyKpis"))
        return;
      KpiService service = requestContext.GetService<KpiService>();
      foreach (KeyValuePair<string, IdentityPropertyKpis.IdentityProperty> keyValuePair in concurrentDictionary2)
      {
        List<KpiMetric> metrics = new List<KpiMetric>()
        {
          new KpiMetric()
          {
            Name = "PropertyReads",
            Value = (double) keyValuePair.Value.Reads
          },
          new KpiMetric()
          {
            Name = "PropertyWrites",
            Value = (double) keyValuePair.Value.Writes
          }
        };
        service.Publish(requestContext, "Microsoft.VisualStudio.IdentityMetrics", keyValuePair.Key, metrics);
      }
    }

    internal class IdentityProperty
    {
      private int m_reads;
      private int m_writes;

      internal IdentityProperty()
      {
        this.m_reads = 0;
        this.m_writes = 0;
      }

      public int Reads => this.m_reads;

      public int Writes => this.m_writes;

      internal void IncrementReads() => Interlocked.Increment(ref this.m_reads);

      internal void IncrementWrites() => Interlocked.Increment(ref this.m_writes);
    }
  }
}
