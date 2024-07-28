// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DebuggerDisplay("Name: {DisplayName} ID:{Id}")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public abstract class IdentityBase : IVssIdentity, IReadOnlyVssIdentity
  {
    private bool m_hasModifiedProperties;

    protected IdentityBase(PropertiesCollection properties)
    {
      this.Properties = properties != null ? properties : new PropertiesCollection();
      this.ResourceVersion = 2;
      this.MetaType = IdentityMetaType.Unknown;
    }

    [DataMember]
    public Guid Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityDescriptor Descriptor { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public SubjectDescriptor SubjectDescriptor { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public SocialDescriptor SocialDescriptor { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ProviderDisplayName { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CustomDisplayName { get; set; }

    public string DisplayName => !string.IsNullOrEmpty(this.CustomDisplayName) ? this.CustomDisplayName : this.ProviderDisplayName;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsActive { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int UniqueUserId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsContainer { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ICollection<IdentityDescriptor> Members { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ICollection<IdentityDescriptor> MemberOf { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ICollection<Guid> MemberIds { get; set; }

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ICollection<Guid> MemberOfIds { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid MasterId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PropertiesCollection Properties { get; private set; }

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ValidateProperties
    {
      get => this.Properties.ValidateNewValues;
      set => this.Properties.ValidateNewValues = value;
    }

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsExternalUser => Guid.TryParse(this.GetProperty<string>("Domain", string.Empty), out Guid _) && this.Descriptor.IdentityType != "Microsoft.TeamFoundation.ServiceIdentity" && this.Descriptor.IdentityType != "Microsoft.TeamFoundation.AggregateIdentity" && this.Descriptor.IdentityType != "Microsoft.TeamFoundation.ImportedIdentity";

    [IgnoreDataMember]
    public Guid LocalScopeId => this.GetProperty<Guid>(nameof (LocalScopeId), new Guid());

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsBindPending => this.Descriptor != (IdentityDescriptor) null && "Microsoft.TeamFoundation.BindPendingIdentity".Equals(this.Descriptor.IdentityType);

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsClaims => this.Descriptor != (IdentityDescriptor) null && "Microsoft.IdentityModel.Claims.ClaimsIdentity".Equals(this.Descriptor.IdentityType);

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsAADServicePrincipal => this.Descriptor != (IdentityDescriptor) null && this.Descriptor.IsAadServicePrincipalType();

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsImported => this.Descriptor != (IdentityDescriptor) null && "Microsoft.TeamFoundation.ImportedIdentity".Equals(this.Descriptor.IdentityType);

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsServiceIdentity => this.Descriptor != (IdentityDescriptor) null && "Microsoft.TeamFoundation.ServiceIdentity".Equals(this.Descriptor.IdentityType);

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ResourceVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int MetaTypeId { get; set; }

    public IdentityMetaType MetaType
    {
      get => (IdentityMetaType) this.MetaTypeId;
      set => this.MetaTypeId = (int) value;
    }

    [IgnoreDataMember]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsCspPartnerUser => this.Descriptor != (IdentityDescriptor) null && this.Descriptor.IsCspPartnerIdentityType();

    public T GetProperty<T>(string name, T defaultValue)
    {
      T obj;
      return this.Properties != null && this.Properties.TryGetValidatedValue<T>(name, out obj) ? obj : defaultValue;
    }

    public bool TryGetProperty(string name, out object value)
    {
      value = (object) null;
      return this.Properties != null && this.Properties.TryGetValue(name, out value);
    }

    public void SetProperty(string name, object value)
    {
      this.m_hasModifiedProperties = true;
      this.Properties[name] = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasModifiedProperties => this.m_hasModifiedProperties;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ResetModifiedProperties() => this.m_hasModifiedProperties = false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void SetAllModifiedProperties() => this.m_hasModifiedProperties = true;

    public override bool Equals(object obj) => obj is IdentityBase identityBase && this.Id == identityBase.Id && IdentityDescriptorComparer.Instance.Equals(this.Descriptor, identityBase.Descriptor) && string.Equals(this.ProviderDisplayName, identityBase.ProviderDisplayName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.CustomDisplayName, identityBase.CustomDisplayName, StringComparison.OrdinalIgnoreCase) && this.IsActive == identityBase.IsActive && this.UniqueUserId == identityBase.UniqueUserId && this.IsContainer == identityBase.IsContainer;

    public override int GetHashCode() => this.Descriptor == (IdentityDescriptor) null ? 0 : this.Descriptor.GetHashCode();

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Identity {0} (IdentityType: {1}; Identifier: {2}; DisplayName: {3})", (object) this.Id, this.Descriptor == (IdentityDescriptor) null ? (object) string.Empty : (object) this.Descriptor.IdentityType, this.Descriptor == (IdentityDescriptor) null ? (object) string.Empty : (object) this.Descriptor.Identifier, (object) this.DisplayName);
  }
}
