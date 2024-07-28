// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Model.ArtifactDefinitionReference
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Model
{
  public class ArtifactDefinitionReference : IEquatable<ArtifactDefinitionReference>
  {
    public string ArtifactType { get; set; }

    public Guid Connection { get; set; }

    public ProjectInfo Project { get; set; }

    public string Source { get; set; }

    public string UniqueResourceIdentifier { get; set; }

    public IDictionary<string, string> Properties { get; set; }

    public bool Equals(ArtifactDefinitionReference other) => other != null && this.Connection.Equals(other.Connection) && ArtifactDefinitionReference.ProjectEquals(this.Project, other.Project) && string.Equals(this.ArtifactType, other.ArtifactType, StringComparison.OrdinalIgnoreCase) && string.Equals(this.UniqueResourceIdentifier, other.UniqueResourceIdentifier, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((ArtifactDefinitionReference) obj);
    }

    public override int GetHashCode() => (((this.ArtifactType != null ? this.ArtifactType.GetHashCode() : 0) * 397 ^ this.Connection.GetHashCode()) * 397 ^ (this.Project != null ? this.Project.GetHashCode() : 0)) * 397 ^ (this.UniqueResourceIdentifier != null ? this.UniqueResourceIdentifier.GetHashCode() : 0);

    private static bool ProjectEquals(ProjectInfo thisProject, ProjectInfo otherProject)
    {
      if (thisProject != null && otherProject != null)
        return thisProject.Equals((object) otherProject);
      return thisProject == null && otherProject == null;
    }
  }
}
