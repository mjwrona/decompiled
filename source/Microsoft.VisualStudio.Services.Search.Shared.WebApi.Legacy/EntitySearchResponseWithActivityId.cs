// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.EntitySearchResponseWithActivityId
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class EntitySearchResponseWithActivityId : SearchSecuredObject
  {
    [ClientResponseContent]
    [DataMember(Name = "response")]
    public EntitySearchResponse Response { get; set; }

    [ClientResponseHeader("activityid")]
    [DataMember(Name = "activityId")]
    public IEnumerable<string> ActivityId { get; set; }
  }
}
