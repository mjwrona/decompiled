// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ProjectRepositoryEntityMetadata
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class ProjectRepositoryEntityMetadata
  {
    [DataMember]
    public CrawlEntityType EntityType { get; set; }

    [DataMember]
    public string EntityName { get; set; }

    [DataMember]
    public string EntityId { get; set; }

    [DataMember]
    public DateTime? EntityLastUpdated { get; set; }

    [DataMember]
    public List<Tuple<DateTime, int>> EntityRecentActivity { get; set; }

    [DataMember]
    public int EntityActivityCount1day { get; set; }

    [DataMember]
    public int EntityActivityCount7days { get; set; }

    [DataMember]
    public int EntityActivityCount30days { get; set; }

    [DataMember]
    public int EntityTrendFactor1Day { get; set; }

    [DataMember]
    public int EntityTrendFactor7Days { get; set; }

    [DataMember]
    public int EntityTrendFactor30Days { get; set; }

    [DataMember]
    public VersionControlType EntityVersionControlType { get; set; }

    [DataMember]
    public string EntityParentId { get; set; }

    [DataMember]
    public string EntityParentName { get; set; }

    [DataMember]
    public int ProjectLikesCount { get; set; }

    [DataMember]
    public string ProjectVisibility { get; set; }

    [DataMember]
    public string[] ProjectTags { get; set; }

    [DataMember]
    public string[] RepositoryLanguages { get; set; }

    [DataMember]
    public int RepositoryForks { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public string ImageUrl { get; set; }

    [DataMember]
    public string ReadmeFilePath { get; set; }

    [DataMember]
    public string[] ProjectLanguages { get; set; }
  }
}
