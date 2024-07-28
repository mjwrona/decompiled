// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.RedeployTriggerDeploymentGroupEventSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class RedeployTriggerDeploymentGroupEventSqlComponent : 
    ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<RedeployTriggerDeploymentGroupEventSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<RedeployTriggerDeploymentGroupEventSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<RedeployTriggerDeploymentGroupEventSqlComponent3>(3)
    }, "RedeployTriggerDeploymentGroupEvent", "ReleaseManagement");

    public RedeployTriggerDeploymentGroupEventSqlComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual IList<RedeployTriggerDeploymentGroupEvent> AddRedeployTriggerDeploymentGroupEvents(
      Guid projectId,
      string eventType,
      int deploymentGroupId,
      IList<RedeployTriggerDeploymentGroupEvent> events)
    {
      return (IList<RedeployTriggerDeploymentGroupEvent>) new List<RedeployTriggerDeploymentGroupEvent>();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Top is optional")]
    public virtual IList<RedeployTriggerDeploymentGroupEvent> GetRedeployTriggerDeploymentGroupEvents(
      Guid projectId,
      int top = 50)
    {
      return (IList<RedeployTriggerDeploymentGroupEvent>) new List<RedeployTriggerDeploymentGroupEvent>();
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    public virtual IList<RedeployTriggerDeploymentGroupEvent> UpdateRedeployTriggerDeploymentGroupEventReadyForProcessing(
      Guid projectId,
      int environmentId,
      bool readyForProcessing,
      DateTime lastModifiedOn)
    {
      return (IList<RedeployTriggerDeploymentGroupEvent>) new List<RedeployTriggerDeploymentGroupEvent>();
    }

    public virtual void DeleteRedeployTriggerDeploymentGroupEvents(
      Guid projectId,
      IList<long> eventIds)
    {
    }
  }
}
