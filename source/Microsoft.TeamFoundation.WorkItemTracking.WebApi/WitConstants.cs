// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.WitConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  public class WitConstants
  {
    public class WorkItemTrackingWebConstants
    {
      public const string RestAreaId = "5264459E-E5E0-4BD8-B118-0985E68A4EC5";
      public static readonly Guid RestAreaGuid = new Guid("5264459E-E5E0-4BD8-B118-0985E68A4EC5");
      public static readonly Guid WorkItemArtifactKind = new Guid("E7626DBD-6075-416C-A31E-DFD48FE3CFDE");
      public const string RestAreaName = "wit";
      public const string DefaultPreviewApiVersion = "1.0-Preview.1";
      public const string DefaultReleaseApiVersion = "1.0-Preview.2";
      public const string JsonPatchMediaType = "application/json-patch+json";
      public const string InvariantUtcTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
      public const string ContentTypeHeaderKey = "Content-Type";
      public const string ProcessRestArea = "processes";
      public const string ApiResourceVersion = "WitApiResourceVersion";
    }

    public class WorkItemTrackingRestResources
    {
      public const string WorkItems = "workItems";
      public const string Template = "workItemTemplate";
      public const string Attachments = "attachments";
      public const string Updates = "updates";
      public const string Revisions = "revisions";
      public const string ClassificationNodes = "classificationNodes";
      public const string AreaStructure = "areas";
      public const string IterationStructure = "iterations";
      public const string RelationTypes = "workItemRelationTypes";
      public const string Queries = "queries";
      public const string TempQueries = "tempQueries";
      public const string QueryResults = "queryResults";
      public const string Wiql = "wiql";
      public const string Fields = "fields";
      public const string WorkItemTypes = "workItemTypes";
      public const string WorkItemTypeCategories = "workItemTypeCategories";
      public const string Provisioning = "provisioning";
      public const string WorkItemIcons = "workItemIcons";
      public const string Expand = "$expand";
    }

    public class WorkItemTrackingLocationIds
    {
      public static readonly Guid WorkItems = new Guid("72C7DDF8-2CDC-4F60-90CD-AB71C14A399B");
      public static readonly Guid WorkItemTemplate = new Guid("62D3D110-0047-428C-AD3C-4FE872C91C74");
      public static readonly Guid Queries = new Guid("A3F8E27F-B199-4C44-AE43-5FC7D33CDA25");
      public static readonly Guid QueriesByProjectAndQueryReference = new Guid("A67D190C-C41F-424B-814D-0E906F659301");
      public static readonly Guid QueriesByProjectAndQueryId = new Guid("A67D190C-C41F-424B-814D-0E906F659301");
      public static readonly Guid QueriesByProject = new Guid("65F04358-6AE7-4214-A368-EFF915A36CE4");
      public static readonly Guid QueryResults = new Guid("7A9F0F1F-DDB3-4C30-9F71-12536D9CAF29");
      public static readonly Guid WiqlWithProjectAndId = new Guid("625D18D7-3CB3-4910-836D-43FF8166F8FE");
      public static readonly Guid WiqlWithProject = new Guid("625D18D7-3CB3-4910-836D-43FF8166F8FE");
      public static readonly Guid WiqlWithId = new Guid("A02355F5-5F8A-4671-8E32-369D23AAC83D");
      public static readonly Guid Wiql = new Guid("1A9C53F7-F243-4447-B110-35EF023636E4");
      public static readonly Guid Attachments = new Guid("E07B5FA4-1499-494D-A496-64B860FD64FF");
      public static readonly Guid Updates = new Guid("6570BF97-D02C-4A91-8D93-3ABE9895B1A9");
      public static readonly Guid Revisions = new Guid("A00C85A5-80FA-4565-99C3-BCD2181434BB");
      public static readonly Guid ClassificationNodes = new Guid("5A172953-1B41-49D3-840A-33F79C3CE89F");
      public static readonly Guid RelationTypes = new Guid("F5D33BC9-5B49-4A3C-A9BD-F3CD46DD2165");
      public static readonly Guid Fields = new Guid("B51FD764-E5C2-4B9B-AAF7-3395CF4BDD94");
      public static readonly Guid WorkItemTypes = new Guid("7C8D7A76-4A09-43E8-B5DF-BD792F4AC6AA");
      public static readonly Guid WorkItemTypeCategories = new Guid("9B9F5734-36C8-415E-BA67-F83B45C31408");
      public static readonly Guid WorkItemTypeTemplate = new Guid("8637AC8B-5EB6-4F90-B3F7-4F2FF576A459");
      public static readonly Guid Provisioning = new Guid("38C19D29-6F06-4FD7-B93E-20C97B32EBEB");
      public static readonly Guid RuleEngine = new Guid("1A3A1536-DCA6-4509-B9C3-DD9BB2981506");
      public static readonly Guid Process = new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af");
      public static readonly Guid WorkItemIcons = new Guid("4e1eb4a5-1970-4228-a682-ec48eb2dca30");
      public static readonly Guid WorkItemStateColors = new Guid("0b83df8a-3496-4ddb-ba44-63634f4cda61");
    }

    public class QueryParameters
    {
      public const string Fields = "fields";
      public const string AsOf = "asOf";
      public const string Expand = "$expand";
      public const string Ids = "ids";
      public const string Depth = "$depth";
      public const string Project = "project";
      public const string Top = "$top";
      public const string Skip = "$skip";
      public const string Area = "area";
      public const string FileName = "fileName";
      public const string ApiVersion = "api-version";
      public const string IncludeDeleted = "$includeDeleted";
      public const string UndeleteDescendants = "$undeleteDescendants";
      public const string ExportGlobalLists = "exportGlobalLists";
    }

    public class SecurityConstants
    {
      public static readonly Guid CommonStructureNodeSecurityNamespaceGuid = new Guid("83E28AD4-2D72-4ceb-97B0-C7726D5502C3");
      public const int WorkItemReadPermission = 16;
      public static readonly Guid IterationNodeSecurityNamespaceGuid = new Guid("BF7BFA03-B2B7-47db-8113-FA2E002CC5B1");
      public static readonly Guid QueryItemSecurityNamespaceGuid = new Guid("71356614-AAD7-4757-8F2C-0FB3BFF6F680");
      public const int CrossProjectQueryPermission = 32;
      public const string QueryItemRootToken = "$";
      public static readonly Guid WorkItemTrackingNamespaceId = new Guid("73E71C45-D483-40D5-BDBA-62FD076F7F87");
      public const string WorkItemTrackingRootToken = "WorkItemTracking";
      public static readonly string WorkItemTrackingArtifactUriQueryToken = "/WorkItemTracking/ArtifactUriQuery";
      public const int WorkItemTrackingReadAcrossProjectPermission = 2;

      public string GetRootClassificationNodeToken(Guid nodeId) => "vstfs:///Classification/Node/" + nodeId.ToString();

      public string GetSharedQueriesFolderToken(Guid projectId, Guid sharedQueriesFolderId) => "$/" + projectId.ToString() + "/" + sharedQueriesFolderId.ToString();

      public class AuthorizationCssNodePermissions
      {
        public const int None = 0;
        public const int GenericRead = 1;
        public const int GenericWrite = 2;
        public const int CreateChildren = 4;
        public const int Delete = 8;
        public const int WorkItemRead = 16;
        public const int WorkItemWrite = 32;
        public const int ManageTestPlans = 64;
        public const int ManageTestSuites = 128;
        public const int ViewEmailAddress = 256;
        public const int AllPermissions = 511;
      }

      public static class AuthorizationIterationNodePermissions
      {
        public static readonly int GenericRead = 1;
        public static readonly int GenericWrite = 2;
        public static readonly int CreateChildren = 4;
        public static readonly int Delete = 8;
        public static readonly int AllPermissions = WitConstants.SecurityConstants.AuthorizationIterationNodePermissions.GenericRead | WitConstants.SecurityConstants.AuthorizationIterationNodePermissions.GenericWrite | WitConstants.SecurityConstants.AuthorizationIterationNodePermissions.CreateChildren | WitConstants.SecurityConstants.AuthorizationIterationNodePermissions.Delete;
      }

      public static class QueryItemPermissions
      {
        public static readonly int None = 0;
        public static readonly int Read = 1;
        public static readonly int Contribute = 2;
        public static readonly int Delete = 4;
        public static readonly int ManagePermissions = 8;
        public static readonly int FullControl = 16;
        public static readonly int CrossProjectExecution = 32;
        public static readonly int RecordQueryExecutionInfo = 64;
        public static readonly int ReadContribute = WitConstants.SecurityConstants.QueryItemPermissions.Read | WitConstants.SecurityConstants.QueryItemPermissions.Contribute;
        public static readonly int ReadContributeDelete = WitConstants.SecurityConstants.QueryItemPermissions.Read | WitConstants.SecurityConstants.QueryItemPermissions.Contribute | WitConstants.SecurityConstants.QueryItemPermissions.Delete;
        public static readonly int ReadManage = WitConstants.SecurityConstants.QueryItemPermissions.Read | WitConstants.SecurityConstants.QueryItemPermissions.ManagePermissions;
        public static readonly int AllPermissions = WitConstants.SecurityConstants.QueryItemPermissions.Read | WitConstants.SecurityConstants.QueryItemPermissions.Contribute | WitConstants.SecurityConstants.QueryItemPermissions.Delete | WitConstants.SecurityConstants.QueryItemPermissions.ManagePermissions | WitConstants.SecurityConstants.QueryItemPermissions.FullControl | WitConstants.SecurityConstants.QueryItemPermissions.CrossProjectExecution | WitConstants.SecurityConstants.QueryItemPermissions.RecordQueryExecutionInfo;
      }
    }

    public class CreateTeamProjectConstants
    {
      public const string DelayQueryProvision = "DelayQueryProvision";
    }

    public static class AzureBoardsDeploymentStatus
    {
      public static readonly string Pending = "pending";
      public static readonly string InProgress = "in_progress";
      public static readonly string Successful = "successful";
      public static readonly string Canceled = "canceled";
      public static readonly string Failed = "failed";
      public static readonly string RolledBack = "rolled_back";
      public static readonly string Unknown = "unknown";
    }
  }
}
