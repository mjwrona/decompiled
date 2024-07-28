// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.UpdateRetainBuildData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class UpdateRetainBuildData
  {
    [XmlAttribute("JobReason")]
    public UpdateRetainBuildReason JobReason { get; set; }

    [XmlAttribute("ProjectId")]
    public Guid ProjectId { get; set; }

    [XmlAttribute("ReleaseDefinitionId")]
    public int ReleaseDefinitionId { get; set; }

    [XmlArray("ReleaseIds")]
    [XmlArrayItem("ReleaseId")]
    [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Serializer uses setter")]
    public Collection<int> ReleaseIds { get; set; }

    [XmlArray("DefinitionEnvironmentsStartedRetainingBuild")]
    [XmlArrayItem("DefinitionEnvironmentStartedRetainingBuild")]
    [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Serializer uses setter")]
    public Collection<int> DefinitionEnvironmentsStartedRetainingBuild { get; set; }

    [XmlArray("DefinitionEnvironmentsStoppedRetainingBuild")]
    [XmlArrayItem("DefinitionEnvironmentStoppedRetainingBuild")]
    [SuppressMessage("Microsoft.Usage", "CA2227", Justification = "Serializer uses setter")]
    public Collection<int> DefinitionEnvironmentsStoppedRetainingBuild { get; set; }

    [XmlAttribute("MinReleaseIdForStartRetainingBuild")]
    public int MinReleaseIdForStartRetainingBuild { get; set; }

    [XmlAttribute("MinReleaseIdForStopRetainingBuild")]
    public int MinReleaseIdForStopRetainingBuild { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public static UpdateRetainBuildData GetUpdateRetainBuildData(
      UpdateRetainBuildReason jobReason,
      Guid projectId,
      int releaseDefinitionId,
      Collection<int> definitionEnvironmentsStartedRetainingBuild = null,
      Collection<int> definitionEnvironmentsStoppedRetainingBuild = null,
      Collection<int> releaseIds = null)
    {
      return new UpdateRetainBuildData()
      {
        JobReason = jobReason,
        ProjectId = projectId,
        ReleaseDefinitionId = releaseDefinitionId,
        DefinitionEnvironmentsStartedRetainingBuild = definitionEnvironmentsStartedRetainingBuild ?? new Collection<int>(),
        DefinitionEnvironmentsStoppedRetainingBuild = definitionEnvironmentsStoppedRetainingBuild ?? new Collection<int>(),
        ReleaseIds = releaseIds ?? new Collection<int>()
      };
    }
  }
}
