// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ValidationStepDriver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ValidationStepDriver : ServicingStepDriver
  {
    public ValidationStepDriver(
      string pluginDirectory,
      IServicingOperationProvider operationProvider,
      ServicingContext servicingContext,
      params string[] servicingOperations)
      : base(servicingContext, pluginDirectory, operationProvider, servicingOperations)
    {
    }

    public ValidationStepDriver(
      ServicingContext servicingContext,
      string pluginDirectory,
      TokenStepDriver tokenStepDriver,
      params ServicingOperationSet[] operationSets)
      : base(servicingContext, pluginDirectory, tokenStepDriver, operationSets)
    {
    }

    public ValidationStepDriver(
      ServicingContext servicingContext,
      IStepPerformerProvider stepPerformerProvider,
      IServicingExecutionHandlerProvider servicingExecutionHandlerProvider,
      IServicingOperationProvider operationProvider,
      params string[] servicingOperations)
      : base(servicingContext, stepPerformerProvider, servicingExecutionHandlerProvider, operationProvider, servicingOperations)
    {
    }

    protected override void PerformOperations(int stepsToPerform)
    {
      int servicingStepCount = this.ServicingStepCount;
      int currentStep = 0;
      foreach (ServicingOperation operation in this.m_operations)
      {
        this.ThrowIfCanceled();
        ServicingStepState operationResolution = ServicingStepState.Passed;
        ServicingStepState groupResolution = ServicingStepState.NotExecuted;
        try
        {
          this.StartOperation(operation);
          foreach (ServicingStepGroup group in operation.Groups)
          {
            this.ThrowIfCanceled();
            ServicingStepGroupExecutionDecision status = this.StartStepGroup(operation, group);
            try
            {
              currentStep = this.PerformServicingSteps(stepsToPerform, servicingStepCount, currentStep, operation, group, group.Steps.ToArray(), status);
            }
            catch (TeamFoundationServicingException ex)
            {
            }
            catch (Exception ex)
            {
              groupResolution = ServicingStepState.Failed;
              throw;
            }
            finally
            {
              if (groupResolution != ServicingStepState.Failed)
                groupResolution = this.m_servicingContext.GroupResolution;
              switch (groupResolution)
              {
                case ServicingStepState.Validated:
                case ServicingStepState.ValidatedWithWarnings:
                  if (operationResolution == ServicingStepState.Validated)
                  {
                    operationResolution = ServicingStepState.ValidatedWithWarnings;
                    break;
                  }
                  break;
                case ServicingStepState.Failed:
                  operationResolution = ServicingStepState.Failed;
                  break;
                case ServicingStepState.PassedWithWarnings:
                  if (operationResolution == ServicingStepState.Passed)
                  {
                    operationResolution = ServicingStepState.PassedWithWarnings;
                    break;
                  }
                  break;
              }
              if (currentStep <= stepsToPerform)
                this.FinishStepGroup(operation, group, groupResolution);
            }
          }
        }
        finally
        {
          if (currentStep <= stepsToPerform)
            this.FinishOperation(operation, operationResolution);
          List<Exception> validationExceptions = this.GetValidationExceptions();
          if (validationExceptions != null && validationExceptions.Count > 0)
            throw new ValidationServicingException(operation.Name, validationExceptions.ToArray());
        }
      }
    }

    private int PerformServicingSteps(
      int stepsToPerform,
      int totalSteps,
      int currentStep,
      ServicingOperation servicingOperation,
      ServicingStepGroup stepGroup,
      ServicingStep[] steps,
      ServicingStepGroupExecutionDecision status)
    {
      foreach (ServicingStep step in steps)
      {
        this.ThrowIfCanceled();
        ++currentStep;
        try
        {
          ServicingStepState servicingStepState;
          if ((step.ExecuteAlways || status == ServicingStepGroupExecutionDecision.Execute) && (this.HostedDeployment && !step.OnPremOnly || !this.HostedDeployment && !step.HostedOnly) && (!step.DetachedOnly || this.m_isDetached) && (this.m_l2TestDeployment || !step.L2Only) && currentStep <= stepsToPerform)
          {
            servicingStepState = this.PerformServicingStep(step, this.m_servicingContext, stepGroup, servicingOperation, currentStep, totalSteps);
          }
          else
          {
            this.m_servicingContext.SkipStep(servicingOperation.Name, stepGroup.Name, step.Name);
            servicingStepState = ServicingStepState.Skipped;
          }
          if (servicingStepState != ServicingStepState.Skipped)
          {
            if (servicingStepState != ServicingStepState.PassedWithSkipChildren)
            {
              if (step.Steps != null)
              {
                if (step.Steps.Length != 0)
                  currentStep = this.PerformServicingSteps(stepsToPerform, totalSteps, currentStep, servicingOperation, stepGroup, step.Steps, status);
              }
            }
          }
        }
        catch (TeamFoundationServicingException ex)
        {
          this.GetValidationExceptions().Add((Exception) ex);
        }
      }
      return currentStep;
    }

    internal override int ServicingStepCount
    {
      get
      {
        int stepCount = 0;
        if (this.m_operations != null)
        {
          foreach (ServicingOperation operation in this.m_operations)
          {
            foreach (ServicingStepGroup group in operation.Groups)
              this.GetStepCount(ref stepCount, group.Steps.ToArray());
          }
        }
        return stepCount;
      }
    }

    private List<Exception> GetValidationExceptions()
    {
      object obj;
      List<Exception> validationExceptions;
      if (this.m_servicingContext.Items.TryGetValue(ServicingItemConstants.ValidationErrors, out obj))
      {
        validationExceptions = (List<Exception>) obj;
      }
      else
      {
        validationExceptions = new List<Exception>();
        this.m_servicingContext.Items[ServicingItemConstants.ValidationErrors] = (object) validationExceptions;
      }
      return validationExceptions;
    }

    private void GetStepCount(ref int stepCount, ServicingStep[] steps)
    {
      foreach (ServicingStep step in steps)
      {
        ++stepCount;
        if (step.Steps != null && step.Steps.Length != 0)
          this.GetStepCount(ref stepCount, step.Steps);
      }
    }
  }
}
