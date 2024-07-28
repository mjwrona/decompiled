// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ScopedRoleMembership
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  [Entity("scopedRoleMemberships", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.ScopedRoleMembership", "Microsoft.DirectoryServices.ScopedRoleMembership"})]
  public class ScopedRoleMembership : GraphObject
  {
    private string _id;
    private Guid? _roleObjectId;
    private Guid? _administrativeUnitObjectId;
    private IdentityInfo _roleMemberInfo;
    private bool _roleMemberInfoInitialized;

    [JsonProperty("id")]
    [Key(true)]
    public string Id
    {
      get => this._id;
      set
      {
        this._id = value;
        this.ChangedProperties.Add(nameof (Id));
      }
    }

    [JsonProperty("roleObjectId")]
    public Guid? RoleObjectId
    {
      get => this._roleObjectId;
      set
      {
        this._roleObjectId = value;
        this.ChangedProperties.Add(nameof (RoleObjectId));
      }
    }

    [JsonProperty("administrativeUnitObjectId")]
    public Guid? AdministrativeUnitObjectId
    {
      get => this._administrativeUnitObjectId;
      set
      {
        this._administrativeUnitObjectId = value;
        this.ChangedProperties.Add(nameof (AdministrativeUnitObjectId));
      }
    }

    [JsonProperty("roleMemberInfo")]
    public IdentityInfo RoleMemberInfo
    {
      get
      {
        if (this._roleMemberInfo != null && !this._roleMemberInfoInitialized)
        {
          this._roleMemberInfo.ItemChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (RoleMemberInfo)));
          this._roleMemberInfoInitialized = true;
        }
        return this._roleMemberInfo;
      }
      set
      {
        this._roleMemberInfo = value;
        this.ChangedProperties.Add(nameof (RoleMemberInfo));
      }
    }
  }
}
