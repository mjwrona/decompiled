// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryUser
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Directories
{
  [DataContract]
  internal class DirectoryUser : 
    DirectoryEntity,
    IDirectoryUser,
    IDirectoryEntity,
    IDirectoryEntityDescriptor
  {
    public string Department
    {
      get => this[nameof (Department)] as string;
      internal set => this[nameof (Department)] = (object) value;
    }

    public bool? Guest
    {
      get => this[nameof (Guest)] as bool?;
      internal set => this[nameof (Guest)] = (object) value;
    }

    public string JobTitle
    {
      get => this[nameof (JobTitle)] as string;
      internal set => this[nameof (JobTitle)] = (object) value;
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

    public string PhysicalDeliveryOfficeName
    {
      get => this[nameof (PhysicalDeliveryOfficeName)] as string;
      internal set => this[nameof (PhysicalDeliveryOfficeName)] = (object) value;
    }

    public string SignInAddress
    {
      get => this[nameof (SignInAddress)] as string;
      internal set => this[nameof (SignInAddress)] = (object) value;
    }

    public string Surname
    {
      get => this[nameof (Surname)] as string;
      internal set => this[nameof (Surname)] = (object) value;
    }

    public string TelephoneNumber
    {
      get => this[nameof (TelephoneNumber)] as string;
      internal set => this[nameof (TelephoneNumber)] = (object) value;
    }

    internal DirectoryUser() => this.EntityType = "User";

    [JsonConstructor]
    private DirectoryUser(
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
      string department,
      bool? guest,
      string jobTitle,
      string mail,
      string mailNickName,
      string physicalDeliveryOfficeName,
      string signInAddress,
      string surname,
      string telephoneNumber,
      bool? active)
      : base(entityId, entityType, originDirectory, originId, localDirectory, localId, principalName, displayName, subjectDescriptor, scopeName, localDescriptor, localPermissions)
    {
      this.Properties.SetIfNotNull<string, object>(nameof (Department), (object) department);
      this.Properties.SetIfNotNull<string, object>(nameof (Guest), (object) guest);
      this.Properties.SetIfNotNull<string, object>("Active", (object) active);
      this.Properties.SetIfNotNull<string, object>(nameof (JobTitle), (object) jobTitle);
      this.Properties.SetIfNotNull<string, object>(nameof (Mail), (object) mail);
      this.Properties.SetIfNotNull<string, object>(nameof (PhysicalDeliveryOfficeName), (object) physicalDeliveryOfficeName);
      this.Properties.SetIfNotNull<string, object>(nameof (SignInAddress), (object) signInAddress);
      this.Properties.SetIfNotNull<string, object>(nameof (Surname), (object) surname);
      this.Properties.SetIfNotNull<string, object>(nameof (TelephoneNumber), (object) telephoneNumber);
    }
  }
}
