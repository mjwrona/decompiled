// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class Release : ReleaseManagementSecuredObject
  {
    [DataMember(EmitDefaultValue = false, Name = "Properties", Order = 1)]
    private PropertiesCollection m_properties;

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public ReleaseStatus Status { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember]
    public DateTime ModifiedOn { get; set; }

    [DataMember]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedFor { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [XmlIgnore]
    public IList<ReleaseEnvironment> Environments { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [XmlIgnore]
    public IDictionary<string, ConfigurationVariableValue> Variables { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    [XmlIgnore]
    public IList<VariableGroup> VariableGroups { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    [XmlIgnore]
    public IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> Artifacts { get; set; }

    [Obsolete("Use ReleaseDefinitionReference instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference ReleaseDefinition
    {
      get => (ShallowReference) this.ReleaseDefinitionReference;
      set => this.ReleaseDefinitionReference = value.ToReleaseDefinitionShallowReference();
    }

    [DataMember(Name = "ReleaseDefinition", EmitDefaultValue = false)]
    public ReleaseDefinitionShallowReference ReleaseDefinitionReference { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public int ReleaseDefinitionRevision { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PoolName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember]
    public ReleaseReason Reason { get; set; }

    [DataMember]
    public string ReleaseNameFormat { get; set; }

    [DataMember]
    public bool KeepForever { get; set; }

    [DataMember]
    public int DefinitionSnapshotRevision { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The name Url is appropariate name for this property and it signifies REST URL of resource")]
    [DataMember]
    public string LogsContainerUrl { get; set; }

    [Obsolete("Use Links instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<string> Tags { get; set; }

    [DataMember]
    public string TriggeringArtifactAlias { get; set; }

    [DataMember]
    public ProjectReference ProjectReference { get; set; }

    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      internal set => this.m_properties = value;
    }

    public Release()
    {
      this.Environments = (IList<ReleaseEnvironment>) new List<ReleaseEnvironment>();
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Artifacts = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>();
      this.Links = new ReferenceLinks();
      this.VariableGroups = (IList<VariableGroup>) new List<VariableGroup>();
      this.Tags = (IList<string>) new List<string>();
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<ReleaseEnvironment> environments = this.Environments;
      if (environments != null)
        environments.ForEach<ReleaseEnvironment>((Action<ReleaseEnvironment>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      IDictionary<string, ConfigurationVariableValue> variables = this.Variables;
      if (variables != null)
        variables.ForEach<KeyValuePair<string, ConfigurationVariableValue>>((Action<KeyValuePair<string, ConfigurationVariableValue>>) (i => i.Value?.SetSecuredObject(token, requiredPermissions)));
      IList<VariableGroup> variableGroups = this.VariableGroups;
      if (variableGroups != null)
        variableGroups.ForEach<VariableGroup>((Action<VariableGroup>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifacts = this.Artifacts;
      if (artifacts != null)
        artifacts.ForEach<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Action<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.ReleaseDefinitionReference?.SetSecuredObject(token, requiredPermissions);
      this.ProjectReference?.SetSecuredObject(token, requiredPermissions);
      ReferenceLinks links = this.Links;
      this.Links = links != null ? links.GetSecuredReferenceLinks(token, requiredPermissions) : (ReferenceLinks) null;
    }
  }
}
