// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeletionJobData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ReleaseDeletionJobData
  {
    [XmlAttribute("ProjectId")]
    public Guid ProjectId { get; set; }

    [XmlAttribute("ReleaseId")]
    public int ReleaseId { get; set; }

    [XmlAttribute("DefinitionId")]
    public int DefinitionId { get; set; }

    [XmlAttribute("ReleaseIds")]
    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Xml serialization requires it to be an object instead of interface.")]
    public int[] ReleaseIds { get; set; }

    [XmlAttribute("Comment")]
    public string Comment { get; set; }

    public static ReleaseDeletionJobData CreateJobData(
      Guid projectId,
      int definitionId,
      int[] releaseIds,
      string comment)
    {
      return new ReleaseDeletionJobData()
      {
        ProjectId = projectId,
        DefinitionId = definitionId,
        ReleaseIds = releaseIds,
        Comment = comment
      };
    }
  }
}
