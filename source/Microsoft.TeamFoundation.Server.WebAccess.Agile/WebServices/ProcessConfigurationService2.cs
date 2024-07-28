// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.ProcessConfigurationService2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/02", Description = "DevOps Process Configuration web service V2.0")]
  [ClientService(ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ComponentName = "vstfs", ServiceName = "ProcessConfigurationService2", CollectionServiceIdentifier = "8DD5BB32-C02D-43C4-B062-9BBA9BC731BE")]
  [ProxyParentClass("ProcessConfigurationService", IgnoreInheritedMethods = true)]
  public class ProcessConfigurationService2 : ProcessConfigurationService
  {
    [WebMethod]
    public CommonProjectConfiguration GetCommonConfiguration2(string projectUri)
    {
      try
      {
        return this.GetConfiguration(projectUri);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public virtual void SetCommonConfiguration2(
      string projectUri,
      CommonProjectConfiguration commonConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      this.SetCommonConfiguration(projectUri, commonConfiguration);
    }

    [WebMethod]
    public void ValidateCommonConfiguration2(
      string projectUri,
      CommonProjectConfiguration commonConfiguration)
    {
      this.ValidateCommonConfiguration(projectUri, commonConfiguration);
    }

    [WebMethod]
    public AgileProjectConfiguration GetAgileConfiguration2(string projectUri) => this.GetAgileConfiguration(projectUri);

    [WebMethod]
    public virtual void SetAgileConfiguration2(
      string projectUri,
      AgileProjectConfiguration agileConfiguration)
    {
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
      this.SetAgileConfiguration(projectUri, agileConfiguration);
    }

    [WebMethod]
    public void ValidateAgileConfiguration2(
      string projectUri,
      AgileProjectConfiguration agileConfiguration)
    {
      this.ValidateAgileConfiguration(projectUri, agileConfiguration);
    }
  }
}
