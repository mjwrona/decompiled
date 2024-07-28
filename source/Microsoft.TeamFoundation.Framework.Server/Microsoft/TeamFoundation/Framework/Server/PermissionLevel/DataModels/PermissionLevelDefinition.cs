// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels.PermissionLevelDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.PermissionLevel;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels
{
  public sealed class PermissionLevelDefinition : ICloneable
  {
    public PermissionLevelDefinition(Guid id) => this.Id = id;

    public Guid Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public PermissionLevelDefinitionType Type { get; set; }

    public PermissionLevelDefinitionScope Scope { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastUpdated { get; set; }

    public PermissionLevelDefinition Clone() => new PermissionLevelDefinition(this.Id)
    {
      Name = this.Name,
      Description = this.Description,
      Type = this.Type,
      Scope = this.Scope,
      IsActive = this.IsActive
    };

    object ICloneable.Clone() => (object) this.Clone();

    private bool Equals(PermissionLevelDefinition obj)
    {
      if (this == obj)
        return true;
      return object.Equals((object) this.Id, (object) obj.Id) && object.Equals((object) this.Name, (object) obj.Name) && object.Equals((object) this.Description, (object) obj.Description) && object.Equals((object) this.Type, (object) obj.Type) && object.Equals((object) this.Scope, (object) obj.Scope) && object.Equals((object) this.IsActive, (object) obj.IsActive);
    }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj != null && this.GetType() == obj.GetType() && this.Equals((PermissionLevelDefinition) obj);
    }

    public override int GetHashCode() => (((((1231 * 3037 + this.Id.GetHashCode()) * 3037 + this.Name.GetHashCode()) * 3037 + this.Description.GetHashCode()) * 3037 + this.Type.GetHashCode()) * 3037 + this.Scope.GetHashCode()) * 3037 + this.IsActive.GetHashCode();
  }
}
