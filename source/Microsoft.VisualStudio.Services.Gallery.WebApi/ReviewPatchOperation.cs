// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ReviewPatchOperation
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public enum ReviewPatchOperation
  {
    [EnumMember] FlagReview = 1,
    [EnumMember] UpdateReview = 2,
    [EnumMember] ReplyToReview = 3,
    [EnumMember] AdminResponseForReview = 4,
    [EnumMember] DeleteAdminReply = 5,
    [EnumMember] DeletePublisherReply = 6,
  }
}
