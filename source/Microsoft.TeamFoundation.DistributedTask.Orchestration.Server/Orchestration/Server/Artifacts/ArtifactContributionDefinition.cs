// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactContributionDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class ArtifactContributionDefinition
  {
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string UniqueSourceIdentifier { get; set; }

    public string EndpointTypeId { get; set; }

    public string DownloadTaskId { get; set; }

    public IDictionary<string, Guid> DownloadTaskIds { get; set; }

    public string ArtifactType { get; set; }

    public ArtifactTriggerConfiguration ArtifactTriggerConfiguration { get; set; }

    public IEnumerable<InputDescriptor> InputDescriptors { get; set; }

    public IEnumerable<DataSourceBinding> DataSourceBindings { get; set; }

    public IDictionary<string, string> BrowsableArtifactTypeMapping { get; set; }

    public IDictionary<string, string> TaskInputMapping { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> YamlInputMapping { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<Guid, IDictionary<string, string>> TaskInputMappings { get; set; }

    public IDictionary<string, string> ArtifactTypeStreamMapping { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsCommitsTraceabilitySupported { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsWorkitemsTraceabilitySupported { get; set; }
  }
}
