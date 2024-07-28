// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepDriver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingStepDriver : IDisposable
  {
    private static readonly string s_area = "Servicing";
    private static readonly string s_layer = nameof (ServicingStepDriver);
    protected List<ServicingOperation> m_operations;
    private string m_servicingFaultInjection;
    private bool m_enableExtendedValidation;
    private HashSet<string> m_servicingSkipSteps;
    private bool m_runEachStepTwice;
    private bool m_dbOnly;
    protected bool m_isDetached;
    protected bool m_l2TestDeployment;
    private TimeSpan m_delayBetweenSteps;
    private TokenStepDriver m_tokenStepDriver;
    protected ServicingOperationSet[] m_operationSets;
    private IDisposableReadOnlyList<IServicingStepValidator> m_extendedValidators;
    protected readonly IStepPerformerProvider m_stepPerformerProvider;
    protected readonly ServicingContext m_servicingContext;
    protected readonly List<IServicingStepGroupExecutionHandler> m_stepGroupExecutionHandlers = new List<IServicingStepGroupExecutionHandler>();
    protected readonly List<IServicingOperationExecutionHandler> m_operationExecutionHandlers = new List<IServicingOperationExecutionHandler>();
    protected readonly IServicingExecutionHandlerProvider m_servicingExecutionHandlerProvider;
    private const string c_validationInjectionEnvironmentVariable = "TFS_INTERNAL_SERVICING_STEP_VALIDATION";
    private const string s_faultInjectionEnvironmentVariable = "TFS_INTERNAL_SERVICING_STEP_FAILURE";
    private const string s_skipStepsEnvironmentVariable = "TFS_INTERNAL_SERVICING_STEP_SKIPSTEPS";
    private const string s_runEachStepTwiceEnvironmentVariable = "TFS_RUN_EACH_STEP_TWICE";

    public ServicingStepDriver(
      ServicingContext servicingContext,
      string pluginDirectory,
      IServicingOperationProvider operationProvider,
      params string[] servicingOperations)
      : this(servicingContext, (IStepPerformerProvider) ExtensionStepPerformerProvider.Get(pluginDirectory, servicingContext.TFLogger), (IServicingExecutionHandlerProvider) new PluginServicingExecutionHandlerProvider(pluginDirectory), operationProvider, servicingOperations)
    {
    }

    public ServicingStepDriver(
      ServicingContext servicingContext,
      string pluginDirectory,
      params ServicingOperationSet[] operationSets)
      : this(servicingContext, (IStepPerformerProvider) ExtensionStepPerformerProvider.Get(pluginDirectory, servicingContext.TFLogger), (IServicingExecutionHandlerProvider) new PluginServicingExecutionHandlerProvider(pluginDirectory), operationSets)
    {
    }

    public ServicingStepDriver(
      ServicingContext servicingContext,
      string pluginDirectory,
      TokenStepDriver tokenStepDriver,
      params ServicingOperationSet[] operationSets)
      : this(servicingContext, (IStepPerformerProvider) ExtensionStepPerformerProvider.Get(pluginDirectory, servicingContext.TFLogger), (IServicingExecutionHandlerProvider) new PluginServicingExecutionHandlerProvider(pluginDirectory), operationSets)
    {
      this.m_tokenStepDriver = tokenStepDriver;
    }

    public ServicingStepDriver(
      ServicingContext servicingContext,
      IStepPerformerProvider stepPerformerProvider,
      IServicingExecutionHandlerProvider servicingExecutionHandlerProvider,
      IServicingOperationProvider operationProvider,
      params string[] servicingOperations)
      : this(servicingContext, stepPerformerProvider, servicingExecutionHandlerProvider, new ServicingOperationSet(operationProvider, servicingOperations))
    {
    }

    public ServicingStepDriver(
      ServicingContext servicingContext,
      IStepPerformerProvider stepPerformerProvider,
      IServicingExecutionHandlerProvider servicingExecutionHandlerProvider,
      params ServicingOperationSet[] operationSets)
    {
      ArgumentUtility.CheckForNull<IStepPerformerProvider>(stepPerformerProvider, nameof (stepPerformerProvider));
      ArgumentUtility.CheckForNull<ServicingOperationSet[]>(operationSets, nameof (operationSets));
      ArgumentUtility.CheckForNull<ServicingContext>(servicingContext, nameof (servicingContext));
      this.m_operationSets = operationSets;
      this.m_stepPerformerProvider = stepPerformerProvider;
      this.m_servicingContext = servicingContext;
      this.m_servicingExecutionHandlerProvider = servicingExecutionHandlerProvider;
      this.m_isDetached = this.m_servicingContext.OperationClass != null && this.m_servicingContext.OperationClass.Equals("AttachCollection", StringComparison.Ordinal);
      string str1 = servicingContext.GetTokenOrDefault<string>("TFS_RUN_EACH_STEP_TWICE", string.Empty);
      if (string.IsNullOrEmpty(str1))
        str1 = Environment.GetEnvironmentVariable("TFS_RUN_EACH_STEP_TWICE", EnvironmentVariableTarget.Machine);
      bool.TryParse(str1, out this.m_runEachStepTwice);
      bool.TryParse(Environment.GetEnvironmentVariable("TFS_DBONLY_UPGRADE"), out this.m_dbOnly);
      try
      {
        double num = 0.0;
        if (this.m_servicingContext.DeploymentRequestContext != null && this.m_servicingContext.DeploymentRequestContext.ServiceHost != null && !string.IsNullOrEmpty(this.m_servicingContext.OperationClass))
        {
          IVssRequestContext deploymentRequestContext = this.m_servicingContext.DeploymentRequestContext;
          CachedRegistryService service = deploymentRequestContext.GetService<CachedRegistryService>();
          CachedRegistryService registryService1 = service;
          IVssRequestContext requestContext1 = deploymentRequestContext;
          RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.ServicingCollectionFaultInjection;
          ref RegistryQuery local1 = ref registryQuery;
          string empty = string.Empty;
          this.m_servicingFaultInjection = registryService1.GetValue(requestContext1, in local1, true, empty);
          string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, FrameworkServerConstants.ServicingCollectionDelay, (object) this.m_servicingContext.OperationClass);
          CachedRegistryService registryService2 = service;
          IVssRequestContext requestContext2 = deploymentRequestContext;
          registryQuery = (RegistryQuery) str2;
          ref RegistryQuery local2 = ref registryQuery;
          num = registryService2.GetValue<double>(requestContext2, in local2, 0.0);
        }
        this.m_delayBetweenSteps = TimeSpan.FromSeconds(num);
        this.m_enableExtendedValidation = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TFS_INTERNAL_SERVICING_STEP_VALIDATION"));
        if (this.m_enableExtendedValidation)
        {
          servicingContext.LogInfo("TFS_INTERNAL_SERVICING_STEP_VALIDATION environmental variable has been set.  Loading extended validators...");
          this.m_extendedValidators = VssExtensionManagementService.GetExtensionsRaw<IServicingStepValidator>(VssExtensionManagementService.DefaultPluginPath);
          foreach (IServicingStepValidator extendedValidator in (IEnumerable<IServicingStepValidator>) this.m_extendedValidators)
            servicingContext.LogInfo("Loaded extended validator: '{0}'", new object[1]
            {
              (object) extendedValidator.GetType().AssemblyQualifiedName
            });
          servicingContext.LogInfo(string.Format("...{0} extended validators loaded.", (object) this.m_extendedValidators.Count));
        }
        string environmentVariable1 = Environment.GetEnvironmentVariable("TFS_INTERNAL_SERVICING_STEP_FAILURE");
        if (!string.IsNullOrEmpty(environmentVariable1))
          this.m_servicingFaultInjection = Environment.ExpandEnvironmentVariables(environmentVariable1);
        string environmentVariable2 = Environment.GetEnvironmentVariable("TFS_INTERNAL_SERVICING_STEP_SKIPSTEPS");
        if (!string.IsNullOrEmpty(environmentVariable2))
          this.m_servicingSkipSteps = new HashSet<string>((IEnumerable<string>) Environment.ExpandEnvironmentVariables(environmentVariable2).Split(','), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        this.m_l2TestDeployment = stepPerformerProvider.TryGetStepPerformer("L2Test", out IStepPerformer _);
        servicingContext.Tokens[ServicingTokenConstants.IsL2TestDeployment] = XmlConvert.ToString(this.m_l2TestDeployment);
      }
      catch (Exception ex)
      {
      }
    }

    public List<IServicingStepGroupExecutionHandler> ServicingStepGroupExecutionHandlers => this.m_stepGroupExecutionHandlers;

    public List<IServicingOperationExecutionHandler> ServicingOperationExecutionHandlers => this.m_operationExecutionHandlers;

    public bool HostedDeployment { get; set; }

    public void Execute() => this.Execute(int.MaxValue);

    public void Dispose()
    {
      if (this.m_servicingExecutionHandlerProvider != null && this.m_servicingExecutionHandlerProvider is IDisposable executionHandlerProvider)
        executionHandlerProvider.Dispose();
      if (this.m_stepPerformerProvider != null && this.m_stepPerformerProvider is IDisposable performerProvider)
        performerProvider.Dispose();
      if (this.m_extendedValidators == null)
        return;
      this.m_extendedValidators.Dispose();
    }

    public void LogLoadedSteps()
    {
      this.GetServicingOperations();
      this.m_servicingContext.LogInfo("Logging all steps that have been loaded, not all are guarenteed to execute:");
      this.m_servicingContext.LogInfo("Target.Operation.Group.Step");
      foreach (ServicingOperation operation in this.m_operations)
      {
        foreach (ServicingStepGroup group in operation.Groups)
        {
          foreach (ServicingStep step in group.Steps)
            this.m_servicingContext.LogInfo("\t\t{0}.{1}.{2}.{3}", new object[4]
            {
              (object) operation.Target,
              (object) operation.Name,
              (object) group.Name,
              (object) step.Name
            });
        }
      }
    }

    internal void Execute(int numberOfStepsToPerform)
    {
      try
      {
        this.GetServicingOperations();
        numberOfStepsToPerform = Math.Min(this.ServicingStepCount, numberOfStepsToPerform);
        this.PerformOperations(numberOfStepsToPerform);
      }
      catch (TeamFoundationServicingException ex)
      {
        throw;
      }
      catch (AggregateException ex)
      {
        ReadOnlyCollection<Exception> innerExceptions = ex.Flatten().InnerExceptions;
        foreach (Exception exception in innerExceptions)
        {
          this.m_servicingContext.LogInfo(exception.ToReadableStackTrace());
          this.m_servicingContext.Error(exception.Message);
        }
        if (innerExceptions.Count == 1)
          throw new TeamFoundationServicingException(innerExceptions[0].Message, (Exception) ex);
        throw new TeamFoundationServicingException(ex.Message, (Exception) ex);
      }
      catch (Exception ex)
      {
        this.m_servicingContext.Error(ex.Message);
        this.m_servicingContext.LogInfo(ex.ToReadableStackTrace());
        throw new TeamFoundationServicingException(ex.Message, ex);
      }
    }

    private void GetServicingOperations()
    {
      this.ThrowIfCanceled();
      if (this.m_operations != null)
        return;
      this.m_operations = new List<ServicingOperation>();
      foreach (ServicingOperationSet operationSet in this.m_operationSets)
        this.m_operations.AddRange((IEnumerable<ServicingOperation>) operationSet.GetServicingOperations());
    }

    protected virtual void PerformOperations(int stepsToPerform)
    {
      int servicingStepCount = this.ServicingStepCount;
      int stepNumber = 0;
      this.m_servicingContext.Items[ServicingItemConstants.HostedDeployment] = (object) this.HostedDeployment;
      for (int index1 = 0; index1 < this.m_operations.Count; ++index1)
      {
        ServicingOperation operation = this.m_operations[index1];
        this.ThrowIfCanceled();
        ServicingStepState operationResolution = ServicingStepState.Passed;
        ServicingStepState groupResolution = ServicingStepState.NotExecuted;
        bool flag = false;
        try
        {
          this.StartOperation(operation);
          for (int index2 = 0; index2 < operation.Groups.Count; ++index2)
          {
            ServicingStepGroup group = operation.Groups[index2];
            this.ThrowIfCanceled();
            ServicingStepGroupExecutionDecision executionDecision = this.StartStepGroup(operation, group);
            try
            {
              for (int index3 = 0; index3 < group.Steps.Count; ++index3)
              {
                ServicingStep step = group.Steps[index3];
                if (step.Steps != null)
                  throw new TeamFoundationServicingException("Only Validation steps have children elements. Validation steps require the ValidationStepDriver. Group Name: " + step.GroupName + " Step Name: " + step.Name);
                this.ThrowIfCanceled();
                if (++stepNumber > stepsToPerform)
                {
                  flag = true;
                  return;
                }
                if (stepNumber == stepsToPerform && (stepsToPerform < servicingStepCount || this.m_runEachStepTwice) && !string.Equals(group.Name, "Repair", StringComparison.OrdinalIgnoreCase))
                  this.m_servicingContext.Tokens[ServicingTokenConstants.TestSqlScriptRerunnability] = "Y";
                if ((step.ExecuteAlways || executionDecision == ServicingStepGroupExecutionDecision.Execute) && (this.HostedDeployment && !step.OnPremOnly || !this.HostedDeployment && !step.HostedOnly) && (!step.DetachedOnly || this.m_isDetached) && (this.m_l2TestDeployment || !step.L2Only) && !this.SkipStepRequestedForTesting(step, stepNumber))
                {
                  int num = (int) this.PerformServicingStep(step, this.m_servicingContext, group, operation, stepNumber, servicingStepCount);
                }
                else
                  this.m_servicingContext.SkipStep(operation.Name, group.Name, step.Name);
              }
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
              if ((stepNumber < stepsToPerform || stepNumber == servicingStepCount) && !flag)
                this.FinishStepGroup(operation, group, groupResolution);
            }
          }
        }
        catch (Exception ex)
        {
          operationResolution = ServicingStepState.Failed;
          throw;
        }
        finally
        {
          if ((stepNumber < stepsToPerform || stepNumber == servicingStepCount) && !flag)
            this.FinishOperation(operation, operationResolution);
        }
      }
    }

    protected void StartOperation(ServicingOperation servicingOperation)
    {
      this.m_servicingContext.StartOperation(servicingOperation);
      foreach (IServicingOperationExecutionHandler executionHandler in this.GetServicingOperationExecutionHandlers(servicingOperation))
        executionHandler.StartOperation(this.m_servicingContext, servicingOperation);
    }

    protected void FinishOperation(
      ServicingOperation servicingOperation,
      ServicingStepState operationResolution)
    {
      foreach (IServicingOperationExecutionHandler executionHandler in this.GetServicingOperationExecutionHandlers(servicingOperation))
        executionHandler.FinishOperation(this.m_servicingContext, servicingOperation, operationResolution);
      this.m_servicingContext.FinishOperation();
    }

    protected virtual ServicingStepGroupExecutionDecision StartStepGroup(
      ServicingOperation servicingOperation,
      ServicingStepGroup stepGroup)
    {
      if (this.m_tokenStepDriver != null)
        return this.m_tokenStepDriver.StartStepGroup(servicingOperation, stepGroup);
      this.m_servicingContext.StartStepGroup(servicingOperation.Name, stepGroup);
      ServicingStepGroupExecutionDecision executionDecision1 = ServicingStepGroupExecutionDecision.Execute;
      foreach (IServicingStepGroupExecutionHandler executionHandler in this.GetServicingOperationExecutionHandlers(servicingOperation).Cast<IServicingStepGroupExecutionHandler>().Union<IServicingStepGroupExecutionHandler>(this.GetServicingStepGroupExecutionHandlers(stepGroup)).ToArray<IServicingStepGroupExecutionHandler>())
      {
        ServicingStepGroupExecutionDecision executionDecision2 = executionHandler.StartStepGroup(this.m_servicingContext, servicingOperation, stepGroup);
        switch (executionDecision2)
        {
          case ServicingStepGroupExecutionDecision.Skip:
            if (executionDecision1 != ServicingStepGroupExecutionDecision.SkipSucceededPreviously)
            {
              executionDecision1 = executionDecision2;
              break;
            }
            break;
          case ServicingStepGroupExecutionDecision.SkipSucceededPreviously:
            executionDecision1 = executionDecision2;
            break;
        }
      }
      return executionDecision1;
    }

    protected virtual void FinishStepGroup(
      ServicingOperation servicingOperation,
      ServicingStepGroup stepGroup,
      ServicingStepState groupResolution)
    {
      if (this.m_tokenStepDriver != null)
      {
        this.m_tokenStepDriver.FinishStepGroup(servicingOperation, stepGroup, groupResolution);
      }
      else
      {
        foreach (IServicingStepGroupExecutionHandler executionHandler in this.GetServicingOperationExecutionHandlers(servicingOperation).Cast<IServicingStepGroupExecutionHandler>().Union<IServicingStepGroupExecutionHandler>(this.GetServicingStepGroupExecutionHandlers(stepGroup)).ToArray<IServicingStepGroupExecutionHandler>())
          executionHandler.FinishStepGroup(this.m_servicingContext, servicingOperation, stepGroup, groupResolution);
        this.m_servicingContext.FinishStepGroup();
      }
    }

    private bool SkipStepRequestedForTesting(ServicingStep step, int stepNumber)
    {
      bool flag = false;
      if (this.m_servicingSkipSteps != null && this.m_servicingSkipSteps.Count > 0)
        flag = this.m_servicingSkipSteps.Contains(step.Name) || this.m_servicingSkipSteps.Contains(stepNumber.ToString());
      if (flag)
        this.m_servicingContext.LogInfo(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Step #{0} '{1}' requested to be skipped for testing", (object) stepNumber, (object) step.Name));
      return flag;
    }

    private IEnumerable<IServicingOperationExecutionHandler> GetServicingOperationExecutionHandlers(
      ServicingOperation servicingOperation)
    {
      List<IServicingOperationExecutionHandler> executionHandlers = new List<IServicingOperationExecutionHandler>(servicingOperation.ExecutionHandlers.Count + this.ServicingOperationExecutionHandlers.Count);
      executionHandlers.AddRange((IEnumerable<IServicingOperationExecutionHandler>) this.ServicingOperationExecutionHandlers);
      if (this.m_servicingExecutionHandlerProvider != null)
      {
        for (int index = 0; index < servicingOperation.ExecutionHandlers.Count; ++index)
        {
          IServicingOperationExecutionHandler executionHandler = this.m_servicingExecutionHandlerProvider.GetServicingOperationExecutionHandler(servicingOperation.ExecutionHandlers[index].HandlerType);
          executionHandlers.Add(executionHandler);
        }
      }
      return (IEnumerable<IServicingOperationExecutionHandler>) executionHandlers;
    }

    private IEnumerable<IServicingStepGroupExecutionHandler> GetServicingStepGroupExecutionHandlers(
      ServicingStepGroup stepGroup)
    {
      List<IServicingStepGroupExecutionHandler> executionHandlers = new List<IServicingStepGroupExecutionHandler>(stepGroup.ExecutionHandlers.Count + this.ServicingStepGroupExecutionHandlers.Count);
      executionHandlers.AddRange((IEnumerable<IServicingStepGroupExecutionHandler>) this.ServicingStepGroupExecutionHandlers);
      if (this.m_servicingExecutionHandlerProvider != null)
      {
        for (int index = 0; index < stepGroup.ExecutionHandlers.Count; ++index)
        {
          IServicingStepGroupExecutionHandler executionHandler = this.m_servicingExecutionHandlerProvider.GetServicingStepGroupExecutionHandler(stepGroup.ExecutionHandlers[index].HandlerType);
          executionHandlers.Add(executionHandler);
        }
      }
      return (IEnumerable<IServicingStepGroupExecutionHandler>) executionHandlers;
    }

    protected virtual ServicingStepState PerformServicingStep(
      ServicingStep step,
      ServicingContext servicingContext,
      ServicingStepGroup group,
      ServicingOperation servicingOperation,
      int stepNumber,
      int totalSteps)
    {
      servicingContext.Tokens[ServicingTokenConstants.CurrentStepName] = step.Name;
      servicingContext.Tokens[ServicingTokenConstants.CurrentStepGroupName] = group.Name;
      if (this.m_tokenStepDriver != null)
        return this.m_tokenStepDriver.PerformServicingStep(step, servicingContext, group, servicingOperation, stepNumber, totalSteps);
      IStepPerformer stepPerformer = this.m_stepPerformerProvider.GetStepPerformer(step.StepPerformer);
      Exception exception = (Exception) null;
      ServicingStepState servicingStepState = ServicingStepState.NotExecuted;
      try
      {
        if (servicingContext.DeploymentRequestContext != null)
          servicingContext.DeploymentRequestContext.TraceEnter(69002, ServicingStepDriver.s_area, ServicingStepDriver.s_layer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PerformServicingStep.{0}.{1}", (object) servicingOperation.Name, (object) step.StepType));
        servicingContext.StartStep(step.Name);
        string stepData = this.GetStepData(step);
        if (!string.IsNullOrEmpty(this.m_servicingFaultInjection))
        {
          string[] strArray = this.m_servicingFaultInjection.Split(';');
          if (strArray.Length < 2)
          {
            TeamFoundationTrace.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} is set but doesn't have enough args.", (object) FrameworkServerConstants.ServicingCollectionFaultInjection));
          }
          else
          {
            string b1 = strArray[0];
            string b2 = strArray[1];
            if (string.Equals(group.Name, b1, StringComparison.OrdinalIgnoreCase) && string.Equals(step.Name, b2, StringComparison.OrdinalIgnoreCase))
              throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Registry key {0} or Environment variable {1} is set to cause this step to fail.", (object) FrameworkServerConstants.ServicingCollectionFaultInjection, (object) "TFS_INTERNAL_SERVICING_STEP_FAILURE"));
          }
        }
        if (this.m_dbOnly && !stepPerformer.Name.Equals("sql", StringComparison.InvariantCultureIgnoreCase))
        {
          servicingContext.LogInfo(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Skipping Step {0}", (object) step.Name));
        }
        else
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Executing step: '{0}' {1}.{2} ({3} of {4})", (object) step.Name, (object) stepPerformer.Name, (object) step.StepType, (object) stepNumber, (object) totalSteps);
          servicingContext.LogInfo(message);
          stepPerformer.PerformStep(servicingOperation.Name, servicingOperation.Target, step.StepType, stepData, servicingContext);
          if (this.m_enableExtendedValidation)
          {
            foreach (IServicingStepValidator extendedValidator in (IEnumerable<IServicingStepValidator>) this.m_extendedValidators)
            {
              if (extendedValidator.ShouldValidate(servicingOperation.Name, servicingOperation.Target, step, stepData, servicingContext))
              {
                servicingContext.LogInfo("Executing extended validator '{0}'", new object[1]
                {
                  (object) extendedValidator.GetType().AssemblyQualifiedName
                });
                extendedValidator.Validate(servicingOperation.Name, servicingOperation.Target, step, stepData, servicingContext);
              }
            }
          }
        }
        if (this.m_runEachStepTwice && !string.Equals(step.StepType, "CreateWarehouse", StringComparison.Ordinal))
          stepPerformer.PerformStep(servicingOperation.Name, servicingOperation.Target, step.StepType, stepData, servicingContext);
        if (this.m_delayBetweenSteps != TimeSpan.Zero)
        {
          servicingContext.LogInfo("Sleeping for {0:.###} seconds.", new object[1]
          {
            (object) this.m_delayBetweenSteps.TotalSeconds
          });
          Thread.Sleep(this.m_delayBetweenSteps);
        }
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      finally
      {
        if (servicingContext.DeploymentRequestContext != null)
          servicingContext.DeploymentRequestContext.TraceLeave(69003, ServicingStepDriver.s_area, ServicingStepDriver.s_layer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PerformServicingStep.{0}.{1}", (object) servicingOperation.Name, (object) step.StepType));
        servicingStepState = servicingContext.FinishStep(exception);
      }
      return servicingStepState;
    }

    protected string GetStepData(ServicingStep step)
    {
      string stepData = (string) null;
      if (step.StepData != null)
        stepData = this.m_servicingContext.ReplaceResources(this.m_servicingContext.ReplaceTokens(step.StepData.OuterXml));
      return stepData;
    }

    internal virtual int ServicingStepCount
    {
      get
      {
        int seed = 0;
        if (this.m_operations != null)
        {
          foreach (ServicingOperation operation in this.m_operations)
            seed = operation.Groups.Aggregate<ServicingStepGroup, int>(seed, (Func<int, ServicingStepGroup, int>) ((count, stepGroup) => count + stepGroup.Steps.Count));
        }
        return seed;
      }
    }

    protected void ThrowIfCanceled()
    {
      if (!(this.m_servicingContext.DeploymentRequestContext is VssRequestContext deploymentRequestContext))
        return;
      deploymentRequestContext.RequestContextInternal(false)?.CheckCanceled();
    }
  }
}
