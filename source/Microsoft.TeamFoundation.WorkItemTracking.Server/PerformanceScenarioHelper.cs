// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PerformanceScenarioHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class PerformanceScenarioHelper : IDisposable
  {
    private const string Rest = "Rest";
    private const string ElapsedTime = "ElapsedTime";
    private const string PerformanceArea = "Performance";
    private const string PerformanceFeature = "Scenario";
    private const string FeatureArea = "FeatureArea";
    private const string Name = "Name";
    private Stopwatch m_stopwatch = new Stopwatch();
    private IVssRequestContext m_requestContext;
    private string m_areaName;
    private string m_featureName;
    private bool m_published;
    private List<MeasurementBlock> m_measurementBlocks = new List<MeasurementBlock>();
    private IDictionary<string, object> m_properties = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);

    public PerformanceScenarioHelper(
      IVssRequestContext requestContext,
      string areaName,
      string featureName)
    {
      this.m_requestContext = requestContext;
      this.m_areaName = areaName;
      this.m_featureName = featureName;
      this.m_stopwatch.Start();
    }

    public IDisposable Measure(string measurementBlockName) => this.m_measurementBlocks.Any<MeasurementBlock>((Func<MeasurementBlock, bool>) (b => b.Name.Equals(measurementBlockName))) ? (IDisposable) null : (IDisposable) new Timing(this, measurementBlockName);

    public void EndScenario()
    {
      if (this.m_published)
        return;
      this.m_stopwatch.Stop();
      this.Publish(this.m_stopwatch.Elapsed);
      this.m_published = true;
    }

    public void Dispose() => this.EndScenario();

    public void Add(string name, TimeSpan duration) => this.m_measurementBlocks.Add(new MeasurementBlock(name, duration));

    public void Add(string name, object value) => this.m_properties[name] = value;

    public void Add(IDictionary<string, object> data)
    {
      if (data == null)
        return;
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) data)
        this.Add(keyValuePair.Key, keyValuePair.Value);
    }

    private void Publish(TimeSpan duration)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      foreach (string key in (IEnumerable<string>) this.m_properties.Keys)
        properties.Add(key, this.m_properties[key]);
      foreach (MeasurementBlock measurementBlock in this.m_measurementBlocks)
        properties.Add(measurementBlock.Name, measurementBlock.Duration.TotalMilliseconds);
      double num = this.m_measurementBlocks.Sum<MeasurementBlock>((Func<MeasurementBlock, double>) (b => b.Duration.TotalMilliseconds));
      properties.Add("Rest", duration.TotalMilliseconds - num);
      properties.Add("ElapsedTime", duration.TotalMilliseconds);
      properties.Add("FeatureArea", this.m_areaName);
      properties.Add("Name", this.m_featureName);
      this.m_requestContext.GetService<CustomerIntelligenceService>().Publish(this.m_requestContext, "Performance", "Scenario", properties);
    }
  }
}
