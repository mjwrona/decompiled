// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.MailMessage
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class MailMessage
  {
    [DataMember]
    public EmailRecipients To { get; set; }

    [DataMember]
    public EmailRecipients CC { get; set; }

    [DataMember]
    public string Subject { get; set; }

    [DataMember]
    public string Body { get; set; }

    [DataMember]
    public EmailRecipients ReplyTo { get; set; }

    [DataMember]
    public DateTime? ReplyBy { get; set; }

    [DataMember]
    public SenderType SenderType { get; set; }

    [DataMember]
    public string InReplyTo { get; set; }

    [DataMember]
    public string MessageId { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need the setter for deserialization")]
    [DataMember]
    public IList<MailSectionType> Sections { get; set; }
  }
}
