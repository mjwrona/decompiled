// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntity
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [DataContract]
  internal abstract class DirectoryEntity : IDirectoryEntity, IDirectoryEntityDescriptor
  {
    private static readonly CamelCasePropertyNamesContractResolver CamelCaseResolver = new CamelCasePropertyNamesContractResolver();

    [DataMember]
    public string EntityId { get; internal set; }

    [DataMember]
    public string EntityType { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string EntityOrigin { get; internal set; }

    [DataMember]
    public string OriginDirectory { get; internal set; }

    [DataMember]
    public string OriginId { get; internal set; }

    [DataMember]
    public string LocalDirectory { get; internal set; }

    [DataMember]
    public string LocalId { get; internal set; }

    public object this[string propertyName]
    {
      get
      {
        object obj = (object) null;
        if (this.Properties != null)
          this.Properties.TryGetValue(propertyName, out obj);
        return obj;
      }
      internal set
      {
        if (this.Properties == null)
          this.Properties = (IDictionary<string, object>) new Dictionary<string, object>();
        this.Properties[propertyName] = value;
      }
    }

    public string PrincipalName
    {
      get => this[nameof (PrincipalName)] as string;
      internal set => this[nameof (PrincipalName)] = (object) value;
    }

    public bool? Active
    {
      get => this[nameof (Active)] as bool?;
      internal set => this[nameof (Active)] = (object) value;
    }

    public string DisplayName
    {
      get => this[nameof (DisplayName)] as string;
      internal set => this[nameof (DisplayName)] = (object) value;
    }

    public Microsoft.VisualStudio.Services.Common.SubjectDescriptor? SubjectDescriptor
    {
      get => this[nameof (SubjectDescriptor)] as Microsoft.VisualStudio.Services.Common.SubjectDescriptor?;
      internal set => this[nameof (SubjectDescriptor)] = (object) value;
    }

    public string ScopeName
    {
      get => this[nameof (ScopeName)] as string;
      internal set => this[nameof (ScopeName)] = (object) value;
    }

    public string LocalDescriptor
    {
      get => this[nameof (LocalDescriptor)]?.ToString();
      internal set => this[nameof (LocalDescriptor)] = (object) value;
    }

    public override string ToString() => string.Format("{0} <{1}>", (object) this.DisplayName, (object) this.EntityId);

    public bool Equals(DirectoryEntity other) => other != null && this.EntityId.Equals(other.EntityId);

    public override int GetHashCode() => this.EntityId == null ? 0 : this.EntityId.GetHashCode();

    internal IDictionary<string, object> Properties { get; set; }

    internal DirectoryEntity()
    {
    }

    [JsonConstructor]
    protected DirectoryEntity(
      string entityId,
      string entityType,
      string originDirectory,
      string originId,
      string localDirectory,
      string localId,
      string principalName,
      string displayName,
      Microsoft.VisualStudio.Services.Common.SubjectDescriptor? subjectDescriptor,
      string scopeName,
      string localDescriptor,
      DirectoryPermissionsEntry[] localPermissions,
      string entityOrigin = null)
    {
      this.EntityId = entityId;
      this.EntityType = entityType;
      this.EntityOrigin = entityOrigin;
      this.OriginDirectory = originDirectory;
      this.OriginId = originId;
      this.LocalDirectory = localDirectory;
      this.LocalId = localId;
      this.Properties = (IDictionary<string, object>) new Dictionary<string, object>();
      this.Properties.SetIfNotNull<string, object>(nameof (PrincipalName), (object) principalName);
      this.Properties.SetIfNotNull<string, object>(nameof (DisplayName), (object) displayName);
      this.Properties.SetIfNotNull<string, object>(nameof (SubjectDescriptor), (object) subjectDescriptor);
      this.Properties.SetIfNotNull<string, object>(nameof (ScopeName), (object) scopeName);
      this.Properties.SetIfNotNull<string, object>(nameof (LocalDescriptor), (object) localDescriptor);
      this.Properties.SetIfNotNull<string, object>("LocalPermissions", (object) localPermissions);
    }

    [JsonExtensionData(ReadData = false, WriteData = true)]
    private IDictionary<string, object> PropertiesToSerializeOut
    {
      get => (IDictionary<string, object>) this.Properties.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (property => DirectoryEntity.CamelCaseResolver.GetResolvedPropertyName(property.Key)), (Func<KeyValuePair<string, object>, object>) (property => property.Value));
      set
      {
      }
    }
  }
}
