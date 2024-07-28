// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.TaskBuildConfigExceptionResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  public class TaskBuildConfigExceptionResolver
  {
    public IDictionary<Tuple<Guid, string>, IList<string>> TaskBuildConfigExceptions(
      IVssRequestContext requestContext)
    {
      string str1 = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) RegistryKeys.TasksBuildConfigExceptionsPath, true, (string) null);
      IDictionary<Tuple<Guid, string>, IList<string>> dictionary = (IDictionary<Tuple<Guid, string>, IList<string>>) new Dictionary<Tuple<Guid, string>, IList<string>>();
      if (!string.IsNullOrEmpty(str1))
      {
        requestContext.TraceAlways(10015566, "TaskHub", TaskResources.TaskBuildConfigExceptionInfo((object) str1));
        string str2 = str1;
        char[] chArray = new char[1]{ ';' };
        foreach (string str3 in str2.Split(chArray))
        {
          string[] strArray = str3.Split(',');
          Guid result;
          if (strArray.Length == 3 && Guid.TryParse(strArray[0], out result))
          {
            IList<string> stringList;
            if (!dictionary.TryGetValue(new Tuple<Guid, string>(result, strArray[1]), out stringList))
            {
              stringList = (IList<string>) new List<string>();
              dictionary.Add(new Tuple<Guid, string>(result, strArray[1]), stringList);
            }
            stringList.Add(strArray[2]);
          }
          else
            requestContext.TraceAlways(10015567, "TaskHub", TaskResources.InvalidTaskBuildConfigExceptionFormatWarning((object) str3));
        }
      }
      return dictionary;
    }

    public string TaskBuildConfigExceptionsToString(
      IDictionary<Tuple<Guid, string>, IList<string>> buildConfigExceptions)
    {
      if (buildConfigExceptions.IsNullOrEmpty<KeyValuePair<Tuple<Guid, string>, IList<string>>>())
        return string.Empty;
      List<string> values = new List<string>();
      foreach (Tuple<Guid, string> key in (IEnumerable<Tuple<Guid, string>>) buildConfigExceptions.Keys)
      {
        foreach (string str in (IEnumerable<string>) buildConfigExceptions[key])
          values.Add(string.Join(",", (object) key.Item1, (object) key.Item2, (object) str));
      }
      return string.Join(";", (IEnumerable<string>) values);
    }
  }
}
