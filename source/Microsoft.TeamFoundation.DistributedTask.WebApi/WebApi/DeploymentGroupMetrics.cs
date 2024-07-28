// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentGroupMetrics
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class DeploymentGroupMetrics
  {
    [DataMember(Name = "Rows")]
    private IList<MetricsRow> m_rows;

    [DataMember]
    public DeploymentGroupReference DeploymentGroup { get; internal set; }

    [DataMember]
    public MetricsColumnsHeader ColumnsHeader { get; internal set; }

    public IList<MetricsRow> Rows
    {
      get
      {
        if (this.m_rows == null)
          this.m_rows = (IList<MetricsRow>) new List<MetricsRow>();
        return this.m_rows;
      }
      internal set => this.m_rows = value;
    }
  }
}
