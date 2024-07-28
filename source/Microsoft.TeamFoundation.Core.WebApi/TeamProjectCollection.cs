// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TeamProjectCollection
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class TeamProjectCollection : TeamProjectCollectionReference
  {
    public TeamProjectCollection()
    {
    }

    public TeamProjectCollection(
      TeamProjectCollectionReference projectCollectionRef)
    {
      this.Id = projectCollectionRef.Id;
      this.Name = projectCollectionRef.Name;
      this.Url = projectCollectionRef.Url;
      this.Links = new ReferenceLinks();
      this.Links.AddLink("self", this.Url);
    }

    [DataMember(Order = 3, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Order = 4)]
    public string State { get; set; }

    [DataMember(Name = "_links", Order = 5, EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(Order = 6, EmitDefaultValue = false)]
    public ProcessCustomizationType ProcessCustomizationType { get; set; }
  }
}
