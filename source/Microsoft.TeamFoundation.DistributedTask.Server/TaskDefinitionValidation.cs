// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskDefinitionValidation
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Core.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class TaskDefinitionValidation
  {
    public static void ValidateTaskDefinition(
      IVssRequestContext requestContext,
      TaskDefinition taskDefinition)
    {
      TaskDefinitionValidator.CheckTaskDefinition(taskDefinition, (Action<TaskInputDefinition>) (x => TaskDefinitionValidation.TaskInputDefinitionValidator(requestContext, x)));
    }

    private static void TaskInputDefinitionValidator(
      IVssRequestContext requestContext,
      TaskInputDefinition taskInputDefinition)
    {
      IInputValidationService service = requestContext.GetService<IInputValidationService>();
      if (taskInputDefinition.Validation == null)
        return;
      InputValidationRequest validationRequest = TaskDefinitionValidation.GetTaskInputValidationRequest(taskInputDefinition);
      service.ValidateInputs(requestContext, validationRequest);
      ValidationItem validationItem = validationRequest.Inputs.First<KeyValuePair<string, ValidationItem>>().Value;
      if (validationItem.IsValid.HasValue && !validationItem.IsValid.Value)
        throw new TaskDefinitionInvalidException(TaskResources.TaskInputValidationInvalid((object) taskInputDefinition.Name, (object) taskInputDefinition.Validation.Expression, (object) validationItem.Reason));
    }

    private static InputValidationRequest GetTaskInputValidationRequest(
      TaskInputDefinition taskInputDefinition)
    {
      InputValidationRequest validationRequest = new InputValidationRequest();
      IDictionary<string, ValidationItem> inputs = validationRequest.Inputs;
      string name = taskInputDefinition.Name;
      InputValidationItem inputValidationItem = new InputValidationItem();
      inputValidationItem.Value = taskInputDefinition.Validation.Expression;
      inputs.Add(name, (ValidationItem) inputValidationItem);
      return validationRequest;
    }
  }
}
