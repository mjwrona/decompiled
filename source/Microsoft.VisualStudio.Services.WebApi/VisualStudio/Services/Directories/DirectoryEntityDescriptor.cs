// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityDescriptor
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
  public class DirectoryEntityDescriptor : IDirectoryEntityDescriptor
  {
    [DataMember]
    private readonly string entityType;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    private readonly string entityOrigin;
    [DataMember]
    private readonly string originDirectory;
    [DataMember]
    private readonly string localId;
    [DataMember]
    private readonly string originId;
    private Lazy<IDictionary<string, object>> properties = new Lazy<IDictionary<string, object>>((Func<IDictionary<string, object>>) (() => (IDictionary<string, object>) new Dictionary<string, object>()));
    private static readonly CamelCasePropertyNamesContractResolver CamelCaseResolver = new CamelCasePropertyNamesContractResolver();

    public string EntityType => this.entityType ?? "Any";

    public string OriginDirectory => this.originDirectory ?? "src";

    public string EntityOrigin => this.entityOrigin;

    public string OriginId => this.originId;

    public string LocalDirectory => "vsd";

    public string LocalId => this.localId;

    public object this[string propertyName]
    {
      get => this.Properties.GetValueOrDefault<string, object>(propertyName);
      private set => this.Properties[propertyName] = value;
    }

    public string PrincipalName
    {
      get => this[nameof (PrincipalName)] as string;
      private set => this[nameof (PrincipalName)] = (object) value;
    }

    public string DisplayName
    {
      get => this[nameof (DisplayName)] as string;
      private set => this[nameof (DisplayName)] = (object) value;
    }

    public DirectoryEntityDescriptor(
      string entityType = null,
      string originDirectory = null,
      string originId = null,
      string localId = null,
      string principalName = null,
      string displayName = null,
      IReadOnlyDictionary<string, object> properties = null,
      DirectoryEntityDescriptor baseEntity = null)
    {
      int num = properties == null ? 0 : (properties.Count > 0 ? 1 : 0);
      if (num != 0)
      {
        properties.CheckForConflict<string, object>(nameof (PrincipalName), (object) principalName, nameof (principalName), nameof (properties));
        properties.CheckForConflict<string, object>(nameof (DisplayName), (object) displayName, nameof (displayName), nameof (properties));
      }
      if (baseEntity == null)
      {
        this.entityType = entityType;
        this.originDirectory = originDirectory;
        this.originId = originId;
        this.localId = localId;
      }
      else
      {
        this.entityType = entityType ?? baseEntity.EntityType;
        this.originDirectory = originDirectory ?? baseEntity.OriginDirectory;
        this.originId = originId ?? baseEntity.OriginId;
        this.localId = localId ?? baseEntity.LocalId;
        this.properties.SetRangeIfRangeNotNullOrEmpty<string, object, IDictionary<string, object>>((IEnumerable<KeyValuePair<string, object>>) baseEntity.Properties);
      }
      if (num != 0)
      {
        if (properties.ContainsKey(nameof (EntityOrigin)))
          this.entityOrigin = (string) properties[nameof (EntityOrigin)];
        this.properties.Value.SetRangeIfRangeNotNull<string, object, IDictionary<string, object>>((IEnumerable<KeyValuePair<string, object>>) properties);
      }
      this.properties.SetIfNotNull<string, object>(nameof (PrincipalName), (object) principalName);
      this.properties.SetIfNotNull<string, object>(nameof (DisplayName), (object) displayName);
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    private IDictionary<string, object> Properties => this.properties.Value;

    [JsonConstructor]
    private DirectoryEntityDescriptor(
      string entityType,
      string originDirectory,
      string originId,
      string localId,
      string principalName,
      string displayName,
      string mail,
      DirectoryPermissionsEntry[] localPermissions,
      string invitationMethod,
      bool? allowIntroductionOfMembersNotPreviouslyInScope,
      bool? rootWithActiveMembership,
      bool? createIfNotFound,
      bool? includeDeploymentLevelCreation,
      string homeDirectory,
      Guid? masterId,
      Guid? userId,
      Guid? tenantId,
      SubjectDescriptor subjectDescriptor,
      string entityOrigin,
      string puid)
      : this(entityType, originDirectory, originId, localId, principalName, displayName)
    {
      if (entityOrigin != null)
        this.entityOrigin = entityOrigin;
      this.properties.SetIfNotNull<string, object>("Mail", (object) mail);
      this.properties.SetIfNotNull<string, object>("LocalPermissions", (object) localPermissions);
      this.properties.SetIfNotNull<string, object>("Puid", (object) puid);
      this.properties.SetIfNotNull<string, object>("TenantId", (object) tenantId);
      this.properties.SetIfNotNull<string, object>("SubjectDescriptor", (object) subjectDescriptor);
      this.properties.SetIfNotNull<string, object>("InvitationMethod", (object) invitationMethod);
      this.properties.SetIfNotNull<string, object>("AllowIntroductionOfMembersNotPreviouslyInScope", (object) allowIntroductionOfMembersNotPreviouslyInScope);
      this.properties.SetIfNotNull<string, object>("RootWithActiveMembership", (object) rootWithActiveMembership);
      this.properties.SetIfNotNull<string, object>("CreateIfNotFound", (object) createIfNotFound);
      this.properties.SetIfNotNull<string, object>("IncludeDeploymentLevelCreation", (object) includeDeploymentLevelCreation);
      this.properties.SetIfNotNull<string, object>("HomeDirectory", (object) homeDirectory);
      this.properties.SetIfNotNull<string, object>("MasterId", (object) masterId);
      this.properties.SetIfNotNull<string, object>("UserId", (object) userId);
    }

    [JsonExtensionData(ReadData = false, WriteData = true)]
    private IDictionary<string, object> PropertiesToSerializeOut
    {
      get => (IDictionary<string, object>) this.Properties.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (property => DirectoryEntityDescriptor.CamelCaseResolver.GetResolvedPropertyName(property.Key)), (Func<KeyValuePair<string, object>, object>) (property => property.Value));
      set
      {
      }
    }
  }
}
