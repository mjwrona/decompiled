// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.Wiki
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [DataContract]
  public class Wiki : WikiCreateParameters
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Name = "headCommit", EmitDefaultValue = false)]
    public string HeadCommit { get; set; }

    [DataMember(Name = "repository", EmitDefaultValue = false)]
    public GitRepository Repository { get; set; }
  }
}
