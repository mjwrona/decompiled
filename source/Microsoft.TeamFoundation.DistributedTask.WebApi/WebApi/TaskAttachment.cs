// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAttachment
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAttachment
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;

    internal TaskAttachment()
    {
    }

    internal TaskAttachment(string type, string name, ReferenceLinks links)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Type = type;
      this.Name = name;
      this.m_links = links;
    }

    public TaskAttachment(string type, string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      this.Type = type;
      this.Name = name;
    }

    [DataMember]
    public string Type { get; internal set; }

    [DataMember]
    public string Name { get; internal set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    [DataMember]
    public DateTime CreatedOn { get; internal set; }

    [DataMember]
    public DateTime LastChangedOn { get; internal set; }

    [DataMember]
    public Guid LastChangedBy { get; internal set; }

    [DataMember]
    public Guid TimelineId { get; set; }

    [DataMember]
    public Guid RecordId { get; set; }
  }
}
