// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiV2
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [DataContract]
  public class WikiV2 : WikiCreateBaseParameters, IBaseResource
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Name = "versions", EmitDefaultValue = true)]
    public IEnumerable<GitVersionDescriptor> Versions { get; set; }

    [DataMember(Name = "properties", EmitDefaultValue = false)]
    public IDictionary<string, string> Properties { get; set; }

    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "remoteUrl", EmitDefaultValue = false)]
    public string RemoteUrl { get; set; }

    [DataMember(Name = "isDisabled", EmitDefaultValue = false)]
    public bool IsDisabled { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      foreach (VersionControlSecuredObject version in this.Versions)
        version.SetSecuredObject(securedObject);
    }
  }
}
