// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemFieldMetadata
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemFieldMetadata
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "referenceName")]
    public string ReferenceName { get; set; }

    [DataMember(Name = "type")]
    public WorkItemFieldType Type { get; set; }

    [DataMember(Name = "alternateNames")]
    public IEnumerable<string> AlternateNames { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this);
  }
}
