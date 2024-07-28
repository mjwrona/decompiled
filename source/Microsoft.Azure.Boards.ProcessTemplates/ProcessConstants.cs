// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessConstants
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public static class ProcessConstants
  {
    public const string Area = "ProcessTemplate";
    private const string c_processRegistryPathRoot = "/Service/Process/Settings/";
    public const string ProcessLimitRegistryPath = "/Service/Process/Settings/ProcessLimit";
    public const string FileSizeLimitRegistryPath = "/Service/Process/Settings/MaxProcessTemplateFileSize";
    public const int DefaultProcessLimit = 128;
    public const int DefaultTemplateFileSizeLimit = 20000000;
    public static readonly int MaxDescriptionLength = 1024;
    public static readonly int MaxNameLength = 256;
    public static readonly int MaxReferenceNameLength = 257;
    public static readonly char[] IllegalProcessNameChars = new char[25]
    {
      ',',
      ';',
      '\'',
      '~',
      ':',
      '/',
      '\\',
      '*',
      '|',
      '?',
      '"',
      '&',
      '%',
      '$',
      '!',
      '+',
      '=',
      '(',
      ')',
      '[',
      ']',
      '{',
      '}',
      '<',
      '>'
    };
    public static readonly char[] IllegalProcessRefNameChars = new char[27]
    {
      ' ',
      '-',
      ',',
      ';',
      '\'',
      '~',
      ':',
      '/',
      '\\',
      '*',
      '|',
      '?',
      '"',
      '&',
      '%',
      '$',
      '!',
      '+',
      '=',
      '(',
      ')',
      '[',
      ']',
      '{',
      '}',
      '<',
      '>'
    };
    public static readonly IReadOnlyDictionary<Guid, List<string>> OobProcessWorkItemTypeMap = (IReadOnlyDictionary<Guid, List<string>>) new Dictionary<Guid, List<string>>()
    {
      {
        ProcessTemplateTypeIdentifiers.VisualStudioScrum,
        new List<string>()
        {
          "microsoft.vsts.workitemtypes.bug",
          "microsoft.vsts.workitemtypes.codereviewrequest",
          "microsoft.vsts.workitemtypes.codereviewresponse",
          "microsoft.vsts.workitemtypes.epic",
          "microsoft.vsts.workitemtypes.feature",
          "microsoft.vsts.workitemtypes.feedbackrequest",
          "microsoft.vsts.workitemtypes.feedbackresponse",
          "microsoft.vsts.workitemtypes.impediment",
          "microsoft.vsts.workitemtypes.productbacklogitem",
          "microsoft.vsts.workitemtypes.sharedparameter",
          "microsoft.vsts.workitemtypes.sharedstep",
          "microsoft.vsts.workitemtypes.task",
          "microsoft.vsts.workitemtypes.testcase",
          "microsoft.vsts.workitemtypes.testplan",
          "microsoft.vsts.workitemtypes.testsuite"
        }
      },
      {
        ProcessTemplateTypeIdentifiers.MsfCmmiProcessImprovement,
        new List<string>()
        {
          "microsoft.vsts.workitemtypes.bug",
          "microsoft.vsts.workitemtypes.changerequest",
          "microsoft.vsts.workitemtypes.codereviewrequest",
          "microsoft.vsts.workitemtypes.codereviewresponse",
          "microsoft.vsts.workitemtypes.epic",
          "microsoft.vsts.workitemtypes.feature",
          "microsoft.vsts.workitemtypes.feedbackrequest",
          "microsoft.vsts.workitemtypes.feedbackresponse",
          "microsoft.vsts.workitemtypes.issue",
          "microsoft.vsts.workitemtypes.requirement",
          "microsoft.vsts.workitemtypes.review",
          "microsoft.vsts.workitemtypes.risk",
          "microsoft.vsts.workitemtypes.sharedparameter",
          "microsoft.vsts.workitemtypes.sharedstep",
          "microsoft.vsts.workitemtypes.task",
          "microsoft.vsts.workitemtypes.testcase",
          "microsoft.vsts.workitemtypes.testplan",
          "microsoft.vsts.workitemtypes.testsuite"
        }
      },
      {
        ProcessTemplateTypeIdentifiers.MsfAgileSoftwareDevelopment,
        new List<string>()
        {
          "microsoft.vsts.workitemtypes.bug",
          "microsoft.vsts.workitemtypes.codereviewrequest",
          "microsoft.vsts.workitemtypes.codereviewresponse",
          "microsoft.vsts.workitemtypes.epic",
          "microsoft.vsts.workitemtypes.feature",
          "microsoft.vsts.workitemtypes.feedbackrequest",
          "microsoft.vsts.workitemtypes.feedbackresponse",
          "microsoft.vsts.workitemtypes.issue",
          "microsoft.vsts.workitemtypes.sharedparameter",
          "microsoft.vsts.workitemtypes.sharedstep",
          "microsoft.vsts.workitemtypes.task",
          "microsoft.vsts.workitemtypes.testcase",
          "microsoft.vsts.workitemtypes.testplan",
          "microsoft.vsts.workitemtypes.testsuite",
          "microsoft.vsts.workitemtypes.userstory"
        }
      },
      {
        ProcessTemplateTypeIdentifiers.MsfHydroProcess,
        new List<string>()
        {
          "microsoft.vsts.workitemtypes.issue",
          "microsoft.vsts.workitemtypes.epic",
          "microsoft.vsts.workitemtypes.task",
          "microsoft.vsts.workitemtypes.testcase",
          "microsoft.vsts.workitemtypes.testplan",
          "microsoft.vsts.workitemtypes.testsuite",
          "microsoft.vsts.workitemtypes.sharedparameter",
          "microsoft.vsts.workitemtypes.sharedstep",
          "microsoft.vsts.workitemtypes.codereviewrequest",
          "microsoft.vsts.workitemtypes.codereviewresponse",
          "microsoft.vsts.workitemtypes.feedbackrequest",
          "microsoft.vsts.workitemtypes.feedbackresponse"
        }
      }
    };
    public const int LastUsed = 10005024;
    public static readonly string ProcessSecurityTokenSeparator = ":";
    public const string ProcessUploadFeatureFlag = "WebAccess.Process.ProcessUpload";
    public const string ProcessHierarchyFeatureFlag = "WebAccess.Process.Hierarchy";
    public const string UnblockWarehouseForInheritedCustomizationCollection = "WorkItemTracking.Server.UnblockWarehouseForInheritedCustomizationCollection";
    public const string XmlTemplateProcess = "WebAccess.Process.XmlTemplateProcess";
    public const string PrecreateProjectProcessTemplateTypeRegistryPath = "/Service/WorkItemTracking/Settings/PreCreateProjectProcessTemplateType";
    public const string CheckForProcessReadinessDisabledFeatureFlag = "WebAccess.Process.CheckForProcessReadinessDisabled";
    public static readonly Guid SystemProcessId = new Guid("D0171D51-FF84-4038-AFC1-8800AB613160");
    public static readonly Guid ProcessTokenSuffix = new Guid("D920DDBB-6F0D-4E10-88AF-3F12AB91326E");
    public static readonly IReadOnlyCollection<string> ReservedProcessNames = (IReadOnlyCollection<string>) new string[2]
    {
      "System",
      "Microsoft"
    };

    public static class Layers
    {
      public const string ProcessService = "TeamFoundationProcessService";
    }

    public static class ProcessProperties
    {
      public const string IsDefault = "IsDefault";
      public const string IsEnabled = "IsEnabled";
    }

    public static class CIConstants
    {
      public const string UpdateEnabledProcessProperty = "UpdateEnabledProcessProperty";
      public const string MigrateProject = "MigratedProject";
      public const string AddInheritedProcess = "AddInheritedProcess";
      public const string RemoveUnsupportedPlugins = "RemoveUnsupportedPlugins";
      public const string Disabled = "Disabled";
      public const string Enabled = "Enabled";
      public const string OldTemplateId = "OldTemplateId";
      public const string OldTemplateName = "OldTemplateName";
      public const string OldTemplateType = "OldTemplateType";
      public const string NewTemplateId = "NewTemplateId";
      public const string NewTemplateName = "NewTemplateName";
      public const string NewTemplateType = "NewTemplateType";
      public const string ProcessId = "ProcessId";
      public const string ProcessName = "ProcessName";
      public const string ParentId = "ParentId";
      public const string ParentName = "ParentName";
      public const string ProjectId = "ProjectId";
      public const string ProjectName = "ProjectName";
    }

    public static class Notifications
    {
      public static readonly Guid Reset = new Guid("A423F515-CFB3-4F62-830B-AC0FEA937F61");
      public static readonly Guid ProcessAdded = new Guid("07B60C22-C4E2-477E-94C3-17EDF11FE117");
      public static readonly Guid ProcessChanged = new Guid("D5BA6EDA-0BC8-4A52-BF6B-0E856B88D941");
      public static readonly Guid ProcessDeleted = new Guid("E4639DFB-21A8-43C0-9A5A-67C96A3C716C");
      public static readonly Guid DefaultProcessChanged = new Guid("169427D9-08F5-4F99-B456-D938F1F483B3");
      public static readonly Guid ProcessEnabledDisabled = new Guid("E49E23BD-5722-4199-9A15-034945AC3D6D");
    }
  }
}
