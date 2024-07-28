// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphScopeCreationContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphScopeCreationContext
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public GroupScopeType ScopeType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid StorageKey { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid CreatorId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AdminGroupName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AdminGroupDescription { get; set; }
  }
}
