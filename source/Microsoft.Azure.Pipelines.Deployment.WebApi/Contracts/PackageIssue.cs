// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.PackageIssue
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts
{
  [DataContract]
  public class PackageIssue
  {
    [DataMember(EmitDefaultValue = false)]
    public string AffectedCpeUri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string AffectedPackage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PackageVersion AffectedVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FixedCpeUri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FixedPackage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PackageVersion FixedVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool FixAvailable { get; set; }
  }
}
