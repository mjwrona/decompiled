// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ServicingOperationConverter
  {
    internal static ServicingOperation[] ToServicingOperations(
      IList<ServicingOperationData> operations,
      IList<ServicingOperationGroupData> operationGroups,
      IList<ServicingStepGroupData> stepGroups,
      IList<ServicingStepData> steps)
    {
      ArgumentUtility.CheckForNull<IList<ServicingOperationData>>(operations, nameof (operations));
      ArgumentUtility.CheckForNull<IList<ServicingOperationGroupData>>(operationGroups, nameof (operationGroups));
      ArgumentUtility.CheckForNull<IList<ServicingStepGroupData>>(stepGroups, nameof (stepGroups));
      ArgumentUtility.CheckForNull<IList<ServicingStepData>>(steps, nameof (steps));
      ServicingOperation[] array1 = new ServicingOperation[operations.Count];
      Dictionary<string, ServicingStep[]> dictionary1 = new Dictionary<string, ServicingStep[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IGrouping<string, ServicingStepData> source in steps.GroupBy<ServicingStepData, string>((Func<ServicingStepData, string>) (s => s.GroupName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        ServicingStepData[] array2 = source.OrderBy<ServicingStepData, int>((Func<ServicingStepData, int>) (s => s.OrderNumber)).ToArray<ServicingStepData>();
        ServicingStep[] servicingStepArray = new ServicingStep[array2.Length];
        for (int index = 0; index < array2.Length; ++index)
        {
          ServicingStepData servicingStepData = array2[index];
          servicingStepArray[index] = new ServicingStep(servicingStepData.GroupName, servicingStepData.StepName, servicingStepData.StepPerformer, servicingStepData.StepType, servicingStepData.StepData)
          {
            Options = servicingStepData.Options
          };
        }
        dictionary1[source.Key] = servicingStepArray;
      }
      Dictionary<string, ServicingOperation> dictionary2 = new Dictionary<string, ServicingOperation>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < operations.Count; ++index)
      {
        ServicingOperationData operation = operations[index];
        ServicingOperation servicingOperation = new ServicingOperation()
        {
          Name = operation.ServicingOperation,
          Target = operation.ServicingTarget
        };
        if (!string.IsNullOrEmpty(operation.Handlers))
        {
          string handlers = operation.Handlers;
          string[] separator = new string[1]{ ";" };
          foreach (string str in handlers.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            servicingOperation.ExecutionHandlers.Add(new ServicingExecutionHandlerData()
            {
              HandlerType = str
            });
        }
        array1[index] = servicingOperation;
        dictionary2.Add(servicingOperation.Name, servicingOperation);
      }
      Dictionary<string, ServicingStepGroupData> dictionary3 = stepGroups.ToDictionary<ServicingStepGroupData, string>((Func<ServicingStepGroupData, string>) (g => g.GroupName));
      foreach (IEnumerable<ServicingOperationGroupData> source in operationGroups.GroupBy<ServicingOperationGroupData, string>((Func<ServicingOperationGroupData, string>) (g => g.ServicingOperation), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
      {
        foreach (ServicingOperationGroupData operationGroupData in source.OrderBy<ServicingOperationGroupData, int>((Func<ServicingOperationGroupData, int>) (g => g.GroupOrderNumber)).ToArray<ServicingOperationGroupData>())
        {
          ServicingStepGroup servicingStepGroup = new ServicingStepGroup()
          {
            Name = operationGroupData.GroupName
          };
          string handlers = dictionary3[operationGroupData.GroupName].Handlers;
          if (!string.IsNullOrEmpty(handlers))
          {
            string str1 = handlers;
            string[] separator = new string[1]{ ";" };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
              servicingStepGroup.ExecutionHandlers.Add(new ServicingExecutionHandlerData()
              {
                HandlerType = str2
              });
          }
          ServicingStep[] collection;
          if (dictionary1.TryGetValue(operationGroupData.GroupName, out collection))
            servicingStepGroup.Steps.AddRange((IEnumerable<ServicingStep>) collection);
          dictionary2[operationGroupData.ServicingOperation].Groups.Add(servicingStepGroup);
        }
      }
      Array.Sort<ServicingOperation>(array1, (IComparer<ServicingOperation>) new ServicingOperationComparer());
      return array1;
    }
  }
}
