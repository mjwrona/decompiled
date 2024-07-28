// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseDefinition : ReleaseDefinitionShallowReference
  {
    private PropertiesCollection properties;

    [DataMember]
    public ReleaseDefinitionSource Source { get; set; }

    [DataMember]
    public int Revision { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember]
    public DateTime ModifiedOn { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }

    [DataMember]
    public bool IsDisabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReleaseReference LastRelease { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, ConfigurationVariableValue> Variables { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "will set this to null explicitly in list calls")]
    [DataMember]
    public IList<int> VariableGroups { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<ReleaseDefinitionEnvironment> Environments { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> Artifacts { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<ReleaseTriggerBase> Triggers { get; set; }

    [DataMember]
    public string ReleaseNameFormat { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [Obsolete("Retention policy at Release Definition level is deprecated. Use the Retention Policy at environment and API version 3.0-preview.2 or later", false)]
    public RetentionPolicy RetentionPolicy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public IList<string> Tags { get; set; }

    [ClientInternalUseOnly(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(EmitDefaultValue = false)]
    internal PipelineProcess PipelineProcess { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PropertiesCollection Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = new PropertiesCollection();
        return this.properties;
      }
      internal set => this.properties = value;
    }

    public ReleaseDefinition()
    {
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Artifacts = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>();
      this.Triggers = (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
      this.VariableGroups = (IList<int>) new List<int>();
      this.Tags = (IList<string>) new List<string>();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.LastRelease?.SetSecuredObject(token, requiredPermissions);
      IDictionary<string, ConfigurationVariableValue> variables = this.Variables;
      if (variables != null)
        variables.ForEach<KeyValuePair<string, ConfigurationVariableValue>>((Action<KeyValuePair<string, ConfigurationVariableValue>>) (i => i.Value.SetSecuredObject(token, requiredPermissions)));
      IList<ReleaseDefinitionEnvironment> environments = this.Environments;
      if (environments != null)
        environments.ForEach<ReleaseDefinitionEnvironment>((Action<ReleaseDefinitionEnvironment>) (i => i.SetSecuredObject(token, requiredPermissions)));
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts = this.Artifacts;
      if (artifacts != null)
        artifacts.ForEach<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Action<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) (i => i.SetSecuredObject(token, requiredPermissions)));
      IList<ReleaseTriggerBase> triggers = this.Triggers;
      if (triggers != null)
        triggers.ForEach<ReleaseTriggerBase>((Action<ReleaseTriggerBase>) (i => i.SetSecuredObject(token, requiredPermissions)));
      this.PipelineProcess?.SetSecuredObject(token, requiredPermissions);
      this.RetentionPolicy?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
