// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.StubDirectoryObject
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [Entity("stubDirectoryObjects", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.StubDirectoryObject", "Microsoft.DirectoryServices.StubDirectoryObject"})]
  [JsonObject(MemberSerialization.OptIn)]
  [ExcludeFromCodeCoverage]
  public class StubDirectoryObject : DirectoryObject
  {
    private string _displayName;
    private string _mail;
    private Stream _thumbnailPhoto;
    private string _userPrincipalName;

    public StubDirectoryObject() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.StubDirectoryObject";

    public StubDirectoryObject(string objectId)
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

    [JsonProperty("mail")]
    public string Mail
    {
      get => this._mail;
      set
      {
        this._mail = value;
        this.ChangedProperties.Add(nameof (Mail));
      }
    }

    [JsonProperty("thumbnailPhoto")]
    public Stream ThumbnailPhoto
    {
      get => this._thumbnailPhoto;
      set
      {
        this._thumbnailPhoto = value;
        this.ChangedProperties.Add(nameof (ThumbnailPhoto));
      }
    }

    [Key(false)]
    [JsonProperty("userPrincipalName")]
    public string UserPrincipalName
    {
      get => this._userPrincipalName;
      set
      {
        this._userPrincipalName = value;
        this.ChangedProperties.Add(nameof (UserPrincipalName));
      }
    }
  }
}
