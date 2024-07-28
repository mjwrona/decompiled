// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.DirectoryLinkChange
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [JsonObject(MemberSerialization.OptIn)]
  [Entity("directoryLinkChanges", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.DirectoryLinkChange", "Microsoft.DirectoryServices.DirectoryLinkChange"})]
  public class DirectoryLinkChange : DirectoryObject
  {
    private string _associationType;
    private string _sourceObjectId;
    private string _sourceObjectType;
    private string _sourceObjectUri;
    private string _targetObjectId;
    private string _targetObjectType;
    private string _targetObjectUri;

    public DirectoryLinkChange() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.DirectoryLinkChange";

    public DirectoryLinkChange(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("associationType")]
    public string AssociationType
    {
      get => this._associationType;
      set
      {
        this._associationType = value;
        this.ChangedProperties.Add(nameof (AssociationType));
      }
    }

    [JsonProperty("sourceObjectId")]
    public string SourceObjectId
    {
      get => this._sourceObjectId;
      set
      {
        this._sourceObjectId = value;
        this.ChangedProperties.Add(nameof (SourceObjectId));
      }
    }

    [JsonProperty("sourceObjectType")]
    public string SourceObjectType
    {
      get => this._sourceObjectType;
      set
      {
        this._sourceObjectType = value;
        this.ChangedProperties.Add(nameof (SourceObjectType));
      }
    }

    [JsonProperty("sourceObjectUri")]
    public string SourceObjectUri
    {
      get => this._sourceObjectUri;
      set
      {
        this._sourceObjectUri = value;
        this.ChangedProperties.Add(nameof (SourceObjectUri));
      }
    }

    [JsonProperty("targetObjectId")]
    public string TargetObjectId
    {
      get => this._targetObjectId;
      set
      {
        this._targetObjectId = value;
        this.ChangedProperties.Add(nameof (TargetObjectId));
      }
    }

    [JsonProperty("targetObjectType")]
    public string TargetObjectType
    {
      get => this._targetObjectType;
      set
      {
        this._targetObjectType = value;
        this.ChangedProperties.Add(nameof (TargetObjectType));
      }
    }

    [JsonProperty("targetObjectUri")]
    public string TargetObjectUri
    {
      get => this._targetObjectUri;
      set
      {
        this._targetObjectUri = value;
        this.ChangedProperties.Add(nameof (TargetObjectUri));
      }
    }
  }
}
