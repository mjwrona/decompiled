// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContentSource
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DataContract]
  public class ContentSource : WebSdkMetadata
  {
    [DataMember(Name = "url")]
    public string Url { get; set; }

    [DataMember(Name = "clientId")]
    public string ClientId { get; set; }

    [DataMember(Name = "contributionId")]
    public string ContributionId { get; set; }

    [DataMember(Name = "contentType")]
    public string ContentType { get; set; }

    [DataMember(Name = "contentLength")]
    public long ContentLength { get; set; }

    [DataMember(Name = "moduleNamespaces")]
    public IEnumerable<string> ModuleNamespaces { get; set; }

    [DataMember(Name = "integrity")]
    public string Integrity { get; set; }

    [DataMember(Name = "priority")]
    public int Priority { get; set; }
  }
}
