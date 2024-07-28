// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.TestConfigurationAdapter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  public class TestConfigurationAdapter : Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestConfiguration
  {
    public TestConfigurationAdapter(
      TestManagementRequestContext testManagementRequestContext,
      Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration testConfiguration,
      ProjectInfo projectInfo)
    {
      if (testConfiguration == null)
        return;
      this.Description = testConfiguration.Description;
      this.Id = testConfiguration.Id;
      this.Name = testConfiguration.Name;
      this.IsDefault = testConfiguration.IsDefault;
      this.State = (TestConfigurationState) testConfiguration.State;
      if (!testConfiguration.Values.IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>())
      {
        this.Values = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>();
        testConfiguration.Values.ForEach((Action<Microsoft.TeamFoundation.TestManagement.Server.NameValuePair>) (nameValuePair => this.Values.Add(new Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair(nameValuePair.Name, nameValuePair.Value))));
      }
      if (testManagementRequestContext == null || projectInfo == null)
        return;
      this.Project = new TeamProjectReference()
      {
        Name = projectInfo.Name,
        Id = projectInfo.Id,
        Url = testManagementRequestContext.ProjectServiceHelper?.GetProjectRepresentation(projectInfo.Name)?.Url
      };
    }

    public TestConfigurationAdapter(
      TestConfigurationCreateUpdateParameters testConfigurationCreateUpdateParameters)
    {
      ArgumentUtility.CheckForNull<TestConfigurationCreateUpdateParameters>(testConfigurationCreateUpdateParameters, "TestConfigurationCreateUpdateParameters");
      this.Description = testConfigurationCreateUpdateParameters.Description;
      this.Name = testConfigurationCreateUpdateParameters.Name;
      this.IsDefault = testConfigurationCreateUpdateParameters.IsDefault;
      this.State = testConfigurationCreateUpdateParameters.State;
      this.Values = testConfigurationCreateUpdateParameters.Values;
    }

    public Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration ToServerTestConfiguration(
      TestManagementRequestContext testManagementRequestContext,
      ProjectInfo projectInfo)
    {
      Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration testConfiguration = new Microsoft.TeamFoundation.TestManagement.Server.TestConfiguration()
      {
        Description = this.Description,
        Id = this.Id,
        Name = this.Name,
        IsDefault = this.IsDefault,
        State = (byte) this.State
      };
      TcmCommonStructureNodeInfo rootNode = testManagementRequestContext?.CSSHelper?.GetRootNode(projectInfo?.Uri, "ProjectModelHierarchy");
      testConfiguration.AreaPath = rootNode?.Path ?? string.Empty;
      if (!this.Values.IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>())
      {
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        List<string> values = new List<string>();
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair nameValuePair in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>) this.Values)
        {
          if (string.IsNullOrWhiteSpace(nameValuePair?.Name))
            throw new InvalidPropertyException("TestConfigurationVariableValue", ServerResources.InvalidPropertyMessage);
          if (stringSet.Contains(nameValuePair.Name))
          {
            values.Add(nameValuePair.Name);
          }
          else
          {
            stringSet.Add(nameValuePair.Name);
            testConfiguration.Values.Add(new Microsoft.TeamFoundation.TestManagement.Server.NameValuePair(nameValuePair.Name, nameValuePair.Value));
          }
        }
        if (values.Count > 0)
          throw new InvalidPropertyException("TestConfigurationVariableValue", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateTestVariable, (object) string.Join(ServerResources.VariableSeparator, (IEnumerable<string>) values)));
      }
      return testConfiguration;
    }
  }
}
