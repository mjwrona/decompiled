// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.DirectoryRole
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
  [Entity("directoryRoles", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.DirectoryRole", "Microsoft.DirectoryServices.DirectoryRole"})]
  public class DirectoryRole : DirectoryObject
  {
    private string _cloudSecurityIdentifier;
    private ChangeTrackingCollection<ScopedRoleMembership> _scopedMembers;
    private bool _scopedMembersInitialized;
    private string _description;
    private string _displayName;
    private bool? _isSystem;
    private bool? _roleDisabled;
    private string _roleTemplateId;

    [JsonProperty("cloudSecurityIdentifier")]
    public string CloudSecurityIdentifier
    {
      get => this._cloudSecurityIdentifier;
      set
      {
        this._cloudSecurityIdentifier = value;
        this.ChangedProperties.Add(nameof (CloudSecurityIdentifier));
      }
    }

    [JsonProperty("scopedMembers")]
    [Link("scopedMembers", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<ScopedRoleMembership> ScopedMembers
    {
      get
      {
        if (this._scopedMembers == null)
          this._scopedMembers = new ChangeTrackingCollection<ScopedRoleMembership>();
        if (!this._scopedMembersInitialized)
        {
          this._scopedMembers.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (ScopedMembers)));
          this._scopedMembersInitialized = true;
        }
        return this._scopedMembers;
      }
      set
      {
        this._scopedMembers = value;
        this.ChangedProperties.Add(nameof (ScopedMembers));
      }
    }

    public DirectoryRole() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.DirectoryRole";

    public DirectoryRole(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("description")]
    public string Description
    {
      get => this._description;
      set
      {
        this._description = value;
        this.ChangedProperties.Add(nameof (Description));
      }
    }

    [JsonProperty("displayName")]
    public string DisplayName
    {
      get => this._displayName;
      set
      {
        this._displayName = value;
        this.ChangedProperties.Add(nameof (DisplayName));
      }
    }

    [JsonProperty("isSystem")]
    public bool? IsSystem
    {
      get => this._isSystem;
      set
      {
        this._isSystem = value;
        this.ChangedProperties.Add(nameof (IsSystem));
      }
    }

    [JsonProperty("roleDisabled")]
    public bool? RoleDisabled
    {
      get => this._roleDisabled;
      set
      {
        this._roleDisabled = value;
        this.ChangedProperties.Add(nameof (RoleDisabled));
      }
    }

    [JsonProperty("roleTemplateId")]
    public string RoleTemplateId
    {
      get => this._roleTemplateId;
      set
      {
        this._roleTemplateId = value;
        this.ChangedProperties.Add(nameof (RoleTemplateId));
      }
    }
  }
}
