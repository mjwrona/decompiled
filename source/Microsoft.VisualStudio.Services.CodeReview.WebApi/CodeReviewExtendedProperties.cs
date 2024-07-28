// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewExtendedProperties
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  [Flags]
  public enum CodeReviewExtendedProperties
  {
    [EnumMember] None = 0,
    [EnumMember] Review = 1,
    [EnumMember] Iteration = 2,
    [EnumMember] Attachment = 4,
    [EnumMember] All = Attachment | Iteration | Review, // 0x00000007
  }
}
