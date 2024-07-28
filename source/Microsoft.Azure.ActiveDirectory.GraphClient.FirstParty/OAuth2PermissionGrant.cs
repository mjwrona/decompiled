// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.OAuth2PermissionGrant
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [Entity("oauth2PermissionGrants", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.OAuth2PermissionGrant", "Microsoft.DirectoryServices.OAuth2PermissionGrant"})]
  [JsonObject(MemberSerialization.OptIn)]
  public class OAuth2PermissionGrant : GraphObject
  {
    private string _clientId;
    private string _consentType;
    private DateTime? _expiryTime;
    private string _objectId;
    private string _principalId;
    private string _resourceId;
    private string _scope;
    private DateTime? _startTime;

    [JsonProperty("clientId")]
    public string ClientId
    {
      get => this._clientId;
      set
      {
        this._clientId = value;
        this.ChangedProperties.Add(nameof (ClientId));
      }
    }

    [JsonProperty("consentType")]
    public string ConsentType
    {
      get => this._consentType;
      set
      {
        this._consentType = value;
        this.ChangedProperties.Add(nameof (ConsentType));
      }
    }

    [JsonProperty("expiryTime")]
    public DateTime? ExpiryTime
    {
      get => this._expiryTime;
      set
      {
        this._expiryTime = value;
        this.ChangedProperties.Add(nameof (ExpiryTime));
      }
    }

    [Key(true)]
    [JsonProperty("objectId")]
    public string ObjectId
    {
      get => this._objectId;
      set
      {
        this._objectId = value;
        this.ChangedProperties.Add(nameof (ObjectId));
      }
    }

    [JsonProperty("principalId")]
    public string PrincipalId
    {
      get => this._principalId;
      set
      {
        this._principalId = value;
        this.ChangedProperties.Add(nameof (PrincipalId));
      }
    }

    [JsonProperty("resourceId")]
    public string ResourceId
    {
      get => this._resourceId;
      set
      {
        this._resourceId = value;
        this.ChangedProperties.Add(nameof (ResourceId));
      }
    }

    [JsonProperty("scope")]
    public string Scope
    {
      get => this._scope;
      set
      {
        this._scope = value;
        this.ChangedProperties.Add(nameof (Scope));
      }
    }

    [JsonProperty("startTime")]
    public DateTime? StartTime
    {
      get => this._startTime;
      set
      {
        this._startTime = value;
        this.ChangedProperties.Add(nameof (StartTime));
      }
    }
  }
}
