// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.InputValidationService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class InputValidationService : IInputValidationService, IVssFrameworkService
  {
    public bool ValidateInput(
      IVssRequestContext requestContext,
      string type,
      string value,
      out string reason)
    {
      ArgumentUtility.CheckForNull<string>(type, nameof (type), "DistributedTask");
      reason = (string) null;
      using (new MethodScope(requestContext, nameof (InputValidationService), nameof (ValidateInput)))
      {
        IInputValidator extension = requestContext.GetExtension<IInputValidator>((Func<IInputValidator, bool>) (v => string.Equals(v.Type, type, StringComparison.Ordinal)));
        if (extension == null)
          return true;
        ValidationItem validationItem;
        switch (type)
        {
          case "expression":
            ExpressionValidationItem expressionValidationItem = new ExpressionValidationItem();
            expressionValidationItem.Value = value;
            validationItem = (ValidationItem) expressionValidationItem;
            break;
          case "input":
            InputValidationItem inputValidationItem = new InputValidationItem();
            inputValidationItem.Value = value;
            validationItem = (ValidationItem) inputValidationItem;
            break;
          default:
            return true;
        }
        return extension.ValidateInput(requestContext, validationItem, out reason);
      }
    }

    public void ValidateInputs(
      IVssRequestContext requestContext,
      InputValidationRequest inputValidationRequest)
    {
      ArgumentUtility.CheckForNull<InputValidationRequest>(inputValidationRequest, nameof (inputValidationRequest), "DistributedTask");
      using (new MethodScope(requestContext, nameof (InputValidationService), nameof (ValidateInputs)))
      {
        if (inputValidationRequest.Inputs.Count <= 0)
          return;
        using (IDisposableReadOnlyList<IInputValidator> extensions = requestContext.GetExtensions<IInputValidator>())
        {
          Dictionary<string, IInputValidator> dictionary = extensions.ToDictionary<IInputValidator, string>((Func<IInputValidator, string>) (v => v.Type), (IEqualityComparer<string>) StringComparer.Ordinal);
          foreach (KeyValuePair<string, ValidationItem> input in (IEnumerable<KeyValuePair<string, ValidationItem>>) inputValidationRequest.Inputs)
          {
            IInputValidator inputValidator = (IInputValidator) null;
            if (dictionary.TryGetValue(input.Value.Type, out inputValidator))
            {
              string reason;
              bool flag = inputValidator.ValidateInput(requestContext, input.Value, out reason);
              input.Value.IsValid = new bool?(flag);
              if (flag)
                reason = (string) null;
              else if (string.IsNullOrEmpty(reason))
                reason = input.Value.Reason ?? TaskResources.TaskInputValidationReason((object) input.Value.Value);
              input.Value.Reason = reason;
            }
            else
              input.Value.IsValid = new bool?(true);
          }
        }
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
