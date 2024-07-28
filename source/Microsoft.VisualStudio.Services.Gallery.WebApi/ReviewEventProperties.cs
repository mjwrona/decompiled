// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ReviewEventProperties
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public class ReviewEventProperties
  {
    [DataMember]
    public long ReviewId;
    [DataMember]
    public string ReviewText;
    [DataMember]
    public string ReplyText;
    [DataMember]
    public DateTime ReviewDate;
    [DataMember]
    public DateTime? ReplyDate;
    [DataMember]
    public Guid UserId;
    [DataMember]
    public string UserDisplayName;
    [DataMember]
    public int Rating;
    [DataMember]
    public ReviewResourceType ResourceType;
    [DataMember]
    public ReviewEventOperation EventOperation;
    [DataMember]
    public Guid ReplyUserId;
    [DataMember]
    public bool IsAdminReply;
    [DataMember]
    public bool IsIgnored;
  }
}
