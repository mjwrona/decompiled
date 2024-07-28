// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.DirectoryRoleTemplate
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [Entity("directoryRoleTemplates", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.DirectoryRoleTemplate", "Microsoft.DirectoryServices.DirectoryRoleTemplate"})]
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  public class DirectoryRoleTemplate : DirectoryObject
  {
    private string _description;
    private string _displayName;

    public DirectoryRoleTemplate() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.DirectoryRoleTemplate";

    public DirectoryRoleTemplate(string objectId)
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
  }
}
