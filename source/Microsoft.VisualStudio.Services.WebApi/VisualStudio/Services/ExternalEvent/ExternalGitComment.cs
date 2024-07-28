// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalGitComment
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  [DataContract]
  public class ExternalGitComment : IExternalArtifact, IAdditionalProperties
  {
    [IgnoreDataMember]
    public static readonly ApiResourceVersion CurrentVersion = new ApiResourceVersion(new Version(1, 0), 1);

    [DataMember]
    public string CommentBody { get; set; }

    [DataMember]
    public string UpdatedAt { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public IDictionary<string, string> Properties { get; set; }

    [DataMember]
    public ExternalGitUser CommentedBy { get; set; }

    [DataMember]
    public ExternalGitRepo Repo { get; set; }

    [DataMember]
    public ExternalGitUser Sender { get; set; }

    [DataMember]
    public IDictionary<string, object> AdditionalProperties { get; set; }
  }
}
