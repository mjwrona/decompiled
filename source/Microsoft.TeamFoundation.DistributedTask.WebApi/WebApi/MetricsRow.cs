// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.MetricsRow
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class MetricsRow
  {
    [DataMember(Name = "Dimensions")]
    private IList<string> m_dimensions;
    [DataMember(Name = "Metrics")]
    private IList<string> m_metrics;

    public IList<string> Dimensions
    {
      get
      {
        if (this.m_dimensions == null)
          this.m_dimensions = (IList<string>) new List<string>();
        return this.m_dimensions;
      }
      internal set => this.m_dimensions = value;
    }

    public IList<string> Metrics
    {
      get
      {
        if (this.m_metrics == null)
          this.m_metrics = (IList<string>) new List<string>();
        return this.m_metrics;
      }
      internal set => this.m_metrics = value;
    }
  }
}
