// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionSeverity
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  [DataContract]
  public enum DiscussionSeverity
  {
    [EnumMember] Unknown,
    [EnumMember] Low,
    [EnumMember] Normal,
    [EnumMember] High,
  }
}
