// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Artifact.WebApi.PipelineArtifact
// Assembly: Microsoft.VisualStudio.Services.Artifact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D39C0B4C-25E7-402A-9BC9-E3DFE7654881
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Artifact.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Artifact.WebApi
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  public class PipelineArtifact
  {
    [JsonProperty(PropertyName = "projectId", Required = Required.Always)]
    public Guid ProjectId { get; set; }

    [JsonProperty(PropertyName = "pipelineId", Required = Required.Always)]
    public int PipelineId { get; set; }

    [JsonProperty(PropertyName = "artifactName", Required = Required.Always)]
    public string ArtifactName { get; set; }

    [JsonProperty(PropertyName = "rootId", Required = Required.Always)]
    public DedupIdentifier RootId { get; set; }

    [JsonProperty(PropertyName = "manifestId", Required = Required.Always)]
    public DedupIdentifier ManifestId { get; set; }

    [JsonProperty(PropertyName = "proofNodes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<string> ProofNodes { get; set; }

    [JsonProperty(PropertyName = "creationDate", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? CreationDate { get; set; }

    [JsonProperty(PropertyName = "status", Required = Required.Always, NullValueHandling = NullValueHandling.Ignore)]
    public PipelineArtifactStatus? Status { get; set; }

    [JsonIgnore]
    public string ETag { get; set; }
  }
}
