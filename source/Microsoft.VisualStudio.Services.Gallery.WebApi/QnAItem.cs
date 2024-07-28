// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.QnAItem
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public abstract class QnAItem
  {
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public string Text { get; set; }

    [DataMember]
    public UserIdentityRef User { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public DateTime UpdatedDate { get; set; }

    [DataMember]
    public QnAItemStatus Status { get; set; }
  }
}
