// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedUpdate
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class FeedUpdate
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? UpstreamEnabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? AllowUpstreamNameConflict { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? HideDeletedPackageVersions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<UpstreamSource> UpstreamSources { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? DefaultViewId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? BadgesEnabled { get; set; }
  }
}
