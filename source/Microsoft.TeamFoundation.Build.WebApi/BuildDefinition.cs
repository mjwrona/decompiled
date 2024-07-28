// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildDefinition
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildDefinition : DefinitionReference
  {
    public BuildDefinition()
    {
      this.BatchSize = 1;
      this.TriggerType = DefinitionTriggerType.None;
      this.DefinitionType = DefinitionType.Xaml;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BatchSize { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public QueueReference Queue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DefinitionTriggerType TriggerType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ContinuousIntegrationQuietPeriod { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DefaultDropLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildArgs { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime DateCreated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReason SupportedReasons { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference LastBuild { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference BuildController { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference LastGoodBuild { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference ProcessTemplate { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ProcessParameters { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<RetentionPolicy> RetentionPolicies { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Schedule> Schedules { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<BuildDefinitionSourceProvider> SourceProviders { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<PropertyValue> Properties { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IEnumerable<PropertyValue> PropertyCollection { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public WorkspaceTemplate WorkspaceTemplate { get; set; }
  }
}
