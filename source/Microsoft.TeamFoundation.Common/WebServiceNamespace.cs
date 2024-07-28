// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WebServiceNamespace
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct WebServiceNamespace
  {
    public const string WIT_ClientService = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ClientServices/03";
    public const string WIT_ClientService_Description = "DevOps WorkItemTracking ClientService web service";
    public const string WIT_SyncEventListener = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/SyncEventsListener/03";
    public const string WIT_ExternalServices = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/ExternalServices/03";
    public const string WIT_ConfigurationSettingsService = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";
    public const string WIT_FaultCodes = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultcodes/03";
    public const string WIT_FaultDetail = "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultdetail/03";
    public const string Integration_Linking = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03";
    public const string Integration_Css = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";
    public const string Integration_Css_Description = "DevOps Classification web service";
    public const string Integration_Css3 = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";
    public const string Integration_Css3_Description = "DevOps Classification web service V3.0";
    public const string Integration_Css4 = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Classification/03";
    public const string Integration_Css4_Description = "DevOps Classification web service V4.0";
    public const string Integration_ProcessTemplate = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessTemplate/03";
    public const string Integration_ProcessTemplate_Description = "DevOps Process Template web service";
    public const string Integration_ProcessConfiguration = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/01";
    public const string Integration_ProcessConfiguration_Description = "DevOps Process Configuration web service";
    public const string Integration_ProcessConfiguration2 = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/02";
    public const string Integration_ProcessConfiguration2_Description = "DevOps Process Configuration web service V2.0";
    public const string Integration_ProcessConfiguration3 = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/02";
    public const string Integration_ProcessConfiguration3_Description = "DevOps Process Configuration web service V3.0";
    public const string Integration_ProcessConfiguration4 = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ProcessConfiguration/02";
    public const string Integration_ProcessConfiguration4_Description = "DevOps Process Configuration web service V4.0";
    public const string IProjectMaintenanceBinding = "IProjectMaintenanceBinding";
    public const string DeleteProjectBinding = "DeleteProjectBinding";
    public const string Integration_Admin = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03";
    public const string Integration_Admin_Description = "DevOps Project Maintenance web service";
    public const string Integration_Registration = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03";
    public const string Integration_Registration_Description = "DevOps Registration web service";
    public const string Integration_Gss = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";
    public const string Integration_Gss_Description = "DevOps Group Security web service";
    public const string Integration_Gss2 = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03";
    public const string Integration_Gss2_Description = "DevOps extended Group Security web service";
    public const string Integration_Ims = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/IdentityManagement/03";
    public const string Integration_Ims_Description = "DevOps Identity Management web service";
    public const string Integration_Auth = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Authorization/03";
    public const string Integration_Auth_Description = "DevOps Authorization web service";
    public const string Integration_ServerStatus = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/ServerStatus/03";
    public const string Integration_ServerStatus_Description = "Azure DevOps Server Status web service";
    public const string Integration_Events = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";
    public const string Integration_Events_Description = "Azure DevOps Server Events web service";
    public const string Integration_DeleteProject = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject";
    public const string Integration_NotificationService = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Notification/03";
    public const string Integration_SyncService = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/SyncService/03";
    public const string Integration_SyncService_Description = "DevOps Sync web service";
    public const string Integration_TeamConfiguration = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/TeamConfiguration/01";
    public const string Integration_TeamConfiguration_Description = "DevOps Team Configuration web service";
    public const string Integration_ProcessManagement_Description = "DevOps Process Management Integration web service";
    public const string Scc_Admin = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/Admin/03";
    public const string Scc_Admin_Description = "DevOps VersionControl Admin web service";
    public const string Scc_Integration_Description = "DevOps VersionControl Integration web service";
    public const string Scc_ClientServices = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/ClientServices/03";
    public const string Scc_ClientServices_Description = "DevOps VersionControl ClientServices web service";
    public const string Scc_ProxyStats = "http://schemas.microsoft.com/TeamFoundation/2005/06/VersionControl/Statistics/03";
    public const string Scc_ProxyStats_Description = "DevOps VersionControl Proxy Statistics web service";
    public const string Tb_Controller = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/BuildController/03";
    public const string Tb_Controller_Description = "DevOps Build Controller web service";
    public const string Tb_Store = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/BuildInfo/03";
    public const string Tb_Store_Description = "DevOps Build Info web service";
    public const string Tb_Integration_Description = "DevOps Build Integration web service";
    public const string Tb_Service = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/BuildService/03";
    public const string Tb_Service_Description = "DevOps Build web service";
    public const string Tb_Agent = "http://schemas.microsoft.com/TeamFoundation/2005/06/Build/AgentService/03";
    public const string Tcm_TestResults = "http://schemas.microsoft.com/TeamFoundation/2007/02/TCM/TestResults/01";
    public const string Tcm_TestResults_Description = "Test Management Results Service";
    public const string Tcm_TestImpact = "http://schemas.microsoft.com/TeamFoundation/2007/02/TCM/TestImpact/01";
    public const string Tcm_TestImpact_Description = "Test Impact Service";
    public const string Tcm_TestManagement = "http://schemas.microsoft.com/TeamFoundation/2010/07/TCM/TestManagement/01";
    public const string Tcm_TestManagement_Description = "Test Management Service";
  }
}
