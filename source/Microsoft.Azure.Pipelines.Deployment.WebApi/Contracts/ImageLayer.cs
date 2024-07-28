// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.ImageLayer
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts
{
  [DataContract]
  public class ImageLayer
  {
    [DataMember(EmitDefaultValue = false)]
    public string Directive { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Arguments { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Size { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }
  }
}
