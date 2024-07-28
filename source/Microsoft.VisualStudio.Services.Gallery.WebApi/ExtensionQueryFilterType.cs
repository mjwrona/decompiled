// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionQueryFilterType
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public enum ExtensionQueryFilterType
  {
    [DataMember] Tag = 1,
    [DataMember] DisplayName = 2,
    [DataMember] Private = 3,
    [DataMember] Id = 4,
    [DataMember] Category = 5,
    [DataMember] ContributionType = 6,
    [DataMember] Name = 7,
    [DataMember] InstallationTarget = 8,
    [DataMember] Featured = 9,
    [DataMember] SearchText = 10, // 0x0000000A
    [DataMember] FeaturedInCategory = 11, // 0x0000000B
    [DataMember] ExcludeWithFlags = 12, // 0x0000000C
    [DataMember] IncludeWithFlags = 13, // 0x0000000D
    [DataMember] Lcid = 14, // 0x0000000E
    [DataMember] InstallationTargetVersion = 15, // 0x0000000F
    [DataMember] InstallationTargetVersionRange = 16, // 0x00000010
    [DataMember] VsixMetadata = 17, // 0x00000011
    [DataMember] PublisherName = 18, // 0x00000012
    [DataMember] PublisherDisplayName = 19, // 0x00000013
    [DataMember] IncludeWithPublisherFlags = 20, // 0x00000014
    [DataMember] OrganizationSharedWith = 21, // 0x00000015
    [DataMember] ProductArchitecture = 22, // 0x00000016
    [DataMember] TargetPlatform = 23, // 0x00000017
    [DataMember] ExtensionName = 24, // 0x00000018
  }
}
