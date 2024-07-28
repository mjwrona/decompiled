// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.Compat2010Helper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  internal class Compat2010Helper
  {
    private static List<TOut> Convert<TIn, TOut>(IEnumerable<TIn> inList, Func<TIn, TOut> converter)
    {
      if (inList == null)
        return (List<TOut>) null;
      List<TOut> outList = new List<TOut>(inList.Count<TIn>());
      outList.AddRange(inList.Select<TIn, TOut>((Func<TIn, TOut>) (x => converter(x))));
      return outList;
    }

    private static TOut[] ConvertToArray<TIn, TOut>(
      IEnumerable<TIn> inList,
      Func<TIn, TOut> converter)
    {
      if (inList == null)
        return (TOut[]) null;
      TOut[] array = new TOut[inList.Count<TIn>()];
      int index = 0;
      foreach (TIn @in in inList)
      {
        array[index] = converter(@in);
        ++index;
      }
      return array;
    }

    private static void Convert<TIn, TOut>(
      IEnumerable<TIn> inList,
      List<TOut> outList,
      Func<TIn, TOut> converter)
    {
      if (outList == null)
        return;
      outList.Clear();
      if (inList == null)
        return;
      outList.AddRange(inList.Select<TIn, TOut>((Func<TIn, TOut>) (x => converter(x))));
    }

    internal static TestResultAttachment Convert(Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment attachment)
    {
      if (attachment == null)
        return (TestResultAttachment) null;
      return new TestResultAttachment()
      {
        ActionPath = attachment.ActionPath,
        AttachmentType = attachment.AttachmentType,
        Comment = attachment.Comment,
        CreationDate = attachment.CreationDate,
        FileName = attachment.FileName,
        Id = attachment.Id,
        IsComplete = attachment.IsComplete,
        IterationId = attachment.IterationId,
        Length = attachment.Length,
        TestResultId = attachment.TestResultId,
        TestRunId = attachment.TestRunId,
        TmiRunId = attachment.TmiRunId
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment Convert(
      TestResultAttachment attachment)
    {
      if (attachment == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment()
      {
        ActionPath = attachment.ActionPath,
        AttachmentType = attachment.AttachmentType,
        Comment = attachment.Comment,
        CreationDate = attachment.CreationDate,
        FileName = attachment.FileName,
        Id = attachment.Id,
        IsComplete = attachment.IsComplete,
        IterationId = attachment.IterationId,
        Length = attachment.Length,
        TestResultId = attachment.TestResultId,
        TestRunId = attachment.TestRunId,
        TmiRunId = attachment.TmiRunId
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[] Convert(
      TestResultAttachment[] attachments)
    {
      return Compat2010Helper.ConvertToArray<TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>((IEnumerable<TestResultAttachment>) attachments, (Func<TestResultAttachment, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestResultAttachment> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, TestResultAttachment>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, TestResultAttachment>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.DefaultAfnStripBinding Convert(
      DefaultAfnStripBinding afnStripBinding)
    {
      if (afnStripBinding == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.DefaultAfnStripBinding) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.DefaultAfnStripBinding()
      {
        TestCaseId = afnStripBinding.TestCaseId,
        TestResultId = afnStripBinding.TestResultId,
        TestRunId = afnStripBinding.TestRunId
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.DefaultAfnStripBinding[] Convert(
      DefaultAfnStripBinding[] afnStripBindingList)
    {
      return Compat2010Helper.ConvertToArray<DefaultAfnStripBinding, Microsoft.TeamFoundation.TestManagement.Server.DefaultAfnStripBinding>((IEnumerable<DefaultAfnStripBinding>) afnStripBindingList, (Func<DefaultAfnStripBinding, Microsoft.TeamFoundation.TestManagement.Server.DefaultAfnStripBinding>) (x => Compat2010Helper.Convert(x)));
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.BugFieldMapping Convert(
      BugFieldMapping bugFieldMapping)
    {
      if (bugFieldMapping == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.BugFieldMapping) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.BugFieldMapping()
      {
        CreatedBy = bugFieldMapping.CreatedBy,
        CreatedDate = bugFieldMapping.CreatedDate,
        FieldMapping = bugFieldMapping.FieldMapping,
        LastUpdated = bugFieldMapping.LastUpdated,
        LastUpdatedBy = bugFieldMapping.LastUpdatedBy,
        Revision = bugFieldMapping.Revision
      };
    }

    internal static BugFieldMapping Convert(Microsoft.TeamFoundation.TestManagement.Server.BugFieldMapping bugFieldMapping)
    {
      if (bugFieldMapping == null)
        return (BugFieldMapping) null;
      return new BugFieldMapping()
      {
        CreatedBy = bugFieldMapping.CreatedBy,
        CreatedDate = bugFieldMapping.CreatedDate,
        FieldMapping = bugFieldMapping.FieldMapping,
        LastUpdated = bugFieldMapping.LastUpdated,
        LastUpdatedBy = bugFieldMapping.LastUpdatedBy,
        Revision = bugFieldMapping.Revision
      };
    }

    internal static UpdatedProperties Convert(Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties updatedProperties)
    {
      if (updatedProperties == null)
        return (UpdatedProperties) null;
      if (updatedProperties is Microsoft.TeamFoundation.TestManagement.Server.BlockedPointProperties)
        return (UpdatedProperties) Compat2010Helper.Convert(updatedProperties as Microsoft.TeamFoundation.TestManagement.Server.BlockedPointProperties);
      return new UpdatedProperties()
      {
        Id = updatedProperties.Id,
        LastUpdated = updatedProperties.LastUpdated,
        LastUpdatedBy = updatedProperties.LastUpdatedBy,
        Revision = updatedProperties.Revision
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties Convert(
      UpdatedProperties updatedProperties)
    {
      if (updatedProperties == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties()
      {
        Id = updatedProperties.Id,
        LastUpdated = updatedProperties.LastUpdated,
        LastUpdatedBy = updatedProperties.LastUpdatedBy,
        Revision = updatedProperties.Revision
      };
    }

    internal static UpdatedProperties[] Convert(Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties[] updatedProperties) => Compat2010Helper.ConvertToArray<Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties, UpdatedProperties>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties>) updatedProperties, (Func<Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties, UpdatedProperties>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery Convert(
      ResultsStoreQuery query)
    {
      if (query == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery()
      {
        DayPrecision = query.DayPrecision,
        QueryText = query.QueryText,
        TeamProjectName = query.TeamProjectName,
        TimeZone = query.TimeZone
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration Convert(
      TestConfiguration testConfiguration)
    {
      if (testConfiguration == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration testConfiguration1 = new Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration()
      {
        AreaPath = testConfiguration.AreaPath,
        Description = testConfiguration.Description,
        Id = testConfiguration.Id,
        IsDefault = testConfiguration.IsDefault,
        LastUpdated = testConfiguration.LastUpdated,
        LastUpdatedBy = testConfiguration.LastUpdatedBy,
        Name = testConfiguration.Name,
        Revision = testConfiguration.Revision,
        State = testConfiguration.State
      };
      Compat2010Helper.Convert<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>((IEnumerable<NameValuePair>) testConfiguration.Values, testConfiguration1.Values, (Func<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return testConfiguration1;
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.NameValuePair Convert(
      NameValuePair x)
    {
      if (x == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.NameValuePair) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.NameValuePair()
      {
        Name = x.Name,
        Value = x.Value
      };
    }

    internal static TestConfiguration Convert(Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration testConfiguration)
    {
      if (testConfiguration == null)
        return (TestConfiguration) null;
      TestConfiguration testConfiguration1 = new TestConfiguration()
      {
        AreaPath = testConfiguration.AreaPath,
        Description = testConfiguration.Description,
        Id = testConfiguration.Id,
        IsDefault = testConfiguration.IsDefault,
        LastUpdated = testConfiguration.LastUpdated,
        LastUpdatedBy = testConfiguration.LastUpdatedBy,
        Name = testConfiguration.Name,
        Revision = testConfiguration.Revision,
        State = testConfiguration.State
      };
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) testConfiguration.Values, testConfiguration1.Values, (Func<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return testConfiguration1;
    }

    private static NameValuePair Convert(Microsoft.TeamFoundation.TestManagement.Server.NameValuePair x)
    {
      if (x == null)
        return (NameValuePair) null;
      return new NameValuePair()
      {
        Name = x.Name,
        Value = x.Value
      };
    }

    internal static List<TestConfiguration> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration, TestConfiguration>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration, TestConfiguration>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestVariable Convert(
      TestVariable variable)
    {
      if (variable == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestVariable) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestVariable testVariable = new Microsoft.TeamFoundation.TestManagement.Server.TestVariable();
      testVariable.Description = variable.Description;
      testVariable.Id = variable.Id;
      testVariable.Name = variable.Name;
      testVariable.Revision = variable.Revision;
      testVariable.Values.AddRange((IEnumerable<string>) variable.Values);
      return testVariable;
    }

    internal static TestVariable Convert(Microsoft.TeamFoundation.TestManagement.Server.TestVariable variable)
    {
      if (variable == null)
        return (TestVariable) null;
      TestVariable testVariable = new TestVariable();
      testVariable.Description = variable.Description;
      testVariable.Id = variable.Id;
      testVariable.Name = variable.Name;
      testVariable.Revision = variable.Revision;
      testVariable.Values.AddRange((IEnumerable<string>) variable.Values);
      return testVariable;
    }

    internal static List<TestVariable> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestVariable> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestVariable, TestVariable>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestVariable>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestVariable, TestVariable>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestSettings Convert(
      TestSettings settings)
    {
      if (settings == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestSettings) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestSettings()
      {
        AreaPath = settings.AreaPath,
        CreatedBy = settings.CreatedBy,
        CreatedDate = settings.CreatedDate,
        Description = settings.Description,
        Id = settings.Id,
        IsAutomated = settings.IsAutomated,
        IsPublic = settings.IsPublic,
        LastUpdated = settings.LastUpdated,
        LastUpdatedBy = settings.LastUpdatedBy,
        MachineRoles = Compat2010Helper.Convert(settings.MachineRoles),
        Name = settings.Name,
        Revision = settings.Revision,
        Settings = settings.Settings
      };
    }

    internal static TestSettings Convert(Microsoft.TeamFoundation.TestManagement.Server.TestSettings settings)
    {
      if (settings == null)
        return (TestSettings) null;
      return new TestSettings()
      {
        AreaPath = settings.AreaPath,
        CreatedBy = settings.CreatedBy,
        CreatedDate = settings.CreatedDate,
        Description = settings.Description,
        Id = settings.Id,
        IsAutomated = settings.IsAutomated,
        IsPublic = settings.IsPublic,
        LastUpdated = settings.LastUpdated,
        LastUpdatedBy = settings.LastUpdatedBy,
        MachineRoles = Compat2010Helper.Convert(settings.MachineRoles),
        Name = settings.Name,
        Revision = settings.Revision,
        Settings = settings.Settings
      };
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole Convert(
      TestSettingsMachineRole testSettingsMachineRole)
    {
      if (testSettingsMachineRole == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole()
      {
        IsExecution = testSettingsMachineRole.IsExecution,
        Name = testSettingsMachineRole.Name
      };
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole[] Convert(
      TestSettingsMachineRole[] testSettingsMachineRole)
    {
      return Compat2010Helper.ConvertToArray<TestSettingsMachineRole, Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>((IEnumerable<TestSettingsMachineRole>) testSettingsMachineRole, (Func<TestSettingsMachineRole, Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>) (x => Compat2010Helper.Convert(x)));
    }

    private static TestSettingsMachineRole Convert(Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole testSettingsMachineRole)
    {
      if (testSettingsMachineRole == null)
        return (TestSettingsMachineRole) null;
      return new TestSettingsMachineRole()
      {
        IsExecution = testSettingsMachineRole.IsExecution,
        Name = testSettingsMachineRole.Name
      };
    }

    private static TestSettingsMachineRole[] Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole[] testSettingsMachineRole)
    {
      return Compat2010Helper.ConvertToArray<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole, TestSettingsMachineRole>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole>) testSettingsMachineRole, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestSettingsMachineRole, TestSettingsMachineRole>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestSettings> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestSettings> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestSettings, TestSettings>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSettings>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestSettings, TestSettings>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState Convert(
      TestResolutionState resolutionState)
    {
      if (resolutionState == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState()
      {
        Id = resolutionState.Id,
        Name = resolutionState.Name,
        TeamProject = resolutionState.TeamProject
      };
    }

    internal static TestResolutionState Convert(Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState resolutionState)
    {
      if (resolutionState == null)
        return (TestResolutionState) null;
      return new TestResolutionState()
      {
        Id = resolutionState.Id,
        Name = resolutionState.Name,
        TeamProject = resolutionState.TeamProject
      };
    }

    internal static List<TestResolutionState> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState, TestResolutionState>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState, TestResolutionState>) (x => Compat2010Helper.Convert(x)));

    internal static DataCollectorInformation Convert(
      Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation dataCollectorInformation)
    {
      if (dataCollectorInformation == null)
        return (DataCollectorInformation) null;
      DataCollectorInformation collectorInformation = new DataCollectorInformation()
      {
        ConfigurationEditorConfiguration = dataCollectorInformation.ConfigurationEditorConfiguration,
        DefaultConfiguration = dataCollectorInformation.DefaultConfiguration,
        Description = dataCollectorInformation.Description,
        Id = dataCollectorInformation.Id,
        TypeUri = dataCollectorInformation.TypeUri
      };
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) dataCollectorInformation.Properties, collectorInformation.Properties, (Func<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return collectorInformation;
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation Convert(
      DataCollectorInformation dataCollectorInformation)
    {
      if (dataCollectorInformation == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation) null;
      Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation collectorInformation = new Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation()
      {
        ConfigurationEditorConfiguration = dataCollectorInformation.ConfigurationEditorConfiguration,
        DefaultConfiguration = dataCollectorInformation.DefaultConfiguration,
        Description = dataCollectorInformation.Description,
        Id = dataCollectorInformation.Id,
        TypeUri = dataCollectorInformation.TypeUri
      };
      Compat2010Helper.Convert<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>((IEnumerable<NameValuePair>) dataCollectorInformation.Properties, collectorInformation.Properties, (Func<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return collectorInformation;
    }

    internal static List<Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation> Convert(
      List<DataCollectorInformation> collectors)
    {
      return Compat2010Helper.Convert<DataCollectorInformation, Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation>((IEnumerable<DataCollectorInformation>) collectors, (Func<DataCollectorInformation, Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<DataCollectorInformation> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation> collectors) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation, DataCollectorInformation>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation>) collectors, (Func<Microsoft.TeamFoundation.TestManagement.Server.DataCollectorInformation, DataCollectorInformation>) (x => Compat2010Helper.Convert(x)));

    internal static TestController Convert(Microsoft.TeamFoundation.TestManagement.Server.TestController testController)
    {
      if (testController == null)
        return (TestController) null;
      TestController testController1 = new TestController()
      {
        Description = testController.Description,
        GroupId = testController.GroupId,
        Name = testController.Name
      };
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) testController.Properties, testController1.Properties, (Func<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return testController1;
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestController Convert(
      TestController testController)
    {
      if (testController == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestController) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestController testController1 = new Microsoft.TeamFoundation.TestManagement.Server.TestController()
      {
        Description = testController.Description,
        GroupId = testController.GroupId,
        Name = testController.Name
      };
      Compat2010Helper.Convert<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>((IEnumerable<NameValuePair>) testController.Properties, testController1.Properties, (Func<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return testController1;
    }

    internal static List<Microsoft.TeamFoundation.TestManagement.Server.TestController> Convert(
      List<TestController> controllers)
    {
      return Compat2010Helper.Convert<TestController, Microsoft.TeamFoundation.TestManagement.Server.TestController>((IEnumerable<TestController>) controllers, (Func<TestController, Microsoft.TeamFoundation.TestManagement.Server.TestController>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestController> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestController> controllers) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestController, TestController>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestController>) controllers, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestController, TestController>) (x => Compat2010Helper.Convert(x)));

    internal static TestEnvironment Convert(Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment testEnvironment)
    {
      if (testEnvironment == null)
        return (TestEnvironment) null;
      TestEnvironment testEnvironment1 = new TestEnvironment()
      {
        ControllerDisplayName = testEnvironment.ControllerDisplayName,
        ControllerName = testEnvironment.ControllerName,
        Description = testEnvironment.Description,
        Id = testEnvironment.Id,
        Name = testEnvironment.Name,
        ProjectName = testEnvironment.ProjectName
      };
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.MachineRole, MachineRole>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.MachineRole>) testEnvironment.MachineRoles, testEnvironment1.MachineRoles, (Func<Microsoft.TeamFoundation.TestManagement.Server.MachineRole, MachineRole>) (x => Compat2010Helper.Convert(x)));
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) testEnvironment.Properties, testEnvironment1.Properties, (Func<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair, NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return testEnvironment1;
    }

    private static MachineRole Convert(Microsoft.TeamFoundation.TestManagement.Server.MachineRole x)
    {
      if (x == null)
        return (MachineRole) null;
      return new MachineRole() { Id = x.Id, Name = x.Name };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment Convert(
      TestEnvironment testEnvironment)
    {
      if (testEnvironment == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment testEnvironment1 = new Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment()
      {
        ControllerDisplayName = testEnvironment.ControllerDisplayName,
        ControllerName = testEnvironment.ControllerName,
        Description = testEnvironment.Description,
        Id = testEnvironment.Id,
        Name = testEnvironment.Name,
        ProjectName = testEnvironment.ProjectName
      };
      Compat2010Helper.Convert<MachineRole, Microsoft.TeamFoundation.TestManagement.Server.MachineRole>((IEnumerable<MachineRole>) testEnvironment.MachineRoles, testEnvironment1.MachineRoles, (Func<MachineRole, Microsoft.TeamFoundation.TestManagement.Server.MachineRole>) (x => Compat2010Helper.Convert(x)));
      Compat2010Helper.Convert<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>((IEnumerable<NameValuePair>) testEnvironment.Properties, testEnvironment1.Properties, (Func<NameValuePair, Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) (x => Compat2010Helper.Convert(x)));
      return testEnvironment1;
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.MachineRole Convert(MachineRole x)
    {
      if (x == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.MachineRole) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.MachineRole() { Id = x.Id, Name = x.Name };
    }

    internal static List<Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment> Convert(
      List<TestEnvironment> environments)
    {
      return Compat2010Helper.Convert<TestEnvironment, Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment>((IEnumerable<TestEnvironment>) environments, (Func<TestEnvironment, Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestEnvironment> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment> environments) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment, TestEnvironment>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment>) environments, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestEnvironment, TestEnvironment>) (x => Compat2010Helper.Convert(x)));

    internal static BuildCoverage Convert(Microsoft.TeamFoundation.TestManagement.Server.BuildCoverage coverage)
    {
      if (coverage == null)
        return (BuildCoverage) null;
      BuildCoverage buildCoverage1 = new BuildCoverage();
      buildCoverage1.Configuration = Compat2010Helper.Convert(coverage.Configuration);
      buildCoverage1.Id = coverage.Id;
      buildCoverage1.LastError = coverage.LastError;
      buildCoverage1.State = coverage.State;
      BuildCoverage buildCoverage2 = buildCoverage1;
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage, ModuleCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage>) coverage.Modules, buildCoverage2.Modules, (Func<Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage, ModuleCoverage>) (x => Compat2010Helper.Convert(x)));
      return buildCoverage2;
    }

    private static ModuleCoverage Convert(Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage coverage)
    {
      if (coverage == null)
        return (ModuleCoverage) null;
      ModuleCoverage moduleCoverage = new ModuleCoverage()
      {
        BlockCount = coverage.BlockCount,
        BlockData = coverage.BlockData,
        CoverageId = coverage.CoverageId,
        ModuleId = coverage.ModuleId,
        Name = coverage.Name,
        Signature = coverage.Signature,
        SignatureAge = coverage.SignatureAge,
        Statistics = Compat2010Helper.Convert(coverage.Statistics)
      };
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.FunctionCoverage, FunctionCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.FunctionCoverage>) coverage.Functions, moduleCoverage.Functions, (Func<Microsoft.TeamFoundation.TestManagement.Server.FunctionCoverage, FunctionCoverage>) (x => Compat2010Helper.Convert(x)));
      return moduleCoverage;
    }

    private static FunctionCoverage Convert(Microsoft.TeamFoundation.TestManagement.Server.FunctionCoverage coverage)
    {
      if (coverage == null)
        return (FunctionCoverage) null;
      return new FunctionCoverage()
      {
        Class = coverage.Class,
        CoverageId = coverage.CoverageId,
        FunctionId = coverage.FunctionId,
        ModuleId = coverage.ModuleId,
        Name = coverage.Name,
        Namespace = coverage.Namespace,
        SourceFile = coverage.SourceFile,
        Statistics = Compat2010Helper.Convert(coverage.Statistics)
      };
    }

    private static CoverageStatistics Convert(Microsoft.TeamFoundation.TestManagement.Server.CoverageStatistics coverageStatistics)
    {
      if (coverageStatistics == null)
        return (CoverageStatistics) null;
      return new CoverageStatistics()
      {
        BlocksCovered = coverageStatistics.BlocksCovered,
        BlocksNotCovered = coverageStatistics.BlocksNotCovered,
        LinesCovered = coverageStatistics.LinesCovered,
        LinesNotCovered = coverageStatistics.LinesNotCovered,
        LinesPartiallyCovered = coverageStatistics.LinesPartiallyCovered
      };
    }

    internal static List<BuildCoverage> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.BuildCoverage> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.BuildCoverage, BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.BuildCoverage>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.BuildCoverage, BuildCoverage>) (x => Compat2010Helper.Convert(x)));

    internal static TestRunCoverage Convert(Microsoft.TeamFoundation.TestManagement.Server.TestRunCoverage coverage)
    {
      if (coverage == null)
        return (TestRunCoverage) null;
      TestRunCoverage testRunCoverage1 = new TestRunCoverage();
      testRunCoverage1.Id = coverage.Id;
      testRunCoverage1.LastError = coverage.LastError;
      testRunCoverage1.State = coverage.State;
      testRunCoverage1.TestRunId = coverage.TestRunId;
      TestRunCoverage testRunCoverage2 = testRunCoverage1;
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage, ModuleCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage>) coverage.Modules, testRunCoverage2.Modules, (Func<Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage, ModuleCoverage>) (x => Compat2010Helper.Convert(x)));
      return testRunCoverage2;
    }

    internal static List<TestRunCoverage> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestRunCoverage> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestRunCoverage, TestRunCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRunCoverage>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRunCoverage, TestRunCoverage>) (x => Compat2010Helper.Convert(x)));

    internal static ImpactedPoint Convert(Microsoft.TeamFoundation.TestManagement.Server.ImpactedPoint point)
    {
      if (point == null)
        return (ImpactedPoint) null;
      return new ImpactedPoint()
      {
        BuildUri = point.BuildUri,
        Confidence = point.Confidence,
        PointId = point.PointId,
        State = point.State,
        SuiteName = point.SuiteName,
        TestCaseId = point.TestCaseId
      };
    }

    internal static List<ImpactedPoint> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.ImpactedPoint> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.ImpactedPoint, ImpactedPoint>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ImpactedPoint>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.ImpactedPoint, ImpactedPoint>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestPoint Convert(TestPoint point)
    {
      if (point == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestPoint) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestPoint()
      {
        AssignedTo = point.AssignedTo,
        Comment = point.Comment,
        ConfigurationId = point.ConfigurationId,
        ConfigurationName = point.ConfigurationName,
        FailureType = TestFailureType.GetFailureTypeFromId((int) point.FailureType),
        LastResolutionStateId = point.LastResolutionStateId,
        LastResultOutcome = point.LastResultOutcome,
        LastResultState = point.LastResultState,
        LastTestResultId = point.LastTestResultId,
        LastTestRunId = point.LastTestRunId,
        LastUpdated = point.LastUpdated,
        LastUpdatedBy = point.LastUpdatedBy,
        PlanId = point.PlanId,
        PointId = point.PointId,
        Revision = point.Revision,
        State = point.State,
        SuiteId = point.SuiteId,
        TestCaseId = point.TestCaseId
      };
    }

    private static TestPoint Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPoint point)
    {
      if (point == null)
        return (TestPoint) null;
      return new TestPoint()
      {
        AssignedTo = point.AssignedTo,
        Comment = point.Comment,
        ConfigurationId = point.ConfigurationId,
        ConfigurationName = point.ConfigurationName,
        FailureType = TestFailureType.GetFailureTypeFromId((int) point.FailureType),
        LastResolutionStateId = point.LastResolutionStateId,
        LastResultOutcome = Microsoft.TeamFoundation.TestManagement.Server.TestResult.ToPreDev12QU2Outcome(point.LastResultOutcome),
        LastResultState = point.LastResultState,
        LastTestResultId = point.LastTestResultId,
        LastTestRunId = point.LastTestRunId,
        LastUpdated = point.LastUpdated,
        LastUpdatedBy = point.LastUpdatedBy,
        PlanId = point.PlanId,
        PointId = point.PointId,
        Revision = point.Revision,
        State = point.State,
        SuiteId = point.SuiteId,
        TestCaseId = point.TestCaseId
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestPoint[] Convert(
      TestPoint[] points)
    {
      return Compat2010Helper.ConvertToArray<TestPoint, Microsoft.TeamFoundation.TestManagement.Server.TestPoint>((IEnumerable<TestPoint>) points, (Func<TestPoint, Microsoft.TeamFoundation.TestManagement.Server.TestPoint>) (x => Compat2010Helper.Convert(x)));
    }

    internal static BlockedPointProperties Convert(Microsoft.TeamFoundation.TestManagement.Server.BlockedPointProperties properties)
    {
      if (properties == null)
        return (BlockedPointProperties) null;
      BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
      blockedPointProperties.Id = properties.Id;
      blockedPointProperties.LastTestResultId = properties.LastTestResultId;
      blockedPointProperties.LastTestRunId = properties.LastTestRunId;
      blockedPointProperties.LastUpdated = properties.LastUpdated;
      blockedPointProperties.LastUpdatedBy = properties.LastUpdatedBy;
      blockedPointProperties.Revision = properties.Revision;
      return blockedPointProperties;
    }

    internal static BlockedPointProperties[] Convert(Microsoft.TeamFoundation.TestManagement.Server.BlockedPointProperties[] blockedPointProperties) => Compat2010Helper.ConvertToArray<Microsoft.TeamFoundation.TestManagement.Server.BlockedPointProperties, BlockedPointProperties>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.BlockedPointProperties>) blockedPointProperties, (Func<Microsoft.TeamFoundation.TestManagement.Server.BlockedPointProperties, BlockedPointProperties>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestPlan Convert(
      TestManagementRequestContext context,
      TestPlan plan)
    {
      if (plan == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestPlan) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestPlan plan1 = new Microsoft.TeamFoundation.TestManagement.Server.TestPlan()
      {
        AreaPath = plan.AreaPath,
        AutomatedTestEnvironmentId = plan.AutomatedTestEnvironmentId,
        AutomatedTestSettingsId = plan.AutomatedTestSettingsId,
        BuildDefinition = plan.BuildDefinition,
        BuildQuality = plan.BuildQuality,
        BuildTakenDate = plan.BuildTakenDate,
        BuildUri = Validator.TranslateBuildUri(plan.BuildUri),
        Description = plan.Description,
        EndDate = plan.EndDate,
        Iteration = plan.Iteration,
        LastUpdated = plan.LastUpdated,
        LastUpdatedBy = plan.LastUpdatedBy,
        ManualTestEnvironmentId = plan.ManualTestEnvironmentId,
        Name = plan.Name,
        Owner = plan.Owner,
        PlanId = plan.PlanId,
        PreviousBuildUri = plan.PreviousBuildUri,
        Revision = plan.Revision,
        RootSuiteId = plan.RootSuiteId,
        StartDate = plan.StartDate,
        State = plan.State,
        TestSettingsId = plan.TestSettingsId
      };
      return Compat2011QU1Helper.Convert(context, plan1);
    }

    internal static TestPlan Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPlan plan)
    {
      if (plan == null)
        return (TestPlan) null;
      return new TestPlan()
      {
        AreaPath = plan.AreaPath,
        AutomatedTestEnvironmentId = plan.AutomatedTestEnvironmentId,
        AutomatedTestSettingsId = plan.AutomatedTestSettingsId,
        BuildDefinition = plan.BuildDefinition,
        BuildQuality = plan.BuildQuality,
        BuildTakenDate = plan.BuildTakenDate,
        BuildUri = plan.BuildUri,
        Description = plan.Description,
        EndDate = plan.EndDate,
        Iteration = plan.Iteration,
        LastUpdated = plan.LastUpdated,
        LastUpdatedBy = plan.LastUpdatedBy,
        ManualTestEnvironmentId = plan.ManualTestEnvironmentId,
        Name = plan.Name,
        Owner = plan.Owner,
        PlanId = plan.PlanId,
        PreviousBuildUri = plan.PreviousBuildUri,
        Revision = plan.Revision,
        RootSuiteId = plan.RootSuiteId,
        StartDate = plan.StartDate,
        State = plan.State,
        TestSettingsId = plan.TestSettingsId
      };
    }

    internal static List<TestPlan> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestPlan> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestPlan, TestPlan>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestPlan, TestPlan>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink Convert(
      TestExternalLink link)
    {
      if (link == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink()
      {
        Description = link.Description,
        LinkId = link.LinkId,
        Uri = link.Uri
      };
    }

    internal static TestExternalLink Convert(Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink link)
    {
      if (link == null)
        return (TestExternalLink) null;
      return new TestExternalLink()
      {
        Description = link.Description,
        LinkId = link.LinkId,
        Uri = link.Uri
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink[] Convert(
      TestExternalLink[] list)
    {
      return Compat2010Helper.ConvertToArray<TestExternalLink, Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink>((IEnumerable<TestExternalLink>) list, (Func<TestExternalLink, Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestExternalLink> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink, TestExternalLink>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestExternalLink, TestExternalLink>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.IdAndRev Convert(IdAndRev id)
    {
      if (id == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.IdAndRev) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.IdAndRev()
      {
        Id = id.Id,
        Revision = id.Revision
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.IdAndRev[] Convert(
      IdAndRev[] list)
    {
      return Compat2010Helper.ConvertToArray<IdAndRev, Microsoft.TeamFoundation.TestManagement.Server.IdAndRev>((IEnumerable<IdAndRev>) list, (Func<IdAndRev, Microsoft.TeamFoundation.TestManagement.Server.IdAndRev>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestPoint> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestPoint> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, TestPoint>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestPoint>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestPoint, TestPoint>) (x => Compat2010Helper.Convert(x)));

    internal static SuitePointCount Convert(Microsoft.TeamFoundation.TestManagement.Server.SuitePointCount pointCount)
    {
      if (pointCount == null)
        return (SuitePointCount) null;
      return new SuitePointCount()
      {
        PointCount = pointCount.PointCount,
        SuiteId = pointCount.SuiteId
      };
    }

    internal static List<SuitePointCount> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.SuitePointCount> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.SuitePointCount, SuitePointCount>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.SuitePointCount>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.SuitePointCount, SuitePointCount>) (x => Compat2010Helper.Convert(x)));

    internal static SkinnyPlan Convert(Microsoft.TeamFoundation.TestManagement.Server.SkinnyPlan plan)
    {
      if (plan == null)
        return (SkinnyPlan) null;
      return new SkinnyPlan() { Id = plan.Id };
    }

    internal static List<SkinnyPlan> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.SkinnyPlan> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.SkinnyPlan, SkinnyPlan>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.SkinnyPlan>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.SkinnyPlan, SkinnyPlan>) (x => Compat2010Helper.Convert(x)));

    internal static TestPointStatistic Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic stat)
    {
      if (stat == null)
        return (TestPointStatistic) null;
      return new TestPointStatistic()
      {
        Count = stat.Count,
        FailureType = TestFailureType.GetFailureTypeFromId((int) stat.FailureType),
        ResolutionStateId = stat.ResolutionStateId,
        ResultOutcome = Microsoft.TeamFoundation.TestManagement.Server.TestResult.ToPreDev12QU2Outcome(stat.ResultOutcome),
        ResultState = stat.ResultState,
        TestPointState = stat.TestPointState
      };
    }

    internal static List<TestPointStatistic> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic> list)
    {
      Compat2011QU1Helper.MergeNotApplicableWithPassedPointStatistic(list);
      return Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic, TestPointStatistic>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestPointStatistic, TestPointStatistic>) (x => Compat2010Helper.Convert(x)));
    }

    private static TestCaseResultIdentifier Convert(
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier testCaseResultIdentifier)
    {
      if (testCaseResultIdentifier == null)
        return (TestCaseResultIdentifier) null;
      return new TestCaseResultIdentifier()
      {
        TestResultId = testCaseResultIdentifier.TestResultId,
        TestRunId = testCaseResultIdentifier.TestRunId
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier Convert(
      TestCaseResultIdentifier testCaseResultIdentifier)
    {
      if (testCaseResultIdentifier == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier()
      {
        TestResultId = testCaseResultIdentifier.TestResultId,
        TestRunId = testCaseResultIdentifier.TestRunId
      };
    }

    internal static List<TestCaseResultIdentifier> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> ids) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier, TestCaseResultIdentifier>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) ids, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier, TestCaseResultIdentifier>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier[] Convert(
      TestCaseResultIdentifier[] ids)
    {
      return Compat2010Helper.ConvertToArray<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>((IEnumerable<TestCaseResultIdentifier>) ids, (Func<TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) (x => Compat2010Helper.Convert(x)));
    }

    internal static TestCaseResult Convert(Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult)
    {
      if (testCaseResult == null)
        return (TestCaseResult) null;
      TestCaseResult testCaseResult1 = new TestCaseResult();
      testCaseResult1.AfnStripId = testCaseResult.AfnStripId;
      testCaseResult1.AutomatedTestId = testCaseResult.AutomatedTestId;
      testCaseResult1.AutomatedTestName = testCaseResult.AutomatedTestName;
      testCaseResult1.AutomatedTestStorage = testCaseResult.AutomatedTestStorage;
      testCaseResult1.AutomatedTestType = testCaseResult.AutomatedTestType;
      testCaseResult1.AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId;
      testCaseResult1.Comment = testCaseResult.Comment;
      testCaseResult1.ComputerName = testCaseResult.ComputerName;
      testCaseResult1.ConfigurationId = testCaseResult.ConfigurationId;
      testCaseResult1.ConfigurationName = testCaseResult.ConfigurationName;
      testCaseResult1.CreationDate = testCaseResult.CreationDate;
      testCaseResult1.DateCompleted = testCaseResult.DateCompleted;
      testCaseResult1.DateStarted = testCaseResult.DateStarted;
      testCaseResult1.Duration = testCaseResult.Duration;
      testCaseResult1.ErrorMessage = testCaseResult.ErrorMessage;
      testCaseResult1.FailureType = TestFailureType.GetFailureTypeFromId((int) testCaseResult.FailureType);
      testCaseResult1.Id = Compat2010Helper.Convert(testCaseResult.Id);
      testCaseResult1.LastUpdated = testCaseResult.LastUpdated;
      testCaseResult1.LastUpdatedBy = testCaseResult.LastUpdatedBy;
      testCaseResult1.Outcome = Microsoft.TeamFoundation.TestManagement.Server.TestResult.ToPreDev12QU2Outcome(testCaseResult.Outcome);
      testCaseResult1.Owner = testCaseResult.Owner;
      testCaseResult1.Priority = testCaseResult.Priority;
      testCaseResult1.ResetCount = testCaseResult.ResetCount;
      testCaseResult1.ResolutionStateId = testCaseResult.ResolutionStateId;
      testCaseResult1.Revision = testCaseResult.Revision;
      testCaseResult1.RunBy = testCaseResult.RunBy;
      testCaseResult1.State = testCaseResult.State;
      testCaseResult1.TestCaseArea = testCaseResult.TestCaseArea;
      testCaseResult1.TestCaseAreaUri = testCaseResult.TestCaseAreaUri;
      testCaseResult1.TestCaseId = testCaseResult.TestCaseId;
      testCaseResult1.TestCaseRevision = testCaseResult.TestCaseRevision;
      testCaseResult1.TestCaseTitle = testCaseResult.TestCaseTitle;
      testCaseResult1.TestPointId = testCaseResult.TestPointId;
      return testCaseResult1;
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult Convert(
      TestCaseResult testCaseResult)
    {
      if (testCaseResult == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult1 = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult();
      testCaseResult1.AfnStripId = testCaseResult.AfnStripId;
      testCaseResult1.AutomatedTestId = testCaseResult.AutomatedTestId;
      testCaseResult1.AutomatedTestName = testCaseResult.AutomatedTestName;
      testCaseResult1.AutomatedTestStorage = testCaseResult.AutomatedTestStorage;
      testCaseResult1.AutomatedTestType = testCaseResult.AutomatedTestType;
      testCaseResult1.AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId;
      testCaseResult1.Comment = testCaseResult.Comment;
      testCaseResult1.ComputerName = testCaseResult.ComputerName;
      testCaseResult1.ConfigurationId = testCaseResult.ConfigurationId;
      testCaseResult1.ConfigurationName = testCaseResult.ConfigurationName;
      testCaseResult1.CreationDate = testCaseResult.CreationDate;
      testCaseResult1.DateCompleted = testCaseResult.DateCompleted;
      testCaseResult1.DateStarted = testCaseResult.DateStarted;
      testCaseResult1.Duration = testCaseResult.Duration;
      testCaseResult1.ErrorMessage = testCaseResult.ErrorMessage;
      testCaseResult1.FailureType = TestFailureType.GetFailureTypeFromId((int) testCaseResult.FailureType);
      testCaseResult1.Id = Compat2010Helper.Convert(testCaseResult.Id);
      testCaseResult1.LastUpdated = testCaseResult.LastUpdated;
      testCaseResult1.LastUpdatedBy = testCaseResult.LastUpdatedBy;
      testCaseResult1.Outcome = testCaseResult.Outcome;
      testCaseResult1.Owner = testCaseResult.Owner;
      testCaseResult1.Priority = testCaseResult.Priority;
      testCaseResult1.ResetCount = testCaseResult.ResetCount;
      testCaseResult1.ResolutionStateId = testCaseResult.ResolutionStateId;
      testCaseResult1.Revision = testCaseResult.Revision;
      testCaseResult1.RunBy = testCaseResult.RunBy;
      testCaseResult1.State = testCaseResult.State;
      testCaseResult1.TestCaseArea = testCaseResult.TestCaseArea;
      testCaseResult1.TestCaseAreaUri = testCaseResult.TestCaseAreaUri;
      testCaseResult1.TestCaseId = testCaseResult.TestCaseId;
      testCaseResult1.TestCaseRevision = testCaseResult.TestCaseRevision;
      testCaseResult1.TestCaseTitle = testCaseResult.TestCaseTitle;
      testCaseResult1.TestPointId = testCaseResult.TestPointId;
      return testCaseResult1;
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] Convert(
      TestCaseResult[] results)
    {
      return Compat2010Helper.ConvertToArray<TestCaseResult, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>((IEnumerable<TestCaseResult>) results, (Func<TestCaseResult, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestCaseResult> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResult>) (x => Compat2010Helper.Convert(x)));

    internal static TestCaseResult[] Convert(Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] results) => Compat2010Helper.ConvertToArray<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) results, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestCaseResult>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestRun Convert(TestRun testRun)
    {
      if (testRun == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestRun) null;
      Microsoft.TeamFoundation.TestManagement.Server.TestRun testRun1 = new Microsoft.TeamFoundation.TestManagement.Server.TestRun();
      testRun1.BuildConfigurationId = testRun.BuildConfigurationId;
      testRun1.BuildFlavor = testRun.BuildFlavor;
      testRun1.BuildNumber = testRun.BuildNumber;
      testRun1.BuildPlatform = testRun.BuildPlatform;
      testRun1.BuildUri = Validator.TranslateBuildUri(testRun.BuildUri);
      testRun1.Comment = testRun.Comment;
      testRun1.CompleteDate = testRun.CompleteDate;
      testRun1.Controller = testRun.Controller;
      testRun1.CreationDate = testRun.CreationDate;
      testRun1.DropLocation = testRun.DropLocation;
      testRun1.DueDate = testRun.DueDate;
      testRun1.ErrorMessage = testRun.ErrorMessage;
      testRun1.IsAutomated = testRun.IsAutomated;
      testRun1.IsBvt = testRun.IsBvt;
      testRun1.Iteration = testRun.Iteration;
      testRun1.LastUpdated = testRun.LastUpdated;
      testRun1.LastUpdatedBy = testRun.LastUpdatedBy;
      testRun1.LegacySharePath = testRun.LegacySharePath;
      testRun1.Owner = testRun.Owner;
      testRun1.PostProcessState = testRun.PostProcessState;
      testRun1.PublicTestSettingsId = testRun.PublicTestSettingsId;
      testRun1.Revision = testRun.Revision;
      testRun1.StartDate = testRun.StartDate;
      testRun1.State = testRun.State;
      testRun1.TeamProject = testRun.TeamProject;
      testRun1.TestEnvironmentId = testRun.TestEnvironmentId;
      testRun1.TestMessageLogId = testRun.TestMessageLogId;
      testRun1.TestPlanId = testRun.TestPlanId;
      testRun1.TestRunId = testRun.TestRunId;
      testRun1.TestSettingsId = testRun.TestSettingsId;
      testRun1.Title = testRun.Title;
      testRun1.Type = testRun.Type;
      testRun1.Version = testRun.Version;
      return testRun1;
    }

    internal static TestRun Convert(Microsoft.TeamFoundation.TestManagement.Server.TestRun testRun)
    {
      if (testRun == null)
        return (TestRun) null;
      return new TestRun()
      {
        BuildConfigurationId = testRun.BuildConfigurationId,
        BuildFlavor = testRun.BuildFlavor,
        BuildNumber = testRun.BuildNumber,
        BuildPlatform = testRun.BuildPlatform,
        BuildUri = testRun.BuildUri,
        Comment = testRun.Comment,
        CompleteDate = testRun.CompleteDate,
        Controller = testRun.Controller,
        CreationDate = testRun.CreationDate,
        DropLocation = testRun.DropLocation,
        DueDate = testRun.DueDate,
        ErrorMessage = testRun.ErrorMessage,
        IsAutomated = testRun.IsAutomated,
        IsBvt = testRun.IsBvt,
        Iteration = testRun.Iteration,
        LastUpdated = testRun.LastUpdated,
        LastUpdatedBy = testRun.LastUpdatedBy,
        LegacySharePath = testRun.LegacySharePath,
        Owner = testRun.Owner,
        PostProcessState = testRun.PostProcessState,
        PublicTestSettingsId = testRun.PublicTestSettingsId,
        Revision = testRun.Revision,
        StartDate = testRun.StartDate,
        State = testRun.State,
        TeamProject = testRun.TeamProject,
        TestEnvironmentId = testRun.TestEnvironmentId,
        TestMessageLogId = testRun.TestMessageLogId,
        TestPlanId = testRun.TestPlanId,
        TestRunId = testRun.TestRunId,
        TestSettingsId = testRun.TestSettingsId,
        Title = testRun.Title,
        Type = testRun.Type,
        Version = testRun.Version
      };
    }

    internal static List<TestRun> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestRun> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestRun, TestRun>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRun>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRun, TestRun>) (x => Compat2010Helper.Convert(x)));

    internal static TestRunStatistic Convert(Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic stat)
    {
      if (stat == null)
        return (TestRunStatistic) null;
      return new TestRunStatistic()
      {
        Count = stat.Count,
        Outcome = Microsoft.TeamFoundation.TestManagement.Server.TestResult.ToPreDev12QU2Outcome(stat.Outcome),
        ResolutionState = Compat2010Helper.Convert(stat.ResolutionState),
        State = stat.State
      };
    }

    internal static List<TestRunStatistic> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic> list)
    {
      Compat2011QU1Helper.MergeNotApplicableWithPassedRunStatistic(list);
      return Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic, TestRunStatistic>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestRunStatistic, TestRunStatistic>) (x => Compat2010Helper.Convert(x)));
    }

    internal static BuildConfiguration Convert(Microsoft.TeamFoundation.TestManagement.Server.BuildConfiguration buildConfiguration)
    {
      if (buildConfiguration == null)
        return (BuildConfiguration) null;
      return new BuildConfiguration()
      {
        BuildConfigurationId = buildConfiguration.BuildConfigurationId,
        BuildFlavor = buildConfiguration.BuildFlavor,
        BuildPlatform = buildConfiguration.BuildPlatform,
        BuildUri = buildConfiguration.BuildUri,
        TeamProjectName = buildConfiguration.TeamProjectName
      };
    }

    internal static TestResultParameter Convert(Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter parameter)
    {
      if (parameter == null)
        return (TestResultParameter) null;
      return new TestResultParameter()
      {
        ActionPath = parameter.ActionPath,
        Actual = parameter.Actual,
        DataType = parameter.DataType,
        Expected = parameter.Expected,
        IterationId = parameter.IterationId,
        ParameterName = parameter.ParameterName,
        TestResultId = parameter.TestResultId,
        TestRunId = parameter.TestRunId
      };
    }

    internal static List<TestResultParameter> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> parametersList) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter, TestResultParameter>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) parametersList, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter, TestResultParameter>) (x => Compat2010Helper.Convert(x)));

    internal static TestActionResult Convert(Microsoft.TeamFoundation.TestManagement.Server.TestActionResult action)
    {
      TestActionResult testActionResult;
      switch (action)
      {
        case null:
          return (TestActionResult) null;
        case Microsoft.TeamFoundation.TestManagement.Server.SharedStepResult _:
          testActionResult = (TestActionResult) new SharedStepResult();
          break;
        case Microsoft.TeamFoundation.TestManagement.Server.TestIterationResult _:
          testActionResult = (TestActionResult) new TestIterationResult();
          break;
        case Microsoft.TeamFoundation.TestManagement.Server.TestStepResult _:
          testActionResult = (TestActionResult) new TestStepResult();
          break;
        default:
          testActionResult = new TestActionResult();
          break;
      }
      testActionResult.ActionPath = action.ActionPath;
      testActionResult.Comment = action.Comment;
      testActionResult.CreationDate = action.CreationDate;
      testActionResult.DateCompleted = action.DateCompleted;
      testActionResult.DateStarted = action.DateStarted;
      testActionResult.Duration = action.Duration;
      testActionResult.ErrorMessage = action.ErrorMessage;
      testActionResult.Id = Compat2010Helper.Convert(action.Id);
      testActionResult.IterationId = action.IterationId;
      testActionResult.LastUpdated = action.LastUpdated;
      testActionResult.LastUpdatedBy = action.LastUpdatedBy;
      testActionResult.Outcome = Microsoft.TeamFoundation.TestManagement.Server.TestResult.ToPreDev12QU2Outcome(action.Outcome);
      testActionResult.SetId = action.SetId;
      return testActionResult;
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestActionResult Convert(
      TestActionResult action)
    {
      Microsoft.TeamFoundation.TestManagement.Server.TestActionResult testActionResult;
      switch (action)
      {
        case null:
          return (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) null;
        case SharedStepResult _:
          testActionResult = (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) new Microsoft.TeamFoundation.TestManagement.Server.SharedStepResult();
          break;
        case TestIterationResult _:
          testActionResult = (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) new Microsoft.TeamFoundation.TestManagement.Server.TestIterationResult();
          break;
        case TestStepResult _:
          testActionResult = (Microsoft.TeamFoundation.TestManagement.Server.TestActionResult) new Microsoft.TeamFoundation.TestManagement.Server.TestStepResult();
          break;
        default:
          testActionResult = new Microsoft.TeamFoundation.TestManagement.Server.TestActionResult();
          break;
      }
      testActionResult.ActionPath = action.ActionPath;
      testActionResult.Comment = action.Comment;
      testActionResult.CreationDate = action.CreationDate;
      testActionResult.DateCompleted = action.DateCompleted;
      testActionResult.DateStarted = action.DateStarted;
      testActionResult.Duration = action.Duration;
      testActionResult.ErrorMessage = action.ErrorMessage;
      testActionResult.Id = Compat2010Helper.Convert(action.Id);
      testActionResult.IterationId = action.IterationId;
      testActionResult.LastUpdated = action.LastUpdated;
      testActionResult.LastUpdatedBy = action.LastUpdatedBy;
      testActionResult.Outcome = action.Outcome;
      testActionResult.SetId = action.SetId;
      return testActionResult;
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.TestActionResult[] Convert(
      TestActionResult[] actionsList)
    {
      return Compat2010Helper.ConvertToArray<TestActionResult, Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>((IEnumerable<TestActionResult>) actionsList, (Func<TestActionResult, Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<TestActionResult> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> actionsList) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult, TestActionResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) actionsList, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult, TestActionResult>) (x => Compat2010Helper.Convert(x)));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev Convert(
      TestCaseResultIdAndRev id)
    {
      if (id == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev()
      {
        Id = Compat2010Helper.Convert(id.Id),
        Revision = id.Revision
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev[] Convert(
      TestCaseResultIdAndRev[] ids)
    {
      return Compat2010Helper.ConvertToArray<TestCaseResultIdAndRev, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>((IEnumerable<TestCaseResultIdAndRev>) ids, (Func<TestCaseResultIdAndRev, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>) (x => Compat2010Helper.Convert(x)));
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter[] Convert(
      TestResultParameter[] testResultParameter)
    {
      return Compat2010Helper.ConvertToArray<TestResultParameter, Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>((IEnumerable<TestResultParameter>) testResultParameter, (Func<TestResultParameter, Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) (parameter =>
      {
        if (parameter == null)
          return (Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter) null;
        return new Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter()
        {
          ActionPath = parameter.ActionPath,
          Actual = parameter.Actual,
          DataType = parameter.DataType,
          Expected = parameter.Expected,
          IterationId = parameter.IterationId,
          ParameterName = parameter.ParameterName,
          TestResultId = parameter.TestResultId,
          TestRunId = parameter.TestRunId
        };
      }));
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest Convert(
      ResultUpdateRequest request)
    {
      if (request == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest()
      {
        ActionResultDeletes = Compat2010Helper.Convert(request.ActionResultDeletes),
        ActionResults = Compat2010Helper.Convert(request.ActionResults),
        AttachmentDeletes = Compat2010Helper.Convert(request.AttachmentDeletes),
        Attachments = Compat2010Helper.Convert(request.Attachments),
        ParameterDeletes = Compat2010Helper.Convert(request.ParameterDeletes),
        Parameters = Compat2010Helper.Convert(request.Parameters),
        TestCaseResult = Compat2010Helper.Convert(request.TestCaseResult),
        TestResultId = request.TestResultId,
        TestRunId = request.TestRunId
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest[] Convert(
      ResultUpdateRequest[] requests)
    {
      return Compat2010Helper.ConvertToArray<ResultUpdateRequest, Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest>((IEnumerable<ResultUpdateRequest>) requests, (Func<ResultUpdateRequest, Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest>) (x => Compat2010Helper.Convert(x)));
    }

    internal static ResultUpdateResponse[] Convert(Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse[] resultUpdateResponse) => Compat2010Helper.ConvertToArray<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse, ResultUpdateResponse>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse>) resultUpdateResponse, (Func<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse, ResultUpdateResponse>) (x =>
    {
      if (x == null)
        return (ResultUpdateResponse) null;
      return new ResultUpdateResponse()
      {
        AttachmentIds = x.AttachmentIds,
        LastUpdated = x.LastUpdated,
        LastUpdatedBy = x.LastUpdatedBy,
        Revision = x.Revision
      };
    }));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity[] Convert(
      TestResultAttachmentIdentity[] attachmentDeletes)
    {
      return Compat2010Helper.ConvertToArray<TestResultAttachmentIdentity, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>((IEnumerable<TestResultAttachmentIdentity>) attachmentDeletes, (Func<TestResultAttachmentIdentity, Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>) (x =>
      {
        if (x == null)
          return (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity) null;
        return new Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity()
        {
          AttachmentId = x.AttachmentId,
          TestResultId = x.TestResultId,
          TestRunId = x.TestRunId
        };
      }));
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry[] Convert(
      TestMessageLogEntry[] logEntries)
    {
      return Compat2010Helper.ConvertToArray<TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>((IEnumerable<TestMessageLogEntry>) logEntries, (Func<TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>) (x =>
      {
        if (x == null)
          return (Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry) null;
        return new Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry()
        {
          DateCreated = x.DateCreated,
          EntryId = x.EntryId,
          LogLevel = x.LogLevel,
          LogUser = x.LogUser,
          Message = x.Message,
          TestMessageLogId = x.TestMessageLogId
        };
      }));
    }

    internal static List<TestMessageLogEntry> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, TestMessageLogEntry>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, TestMessageLogEntry>) (x =>
    {
      if (x == null)
        return (TestMessageLogEntry) null;
      return new TestMessageLogEntry()
      {
        DateCreated = x.DateCreated,
        EntryId = x.EntryId,
        LogLevel = x.LogLevel,
        LogUser = x.LogUser,
        Message = x.Message,
        TestMessageLogId = x.TestMessageLogId
      };
    }));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseAndOwner[] Convert(
      TestCaseAndOwner[] testCases)
    {
      return Compat2010Helper.ConvertToArray<TestCaseAndOwner, Microsoft.TeamFoundation.TestManagement.Server.TestCaseAndOwner>((IEnumerable<TestCaseAndOwner>) testCases, (Func<TestCaseAndOwner, Microsoft.TeamFoundation.TestManagement.Server.TestCaseAndOwner>) (x =>
      {
        if (x == null)
          return (Microsoft.TeamFoundation.TestManagement.Server.TestCaseAndOwner) null;
        return new Microsoft.TeamFoundation.TestManagement.Server.TestCaseAndOwner()
        {
          Id = x.Id,
          Owner = x.Owner
        };
      }));
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite Convert(
      ServerTestSuite suite)
    {
      if (suite == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite) null;
      Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite serverTestSuite = new Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite()
      {
        Description = suite.Description,
        Id = suite.Id,
        InheritDefaultConfigurations = suite.InheritDefaultConfigurations,
        LastError = suite.LastError,
        LastPopulated = suite.LastPopulated,
        LastUpdated = suite.LastUpdated,
        LastUpdatedBy = suite.LastUpdatedBy,
        ParentId = suite.ParentId,
        PlanId = suite.PlanId,
        QueryString = suite.QueryString,
        RequirementId = suite.RequirementId,
        Revision = suite.Revision,
        State = suite.State,
        SuiteType = suite.SuiteType,
        Title = suite.Title
      };
      serverTestSuite.DefaultConfigurationNames.AddRange((IEnumerable<string>) suite.DefaultConfigurationNames);
      serverTestSuite.DefaultConfigurations.AddRange((IEnumerable<int>) suite.DefaultConfigurations);
      Compat2010Helper.Convert<TestSuiteEntry, Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry>((IEnumerable<TestSuiteEntry>) suite.ServerEntries, serverTestSuite.ServerEntries, (Func<TestSuiteEntry, Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry>) (x => Compat2010Helper.Convert(x)));
      return serverTestSuite;
    }

    internal static ServerTestSuite Convert(Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite suite)
    {
      if (suite == null)
        return (ServerTestSuite) null;
      ServerTestSuite serverTestSuite = new ServerTestSuite()
      {
        Description = suite.Description,
        Id = suite.Id,
        InheritDefaultConfigurations = suite.InheritDefaultConfigurations,
        LastError = suite.LastError,
        LastPopulated = suite.LastPopulated,
        LastUpdated = suite.LastUpdated,
        LastUpdatedBy = suite.LastUpdatedBy,
        ParentId = suite.ParentId,
        PlanId = suite.PlanId,
        QueryString = suite.QueryString,
        RequirementId = suite.RequirementId,
        Revision = suite.Revision,
        State = suite.State,
        SuiteType = suite.SuiteType,
        Title = suite.Title
      };
      serverTestSuite.DefaultConfigurationNames.AddRange((IEnumerable<string>) suite.DefaultConfigurationNames);
      serverTestSuite.DefaultConfigurations.AddRange((IEnumerable<int>) suite.DefaultConfigurations);
      Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry, TestSuiteEntry>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry>) suite.ServerEntries, serverTestSuite.ServerEntries, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry, TestSuiteEntry>) (x => Compat2010Helper.Convert(x)));
      return serverTestSuite;
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry Convert(
      TestSuiteEntry entry)
    {
      if (entry == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry()
      {
        EntryId = entry.EntryId,
        EntryType = entry.EntryType,
        PointAssignments = Compat2010Helper.ConvertToArray<TestPointAssignment, Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment>((IEnumerable<TestPointAssignment>) entry.PointAssignments, (Func<TestPointAssignment, Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment>) (x => Compat2010Helper.Convert(x)))
      };
    }

    private static TestSuiteEntry Convert(Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry entry)
    {
      if (entry == null)
        return (TestSuiteEntry) null;
      return new TestSuiteEntry()
      {
        EntryId = entry.EntryId,
        EntryType = entry.EntryType,
        PointAssignments = Compat2010Helper.ConvertToArray<Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment, TestPointAssignment>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment>) entry.PointAssignments, (Func<Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment, TestPointAssignment>) (x => Compat2010Helper.Convert(x)))
      };
    }

    private static Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment Convert(
      TestPointAssignment x)
    {
      if (x == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment()
      {
        AssignedTo = x.AssignedTo,
        ConfigurationId = x.ConfigurationId,
        ConfigurationName = x.ConfigurationName,
        TestCaseId = x.TestCaseId
      };
    }

    private static TestPointAssignment Convert(Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment x)
    {
      if (x == null)
        return (TestPointAssignment) null;
      return new TestPointAssignment()
      {
        AssignedTo = x.AssignedTo,
        ConfigurationId = x.ConfigurationId,
        ConfigurationName = x.ConfigurationName,
        TestCaseId = x.TestCaseId
      };
    }

    internal static List<Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry> Convert(
      List<TestSuiteEntry> entries)
    {
      return Compat2010Helper.Convert<TestSuiteEntry, Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry>((IEnumerable<TestSuiteEntry>) entries, (Func<TestSuiteEntry, Microsoft.TeamFoundation.TestManagement.Server.TestSuiteEntry>) (x => Compat2010Helper.Convert(x)));
    }

    internal static List<ServerTestSuite> Convert(List<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite> list) => Compat2010Helper.Convert<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite, ServerTestSuite>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite>) list, (Func<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite, ServerTestSuite>) (x => Compat2010Helper.Convert(x)));

    internal static SuiteIdAndType[] Convert(Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType[] suiteIdAndType) => suiteIdAndType == null ? (SuiteIdAndType[]) null : Compat2010Helper.ConvertToArray<Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType, SuiteIdAndType>((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType>) suiteIdAndType, (Func<Microsoft.TeamFoundation.TestManagement.Server.SuiteIdAndType, SuiteIdAndType>) (x => new SuiteIdAndType()
    {
      SuiteId = x.SuiteId,
      SuiteType = x.SuiteType
    }));

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment[] Convert(
      TestPointAssignment[] assignments)
    {
      return Compat2010Helper.ConvertToArray<TestPointAssignment, Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment>((IEnumerable<TestPointAssignment>) assignments, (Func<TestPointAssignment, Microsoft.TeamFoundation.TestManagement.Server.TestPointAssignment>) (x => Compat2010Helper.Convert(x)));
    }

    internal static void ValidateQueryBasedSuite(
      ServerTestSuite testSuite,
      string projectName,
      TfsTestManagementRequestContext m_tmRequestContext)
    {
      bool flag;
      try
      {
        flag = Compat2010Helper.CheckIfQueryBasedSuite(testSuite, projectName, m_tmRequestContext);
      }
      catch
      {
        flag = false;
      }
      if (flag)
        throw new TeamFoundationServerException(ServerResources.TestSuiteCannotBeUpdated);
    }

    private static bool CheckIfQueryBasedSuite(
      ServerTestSuite testSuite,
      string projectName,
      TfsTestManagementRequestContext m_tmRequestContext)
    {
      IdAndRev[] list = new IdAndRev[1]
      {
        new IdAndRev() { Id = testSuite.Id, Revision = 0 }
      };
      List<Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite> serverTestSuiteList = Microsoft.TeamFoundation.TestManagement.Server.ServerTestSuite.Fetch((TestManagementRequestContext) m_tmRequestContext, projectName, Compat2010Helper.Convert(list), (List<int>) null);
      return serverTestSuiteList != null && serverTestSuiteList.Count == 1 && Compat2010Helper.ValidateIfQueryStringIsHierarchical(serverTestSuiteList[0].QueryString);
    }

    private static bool ValidateIfQueryStringIsHierarchical(string queryString) => queryString != null && Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems != WiqlAdapter.GetQueryMode(Parser.ParseSyntax(queryString));

    internal static void ValidateCompatibleResultOutcome(
      ResultUpdateRequest[] requests,
      string projectName,
      TfsTestManagementRequestContext context)
    {
      Compat2011QU1Helper.ValidateCompatibleResultOutcome(Compat2010Helper.Convert(requests), projectName, context);
    }
  }
}
