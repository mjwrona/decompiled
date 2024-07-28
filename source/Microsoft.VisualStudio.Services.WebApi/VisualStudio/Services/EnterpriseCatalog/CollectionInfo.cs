// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EnterpriseCatalog.CollectionInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.EnterpriseCatalog
{
  [DataContract]
  [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class CollectionInfo
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string OwnerSignInAddress { get; set; }

    private string DebuggerDisplay => "Name: " + this.Name;
  }
}
