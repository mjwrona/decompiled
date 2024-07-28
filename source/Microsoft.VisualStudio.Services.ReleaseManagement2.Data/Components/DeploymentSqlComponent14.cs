// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DeploymentSqlComponent14
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DeploymentSqlComponent14 : DeploymentSqlComponent13
  {
    public override DeploymentGate UpdateIgnoredGates(
      Guid projectId,
      int releaseId,
      int stepId,
      IEnumerable<string> gatesToIgnore,
      string beforeGatesIgnored,
      string afterGatesIgnored,
      string comment,
      Guid changedBy,
      bool markProcessed)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateIgnoredGates", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt("gateStepId", stepId);
      this.BindStringTable(nameof (gatesToIgnore), gatesToIgnore);
      this.BindString(nameof (beforeGatesIgnored), beforeGatesIgnored, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (afterGatesIgnored), afterGatesIgnored, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid(nameof (changedBy), changedBy);
      this.BindString(nameof (comment), comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean(nameof (markProcessed), markProcessed);
      return this.GetDeploymentGateObject();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Will be overridden in derived classes.")]
    protected override DeploymentGateBinder GetDeploymentGateBinder() => (DeploymentGateBinder) new DeploymentGateBinder2((ReleaseManagementSqlResourceComponentBase) this);
  }
}
