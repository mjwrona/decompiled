// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.DirectoryMemberRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public class DirectoryMemberRequest
  {
    private string status;
    private HashSet<string> distinctPropertiesToReturnWithDefaults;

    public IReadOnlyList<DirectoryMember> Members { get; }

    public string Profile { get; }

    public string License { get; }

    public IEnumerable<string> LocalGroups { get; }

    public IEnumerable<string> PropertiesToReturn { get; }

    public IEnumerable<string> DistinctPropertiesToReturnWithDefaults
    {
      get
      {
        if (this.distinctPropertiesToReturnWithDefaults == null)
          this.distinctPropertiesToReturnWithDefaults = DirectoryMemberRequest.GetDistinctPropertiesWithDefaults(this.PropertiesToReturn);
        return (IEnumerable<string>) this.distinctPropertiesToReturnWithDefaults;
      }
    }

    public Guid? TenantId { get; set; }

    public string Status
    {
      get => this.status ?? "Success";
      set => this.status = value;
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    public static DirectoryMemberRequest Create(
      IEnumerable<IDirectoryEntityDescriptor> entities,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new DirectoryMemberRequest(entities, DirectoryMemberRequest.\u003C\u003EO.\u003C0\u003E__Create ?? (DirectoryMemberRequest.\u003C\u003EO.\u003C0\u003E__Create = new Func<IDirectoryEntityDescriptor, DirectoryMemberRequest, DirectoryMember>(DirectoryMember.Create)), profile, license, localGroups, propertiesToReturn);
    }

    internal DirectoryMemberRequest()
      : this(Enumerable.Empty<IDirectoryEntityDescriptor>())
    {
    }

    internal DirectoryMemberRequest(
      IEnumerable<IDirectoryEntityDescriptor> entities,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
      : this(entities, (Func<IDirectoryEntityDescriptor, DirectoryMemberRequest, DirectoryMember>) ((e, r) => new DirectoryMember(e, r)), profile, license, localGroups, propertiesToReturn)
    {
    }

    internal DirectoryMemberRequest(
      IEnumerable<IDirectoryEntityDescriptor> entities,
      Func<IDirectoryEntityDescriptor, DirectoryMemberRequest, DirectoryMember> createMember,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      DirectoryMemberRequest directoryMemberRequest = this;
      if (entities == null)
        throw new DirectoryParameterException("Parameter 'entities' cannot be null");
      this.Members = (IReadOnlyList<DirectoryMember>) entities.Select<IDirectoryEntityDescriptor, DirectoryMember>((Func<IDirectoryEntityDescriptor, DirectoryMember>) (e => createMember(e, directoryMemberRequest))).ToList<DirectoryMember>();
      this.Profile = profile;
      this.License = license;
      this.LocalGroups = localGroups;
      this.PropertiesToReturn = propertiesToReturn;
    }

    private static HashSet<string> GetDistinctPropertiesWithDefaults(IEnumerable<string> properties)
    {
      HashSet<string> propertiesWithDefaults = new HashSet<string>();
      if (properties != null)
        propertiesWithDefaults.UnionWith(properties);
      propertiesWithDefaults.Add("PrincipalName");
      propertiesWithDefaults.Add("DisplayName");
      propertiesWithDefaults.Add("SubjectDescriptor");
      return propertiesWithDefaults;
    }
  }
}
