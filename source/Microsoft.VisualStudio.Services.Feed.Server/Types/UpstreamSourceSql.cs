// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Types.UpstreamSourceSql
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server.Types
{
  public class UpstreamSourceSql
  {
    private string location;

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Protocol { get; set; }

    public string Location
    {
      get => new Uri(this.location).AbsoluteUri;
      set => this.location = value;
    }

    public UpstreamSourceType UpstreamSourceType { get; set; }

    public DateTime? DeletedDate { get; set; }

    public Guid FeedId { get; set; }

    public int UpstreamIndex { get; set; }

    public Guid? InternalUpstreamCollectionId { get; set; }

    public Guid? InternalUpstreamProjectId { get; set; }

    public Guid? InternalUpstreamFeedId { get; set; }

    public Guid? InternalUpstreamViewId { get; set; }

    public Guid? ServiceEndpointId { get; set; }

    public Guid? ServiceEndpointProjectId { get; set; }

    public UpstreamSourceSql()
    {
    }

    public UpstreamSourceSql(UpstreamSource upstreamSource)
    {
      this.Id = upstreamSource.Id;
      this.Name = upstreamSource.Name;
      this.Protocol = upstreamSource.Protocol;
      this.Location = upstreamSource.Location;
      this.UpstreamSourceType = upstreamSource.UpstreamSourceType;
      this.DeletedDate = upstreamSource.DeletedDate;
      this.InternalUpstreamCollectionId = upstreamSource.InternalUpstreamCollectionId;
      this.InternalUpstreamProjectId = upstreamSource.InternalUpstreamProjectId;
      this.InternalUpstreamFeedId = upstreamSource.InternalUpstreamFeedId;
      this.InternalUpstreamViewId = upstreamSource.InternalUpstreamViewId;
      this.ServiceEndpointId = upstreamSource.ServiceEndpointId;
      this.ServiceEndpointProjectId = upstreamSource.ServiceEndpointProjectId;
    }

    public UpstreamSource ToUpstreamSource() => new UpstreamSource()
    {
      Id = this.Id,
      Name = this.Name,
      Protocol = this.Protocol,
      Location = this.Location,
      UpstreamSourceType = this.UpstreamSourceType,
      DeletedDate = this.DeletedDate,
      InternalUpstreamCollectionId = this.InternalUpstreamCollectionId,
      InternalUpstreamProjectId = this.InternalUpstreamProjectId,
      InternalUpstreamFeedId = this.InternalUpstreamFeedId,
      InternalUpstreamViewId = this.InternalUpstreamViewId,
      ServiceEndpointId = this.ServiceEndpointId,
      ServiceEndpointProjectId = this.ServiceEndpointProjectId
    };
  }
}
