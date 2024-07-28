// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalGitCommit
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  public class ExternalGitCommit : IExternalArtifact, IAdditionalProperties
  {
    [DataMember]
    public string Sha;
    [DataMember]
    public string Message;
    [DataMember]
    public DateTime CommitedDate;
    [DataMember]
    public DateTime? PushedDate;
    [DataMember]
    public ExternalGitUser Author;
    [DataMember]
    public ExternalGitRepo Repo;
    [DataMember]
    public string WebUrl;

    [DataMember]
    public IDictionary<string, object> AdditionalProperties { get; set; }
  }
}
