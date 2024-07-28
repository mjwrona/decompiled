// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedIdsResult
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class FeedIdsResult : FeedSecuredObject
  {
    public FeedIdsResult(string name, Guid id, string? projectName, Guid? projectId)
    {
      this.Name = name;
      this.Id = id;
      this.ProjectName = projectName;
      this.ProjectId = projectId;
    }

    [DataMember]
    public string Name { get; }

    [DataMember]
    public Guid Id { get; }

    [DataMember(EmitDefaultValue = false)]
    public string? ProjectName { get; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ProjectId { get; }
  }
}
