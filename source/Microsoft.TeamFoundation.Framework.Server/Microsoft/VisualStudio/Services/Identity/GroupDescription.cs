// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupDescription
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class GroupDescription
  {
    public GroupDescription(string name, string description)
    {
      this.Name = name;
      this.Description = description;
      this.SpecialType = SpecialGroupType.Generic;
    }

    public GroupDescription(
      IdentityDescriptor descriptor,
      string name,
      string description,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool hasRestrictedVisibility = false,
      bool scopeLocal = false)
      : this(descriptor, name, description, specialType, hasRestrictedVisibility, scopeLocal, Guid.Empty, Guid.Empty)
    {
    }

    public GroupDescription(
      Guid id,
      IdentityDescriptor descriptor,
      string name,
      string description,
      SpecialGroupType specialType = SpecialGroupType.Generic,
      bool hasRestrictedVisibility = false,
      bool scopeLocal = false)
      : this(descriptor, name, description, specialType, hasRestrictedVisibility, scopeLocal, Guid.Empty, id)
    {
    }

    internal GroupDescription(
      IdentityDescriptor descriptor,
      string name,
      string description,
      SpecialGroupType specialType,
      bool hasRestrictedVisibility,
      bool scopeLocal,
      Guid scopeId,
      Guid id,
      bool active = true,
      string virtualPlugin = null)
    {
      this.Descriptor = descriptor;
      this.Name = name;
      this.Description = description;
      this.VirtualPlugin = virtualPlugin;
      this.SpecialType = specialType;
      this.HasRestrictedVisibility = hasRestrictedVisibility;
      this.ScopeLocal = scopeLocal;
      this.ScopeId = scopeId;
      this.Id = id;
      this.Active = active;
    }

    public string Name { get; set; }

    public string Description { get; set; }

    public string VirtualPlugin { get; set; }

    public IdentityDescriptor Descriptor { get; internal set; }

    public Guid Id { get; internal set; }

    public SpecialGroupType SpecialType { get; internal set; }

    public bool HasRestrictedVisibility { get; internal set; }

    public Guid ScopeId { get; internal set; }

    public bool ScopeLocal { get; internal set; }

    public bool Active { get; internal set; }

    internal static GroupDescription Convert(Guid hostDomainId, Microsoft.VisualStudio.Services.Identity.Identity group)
    {
      Guid scopeId = group.GetProperty<Guid>("ScopeId", Guid.Empty);
      if (scopeId == Guid.Empty)
      {
        string property = group.GetProperty<string>("Domain", (string) null);
        scopeId = string.IsNullOrEmpty(property) || !LinkingUtilities.IsUriWellFormed(property) ? hostDomainId : new Guid(LinkingUtilities.DecodeUri(property).ToolSpecificId);
      }
      string property1 = group.GetProperty<string>("Account", string.Empty);
      string property2 = group.GetProperty<string>("Description", string.Empty);
      string property3 = group.GetProperty<string>("VirtualPlugin", string.Empty);
      bool hasRestrictedVisibility = group.GetProperty<string>("RestrictedVisible", (string) null) != null;
      bool scopeLocal = group.GetProperty<string>("CrossProject", (string) null) == null;
      SpecialGroupType specialGroupType = IdentityHelper.GetSpecialGroupType((IReadOnlyVssIdentity) group);
      return new GroupDescription(group.Descriptor, property1, property2, specialGroupType, hasRestrictedVisibility, scopeLocal, scopeId, group.Id, group.IsActive, property3);
    }
  }
}
