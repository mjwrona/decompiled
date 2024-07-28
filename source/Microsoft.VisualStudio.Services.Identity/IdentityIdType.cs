// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdType
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public class IdentityIdType
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid ToIdentity { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid TargetMasterId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid SourceMasterId { get; set; }
  }
}
