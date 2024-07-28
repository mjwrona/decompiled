// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.OnReleaseCreatedOrUpdatedJobData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  public class OnReleaseCreatedOrUpdatedJobData
  {
    [XmlAttribute("ReleaseId")]
    public int ReleaseId { get; set; }

    [XmlAttribute("ProjectId")]
    public Guid ProjectId { get; set; }

    [XmlAttribute("ReleaseDefinitionSnapshot")]
    public string DefinitionSnapshot { get; set; }

    [XmlAttribute("ReleaseDefinitionSnapshotRevision")]
    public int DefinitionSnapshotRevision { get; set; }

    [XmlAttribute("ReleaseHistoryChangeType")]
    public ReleaseHistoryChangeTypes ChangeType { get; set; }
  }
}
