// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.Wiki
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki
{
  [DataContract]
  public class Wiki : SearchSecuredV2Object
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "mappedPath")]
    public string MappedPath { get; set; }

    [DataMember(Name = "version")]
    public string Version { get; set; }
  }
}
