// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ExtensionProperty
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
  [Entity("extensionPropertys", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.ExtensionProperty", "Microsoft.DirectoryServices.ExtensionProperty"})]
  public class ExtensionProperty : DirectoryObject
  {
    private string _appDisplayName;
    private string _name;
    private string _dataType;
    private bool? _isSyncedFromOnPremises;
    private ChangeTrackingCollection<string> _targetObjects;
    private bool _targetObjectsInitialized;

    public ExtensionProperty() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.ExtensionProperty";

    public ExtensionProperty(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("appDisplayName")]
    public string AppDisplayName
    {
      get => this._appDisplayName;
      set
      {
        this._appDisplayName = value;
        this.ChangedProperties.Add(nameof (AppDisplayName));
      }
    }

    [JsonProperty("name")]
    public string Name
    {
      get => this._name;
      set
      {
        this._name = value;
        this.ChangedProperties.Add(nameof (Name));
      }
    }

    [JsonProperty("dataType")]
    public string DataType
    {
      get => this._dataType;
      set
      {
        this._dataType = value;
        this.ChangedProperties.Add(nameof (DataType));
      }
    }

    [JsonProperty("isSyncedFromOnPremises")]
    public bool? IsSyncedFromOnPremises
    {
      get => this._isSyncedFromOnPremises;
      set
      {
        this._isSyncedFromOnPremises = value;
        this.ChangedProperties.Add(nameof (IsSyncedFromOnPremises));
      }
    }

    [JsonProperty("targetObjects")]
    public ChangeTrackingCollection<string> TargetObjects
    {
      get
      {
        if (this._targetObjects == null)
          this._targetObjects = new ChangeTrackingCollection<string>();
        if (!this._targetObjectsInitialized)
        {
          this._targetObjects.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (TargetObjects)));
          this._targetObjectsInitialized = true;
        }
        return this._targetObjects;
      }
      set
      {
        this._targetObjects = value;
        this.ChangedProperties.Add(nameof (TargetObjects));
      }
    }
  }
}
