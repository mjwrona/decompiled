// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Feed
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class Feed : FeedCore
  {
    [DataMember]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? UpstreamEnabledChangedDate { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<FeedPermission> Permissions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeletedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? PermanentDeletedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ScheduledPermanentDeleteDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HideDeletedPackageVersions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid DefaultViewId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool BadgesEnabled { get; set; }

    public FeedVisibility Visibility => ((FeedVisibility?) this.View?.Visibility).GetValueOrDefault();
  }
}
