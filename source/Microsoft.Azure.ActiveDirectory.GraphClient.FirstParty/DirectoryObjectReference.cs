// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.DirectoryObjectReference
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  [Entity("directoryObjectReferences", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.DirectoryObjectReference", "Microsoft.DirectoryServices.DirectoryObjectReference"})]
  [ExcludeFromCodeCoverage]
  public class DirectoryObjectReference : DirectoryObject
  {
    private string _description;
    private string _displayName;
    private Guid? _externalContextId;

    public DirectoryObjectReference() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.DirectoryObjectReference";

    public DirectoryObjectReference(string objectId)
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

    [JsonProperty("externalContextId")]
    public Guid? ExternalContextId
    {
      get => this._externalContextId;
      set
      {
        this._externalContextId = value;
        this.ChangedProperties.Add(nameof (ExternalContextId));
      }
    }
  }
}
