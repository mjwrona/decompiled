// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitLfsErrorResponse
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [ClientIgnore]
  public class GitLfsErrorResponse
  {
    [DataMember(Name = "message", EmitDefaultValue = false)]
    public string Message { get; set; }

    [DataMember(Name = "documentation_url", EmitDefaultValue = false)]
    public Uri DocumentationUrl { get; set; }

    [DataMember(Name = "request_id", EmitDefaultValue = false)]
    public string RequestId { get; set; }
  }
}
