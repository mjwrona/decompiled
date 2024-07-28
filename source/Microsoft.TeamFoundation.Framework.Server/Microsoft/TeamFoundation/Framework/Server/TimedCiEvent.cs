// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TimedCiEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TimedCiEvent : IDisposable
  {
    private string m_eventArea;
    private string m_eventFeature;
    private IVssRequestContext m_requestContext;
    private Dictionary<string, object> m_properties;
    private Stopwatch m_timer;
    public const string s_timePropertyName = "Time";
    private const string s_area = "TimedCIEventDisposable";
    private const string s_layer = "BusinessLogic";

    public TimedCiEvent(
      IVssRequestContext requestContext,
      string area,
      [CallerMemberName] string feature = "feature",
      Dictionary<string, object> properties = null)
    {
      this.m_requestContext = requestContext;
      this.m_eventArea = area;
      this.m_eventFeature = feature;
      this.m_properties = properties != null ? properties : new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      this.m_timer = new Stopwatch();
      this.m_timer.Start();
    }

    public void Dispose()
    {
      try
      {
        this.m_properties["Time"] = (object) this.m_timer.ElapsedMilliseconds;
        CustomerIntelligenceData properties = new CustomerIntelligenceData((IDictionary<string, object>) this.m_properties);
        this.m_requestContext.GetService<CustomerIntelligenceService>().Publish(this.m_requestContext, this.m_eventArea, this.m_eventFeature, properties);
      }
      catch (Exception ex)
      {
        this.m_requestContext.TraceException(93210, "TimedCIEventDisposable", "BusinessLogic", ex);
      }
    }

    public Dictionary<string, object> Properties => this.m_properties;

    public object this[string key]
    {
      get => this.m_properties[key];
      set => this.m_properties[key] = value;
    }
  }
}
