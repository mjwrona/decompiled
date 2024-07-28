// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation.InputValidator
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class InputValidator : IInputValidator
  {
    public InputValidationResult Validate(InputValidationContext context)
    {
      if (string.IsNullOrEmpty(context.Expression))
        return InputValidationResult.Succeeded;
      InputValidationResult validationResult = new InputValidationResult();
      try
      {
        IExpressionNode tree = new ExpressionParser().CreateTree(context.Expression, context.TraceWriter, (IEnumerable<INamedValueInfo>) InputValidationConstants.NamedValues, (IEnumerable<IFunctionInfo>) InputValidationConstants.Functions);
        validationResult.IsValid = !context.Evaluate || tree.Evaluate<bool>(context.TraceWriter, context.SecretMasker, (object) context, context.EvaluationOptions);
      }
      catch (Exception ex) when (
      {
        // ISSUE: unable to correctly present filter
        int num;
        switch (ex)
        {
          case ParseException _:
          case RegularExpressionInvalidOptionsException _:
            num = 1;
            break;
          default:
            num = ex is NotSupportedException ? 1 : 0;
            break;
        }
        if ((uint) num > 0U)
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
        validationResult.Reason = ex.Message;
      }
      return validationResult;
    }
  }
}
