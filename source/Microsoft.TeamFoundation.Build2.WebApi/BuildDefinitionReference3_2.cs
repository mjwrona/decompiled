// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionReference3_2
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildDefinitionReference3_2 : DefinitionReference
  {
    [DataMember(Name = "Metrics", EmitDefaultValue = false)]
    private List<BuildMetric> m_serializedMetrics;
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;
    private List<BuildMetric> m_metrics;
    private List<DefinitionReference> m_drafts;

    public BuildDefinitionReference3_2()
    {
      this.Type = DefinitionType.Build;
      this.QueueStatus = DefinitionQueueStatus.Enabled;
    }

    [DataMember(EmitDefaultValue = false, Name = "Quality")]
    public Microsoft.TeamFoundation.Build.WebApi.DefinitionQuality? DefinitionQuality { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef AuthoredBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(EmitDefaultValue = false, Name = "draftOf")]
    public DefinitionReference ParentDefinition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<DefinitionReference> Drafts
    {
      get => this.m_drafts ?? (this.m_drafts = new List<DefinitionReference>());
      internal set => this.m_drafts = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public AgentPoolQueue Queue { get; set; }

    public List<BuildMetric> Metrics
    {
      get => this.m_metrics ?? (this.m_metrics = new List<BuildMetric>());
      internal set => this.m_metrics = value;
    }

    public ReferenceLinks Links => this.m_links ?? (this.m_links = new ReferenceLinks());

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<BuildMetric>(ref this.m_serializedMetrics, ref this.m_metrics, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<BuildMetric>(ref this.m_metrics, ref this.m_serializedMetrics);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedMetrics = (List<BuildMetric>) null;
  }
}
