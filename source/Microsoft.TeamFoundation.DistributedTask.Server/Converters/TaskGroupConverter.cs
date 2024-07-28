// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.TaskGroupConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class TaskGroupConverter
  {
    public static TaskGroup ToTaskGroup(
      this TaskGroupCreateParameter taskGroupCreateParameter)
    {
      TaskGroup taskGroup1 = new TaskGroup();
      taskGroup1.DefinitionType = "metaTask";
      taskGroup1.Name = taskGroupCreateParameter.Name;
      taskGroup1.FriendlyName = taskGroupCreateParameter.FriendlyName;
      taskGroup1.Author = taskGroupCreateParameter.Author;
      taskGroup1.Description = taskGroupCreateParameter.Description;
      taskGroup1.Tasks = taskGroupCreateParameter.Tasks;
      taskGroup1.ParentDefinitionId = taskGroupCreateParameter.ParentDefinitionId;
      taskGroup1.IconUrl = taskGroupCreateParameter.IconUrl;
      taskGroup1.InstanceNameFormat = taskGroupCreateParameter.InstanceNameFormat;
      taskGroup1.Category = taskGroupCreateParameter.Category;
      taskGroup1.Version = taskGroupCreateParameter.Version;
      taskGroup1.CreatedBy = new IdentityRef();
      TaskGroup taskGroup2 = taskGroup1;
      if (taskGroupCreateParameter.Inputs.Any<TaskInputDefinition>())
      {
        taskGroup2.Inputs.Clear();
        foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) taskGroupCreateParameter.Inputs)
          taskGroup2.Inputs.Add(input);
      }
      if (taskGroupCreateParameter.RunsOn.Any<string>())
      {
        taskGroup2.RunsOn.Clear();
        foreach (string str in (IEnumerable<string>) taskGroupCreateParameter.RunsOn)
          taskGroup2.RunsOn.Add(str);
      }
      if (taskGroupCreateParameter.ParentDefinitionId.HasValue)
        taskGroup2.Version.IsTest = true;
      return taskGroup2;
    }

    public static TaskGroup ToTaskGroup(
      this TaskGroupUpdateParameter taskGroupUpdateParameter)
    {
      TaskGroup taskGroup1 = new TaskGroup();
      taskGroup1.DefinitionType = "metaTask";
      taskGroup1.Id = taskGroupUpdateParameter.Id;
      taskGroup1.Name = taskGroupUpdateParameter.Name;
      taskGroup1.FriendlyName = taskGroupUpdateParameter.FriendlyName;
      taskGroup1.Author = taskGroupUpdateParameter.Author;
      taskGroup1.Description = taskGroupUpdateParameter.Description;
      taskGroup1.Comment = taskGroupUpdateParameter.Comment;
      taskGroup1.Tasks = taskGroupUpdateParameter.Tasks;
      taskGroup1.ParentDefinitionId = taskGroupUpdateParameter.ParentDefinitionId;
      taskGroup1.IconUrl = taskGroupUpdateParameter.IconUrl;
      taskGroup1.InstanceNameFormat = taskGroupUpdateParameter.InstanceNameFormat;
      taskGroup1.Category = taskGroupUpdateParameter.Category;
      taskGroup1.Revision = taskGroupUpdateParameter.Revision;
      taskGroup1.Version = taskGroupUpdateParameter.Version;
      taskGroup1.CreatedBy = new IdentityRef();
      taskGroup1.ModifiedBy = new IdentityRef();
      taskGroup1.CreatedOn = new DateTime();
      taskGroup1.ModifiedOn = new DateTime();
      TaskGroup taskGroup2 = taskGroup1;
      if (taskGroupUpdateParameter.Inputs.Any<TaskInputDefinition>())
      {
        foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) taskGroupUpdateParameter.Inputs)
          taskGroup2.Inputs.Add(input);
      }
      if (taskGroupUpdateParameter.RunsOn.Any<string>())
      {
        taskGroup2.RunsOn.Clear();
        foreach (string str in (IEnumerable<string>) taskGroupUpdateParameter.RunsOn)
          taskGroup2.RunsOn.Add(str);
      }
      return taskGroup2;
    }

    public static TaskGroupUpdateParameter ToTaskGroupUpdateParameter(this TaskGroup taskGroup)
    {
      TaskGroupUpdateParameter groupUpdateParameter = new TaskGroupUpdateParameter()
      {
        Id = taskGroup.Id,
        Name = taskGroup.Name,
        Description = taskGroup.Description,
        Comment = taskGroup.Comment,
        ParentDefinitionId = taskGroup.ParentDefinitionId
      };
      if (taskGroup.Inputs.Any<TaskInputDefinition>())
      {
        foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) taskGroup.Inputs)
          groupUpdateParameter.Inputs.Add(input);
      }
      if (taskGroup.Tasks.Any<TaskGroupStep>())
      {
        foreach (TaskGroupStep task in (IEnumerable<TaskGroupStep>) taskGroup.Tasks)
          groupUpdateParameter.Tasks.Add(task);
      }
      return groupUpdateParameter;
    }
  }
}
