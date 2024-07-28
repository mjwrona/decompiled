// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.TaskOverrideResolver2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal class TaskOverrideResolver2
  {
    public static void WriteOverrides(
      Action<string> infoWriter,
      IVssRequestContext requestContext,
      IDictionary<Guid, IDictionary<string, string>> overrideDictionary,
      bool forDeployment)
    {
      PipelineBuilderService pipelineBuilderService = new PipelineBuilderService();
      string str = TaskOverrideResolver2.TaskOverridesToString(overrideDictionary);
      if (forDeployment)
      {
        IVssRequestContext deploymentHostContext = requestContext.ToDeploymentHostContext();
        deploymentHostContext.GetService<IVssRegistryService>().SetValue<string>(deploymentHostContext, RegistryKeys.TasksVersionOverridePath, str);
      }
      else
        requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, RegistryKeys.TasksVersionOverridePath, str);
      if (infoWriter == null)
        return;
      infoWriter("Task Overrides New Key Value: " + str);
    }

    public static IDictionary<Guid, IDictionary<string, string>> TaskOverrides(
      IVssRequestContext requestContext)
    {
      bool regKeyNotExist;
      IDictionary<Guid, IDictionary<string, string>> dictionary1 = TaskOverrideResolver2.TaskOverrides(requestContext, false, out regKeyNotExist);
      IDictionary<Guid, IDictionary<string, string>> dictionary2 = TaskOverrideResolver2.TaskOverrides(requestContext, true, out regKeyNotExist);
      Dictionary<Guid, IDictionary<string, string>> dictionary3 = new Dictionary<Guid, IDictionary<string, string>>();
      foreach (KeyValuePair<Guid, IDictionary<string, string>> keyValuePair in (IEnumerable<KeyValuePair<Guid, IDictionary<string, string>>>) dictionary2)
        dictionary3.Add(keyValuePair.Key, (IDictionary<string, string>) new Dictionary<string, string>(keyValuePair.Value));
      foreach (KeyValuePair<Guid, IDictionary<string, string>> keyValuePair1 in (IEnumerable<KeyValuePair<Guid, IDictionary<string, string>>>) dictionary1)
      {
        Guid key1 = keyValuePair1.Key;
        IDictionary<string, string> dictionary4;
        if (dictionary2.TryGetValue(key1, out dictionary4))
        {
          foreach (KeyValuePair<string, string> keyValuePair2 in (IEnumerable<KeyValuePair<string, string>>) keyValuePair1.Value)
          {
            string key2 = keyValuePair2.Key;
            string str1 = keyValuePair2.Value;
            string str2;
            if (dictionary4.TryGetValue(key2, out str2))
            {
              if (!(str2 == str1))
                dictionary3[key1][key2] = str1;
            }
            else
              dictionary3[key1].Add(key2, str1);
          }
        }
        else
          dictionary3.Add(key1, (IDictionary<string, string>) new Dictionary<string, string>(keyValuePair1.Value));
      }
      return (IDictionary<Guid, IDictionary<string, string>>) dictionary3;
    }

    public static IDictionary<Guid, IDictionary<string, string>> TaskOverrides(
      IVssRequestContext requestContext,
      bool forDeployment,
      out bool regKeyNotExist)
    {
      string taskOverrides;
      if (forDeployment)
      {
        IVssRequestContext deploymentHostContext = requestContext.ToDeploymentHostContext();
        taskOverrides = deploymentHostContext.GetService<IVssRegistryService>().GetValue<string>(deploymentHostContext, (RegistryQuery) RegistryKeys.TasksVersionOverridePath, false, (string) null);
      }
      else
        taskOverrides = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) RegistryKeys.TasksVersionOverridePath, false, (string) null);
      regKeyNotExist = taskOverrides == null;
      IDictionary<Guid, IDictionary<string, string>> dictionary;
      if (!string.IsNullOrEmpty(taskOverrides))
      {
        requestContext.TraceAlways(10015561, "TaskHub", TaskResources.TaskOverrideInfo((object) taskOverrides));
        dictionary = TaskOverrideResolver2.ParseTask((Action<string>) (taskOverride => requestContext.TraceAlways(10015560, "TaskHub", TaskResources.InvalidTaskOverrideFormatWarning((object) taskOverride))), (Action<string>) (taskOverride => requestContext.TraceAlways(10015560, "TaskHub", "Duplicate taskId " + taskOverride)), taskOverrides);
      }
      else
        dictionary = (IDictionary<Guid, IDictionary<string, string>>) new Dictionary<Guid, IDictionary<string, string>>();
      return dictionary;
    }

    internal static IDictionary<Guid, IDictionary<string, string>> ParseTask(
      Action<string> invalidTaskFormatAction,
      Action<string> dupTaskFormatAction,
      string taskOverrides)
    {
      IDictionary<Guid, IDictionary<string, string>> task = (IDictionary<Guid, IDictionary<string, string>>) new Dictionary<Guid, IDictionary<string, string>>();
      string str1 = taskOverrides;
      char[] chArray = new char[1]{ ';' };
      foreach (string str2 in str1.Split(chArray))
      {
        string[] strArray = str2.Split(',');
        Guid result;
        if (strArray.Length == 3 && Guid.TryParse(strArray[0], out result))
        {
          IDictionary<string, string> dictionary;
          if (!task.TryGetValue(result, out dictionary))
          {
            dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
            task.Add(result, dictionary);
          }
          if (dictionary.ContainsKey(strArray[1]))
            dupTaskFormatAction(str2);
          else
            dictionary.Add(strArray[1], strArray[2]);
        }
        else if (invalidTaskFormatAction != null)
          invalidTaskFormatAction(str2);
      }
      return task;
    }

    public static string TaskOverridesToString(
      IDictionary<Guid, IDictionary<string, string>> overrides)
    {
      List<string> values = new List<string>();
      foreach (Guid key1 in (IEnumerable<Guid>) overrides.Keys)
      {
        foreach (string key2 in (IEnumerable<string>) overrides[key1].Keys)
          values.Add(string.Join(",", (object) key1, (object) key2, (object) overrides[key1][key2]));
      }
      return string.Join(";", (IEnumerable<string>) values);
    }
  }
}
