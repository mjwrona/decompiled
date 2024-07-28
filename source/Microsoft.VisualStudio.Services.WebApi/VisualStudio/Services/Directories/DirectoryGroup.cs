// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryGroup
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [DataContract]
  internal class DirectoryGroup : 
    DirectoryEntity,
    IDirectoryGroup,
    IDirectoryEntity,
    IDirectoryEntityDescriptor
  {
    public string Description
    {
      get => this[nameof (Description)] as string;
      internal set => this[nameof (Description)] = (object) value;
    }

    public string Mail
    {
      get => this[nameof (Mail)] as string;
      internal set => this[nameof (Mail)] = (object) value;
    }

    public string MailNickname
    {
      get => this[nameof (MailNickname)] as string;
      internal set => this[nameof (MailNickname)] = (object) value;
    }

    internal DirectoryGroup() => this.EntityType = "Group";

    [JsonConstructor]
    private DirectoryGroup(
      string entityId,
      string entityType,
      string originDirectory,
      string originId,
      string localDirectory,
      string localId,
      string principalName,
      string displayName,
      string scopeName,
      Microsoft.VisualStudio.Services.Common.SubjectDescriptor? subjectDescriptor,
      string localDescriptor,
      DirectoryPermissionsEntry[] localPermissions,
      string description,
      string mail,
      string mailNickname,
      bool? active)
    {
      string entityId1 = entityId;
      string entityType1 = entityType;
      string originDirectory1 = originDirectory;
      string originId1 = originId;
      string localDirectory1 = localDirectory;
      string localId1 = localId;
      string principalName1 = principalName;
      string displayName1 = displayName;
      string str = scopeName;
      Microsoft.VisualStudio.Services.Common.SubjectDescriptor? subjectDescriptor1 = subjectDescriptor;
      string scopeName1 = str;
      string localDescriptor1 = localDescriptor;
      DirectoryPermissionsEntry[] localPermissions1 = localPermissions;
      // ISSUE: explicit constructor call
      base.\u002Ector(entityId1, entityType1, originDirectory1, originId1, localDirectory1, localId1, principalName1, displayName1, subjectDescriptor1, scopeName1, localDescriptor1, localPermissions1);
      this.Properties.SetIfNotNull<string, object>(nameof (Description), (object) description);
      this.Properties.SetIfNotNull<string, object>(nameof (Mail), (object) mail);
      this.Properties.SetIfNotNull<string, object>(nameof (MailNickname), (object) mailNickname);
      this.Properties.SetIfNotNull<string, object>("Active", (object) active);
    }
  }
}
