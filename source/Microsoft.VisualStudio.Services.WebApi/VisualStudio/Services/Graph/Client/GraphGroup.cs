// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphGroup
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphGroup : GraphMember
  {
    public override string SubjectKind => "group";

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; private set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal string SpecialType { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeSpecialType() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal Guid ScopeId { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeScopeId() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal string ScopeType { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeScopeType() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal string ScopeName { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeScopeName() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal Guid LocalScopeId { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeLocalScopeId() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal Guid SecuringHostId { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeSecuringHostId() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal bool IsRestrictedVisible { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeIsRestrictedVisible() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal bool IsCrossProject { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeIsIsCrossProject() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal bool IsGlobalScope { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool ShouldSerializeIsGlobalScope() => this.ShoudSerializeInternals;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClientInternalUseOnly(true)]
    internal bool IsDeleted { get; private set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal GraphGroup(
      string origin,
      string originId,
      SubjectDescriptor descriptor,
      IdentityDescriptor legacyDescriptor,
      string displayName,
      ReferenceLinks links,
      string url,
      string domain,
      string principalName,
      string mailAddress,
      string description,
      string specialType,
      Guid scopeId,
      string scopeType,
      string scopeName,
      Guid localScopeId,
      Guid securingHostId,
      bool isRestrictedVisible,
      bool isCrossProject,
      bool isGlobalScope,
      bool isDeleted)
      : base(origin, originId, descriptor, legacyDescriptor, displayName, links, url, domain, principalName, mailAddress)
    {
      this.Description = description;
      this.SpecialType = specialType;
      this.ScopeId = scopeId;
      this.ScopeType = scopeType;
      this.ScopeName = scopeName;
      this.LocalScopeId = localScopeId;
      this.SecuringHostId = securingHostId;
      this.IsRestrictedVisible = isRestrictedVisible;
      this.IsCrossProject = isCrossProject;
      this.IsGlobalScope = isGlobalScope;
      this.IsDeleted = isDeleted;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal GraphGroup(
      GraphGroup group,
      string origin = null,
      string originId = null,
      SubjectDescriptor? descriptor = null,
      IdentityDescriptor legacyDescriptor = null,
      string displayName = null,
      ReferenceLinks links = null,
      string url = null,
      string domain = null,
      string principalName = null,
      string mailAddress = null,
      string description = null,
      string specialType = null,
      Guid? scopeId = null,
      string scopeType = null,
      string scopeName = null,
      Guid? localScopeId = null,
      Guid? securingHostId = null,
      bool? isRestrictedVisible = null,
      bool? isCrossProject = null,
      bool? isGlobalScope = null,
      bool? isDeleted = null)
    {
      string origin1 = origin ?? group?.Origin;
      string originId1 = originId ?? group?.OriginId;
      SubjectDescriptor descriptor1 = descriptor ?? (group != null ? group.Descriptor : new SubjectDescriptor());
      IdentityDescriptor legacyDescriptor1 = legacyDescriptor;
      if ((object) legacyDescriptor1 == null)
        legacyDescriptor1 = group?.LegacyDescriptor ?? (IdentityDescriptor) null;
      string displayName1 = displayName ?? group?.DisplayName;
      ReferenceLinks links1 = links ?? group?.Links;
      string url1 = url ?? group?.Url;
      string domain1 = domain ?? group?.Domain;
      string principalName1 = principalName ?? group?.PrincipalName;
      string mailAddress1 = mailAddress ?? group?.MailAddress;
      string description1 = description ?? group?.Description;
      string specialType1 = specialType ?? group?.SpecialType;
      Guid? nullable1 = scopeId;
      Guid scopeId1 = nullable1 ?? (group != null ? group.ScopeId : new Guid());
      string scopeType1 = scopeType ?? group?.ScopeType;
      string scopeName1 = scopeName ?? group?.ScopeName;
      nullable1 = localScopeId;
      Guid localScopeId1 = nullable1 ?? (group != null ? group.LocalScopeId : new Guid());
      nullable1 = securingHostId;
      Guid securingHostId1 = nullable1 ?? (group != null ? group.SecuringHostId : new Guid());
      bool? nullable2 = isRestrictedVisible;
      int num1 = (int) nullable2 ?? (group != null ? (group.IsRestrictedVisible ? 1 : 0) : 0);
      nullable2 = isCrossProject;
      int num2 = (int) nullable2 ?? (group != null ? (group.IsCrossProject ? 1 : 0) : 0);
      nullable2 = isGlobalScope;
      int num3 = (int) nullable2 ?? (group != null ? (group.IsGlobalScope ? 1 : 0) : 0);
      nullable2 = isDeleted;
      int num4 = (int) nullable2 ?? (group != null ? (group.IsDeleted ? 1 : 0) : 0);
      // ISSUE: explicit constructor call
      this.\u002Ector(origin1, originId1, descriptor1, legacyDescriptor1, displayName1, links1, url1, domain1, principalName1, mailAddress1, description1, specialType1, scopeId1, scopeType1, scopeName1, localScopeId1, securingHostId1, num1 != 0, num2 != 0, num3 != 0, num4 != 0);
    }

    protected GraphGroup()
    {
    }
  }
}
