// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LicenseFeatures
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LicenseFeatures
  {
    public const string NoneRequired = "00000000-0000-0000-0000-000000000000";
    public const string ViewMyWorkItems = "4F5D9D44-EC3F-4AF1-921A-DEE2147C54DC";
    public const string StandardFeatures = "D91355E2-2A55-4CBE-9636-4D73F70FBA7C";
    public const string AgileBoards = "181DDF83-AFAF-4982-97D9-870AE96BB8D3";
    public const string BacklogManagement = "EC7545A3-E5DB-40E8-B0D0-F64DF7619BBA";
    public const string FeedbackLicenseFeature = "BB000720-4CF7-466A-BA47-1AB40B7A8DFB";
    public const string Testmanagement = "8D00EEB7-D5AD-4141-B601-FBB500F264BA";
    public const string TestManagementForBasicUsers = "2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5";
    public const string TestManagementForExpressSku = "2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9";
    public const string TestManagementForStakeholder = "D104EA57-16EA-4191-9B60-160D664EE9A8";
    public const string TestManagementInBuildAndRelease = "402E4502-9389-420C-BA11-796CDA2E4867";
    public const string ReleaseManagementStakeholder = "F2CB3207-42D8-42CA-81F3-43F9C5182AF7";
    public const string ReleaseManagement = "321CEBB8-72F6-40E6-836D-47580566CCF4";
    public const string PortfolioBacklogManagement = "68DFF179-850A-4F20-A489-E6BD6E6A17EC";
    public const string ChartViewing = "A510D786-5E4B-432F-8C31-F9D7B42CF17E";
    public const string ChartAuthoring = "F01E607A-F55C-4038-8F4B-7956712DEA22";
    public const string Code = "2FF0A29B-5679-44f6-8FAD-F5968AE3E32E";
    public const string Build = "D8BE799B-2716-4c4b-A939-E97C46A28CAA";
    public const string Admin = "65AC9DB3-BB0A-42fe-B584-A690FB0D817B";
    public const string AdvancedHomePage = "509B6940-9948-47e6-A392-99E0CA873F65";
    public const string AdvancedBacklogManagement = "CEDD6BE8-B717-4a0a-8BFD-C4E9B4CAA071";
    public const string AdvancedPortfolioBacklogManagement = "F1026762-C08D-4de3-9C17-7587296F3CFE";
    public const string EnterpriseFirstPartyExtensions = "2320EAAE-F172-408F-A8C6-1021F7BC7779";
    public const string Plans = "E0FB3E99-BEE8-4D80-BB71-5298FAD4CB15";
    public static readonly Guid NoneRequiredId = new Guid("00000000-0000-0000-0000-000000000000");
    public static readonly Guid StandardFeaturesId = new Guid("D91355E2-2A55-4CBE-9636-4D73F70FBA7C");
    public static readonly Guid ViewMyWorkItemsId = new Guid("4F5D9D44-EC3F-4AF1-921A-DEE2147C54DC");
    public static readonly Guid ChartAuthoringId = new Guid("F01E607A-F55C-4038-8F4B-7956712DEA22");
    public static readonly Guid AgileBoardsId = new Guid("181DDF83-AFAF-4982-97D9-870AE96BB8D3");
    public static readonly Guid BacklogManagementId = new Guid("EC7545A3-E5DB-40E8-B0D0-F64DF7619BBA");
    public static readonly Guid FeedbackId = new Guid("BB000720-4CF7-466A-BA47-1AB40B7A8DFB");
    public static readonly Guid TestManagementId = new Guid("8D00EEB7-D5AD-4141-B601-FBB500F264BA");
    public static readonly Guid TestManagementForBasicUsersId = new Guid("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5");
    public static readonly Guid TestManagementForExpressSkuId = new Guid("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9");
    public static readonly Guid ReleaseManagementStakeholderId = new Guid("F2CB3207-42D8-42CA-81F3-43F9C5182AF7");
    public static readonly Guid ReleaseManagementId = new Guid("321CEBB8-72F6-40E6-836D-47580566CCF4");
    public static readonly Guid PortfolioBacklogManagementId = new Guid("68DFF179-850A-4F20-A489-E6BD6E6A17EC");
    public static readonly Guid ChartViewingId = new Guid("A510D786-5E4B-432F-8C31-F9D7B42CF17E");
    public static readonly Guid CodeId = new Guid("2FF0A29B-5679-44f6-8FAD-F5968AE3E32E");
    public static readonly Guid BuildId = new Guid("D8BE799B-2716-4c4b-A939-E97C46A28CAA");
    public static readonly Guid AdminId = new Guid("65AC9DB3-BB0A-42fe-B584-A690FB0D817B");
    public static readonly Guid AdvancedHomePageId = new Guid("509B6940-9948-47e6-A392-99E0CA873F65");
    public static readonly Guid AdvancedBacklogManagementId = new Guid("CEDD6BE8-B717-4a0a-8BFD-C4E9B4CAA071");
    public static readonly Guid AdvancedPortfolioBacklogManagementId = new Guid("F1026762-C08D-4de3-9C17-7587296F3CFE");
    public static readonly Guid EnterpriseFirstPartyExtensionsId = new Guid("2320EAAE-F172-408F-A8C6-1021F7BC7779");
    public static readonly Guid TestManagementForStakeholderId = new Guid("D104EA57-16EA-4191-9B60-160D664EE9A8");
    public static readonly Guid PlansId = new Guid("E0FB3E99-BEE8-4D80-BB71-5298FAD4CB15");
  }
}
