// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notification.Notification
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notification
{
  [DataContract]
  public class Notification
  {
    [DataMember]
    public long Id { get; set; }

    [DataMember]
    public Guid Recipient { get; set; }

    [DataMember]
    public string Scope { get; set; }

    [DataMember]
    public string Content { get; set; }

    [DataMember]
    public string Category { get; set; }

    [DataMember]
    public DateTime CreatedTime { get; set; }

    [DataMember]
    public string ActionUrl { get; set; }

    public Notification()
    {
    }

    public Notification(
      Guid recipient,
      string scope,
      string content,
      string category,
      DateTime createdTime,
      string actionUrl)
      : this(-1L, recipient, scope, content, category, createdTime, actionUrl)
    {
    }

    public Notification(
      long id,
      Guid recipient,
      string scope,
      string content,
      string category,
      DateTime createdTime,
      string actionUrl)
    {
      this.Id = id;
      this.Recipient = recipient;
      this.Scope = scope;
      this.Content = content;
      this.Category = category;
      this.CreatedTime = createdTime;
      this.ActionUrl = actionUrl;
    }
  }
}
