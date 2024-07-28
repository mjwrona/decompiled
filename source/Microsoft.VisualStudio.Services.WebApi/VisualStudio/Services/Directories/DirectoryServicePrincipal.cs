// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryServicePrincipal
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [DataContract]
  internal class DirectoryServicePrincipal : 
    DirectoryEntity,
    IDirectoryServicePrincipal,
    IDirectoryEntity,
    IDirectoryEntityDescriptor
  {
    [DataMember]
    public string AppId
    {
      get => this[nameof (AppId)] as string;
      internal set => this[nameof (AppId)] = (object) value;
    }

    internal DirectoryServicePrincipal() => this.EntityType = "ServicePrincipal";

    [JsonConstructor]
    private DirectoryServicePrincipal(
      string entityId,
      string entityType,
      string originDirectory,
      string originId,
      string localDirectory,
      string localId,
      string principalName,
      string displayName,
      Microsoft.VisualStudio.Services.Common.SubjectDescriptor? subjectDescriptor,
      string scopeName,
      string localDescriptor,
      DirectoryPermissionsEntry[] localPermissions,
      string appId,
      bool? active)
      : base(entityId, entityType, originDirectory, originId, localDirectory, localId, principalName, displayName, subjectDescriptor, scopeName, localDescriptor, localPermissions)
    {
      this.Properties.SetIfNotNull<string, object>(nameof (AppId), (object) appId);
      this.Properties.SetIfNotNull<string, object>("MailNickname", (object) appId);
      this.Properties.SetIfNotNull<string, object>("Active", (object) active);
    }
  }
}
