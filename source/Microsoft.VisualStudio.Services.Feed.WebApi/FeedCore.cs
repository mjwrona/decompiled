// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedCore
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
  public class FeedCore : FeedSecuredObject
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool UpstreamEnabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [ClientInternalUseOnly(false)]
    public bool AllowUpstreamNameConflict { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public FeedView View { get; set; }

    [DataMember]
    public Guid? ViewId
    {
      get => this.View?.Id;
      private set
      {
      }
    }

    [DataMember]
    public string ViewName
    {
      get => this.View?.Name;
      private set
      {
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string FullyQualifiedName
    {
      get => this.View != null ? this.Name + "@" + this.View.Name : this.Name;
      private set
      {
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string FullyQualifiedId
    {
      get => this.View != null ? string.Format("{0}@{1}", (object) this.Id, (object) this.View.Id) : this.Id.ToString();
      private set
      {
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public bool IsReadOnly
    {
      get => this.View != null || this.Capabilities.HasFlag((Enum) FeedCapabilities.UnderMaintenance);
      private set
      {
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public IList<UpstreamSource> UpstreamSources { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public FeedCapabilities Capabilities { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ProjectReference Project { get; set; }
  }
}
