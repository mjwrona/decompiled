// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinitionDeletionJobData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseDefinitionDeletionJobData
  {
    [XmlAttribute("ProjectId")]
    public Guid ProjectId { get; set; }

    [XmlAttribute("ReleaseDefinitionIds")]
    public List<int> ReleaseDefinitionIds { get; set; }

    [XmlAttribute("Comment")]
    public string Comment { get; set; }

    public static ReleaseDefinitionDeletionJobData CreateJobData(
      Guid projectId,
      List<int> releaseDefinitionIds,
      string comment)
    {
      return new ReleaseDefinitionDeletionJobData()
      {
        ProjectId = projectId,
        ReleaseDefinitionIds = releaseDefinitionIds,
        Comment = comment
      };
    }
  }
}
