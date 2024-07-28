// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.AppRoleAssignment
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [Entity("appRoleAssignments", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.AppRoleAssignment", "Microsoft.DirectoryServices.AppRoleAssignment"})]
  [JsonObject(MemberSerialization.OptIn)]
  public class AppRoleAssignment : DirectoryObject
  {
    private DateTime? _creationTimestamp;
    private Guid? _id;
    private string _principalDisplayName;
    private Guid? _principalId;
    private string _principalType;
    private string _resourceDisplayName;
    private Guid? _resourceId;

    public AppRoleAssignment() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.AppRoleAssignment";

    public AppRoleAssignment(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("creationTimestamp")]
    public DateTime? CreationTimestamp
    {
      get => this._creationTimestamp;
      set
      {
        this._creationTimestamp = value;
        this.ChangedProperties.Add(nameof (CreationTimestamp));
      }
    }

    [JsonProperty("id")]
    public Guid? Id
    {
      get => this._id;
      set
      {
        this._id = value;
        this.ChangedProperties.Add(nameof (Id));
      }
    }

    [JsonProperty("principalDisplayName")]
    public string PrincipalDisplayName
    {
      get => this._principalDisplayName;
      set
      {
        this._principalDisplayName = value;
        this.ChangedProperties.Add(nameof (PrincipalDisplayName));
      }
    }

    [JsonProperty("principalId")]
    public Guid? PrincipalId
    {
      get => this._principalId;
      set
      {
        this._principalId = value;
        this.ChangedProperties.Add(nameof (PrincipalId));
      }
    }

    [JsonProperty("principalType")]
    public string PrincipalType
    {
      get => this._principalType;
      set
      {
        this._principalType = value;
        this.ChangedProperties.Add(nameof (PrincipalType));
      }
    }

    [JsonProperty("resourceDisplayName")]
    public string ResourceDisplayName
    {
      get => this._resourceDisplayName;
      set
      {
        this._resourceDisplayName = value;
        this.ChangedProperties.Add(nameof (ResourceDisplayName));
      }
    }

    [JsonProperty("resourceId")]
    public Guid? ResourceId
    {
      get => this._resourceId;
      set
      {
        this._resourceId = value;
        this.ChangedProperties.Add(nameof (ResourceId));
      }
    }
  }
}
