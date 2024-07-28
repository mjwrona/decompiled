// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildServer
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class BuildServer
  {
    public BuildServer() => this.Status = ServiceHostStatus.Offline;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsVirtual { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MessageQueueUrl { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool RequireClientCertificates { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ServiceHostStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<ShallowReference> Agents { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Controller { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StatusChangedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Version { get; set; }
  }
}
