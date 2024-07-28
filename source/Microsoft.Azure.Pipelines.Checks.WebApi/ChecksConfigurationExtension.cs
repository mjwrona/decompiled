// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.ChecksConfigurationExtension
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.Azure.Pipelines.TaskCheck.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ChecksConfigurationExtension
  {
    public static CheckConfiguration CreateCheckConfigurationObject(this CheckType type)
    {
      CheckConfiguration configurationObject = (CheckConfiguration) null;
      if (type != null)
        configurationObject = string.Equals(type.Name, "Approval", StringComparison.OrdinalIgnoreCase) || type.Id == ApprovalCheckConstants.ApprovalCheckTypeId ? (CheckConfiguration) new ApprovalCheckConfiguration() : (string.Equals(type.Name, "Task Check", StringComparison.OrdinalIgnoreCase) || type.Id == TaskCheckConstants.TaskCheckTypeId ? (CheckConfiguration) new TaskCheckConfiguration() : (CheckConfiguration) new GenericCheckConfiguration());
      return configurationObject;
    }

    public static void PopulateSettings(this CheckConfiguration configuration, string settings)
    {
      if (string.IsNullOrWhiteSpace(settings) || configuration == null || !JObject.Parse(settings).HasValues)
        return;
      if (configuration.GetType() == typeof (ApprovalCheckConfiguration))
      {
        if (!(configuration is ApprovalCheckConfiguration checkConfiguration))
          return;
        checkConfiguration.Settings = JsonUtility.FromString<ApprovalConfigSettings>(settings);
      }
      else if (configuration.GetType() == typeof (TaskCheckConfiguration))
      {
        if (!(configuration is TaskCheckConfiguration checkConfiguration))
          return;
        checkConfiguration.Settings = JsonUtility.FromString<TaskCheckConfig>(settings);
      }
      else
      {
        if (!(configuration is GenericCheckConfiguration checkConfiguration))
          return;
        checkConfiguration.Settings = JObject.Parse(settings);
      }
    }

    public static object GetCheckConfigurationSettings(this CheckConfiguration configuration)
    {
      object configurationSettings = (object) null;
      if (configuration == null)
        return (object) null;
      if (configuration.GetType() == typeof (ApprovalCheckConfiguration))
      {
        if (configuration is ApprovalCheckConfiguration checkConfiguration1)
          configurationSettings = (object) checkConfiguration1.Settings;
      }
      else if (configuration.GetType() == typeof (TaskCheckConfiguration) && configuration is TaskCheckConfiguration checkConfiguration2)
        configurationSettings = (object) checkConfiguration2.Settings;
      if (configurationSettings == null && configuration is GenericCheckConfiguration checkConfiguration3)
        configurationSettings = (object) checkConfiguration3.Settings;
      return configurationSettings;
    }

    public static string ToStringEx(this CheckConfiguration cc)
    {
      try
      {
        string str1 = "Generic";
        string str2 = "";
        switch (cc)
        {
          case ApprovalCheckConfiguration checkConfiguration1:
            str1 = "Approval";
            int? requiredApprovers = checkConfiguration1.Settings?.MinRequiredApprovers;
            ApprovalConfigSettings settings = checkConfiguration1.Settings;
            IEnumerable<\u003C\u003Ef__AnonymousType1<string, string>> datas;
            if (settings == null)
            {
              datas = null;
            }
            else
            {
              List<IdentityRef> approvers = settings.Approvers;
              datas = approvers != null ? approvers.Select(approver =>
              {
                SubjectDescriptor descriptor = approver.Descriptor;
                string identifier = descriptor.Identifier;
                descriptor = approver.Descriptor;
                string subjectType = descriptor.SubjectType;
                return new
                {
                  Identifier = identifier,
                  SubjectType = subjectType
                };
              }) : null;
            }
            str2 = JsonUtility.ToString((object) new
            {
              MinRequiredApprovers = requiredApprovers,
              ApporverList = datas
            });
            break;
          case TaskCheckConfiguration checkConfiguration2:
            str1 = "Task Check";
            string str3 = (string) null;
            string str4;
            if (checkConfiguration2.Settings?.Inputs != null && checkConfiguration2.Settings.Inputs.TryGetValue("waitForCompletion", out str4))
              str3 = str4;
            str2 = JsonUtility.ToString((object) new
            {
              DisplayName = checkConfiguration2.Settings?.DisplayName,
              TaskName = checkConfiguration2.Settings?.DefinitionRef?.Name,
              TaskId = checkConfiguration2.Settings?.DefinitionRef?.Id,
              TaskVersion = checkConfiguration2.Settings?.DefinitionRef?.Version,
              RetryInterval = (int?) checkConfiguration2.Settings?.RetryInterval,
              IsAsync = str3
            });
            break;
        }
        return JsonUtility.ToString((object) new
        {
          CheckConfigurationId = cc.Id,
          CheckType = str1,
          CheckTypeId = cc.Type?.Id,
          ResourceType = cc.Resource?.Type,
          ResourceId = cc.Resource?.Id,
          Timeout = cc.Timeout,
          Settings = str2,
          IsDisabled = cc.IsDisabled
        });
      }
      catch (Exception ex)
      {
        return string.Format("{0}() encountered {1}", (object) nameof (ToStringEx), (object) ex.GetType());
      }
    }
  }
}
