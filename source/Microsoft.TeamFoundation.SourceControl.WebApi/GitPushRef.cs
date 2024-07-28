// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPushRef
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ServiceEventObject]
  [DataContract]
  [KnownType(typeof (GitPush))]
  public class GitPushRef : VersionControlSecuredObject
  {
    [DataMember(Name = "pushedBy", EmitDefaultValue = false)]
    public IdentityRef PushedBy { get; set; }

    [DataMember(Name = "pushId", EmitDefaultValue = false)]
    public int PushId { get; set; }

    [Obsolete("This is unused as of Dev15 M115 and may be deleted in the future")]
    [DataMember(Name = "pushCorrelationId", EmitDefaultValue = false)]
    public Guid PushCorrelationId { get; set; }

    [DataMember(Name = "date", EmitDefaultValue = false)]
    public DateTime Date { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }
  }
}
