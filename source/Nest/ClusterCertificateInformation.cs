// Decompiled with JetBrains decompiler
// Type: Nest.ClusterCertificateInformation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ClusterCertificateInformation
  {
    [DataMember(Name = "path")]
    public string Path { get; internal set; }

    [DataMember(Name = "alias")]
    public string Alias { get; internal set; }

    [DataMember(Name = "format")]
    public string Format { get; internal set; }

    [DataMember(Name = "subject_dn")]
    public string SubjectDomainName { get; internal set; }

    [DataMember(Name = "serial_number")]
    public string SerialNumber { get; internal set; }

    [DataMember(Name = "has_private_key")]
    public bool HasPrivateKey { get; internal set; }

    [DataMember(Name = "expiry")]
    public DateTimeOffset Expiry { get; internal set; }
  }
}
