// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectPropertyName
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Server.Types
{
  internal class ProjectPropertyName
  {
    public ProjectPropertyName(Guid projectId, string propertyName)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(propertyName, nameof (propertyName));
      this.ProjectId = projectId;
      this.PropertyName = propertyName;
    }

    public Guid ProjectId { get; }

    public string PropertyName { get; }

    public override bool Equals(object obj) => obj is ProjectPropertyName projectPropertyName && this.ProjectId == projectPropertyName.ProjectId && TFStringComparer.TeamProjectPropertyName.Equals(this.PropertyName, projectPropertyName.PropertyName);

    public override int GetHashCode() => this.ProjectId.GetHashCode() ^ TFStringComparer.TeamProjectPropertyName.GetHashCode(this.PropertyName);
  }
}
