// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class CloneOptions
  {
    [DataMember(Name = "relatedLinkComment", EmitDefaultValue = false)]
    public string RelatedLinkComment { get; set; }

    [DataMember(Name = "copyAllSuites", EmitDefaultValue = true)]
    public bool CopyAllSuites { get; set; }

    [DataMember(Name = "copyAncestorHierarchy", EmitDefaultValue = true)]
    public bool CopyAncestorHierarchy { get; set; }

    [DataMember(Name = "destinationWorkItemType", EmitDefaultValue = false)]
    public string DestinationWorkItemType { get; set; }

    [DataMember(Name = "cloneRequirements", EmitDefaultValue = true)]
    public bool CloneRequirements { get; set; }

    [DataMember(Name = "overrideParameters", EmitDefaultValue = false)]
    public Dictionary<string, string> OverrideParameters { get; set; }
  }
}
