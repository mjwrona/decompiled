// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Fingerprint
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public class Fingerprint
  {
    [DataMember(EmitDefaultValue = false)]
    public string V1Name;
    [DataMember(EmitDefaultValue = false)]
    public List<string> V2Blob;
    [DataMember(EmitDefaultValue = false)]
    public string V2Name;
  }
}
