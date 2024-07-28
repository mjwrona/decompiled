// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStartMetadata
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseStartMetadata
  {
    [DataMember(EmitDefaultValue = false, Name = "Properties", Order = 1)]
    private PropertiesCollection m_properties;

    [DataMember]
    public int DefinitionId { get; set; }

    [DataMember]
    public string Description { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public IList<ArtifactMetadata> Artifacts { get; set; }

    [DataMember]
    public bool IsDraft { get; set; }

    [DataMember]
    public ReleaseReason Reason { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public IList<string> ManualEnvironments { get; set; }

    [DataMember]
    public IDictionary<string, ConfigurationVariableValue> Variables { get; set; }

    [DataMember]
    public IList<ReleaseStartEnvironmentMetadata> EnvironmentsMetadata { get; set; }

    [DataMember]
    [ClientInternalUseOnly(true)]
    public Guid? CreatedFor { get; set; }

    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      set => this.m_properties = value;
    }

    public ReleaseStartMetadata()
    {
      this.Artifacts = (IList<ArtifactMetadata>) new List<ArtifactMetadata>();
      this.EnvironmentsMetadata = (IList<ReleaseStartEnvironmentMetadata>) new List<ReleaseStartEnvironmentMetadata>();
      this.Variables = (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
