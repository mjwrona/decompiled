// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageDetails
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts
{
  [DataContract]
  public class ImageDetails
  {
    [DataMember]
    public string ImageName { get; set; }

    [DataMember]
    public string ImageUri { get; set; }

    [DataMember]
    public byte[] Hash { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string BaseImageName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string BaseImageUri { get; set; }

    [DataMember]
    public int? Distance { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ImageType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string MediaType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<string> Tags { get; set; }

    [DataMember]
    public List<ImageLayer> LayerInfo { get; set; }

    [DataMember]
    public int RunId { get; set; }

    [DataMember]
    public string PipelineVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PipelineName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PipelineId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ImageSize { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string JobName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreateTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Creator { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string LogsUri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactStorageSourceUri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ContextUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RevisionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> BuildOptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RepositoryTypeName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RepositoryId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RepositoryName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Branch { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ProjectId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ImageFingerprint ImageFingerprint { get; set; }
  }
}
