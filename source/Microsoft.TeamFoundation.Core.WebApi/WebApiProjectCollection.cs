// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.WebApiProjectCollection
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class WebApiProjectCollection : WebApiProjectCollectionRef
  {
    public WebApiProjectCollection()
    {
    }

    public WebApiProjectCollection(WebApiProjectCollectionRef projectCollectionRef)
    {
      this.Id = projectCollectionRef.Id;
      this.Name = projectCollectionRef.Name;
      this.CollectionUrl = projectCollectionRef.CollectionUrl;
      this.Url = projectCollectionRef.Url;
    }

    [DataMember(Order = 4, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Order = 5)]
    public string State { get; set; }
  }
}
