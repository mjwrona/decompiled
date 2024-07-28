// Decompiled with JetBrains decompiler
// Type: Galleries.Domain.Model.Project
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Galleries.Domain.Model
{
  [DataContract]
  public class Project
  {
    public Project()
    {
      this.Categories = (IList<Category>) new List<Category>();
      this.Metadata = (IDictionary<string, string>) new Dictionary<string, string>();
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public IDictionary<string, string> Metadata { get; set; }

    [DataMember]
    public IList<Category> Categories { get; set; }

    [DataMember]
    public List<string> Tags { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public DateTime ModifiedDate { get; set; }

    [DataMember]
    public Release CurrentRelease { get; set; }

    [DataMember]
    public IList<Release> Releases { get; set; }

    [DataMember]
    public bool FileReleaseEnabled { get; set; }

    [DataMember]
    public bool DiscussionsEnabled { get; set; }

    [DataMember]
    public bool WorkItemTrackingEnabled { get; set; }

    [DataMember]
    public bool SourceCodeBrowsingEnabled { get; set; }

    [DataMember]
    public bool IsPublished { get; set; }

    [DataMember]
    public string InitialProjectName { get; set; }

    [DataMember]
    public int AffiliateId { get; set; }
  }
}
