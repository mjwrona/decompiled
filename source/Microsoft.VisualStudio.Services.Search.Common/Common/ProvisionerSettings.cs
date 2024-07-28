// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ProvisionerSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ProvisionerSettings
  {
    public ProvisionerSettings()
    {
    }

    public ProvisionerSettings(IVssRequestContext requestContext)
    {
      this.Replicas = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/Replicas");
      this.SharedIndexPrimaries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SharedIndexPrimaries");
      this.CodeSharedIndexPrimaries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CodeSharedIndexPrimaries", TeamFoundationHostType.Deployment, 12);
      this.DedicatedIndexPrimaries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/DedicatedIndexPrimaries");
      this.DedicatedWorkItemIndexPrimaries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/DedicatedWorkItemIndexPrimaries");
      this.SettingsSharedIndexPrimaries = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SettingsSharedIndexPrimaries");
      this.RefreshRate = requestContext.GetConfigValue("/Service/ALMSearch/Settings/RefreshRate");
      this.MaxAccountsPerIndex = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxAccountsPerIndex");
      this.ActiveIndicesCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/ActiveIndicesCount");
      this.MaxReposInOneShard = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxReposInOneShard");
      this.MaxAccountsToOnboardOnce = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxAccountsToOnboardOnce");
      this.IndexCreationWaitTimeInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/IndexCreationWaitTimeInSec");
      this.ProjectRouting = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ProjectRouting");
      this.ProjectRouteLevel = !string.IsNullOrWhiteSpace(this.ProjectRouting) ? (RouteLevel) Enum.Parse(typeof (RouteLevel), this.ProjectRouting) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/ProjectRouting] is unvailable and the current value is null/empty.");
      this.WorkItemRouting = requestContext.GetConfigValue("/Service/ALMSearch/Settings/WorkItemRouting");
      this.WorkItemRouteLevel = !string.IsNullOrWhiteSpace(this.WorkItemRouting) ? (RouteLevel) Enum.Parse(typeof (RouteLevel), this.WorkItemRouting) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/WorkItemRouting] is unvailable and the current value is null/empty.");
      this.WikiRouting = requestContext.GetConfigValue("/Service/ALMSearch/Settings/WikiRouting");
      this.WikiRouteLevel = !string.IsNullOrWhiteSpace(this.WikiRouting) ? (RouteLevel) Enum.Parse(typeof (RouteLevel), this.WikiRouting) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/WikiRouting] is unvailable and the current value is null/empty.");
      this.PackageRouting = requestContext.GetConfigValue("/Service/ALMSearch/Settings/PackageRouting");
      this.PackageVersionRouteLevel = !string.IsNullOrWhiteSpace(this.PackageRouting) ? (RouteLevel) Enum.Parse(typeof (RouteLevel), this.PackageRouting) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/PackageRouting] is unvailable and the current value is null/empty.");
      this.BoardVersionRouteLevel = RouteLevel.Collection;
      this.SettingRouteLevel = RouteLevel.None;
      this.DefaultCodeDocumentContractTypeString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/DefaultCodeDocumentContractType");
      this.DefaultCodeDocumentContractType = !string.IsNullOrWhiteSpace(this.DefaultCodeDocumentContractTypeString) ? (DocumentContractType) Enum.Parse(typeof (DocumentContractType), this.DefaultCodeDocumentContractTypeString) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/DefaultCodeDocumentContractType] is unvailable and the current value is null/empty.");
      this.ProjectDocumentContractTypeString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ProjectDocumentContractType");
      this.ProjectDocumentContractType = !string.IsNullOrWhiteSpace(this.ProjectDocumentContractTypeString) ? (DocumentContractType) Enum.Parse(typeof (DocumentContractType), this.ProjectDocumentContractTypeString) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/ProjectDocumentContractType] is unvailable and the current value is null/empty.");
      this.PackageVersionDocumentContractTypeString = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/PackageVersionDocumentContractType", TeamFoundationHostType.Deployment, "PackageVersionContract");
      this.PackageVersionDocumentContractType = !string.IsNullOrWhiteSpace(this.PackageVersionDocumentContractTypeString) ? (DocumentContractType) Enum.Parse(typeof (DocumentContractType), this.PackageVersionDocumentContractTypeString) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/PackageVersionDocumentContractType] is unvailable and the current value is null/empty.");
      this.WorkItemDocumentContractTypeString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/WorkItemDocumentContractType");
      this.WorkItemDocumentContractType = !string.IsNullOrWhiteSpace(this.WorkItemDocumentContractTypeString) ? (DocumentContractType) Enum.Parse(typeof (DocumentContractType), this.WorkItemDocumentContractTypeString) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/WorkItemDocumentContractType] is unvailable and the current value is null/empty.");
      this.WikiDocumentContractTypeString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/WikiDocumentContractType");
      this.WikiDocumentContractType = !string.IsNullOrWhiteSpace(this.WikiDocumentContractTypeString) ? (DocumentContractType) Enum.Parse(typeof (DocumentContractType), this.WikiDocumentContractTypeString) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/WikiDocumentContractType] is unvailable and the current value is null/empty.");
      this.IndexingVersionString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/IndexingVersion");
      this.IndexingVersion = !string.IsNullOrWhiteSpace(this.IndexingVersionString) ? new Version(this.IndexingVersionString) : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/IndexingVersion] is unvailable and the current value is null/empty.");
    }

    private string DefaultCodeDocumentContractTypeString { get; set; }

    public DocumentContractType DefaultCodeDocumentContractType { get; set; }

    private string ProjectDocumentContractTypeString { get; set; }

    public DocumentContractType ProjectDocumentContractType { get; set; }

    private string PackageVersionDocumentContractTypeString { get; set; }

    public DocumentContractType PackageVersionDocumentContractType { get; set; }

    public string WorkItemDocumentContractTypeString { get; set; }

    public DocumentContractType WorkItemDocumentContractType { get; set; }

    public string WikiDocumentContractTypeString { get; set; }

    public DocumentContractType WikiDocumentContractType { get; set; }

    public string IndexingVersionString { get; set; }

    public Version IndexingVersion { get; set; }

    public int Replicas { get; set; }

    public int SharedIndexPrimaries { get; set; }

    public int CodeSharedIndexPrimaries { get; set; }

    public int DedicatedIndexPrimaries { get; set; }

    public int DedicatedWorkItemIndexPrimaries { get; set; }

    public int SettingsSharedIndexPrimaries { get; set; }

    public string RefreshRate { get; set; }

    public string ProjectRouting { get; set; }

    public string WorkItemRouting { get; set; }

    public string WikiRouting { get; set; }

    public string PackageRouting { get; set; }

    public RouteLevel ProjectRouteLevel { get; set; }

    public RouteLevel WorkItemRouteLevel { get; set; }

    public RouteLevel WikiRouteLevel { get; set; }

    public RouteLevel PackageVersionRouteLevel { get; set; }

    public RouteLevel BoardVersionRouteLevel { get; set; }

    public RouteLevel SettingRouteLevel { get; set; }

    public int MaxAccountsPerIndex { get; set; }

    public int ActiveIndicesCount { get; set; }

    public int MaxReposInOneShard { get; set; }

    public int MaxAccountsToOnboardOnce { get; set; }

    public int IndexCreationWaitTimeInSec { get; set; }

    public RouteLevel GetRouteLevel(IEntityType entityType)
    {
      string str = entityType != null ? entityType.Name : throw new ArgumentNullException(nameof (entityType));
      if (str != null)
      {
        switch (str.Length)
        {
          case 4:
            switch (str[0])
            {
              case 'C':
                if (str == "Code")
                  return RouteLevel.Custom;
                break;
              case 'W':
                if (str == "Wiki")
                  return this.WikiRouteLevel;
                break;
            }
            break;
          case 5:
            if (str == "Board")
              return this.BoardVersionRouteLevel;
            break;
          case 7:
            switch (str[0])
            {
              case 'P':
                if (str == "Package")
                  return this.PackageVersionRouteLevel;
                break;
              case 'S':
                if (str == "Setting")
                  return this.SettingRouteLevel;
                break;
            }
            break;
          case 8:
            if (str == "WorkItem")
              return this.WorkItemRouteLevel;
            break;
          case 11:
            if (str == "ProjectRepo")
              return this.ProjectRouteLevel;
            break;
        }
      }
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Entity Type : {0} is not supported", (object) entityType.Name));
    }
  }
}
