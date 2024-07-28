// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.DuplicateOperationValidator
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Edm.Validation
{
  internal class DuplicateOperationValidator
  {
    private readonly HashSetInternal<string> functionsParameterNameHash = new HashSetInternal<string>();
    private readonly HashSetInternal<string> functionsParameterTypeHash = new HashSetInternal<string>();
    private readonly HashSetInternal<string> actionsNameHash = new HashSetInternal<string>();
    private readonly ValidationContext context;

    internal DuplicateOperationValidator(ValidationContext context) => this.context = context;

    public static bool IsDuplicateOperation(
      IEdmOperation operation,
      IEnumerable<IEdmOperation> candidateDuplicateOperations)
    {
      DuplicateOperationValidator operationValidator = new DuplicateOperationValidator((ValidationContext) null);
      foreach (IEdmOperation duplicateOperation in candidateDuplicateOperations)
        operationValidator.ValidateNotDuplicate(duplicateOperation, true);
      return operationValidator.ValidateNotDuplicate(operation, true);
    }

    public bool ValidateNotDuplicate(IEdmOperation operation, bool skipError)
    {
      bool flag = false;
      string p0 = operation.FullName();
      if (operation is IEdmFunction function)
      {
        string thingToAdd1 = DuplicateOperationValidator.BuildInternalUniqueParameterNameFunctionString(function);
        if (this.functionsParameterNameHash.Contains(thingToAdd1))
        {
          flag = true;
          if (!skipError)
            this.context.AddError(function.Location(), EdmErrorCode.DuplicateFunctions, function.IsBound ? Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames((object) p0) : Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames((object) p0));
        }
        else
          this.functionsParameterNameHash.Add(thingToAdd1);
        string thingToAdd2 = DuplicateOperationValidator.BuildInternalUniqueParameterTypeFunctionString(function);
        if (this.functionsParameterTypeHash.Contains(thingToAdd2))
        {
          flag = true;
          if (!skipError)
            this.context.AddError(function.Location(), EdmErrorCode.DuplicateFunctions, function.IsBound ? Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes((object) p0) : Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes((object) p0));
        }
        else
          this.functionsParameterTypeHash.Add(thingToAdd2);
      }
      else
      {
        IEdmAction action = operation as IEdmAction;
        string thingToAdd = DuplicateOperationValidator.BuildInternalUniqueActionString(action);
        if (this.actionsNameHash.Contains(thingToAdd))
        {
          flag = true;
          if (!skipError)
            this.context.AddError(action.Location(), EdmErrorCode.DuplicateActions, action.IsBound ? Strings.EdmModel_Validator_Semantic_ModelDuplicateBoundActions((object) p0) : Strings.EdmModel_Validator_Semantic_ModelDuplicateUnBoundActions((object) p0));
        }
        else
          this.actionsNameHash.Add(thingToAdd);
      }
      return flag;
    }

    private static string BuildInternalUniqueParameterNameFunctionString(IEdmFunction function)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(function.IsBound);
      stringBuilder.Append("-");
      stringBuilder.Append(function.Namespace);
      stringBuilder.Append("-");
      stringBuilder.Append(function.Name);
      stringBuilder.Append("-");
      if (!function.Parameters.Any<IEdmOperationParameter>())
        return stringBuilder.ToString();
      if (function.IsBound)
      {
        IEdmOperationParameter operationParameter1 = function.Parameters.FirstOrDefault<IEdmOperationParameter>();
        stringBuilder.Append(operationParameter1.Type.FullName());
        stringBuilder.Append("-");
        foreach (IEdmOperationParameter operationParameter2 in (IEnumerable<IEdmOperationParameter>) function.Parameters.Skip<IEdmOperationParameter>(1).OrderBy<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Name)))
        {
          stringBuilder.Append(operationParameter2.Name);
          stringBuilder.Append("-");
        }
      }
      else
      {
        foreach (IEdmOperationParameter operationParameter in (IEnumerable<IEdmOperationParameter>) function.Parameters.OrderBy<IEdmOperationParameter, string>((Func<IEdmOperationParameter, string>) (p => p.Name)))
        {
          stringBuilder.Append(operationParameter.Name);
          stringBuilder.Append("-");
        }
      }
      return stringBuilder.ToString();
    }

    private static string BuildInternalUniqueParameterTypeFunctionString(IEdmFunction function)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(function.IsBound);
      stringBuilder.Append("-");
      stringBuilder.Append(function.Namespace);
      stringBuilder.Append("-");
      stringBuilder.Append(function.Name);
      stringBuilder.Append("-");
      foreach (IEdmOperationParameter parameter in function.Parameters)
      {
        stringBuilder.Append(parameter.Type.FullName());
        stringBuilder.Append("-");
      }
      return stringBuilder.ToString();
    }

    private static string BuildInternalUniqueActionString(IEdmAction action)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(action.IsBound);
      stringBuilder.Append("-");
      stringBuilder.Append(action.Namespace);
      stringBuilder.Append("-");
      stringBuilder.Append(action.Name);
      stringBuilder.Append("-");
      if (!action.Parameters.Any<IEdmOperationParameter>())
        return stringBuilder.ToString();
      if (action.IsBound)
      {
        IEdmOperationParameter operationParameter = action.Parameters.FirstOrDefault<IEdmOperationParameter>();
        stringBuilder.Append(operationParameter.Type.FullName());
      }
      return stringBuilder.ToString();
    }
  }
}
