// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLfsObjectAction
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DataContract]
  public class GitLfsObjectAction
  {
    public GitLfsObjectAction()
    {
    }

    public GitLfsObjectAction(string href) => this.Href = href;

    [DataMember(Name = "href", EmitDefaultValue = false, IsRequired = true)]
    public string Href { get; set; }

    [DataMember(Name = "header", EmitDefaultValue = false, IsRequired = false)]
    public IDictionary<string, string> Header { get; set; }
  }
}
