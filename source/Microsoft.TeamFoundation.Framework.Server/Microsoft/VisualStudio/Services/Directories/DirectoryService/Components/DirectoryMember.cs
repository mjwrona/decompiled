// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.DirectoryMember
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public class DirectoryMember
  {
    private readonly Lazy<Guid?> localIdAsGuid;
    private readonly Lazy<Guid?> originIdAsGuid;
    private Guid? tenantId;
    private string status;
    private IDictionary<string, object> properties;

    public IDirectoryEntityDescriptor RequestEntity { get; }

    public Guid? LocalIdAsGuid => this.localIdAsGuid.Value;

    public Guid? OriginIdAsGuid => this.originIdAsGuid.Value;

    public string ResolvedEntityType { get; set; }

    public string ResolvedOriginDirectory { get; set; }

    public TeamFoundationHostType VsdIdentityReadAtHostType { get; set; }

    public TeamFoundationHostType VsdIdentityPersistedAtHostType { get; set; }

    public Microsoft.VisualStudio.Services.Identity.Identity VsdIdentity { get; set; }

    public GroupDescription VsdGroupDescription { get; set; }

    public User UserServiceUser { get; set; }

    public bool RequiresPersist { get; set; }

    public bool RequiresToPersistInUserService { get; set; }

    public bool SkippingMsaObjectLookup { get; set; }

    public string EntityState { get; set; }

    public string Profile => this.Request?.Profile;

    public string License => this.Request?.License;

    public IEnumerable<string> LocalGroups => this.Request?.LocalGroups;

    public IEnumerable<string> PropertiesToReturn => this.Request?.PropertiesToReturn;

    public IEnumerable<string> DistinctPropertiesToReturnWithDefaults => this.Request?.DistinctPropertiesToReturnWithDefaults;

    public IList<VsdIdentityResult> NearMissIdentityResults { get; } = (IList<VsdIdentityResult>) new List<VsdIdentityResult>();

    public Guid? TenantId
    {
      get
      {
        Guid? tenantId = this.tenantId;
        if (tenantId.HasValue)
          return tenantId;
        return this.Request?.TenantId;
      }
      set => this.tenantId = value;
    }

    public string Status
    {
      get
      {
        if (this.status != null && this.status != "Success")
          return this.status;
        return this.Request != null ? this.Request.Status : "Success";
      }
      set => this.status = value;
    }

    public Exception Exception { get; set; }

    [JsonIgnore]
    public IDictionary<string, object> Properties
    {
      get
      {
        if (this.properties == null)
          this.properties = (IDictionary<string, object>) new Dictionary<string, object>();
        return this.properties;
      }
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    public static DirectoryMember Create(
      IDirectoryEntityDescriptor requestEntity,
      DirectoryMemberRequest request)
    {
      return new DirectoryMember(requestEntity, request);
    }

    public T GetRequestEntityProperty<T>(string propertyName, T defaultPropertyValue = null)
    {
      object obj = this.RequestEntity[propertyName];
      if (obj == null)
        return defaultPropertyValue;
      if (obj is T requestEntityProperty)
        return requestEntityProperty;
      return typeof (string) == typeof (T) ? (T) obj.ToString() : defaultPropertyValue;
    }

    internal DirectoryMember()
      : this((IDirectoryEntityDescriptor) new DirectoryEntityDescriptor())
    {
    }

    internal DirectoryMember(IDirectoryEntityDescriptor requestEntity)
      : this(requestEntity, (DirectoryMemberRequest) null)
    {
    }

    internal DirectoryMember(
      IDirectoryEntityDescriptor requestEntity,
      DirectoryMemberRequest request)
    {
      this.Request = request;
      if (requestEntity == null)
      {
        this.Status = "InvalidRequestEntity";
        requestEntity = (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor();
      }
      this.RequestEntity = requestEntity;
      this.ResolvedEntityType = requestEntity.EntityType ?? "Any";
      this.tenantId = this.GetRequestEntityProperty<Guid?>(nameof (TenantId));
      this.localIdAsGuid = new Lazy<Guid?>((Func<Guid?>) (() => DirectoryMember.GetGuid(this.RequestEntity.LocalId)));
      this.originIdAsGuid = new Lazy<Guid?>((Func<Guid?>) (() => DirectoryMember.GetGuid(this.RequestEntity.OriginId)));
    }

    private DirectoryMemberRequest Request { get; }

    private static Guid? GetGuid(string id)
    {
      Guid result;
      return !Guid.TryParse(id, out result) ? new Guid?() : new Guid?(result);
    }
  }
}
