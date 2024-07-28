// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseSqlComponent27
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseSqlComponent27 : ReleaseSqlComponent26
  {
    protected override void BindReleaseEnvironments(
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      this.BindReleaseEnvironmentTable10(nameof (releaseEnvironments), releaseEnvironments);
    }

    public override Release UpdateReleaseEnvironmentConditions(
      Guid projectId,
      int releaseId,
      IEnumerable<ReleaseEnvironment> releaseEnvironments)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateReleaseEnvironmentConditions", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindReleaseEnvironments(releaseEnvironments);
      return this.GetReleaseObject(projectId);
    }
  }
}
