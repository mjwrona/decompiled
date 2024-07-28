// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.AdministrativeUnit
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [Entity("administrativeUnits", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.AdministrativeUnit", "Microsoft.DirectoryServices.AdministrativeUnit"})]
  [ExcludeFromCodeCoverage]
  [JsonObject(MemberSerialization.OptIn)]
  public class AdministrativeUnit : DirectoryObject
  {
    private string _displayName;
    private string _description;
    private ChangeTrackingCollection<ScopedRoleMembership> _administrators;
    private bool _administratorsInitialized;

    public AdministrativeUnit() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.AdministrativeUnit";

    public AdministrativeUnit(string objectId)
      : this()
    {
      this.ObjectId = objectId;
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

    [JsonProperty("administrators")]
    [JsonConverter(typeof (AadJsonConverter))]
    [Link("administrators", false)]
    public ChangeTrackingCollection<ScopedRoleMembership> Administrators
    {
      get
      {
        if (this._administrators == null)
          this._administrators = new ChangeTrackingCollection<ScopedRoleMembership>();
        if (!this._administratorsInitialized)
        {
          this._administrators.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (Administrators)));
          this._administratorsInitialized = true;
        }
        return this._administrators;
      }
      set
      {
        this._administrators = value;
        this.ChangedProperties.Add(nameof (Administrators));
      }
    }
  }
}
