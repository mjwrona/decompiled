// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemField
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemField : SearchSecuredObject
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "referenceName")]
    public string ReferenceName { get; set; }

    [DataMember(Name = "value")]
    public string Value { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });

    public override bool Equals(object obj) => obj is WorkItemField workItemField && this.Name == workItemField.Name && this.ReferenceName == workItemField.ReferenceName && this.Value == workItemField.Value;

    public override int GetHashCode() => this.ReferenceName.GetHashCode() ^ this.Value.GetHashCode();
  }
}
