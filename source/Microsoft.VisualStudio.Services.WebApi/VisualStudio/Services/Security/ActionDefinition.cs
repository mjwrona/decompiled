// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Security.ActionDefinition
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Security
{
  [DataContract]
  public sealed class ActionDefinition
  {
    public ActionDefinition()
    {
    }

    public ActionDefinition(Guid namespaceId, int bit, string name, string displayName)
    {
      this.Bit = bit;
      this.Name = name;
      this.DisplayName = displayName;
      this.NamespaceId = namespaceId;
    }

    [DataMember]
    public int Bit { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public Guid NamespaceId { get; set; }
  }
}
