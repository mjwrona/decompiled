// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ArtifactContributionDefinition
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ArtifactContributionDefinition
  {
    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UniqueSourceIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string EndpointTypeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DownloadTaskId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ArtifactTriggerConfiguration ArtifactTriggerConfiguration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<InputDescriptor> InputDescriptors { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<DataSourceBinding> DataSourceBindings { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsCommitsTraceabilitySupported { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsWorkitemsTraceabilitySupported { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need it for deserialization from Json")]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> BrowsableArtifactTypeMapping { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need it for deserialization from Json")]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> TaskInputMapping { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need it for deserialization from Json")]
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> ArtifactTypeStreamMapping { get; set; }
  }
}
