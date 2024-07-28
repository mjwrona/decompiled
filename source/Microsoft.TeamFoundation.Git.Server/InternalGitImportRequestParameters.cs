// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.InternalGitImportRequestParameters
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DataContract]
  public class InternalGitImportRequestParameters : GitImportRequestParameters
  {
    [DataMember]
    [DefaultValue("")]
    public string UserAgent { get; set; }

    public InternalGitImportRequestParameters()
    {
    }

    public InternalGitImportRequestParameters(
      GitImportRequestParameters importRequestParameter,
      string userAgent)
      : base(importRequestParameter)
    {
      if (string.IsNullOrWhiteSpace(userAgent))
        this.UserAgent = string.Empty;
      else if (userAgent.Length > 500)
        this.UserAgent = userAgent.Substring(0, 500);
      else
        this.UserAgent = userAgent;
    }
  }
}
