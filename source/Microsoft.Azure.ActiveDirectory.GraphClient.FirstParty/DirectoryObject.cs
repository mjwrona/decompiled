// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.DirectoryObject
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [Entity("directoryObjects", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.DirectoryObject", "Microsoft.DirectoryServices.DirectoryObject"})]
  [JsonObject(MemberSerialization.OptIn)]
  public class DirectoryObject : GraphObject
  {
    private string _objectType;
    private string _objectId;
    private DateTime? _deletionTimestamp;
    private GraphObject _createdOnBehalfOf;
    private ChangeTrackingCollection<GraphObject> _createdObjects;
    private bool _createdObjectsInitialized;
    private GraphObject _manager;
    private ChangeTrackingCollection<GraphObject> _directReports;
    private bool _directReportsInitialized;
    private ChangeTrackingCollection<GraphObject> _members;
    private bool _membersInitialized;
    private ChangeTrackingCollection<GraphObject> _memberOf;
    private bool _memberOfInitialized;
    private ChangeTrackingCollection<GraphObject> _owners;
    private bool _ownersInitialized;
    private ChangeTrackingCollection<GraphObject> _ownedObjects;
    private bool _ownedObjectsInitialized;

    public override void ValidateProperties(bool isCreate)
    {
      base.ValidateProperties(isCreate);
      if (isCreate && !string.IsNullOrEmpty(this.ObjectId))
        throw new PropertyValidationException("ObjectId should be null for create.");
    }

    [JsonProperty("objectType")]
    public string ObjectType
    {
      get => this._objectType;
      set
      {
        this._objectType = value;
        this.ChangedProperties.Add(nameof (ObjectType));
      }
    }

    [JsonProperty("objectId")]
    [Key(true)]
    public string ObjectId
    {
      get => this._objectId;
      set
      {
        this._objectId = value;
        this.ChangedProperties.Add(nameof (ObjectId));
      }
    }

    [JsonProperty("deletionTimestamp")]
    public DateTime? DeletionTimestamp
    {
      get => this._deletionTimestamp;
      set
      {
        this._deletionTimestamp = value;
        this.ChangedProperties.Add(nameof (DeletionTimestamp));
      }
    }

    [JsonProperty("createdOnBehalfOf")]
    [Link("createdOnBehalfOf", true)]
    [JsonConverter(typeof (AadJsonConverter))]
    public GraphObject CreatedOnBehalfOf
    {
      get => this._createdOnBehalfOf;
      set
      {
        this._createdOnBehalfOf = value;
        this.ChangedProperties.Add(nameof (CreatedOnBehalfOf));
      }
    }

    [Link("createdObjects", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("createdObjects")]
    public ChangeTrackingCollection<GraphObject> CreatedObjects
    {
      get
      {
        if (this._createdObjects == null)
          this._createdObjects = new ChangeTrackingCollection<GraphObject>();
        if (!this._createdObjectsInitialized)
        {
          this._createdObjects.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (CreatedObjects)));
          this._createdObjectsInitialized = true;
        }
        return this._createdObjects;
      }
      set
      {
        this._createdObjects = value;
        this.ChangedProperties.Add(nameof (CreatedObjects));
      }
    }

    [JsonConverter(typeof (AadJsonConverter))]
    [Link("manager", true)]
    [JsonProperty("manager")]
    public GraphObject Manager
    {
      get => this._manager;
      set
      {
        this._manager = value;
        this.ChangedProperties.Add(nameof (Manager));
      }
    }

    [JsonConverter(typeof (AadJsonConverter))]
    [JsonProperty("directReports")]
    [Link("directReports", false)]
    public ChangeTrackingCollection<GraphObject> DirectReports
    {
      get
      {
        if (this._directReports == null)
          this._directReports = new ChangeTrackingCollection<GraphObject>();
        if (!this._directReportsInitialized)
        {
          this._directReports.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (DirectReports)));
          this._directReportsInitialized = true;
        }
        return this._directReports;
      }
      set
      {
        this._directReports = value;
        this.ChangedProperties.Add(nameof (DirectReports));
      }
    }

    [JsonConverter(typeof (AadJsonConverter))]
    [Link("members", false)]
    [JsonProperty("members")]
    public ChangeTrackingCollection<GraphObject> Members
    {
      get
      {
        if (this._members == null)
          this._members = new ChangeTrackingCollection<GraphObject>();
        if (!this._membersInitialized)
        {
          this._members.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (Members)));
          this._membersInitialized = true;
        }
        return this._members;
      }
      set
      {
        this._members = value;
        this.ChangedProperties.Add(nameof (Members));
      }
    }

    [JsonProperty("memberOf")]
    [JsonConverter(typeof (AadJsonConverter))]
    [Link("memberOf", false)]
    public ChangeTrackingCollection<GraphObject> MemberOf
    {
      get
      {
        if (this._memberOf == null)
          this._memberOf = new ChangeTrackingCollection<GraphObject>();
        if (!this._memberOfInitialized)
        {
          this._memberOf.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (MemberOf)));
          this._memberOfInitialized = true;
        }
        return this._memberOf;
      }
      set
      {
        this._memberOf = value;
        this.ChangedProperties.Add(nameof (MemberOf));
      }
    }

    [JsonProperty("owners")]
    [Link("owners", false)]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<GraphObject> Owners
    {
      get
      {
        if (this._owners == null)
          this._owners = new ChangeTrackingCollection<GraphObject>();
        if (!this._ownersInitialized)
        {
          this._owners.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (Owners)));
          this._ownersInitialized = true;
        }
        return this._owners;
      }
      set
      {
        this._owners = value;
        this.ChangedProperties.Add(nameof (Owners));
      }
    }

    [Link("ownedObjects", false)]
    [JsonProperty("ownedObjects")]
    [JsonConverter(typeof (AadJsonConverter))]
    public ChangeTrackingCollection<GraphObject> OwnedObjects
    {
      get
      {
        if (this._ownedObjects == null)
          this._ownedObjects = new ChangeTrackingCollection<GraphObject>();
        if (!this._ownedObjectsInitialized)
        {
          this._ownedObjects.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (OwnedObjects)));
          this._ownedObjectsInitialized = true;
        }
        return this._ownedObjects;
      }
      set
      {
        this._ownedObjects = value;
        this.ChangedProperties.Add(nameof (OwnedObjects));
      }
    }
  }
}
