// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ReleaseReference
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlType("releaseRefDetails")]
  public class ReleaseReference : IEquatable<ReleaseReference>
  {
    public int ReleaseRefId { get; set; }

    public string ReleaseUri { get; set; }

    public string ReleaseEnvUri { get; set; }

    public int ReleaseId { get; set; }

    public int ReleaseEnvId { get; set; }

    public int ReleaseDefId { get; set; }

    public int ReleaseEnvDefId { get; set; }

    public int Attempt { get; set; }

    public string ReleaseName { get; set; }

    public string ReleaseEnvName { get; set; }

    public DateTime ReleaseCreationDate { get; set; }

    public DateTime EnvironmentCreationDate { get; set; }

    public int PrimaryArtifactBuildId { get; set; }

    public string PrimaryArtifactProjectId { get; set; }

    public string PrimaryArtifactType { get; set; }

    public bool Equals(ReleaseReference other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.ReleaseId == other.ReleaseId && this.ReleaseEnvId == other.ReleaseEnvId;
    }

    public override int GetHashCode() => (391 + this.ReleaseId) * 23 + this.ReleaseEnvId;
  }
}
