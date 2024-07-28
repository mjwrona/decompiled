// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PublisherPermissions
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [Flags]
  [DataContract]
  public enum PublisherPermissions
  {
    [DataMember] Read = 1,
    [DataMember] UpdateExtension = 2,
    [DataMember] CreatePublisher = 4,
    [DataMember] PublishExtension = 8,
    [DataMember] Admin = 16, // 0x00000010
    [DataMember] TrustedPartner = 32, // 0x00000020
    [DataMember] PrivateRead = 64, // 0x00000040
    [DataMember] DeleteExtension = 128, // 0x00000080
    [DataMember] EditSettings = 256, // 0x00000100
    [DataMember] ViewPermissions = 512, // 0x00000200
    [DataMember] ManagePermissions = 1024, // 0x00000400
    [DataMember] DeletePublisher = 2048, // 0x00000800
  }
}
