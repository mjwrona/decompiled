// Decompiled with JetBrains decompiler
// Type: Nest.SlackActionMessageResult
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SlackActionMessageResult
  {
    [DataMember(Name = "message")]
    public ISlackMessage Message { get; set; }

    [DataMember(Name = "reason")]
    public string Reason { get; set; }

    [DataMember(Name = "request")]
    public HttpInputRequestResult Request { get; set; }

    [DataMember(Name = "response")]
    public HttpInputResponseResult Response { get; set; }

    [DataMember(Name = "status")]
    public Status Status { get; set; }

    [DataMember(Name = "to")]
    public string To { get; set; }
  }
}
