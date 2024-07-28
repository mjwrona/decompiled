// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.TraceFilter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class TraceFilter : IEquatable<TraceFilter>
  {
    [DataMember]
    public Guid TraceId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Tracepoint { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ProcessName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UserLogin { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Service { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Method { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TraceLevel Level { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Area { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Layer { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string UserAgent { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ServiceHost { get; set; }

    public string[] Tags { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ExceptionType { get; set; }

    [DataMember]
    public DateTime TimeCreated { get; set; }

    [DataMember]
    public bool IsEnabled { get; set; }

    [DataMember]
    public Guid OwnerId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Owner { get; set; }

    public bool Equals(TraceFilter other) => other.TraceId == this.TraceId;
  }
}
