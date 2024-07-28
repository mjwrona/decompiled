// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.BuildProvenance
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public class BuildProvenance
  {
    [DataMember(EmitDefaultValue = false)]
    public string Id;
    [DataMember(EmitDefaultValue = false)]
    public string ProjectId;
    [DataMember(EmitDefaultValue = false)]
    public IList<Command> Commands;
    [DataMember(EmitDefaultValue = false)]
    public IList<Artifact> BuiltArtifacts;
    [DataMember(EmitDefaultValue = false)]
    public DateTime CreateTime;
    [DataMember(EmitDefaultValue = false)]
    public DateTime StartTime;
    [DataMember(EmitDefaultValue = false)]
    public DateTime EndTime;
    [DataMember(EmitDefaultValue = false)]
    public string Creator;
    [DataMember(EmitDefaultValue = false)]
    public string LogsUri;
    [DataMember(EmitDefaultValue = false)]
    public Source SourceProvenance;
    [DataMember(EmitDefaultValue = false)]
    public string TriggerId;
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> BuildOptions;
    [DataMember(EmitDefaultValue = false)]
    public string BuilderVersion;

    [DataMember]
    public List<BuildArtifact> BuildArtifacts { get; set; }
  }
}
