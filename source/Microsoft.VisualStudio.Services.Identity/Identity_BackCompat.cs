// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Identity_BackCompat
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DebuggerDisplay("{DisplayName}")]
  public sealed class Identity_BackCompat
  {
    private HashSet<string> m_modifiedProperties;

    public Identity_BackCompat()
    {
    }

    internal Identity_BackCompat(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.Id = identity.Id;
      this.Descriptor = identity.Descriptor.ToBackCompatDescriptor();
      this.ProviderDisplayName = identity.ProviderDisplayName;
      this.CustomDisplayName = identity.CustomDisplayName;
      this.IsActive = identity.IsActive;
      this.UniqueUserId = identity.UniqueUserId;
      this.IsContainer = identity.IsContainer;
      this.Members = identity.Members.ToBackCompatDescriptorCollection();
      this.MemberOf = identity.MemberOf.ToBackCompatDescriptorCollection();
      this.MemberIds = identity.MemberIds;
      this.MemberOfIds = identity.MemberOfIds;
      this.MasterId = identity.MasterId;
      this.Properties = new Dictionary<string, object>((IDictionary<string, object>) identity.Properties, (IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
    }

    internal static IList<Identity_BackCompat> Convert(IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList)
    {
      if (identityList == null)
        return (IList<Identity_BackCompat>) null;
      IList<Identity_BackCompat> identityBackCompatList = (IList<Identity_BackCompat>) new List<Identity_BackCompat>(identityList.Count);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
        identityBackCompatList.Add(identity != null ? new Identity_BackCompat(identity) : (Identity_BackCompat) null);
      return identityBackCompatList;
    }

    internal static IList<Microsoft.VisualStudio.Services.Identity.Identity> Convert(
      IList<Identity_BackCompat> identityList)
    {
      if (identityList == null)
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList1 = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>(identityList.Count);
      foreach (Identity_BackCompat identity in (IEnumerable<Identity_BackCompat>) identityList)
        identityList1.Add(identity?.ToIdentity());
      return identityList1;
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity ToIdentity()
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity1.Id = this.Id;
      identity1.Descriptor = this.Descriptor.ToDescriptor();
      identity1.ProviderDisplayName = this.ProviderDisplayName;
      identity1.CustomDisplayName = this.CustomDisplayName;
      identity1.IsActive = this.IsActive;
      identity1.UniqueUserId = this.UniqueUserId;
      identity1.IsContainer = this.IsContainer;
      identity1.Members = this.Members.ToDescriptorCollection();
      identity1.MemberOf = this.MemberOf.ToDescriptorCollection();
      identity1.MemberIds = this.MemberIds;
      identity1.MemberOfIds = this.MemberOfIds;
      identity1.MasterId = this.MasterId;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
      if (this.Properties != null)
      {
        foreach (KeyValuePair<string, object> property in this.Properties)
          identity2.SetProperty(property.Key, property.Value);
      }
      return identity2;
    }

    public Guid Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public IdentityDescriptor_BackCompat Descriptor { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public string ProviderDisplayName { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public string CustomDisplayName { get; set; }

    public string DisplayName => !string.IsNullOrEmpty(this.CustomDisplayName) ? this.CustomDisplayName : this.ProviderDisplayName;

    public bool IsActive { get; set; }

    public int UniqueUserId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public bool IsContainer { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public ICollection<IdentityDescriptor_BackCompat> Members { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public ICollection<IdentityDescriptor_BackCompat> MemberOf { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    internal ICollection<Guid> MemberIds { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    internal ICollection<Guid> MemberOfIds { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Guid MasterId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);

    public T GetProperty<T>(string name, T defaultValue)
    {
      object obj;
      return this.TryGetProperty(name, out obj) ? (T) obj : defaultValue;
    }

    public bool TryGetProperty(string name, out object value) => this.Properties.TryGetValue(name, out value);

    public void SetProperty(string name, object value)
    {
      if (!IdentityAttributeTags.ReadOnlyProperties.Contains(name))
      {
        PropertyValidation.ValidatePropertyName(name);
        PropertyValidation.ValidatePropertyValue(name, value);
        if (this.m_modifiedProperties == null)
          this.m_modifiedProperties = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
        this.m_modifiedProperties.Add(name);
      }
      if (value != null)
        this.Properties[name] = value;
      else
        this.Properties.Remove(name);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal HashSet<string> GetModifiedProperties() => this.m_modifiedProperties;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void ResetModifiedProperties() => this.m_modifiedProperties = (HashSet<string>) null;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void SetAllModifiedProperties()
    {
      foreach (string key in this.Properties.Keys)
      {
        if (!IdentityAttributeTags.ReadOnlyProperties.Contains(key))
        {
          if (this.m_modifiedProperties == null)
            this.m_modifiedProperties = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.IdentityPropertyName);
          this.m_modifiedProperties.Add(key);
        }
      }
    }

    public Identity_BackCompat Clone()
    {
      Identity_BackCompat identityBackCompat = new Identity_BackCompat()
      {
        Id = this.Id,
        Descriptor = new IdentityDescriptor_BackCompat(this.Descriptor),
        ProviderDisplayName = this.ProviderDisplayName,
        CustomDisplayName = this.CustomDisplayName,
        IsActive = this.IsActive,
        UniqueUserId = this.UniqueUserId,
        IsContainer = this.IsContainer,
        Members = this.Members,
        MemberOf = this.MemberOf,
        MemberIds = this.MemberIds,
        MemberOfIds = this.MemberOfIds,
        MasterId = this.MasterId
      };
      foreach (KeyValuePair<string, object> property in this.Properties)
        identityBackCompat.Properties.Add(property.Key, property.Value);
      return identityBackCompat;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Identity {0} (IdentityType: {1}; Identifier: {2}; DisplayName: {3}", (object) this.Id, this.Descriptor == null ? (object) string.Empty : (object) this.Descriptor.IdentityType, this.Descriptor == null ? (object) string.Empty : (object) this.Descriptor.Identifier, (object) this.DisplayName);
  }
}
