// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.Occurrence
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  [ClientInternalUseOnly(true)]
  public class Occurrence
  {
    [DataMember(EmitDefaultValue = false)]
    public string Name;
    [DataMember(EmitDefaultValue = false)]
    public string ResourceUri;
    [DataMember(EmitDefaultValue = false)]
    public string NoteName;
    [DataMember(EmitDefaultValue = false)]
    public NoteKind Kind;
    [DataMember(EmitDefaultValue = false)]
    public string Remediation;
    [DataMember(EmitDefaultValue = false)]
    public DateTime CreateTime;
    [DataMember(EmitDefaultValue = false)]
    public DateTime UpdateTime;
    private object details;

    [DataMember(EmitDefaultValue = false)]
    public VulnerabilityOccurrence Vulnerability
    {
      get => this.DetailsCase != Occurrence.DetailsOneofCase.Vulnerability ? (VulnerabilityOccurrence) null : (VulnerabilityOccurrence) this.details;
      set
      {
        this.details = (object) value;
        this.DetailsCase = value == null ? Occurrence.DetailsOneofCase.None : Occurrence.DetailsOneofCase.Vulnerability;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public BuildOccurrence Build
    {
      get => this.DetailsCase != Occurrence.DetailsOneofCase.Build ? (BuildOccurrence) null : (BuildOccurrence) this.details;
      set
      {
        this.details = (object) value;
        this.DetailsCase = value == null ? Occurrence.DetailsOneofCase.None : Occurrence.DetailsOneofCase.Build;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public ImageOccurrence Image
    {
      get => this.DetailsCase != Occurrence.DetailsOneofCase.Image ? (ImageOccurrence) null : (ImageOccurrence) this.details;
      set
      {
        this.details = (object) value;
        this.DetailsCase = value == null ? Occurrence.DetailsOneofCase.None : Occurrence.DetailsOneofCase.Image;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public DeploymentOccurrence Deployment
    {
      get => this.DetailsCase != Occurrence.DetailsOneofCase.Deployment ? (DeploymentOccurrence) null : (DeploymentOccurrence) this.details;
      set
      {
        this.details = (object) value;
        this.DetailsCase = value == null ? Occurrence.DetailsOneofCase.None : Occurrence.DetailsOneofCase.Deployment;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public AttestationOccurrence Attestation
    {
      get => this.DetailsCase != Occurrence.DetailsOneofCase.Attestation ? (AttestationOccurrence) null : (AttestationOccurrence) this.details;
      set
      {
        this.details = (object) value;
        this.DetailsCase = value == null ? Occurrence.DetailsOneofCase.None : Occurrence.DetailsOneofCase.Attestation;
      }
    }

    public Occurrence.DetailsOneofCase DetailsCase { get; private set; }

    [DataContract]
    public enum DetailsOneofCase
    {
      None = 0,
      Vulnerability = 8,
      Build = 9,
      Image = 10, // 0x0000000A
      Deployment = 11, // 0x0000000B
      Attestation = 12, // 0x0000000C
    }
  }
}
