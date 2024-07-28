// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.Client.PermissionLevelDefinition
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PermissionLevel.Client
{
  [DataContract]
  public sealed class PermissionLevelDefinition
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public PermissionLevelDefinitionType Type { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public PermissionLevelDefinitionScope Scope { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastUpdated { get; set; }

    private bool Equals(PermissionLevelDefinition obj)
    {
      if (this == obj)
        return true;
      return object.Equals((object) this.Id, (object) obj.Id) && object.Equals((object) this.Name, (object) obj.Name) && object.Equals((object) this.Description, (object) obj.Description) && object.Equals((object) this.Type, (object) obj.Type) && object.Equals((object) this.Scope, (object) obj.Scope);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((PermissionLevelDefinition) obj);
    }

    public override int GetHashCode() => ((((1231 * 3037 + this.Id.GetHashCode()) * 3037 + this.Name.GetHashCode()) * 3037 + this.Description.GetHashCode()) * 3037 + this.Type.GetHashCode()) * 3037 + this.Scope.GetHashCode();
  }
}
