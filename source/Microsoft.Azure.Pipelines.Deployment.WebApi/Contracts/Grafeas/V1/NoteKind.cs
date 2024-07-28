// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.NoteKind
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public enum NoteKind
  {
    [EnumMember(Value = "NOTE_KIND_UNSPECIFIED")] Unspecified = 0,
    [EnumMember(Value = "VULNERABILITY")] Vulnerability = 1,
    [EnumMember(Value = "BUILD")] Build = 2,
    [EnumMember(Value = "IMAGE")] Image = 3,
    [EnumMember(Value = "DEPLOYMENT")] Deployment = 5,
    [EnumMember(Value = "ATTESTATION")] Attestation = 7,
  }
}
