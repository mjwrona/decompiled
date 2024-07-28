// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySelf
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DebuggerDisplay("{DisplayName}")]
  [DataContract]
  public class IdentitySelf
  {
    [DataMember]
    public Guid Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember]
    public string DisplayName { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember]
    public string AccountName { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember]
    public string Origin { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember]
    public string OriginId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember]
    public string Domain { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IEnumerable<TenantInfo> Tenants { get; set; }
  }
}
