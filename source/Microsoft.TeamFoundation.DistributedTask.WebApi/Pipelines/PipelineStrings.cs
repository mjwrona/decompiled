// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineStrings
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal static class PipelineStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (PipelineStrings), typeof (PipelineStrings).GetTypeInfo().Assembly);

    public static ResourceManager Manager => PipelineStrings.s_resMgr;

    private static string Get(string resourceName) => PipelineStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? PipelineStrings.Get(resourceName) : PipelineStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) PipelineStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? PipelineStrings.GetInt(resourceName) : (int) PipelineStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) PipelineStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? PipelineStrings.GetBool(resourceName) : (bool) PipelineStrings.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => PipelineStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = PipelineStrings.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string AmbiguousQueueSpecification(object arg0) => PipelineStrings.Format(nameof (AmbiguousQueueSpecification), arg0);

    public static string AmbiguousQueueSpecification(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (AmbiguousQueueSpecification), culture, arg0);

    public static string AmbiguousSecureFileSpecification(object arg0) => PipelineStrings.Format(nameof (AmbiguousSecureFileSpecification), arg0);

    public static string AmbiguousSecureFileSpecification(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (AmbiguousSecureFileSpecification), culture, arg0);

    public static string AmbiguousServiceEndpointSpecification(object arg0) => PipelineStrings.Format(nameof (AmbiguousServiceEndpointSpecification), arg0);

    public static string AmbiguousServiceEndpointSpecification(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (AmbiguousServiceEndpointSpecification), culture, arg0);

    public static string AmbiguousTaskSpecification(object arg0, object arg1) => PipelineStrings.Format(nameof (AmbiguousTaskSpecification), arg0, arg1);

    public static string AmbiguousTaskSpecification(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (AmbiguousTaskSpecification), culture, arg0, arg1);

    public static string AmbiguousVariableGroupSpecification(object arg0) => PipelineStrings.Format(nameof (AmbiguousVariableGroupSpecification), arg0);

    public static string AmbiguousVariableGroupSpecification(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (AmbiguousVariableGroupSpecification), culture, arg0);

    public static string AzureKeyVaultTaskName(object arg0) => PipelineStrings.Format(nameof (AzureKeyVaultTaskName), arg0);

    public static string AzureKeyVaultTaskName(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (AzureKeyVaultTaskName), culture, arg0);

    public static string ContainerResourceInvalidRegistryEndpointType(
      object arg0,
      object arg1,
      object arg2)
    {
      return PipelineStrings.Format(nameof (ContainerResourceInvalidRegistryEndpointType), arg0, arg1, arg2);
    }

    public static string ContainerResourceInvalidRegistryEndpointType(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (ContainerResourceInvalidRegistryEndpointType), culture, arg0, arg1, arg2);
    }

    public static string ContainerResourceNotFound(object arg0) => PipelineStrings.Format(nameof (ContainerResourceNotFound), arg0);

    public static string ContainerResourceNotFound(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (ContainerResourceNotFound), culture, arg0);

    public static string ContainerEndpointNotFound(object arg0, object arg1) => PipelineStrings.Format(nameof (ContainerEndpointNotFound), arg0, arg1);

    public static string ContainerEndpointNotFound(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (ContainerEndpointNotFound), culture, arg0, arg1);

    public static string CheckoutMultipleRepositoriesCannotIncludeNone() => PipelineStrings.Get(nameof (CheckoutMultipleRepositoriesCannotIncludeNone));

    public static string CheckoutMultipleRepositoriesCannotIncludeNone(CultureInfo culture) => PipelineStrings.Get(nameof (CheckoutMultipleRepositoriesCannotIncludeNone), culture);

    public static string CheckoutStepRepositoryNotSupported(object arg0) => PipelineStrings.Format(nameof (CheckoutStepRepositoryNotSupported), arg0);

    public static string CheckoutStepRepositoryNotSupported(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (CheckoutStepRepositoryNotSupported), culture, arg0);

    public static string CheckoutMustBeTheFirstStep() => PipelineStrings.Get(nameof (CheckoutMustBeTheFirstStep));

    public static string CheckoutMustBeTheFirstStep(CultureInfo culture) => PipelineStrings.Get(nameof (CheckoutMustBeTheFirstStep), culture);

    public static string ExpressionInvalid(object arg0) => PipelineStrings.Format(nameof (ExpressionInvalid), arg0);

    public static string ExpressionInvalid(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (ExpressionInvalid), culture, arg0);

    public static string DemandExpansionInvalid(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (DemandExpansionInvalid), arg0, arg1, arg2);

    public static string DemandExpansionInvalid(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (DemandExpansionInvalid), culture, arg0, arg1, arg2);
    }

    public static string PhaseGraphCycleDetected(object arg0, object arg1) => PipelineStrings.Format(nameof (PhaseGraphCycleDetected), arg0, arg1);

    public static string PhaseGraphCycleDetected(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (PhaseGraphCycleDetected), culture, arg0, arg1);

    public static string StageGraphCycleDetected(object arg0, object arg1) => PipelineStrings.Format(nameof (StageGraphCycleDetected), arg0, arg1);

    public static string StageGraphCycleDetected(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StageGraphCycleDetected), culture, arg0, arg1);

    public static string StagePhaseGraphCycleDetected(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (StagePhaseGraphCycleDetected), arg0, arg1, arg2);

    public static string StagePhaseGraphCycleDetected(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (StagePhaseGraphCycleDetected), culture, arg0, arg1, arg2);
    }

    public static string InvalidRegexOptions(object arg0, object arg1) => PipelineStrings.Format(nameof (InvalidRegexOptions), arg0, arg1);

    public static string InvalidRegexOptions(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (InvalidRegexOptions), culture, arg0, arg1);

    public static string InvalidRetryStageNeverRun(object arg0) => PipelineStrings.Format(nameof (InvalidRetryStageNeverRun), arg0);

    public static string InvalidRetryStageNeverRun(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (InvalidRetryStageNeverRun), culture, arg0);

    public static string InvalidRetryStageNotComplete(object arg0) => PipelineStrings.Format(nameof (InvalidRetryStageNotComplete), arg0);

    public static string InvalidRetryStageNotComplete(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (InvalidRetryStageNotComplete), culture, arg0);

    public static string InvalidTypeForLengthFunction(object arg0) => PipelineStrings.Format(nameof (InvalidTypeForLengthFunction), arg0);

    public static string InvalidTypeForLengthFunction(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (InvalidTypeForLengthFunction), culture, arg0);

    public static string InvalidValidationOptionNoImplementation(object arg0, object arg1) => PipelineStrings.Format(nameof (InvalidValidationOptionNoImplementation), arg0, arg1);

    public static string InvalidValidationOptionNoImplementation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (InvalidValidationOptionNoImplementation), culture, arg0, arg1);
    }

    public static string PhaseDependencyNotFound(object arg0, object arg1) => PipelineStrings.Format(nameof (PhaseDependencyNotFound), arg0, arg1);

    public static string PhaseDependencyNotFound(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (PhaseDependencyNotFound), culture, arg0, arg1);

    public static string StagePhaseDependencyNotFound(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (StagePhaseDependencyNotFound), arg0, arg1, arg2);

    public static string StagePhaseDependencyNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (StagePhaseDependencyNotFound), culture, arg0, arg1, arg2);
    }

    public static string StageDependencyNotFound(object arg0, object arg1) => PipelineStrings.Format(nameof (StageDependencyNotFound), arg0, arg1);

    public static string StageDependencyNotFound(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StageDependencyNotFound), culture, arg0, arg1);

    public static string PhaseJobNameInvalidForSlicing(object arg0) => PipelineStrings.Format(nameof (PhaseJobNameInvalidForSlicing), arg0);

    public static string PhaseJobNameInvalidForSlicing(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (PhaseJobNameInvalidForSlicing), culture, arg0);

    public static string PhaseJobNumberDoesNotExist(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (PhaseJobNumberDoesNotExist), arg0, arg1, arg2);

    public static string PhaseJobNumberDoesNotExist(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (PhaseJobNumberDoesNotExist), culture, arg0, arg1, arg2);
    }

    public static string PhaseJobMatrixExpansionExceedLimit(object arg0, object arg1) => PipelineStrings.Format(nameof (PhaseJobMatrixExpansionExceedLimit), arg0, arg1);

    public static string PhaseJobMatrixExpansionExceedLimit(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (PhaseJobMatrixExpansionExceedLimit), culture, arg0, arg1);
    }

    public static string PhaseJobSlicingExpansionExceedLimit(object arg0, object arg1) => PipelineStrings.Format(nameof (PhaseJobSlicingExpansionExceedLimit), arg0, arg1);

    public static string PhaseJobSlicingExpansionExceedLimit(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (PhaseJobSlicingExpansionExceedLimit), culture, arg0, arg1);
    }

    public static string PhaseMatrixConfigurationDoesNotExist(object arg0, object arg1) => PipelineStrings.Format(nameof (PhaseMatrixConfigurationDoesNotExist), arg0, arg1);

    public static string PhaseMatrixConfigurationDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (PhaseMatrixConfigurationDoesNotExist), culture, arg0, arg1);
    }

    public static string PhaseNameInvalid(object arg0) => PipelineStrings.Format(nameof (PhaseNameInvalid), arg0);

    public static string PhaseNameInvalid(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (PhaseNameInvalid), culture, arg0);

    public static string StageNameInvalid(object arg0) => PipelineStrings.Format(nameof (StageNameInvalid), arg0);

    public static string StageNameInvalid(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (StageNameInvalid), culture, arg0);

    public static string StagePhaseNameInvalid(object arg0, object arg1) => PipelineStrings.Format(nameof (StagePhaseNameInvalid), arg0, arg1);

    public static string StagePhaseNameInvalid(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StagePhaseNameInvalid), culture, arg0, arg1);

    public static string PhaseNamesMustBeUnique(object arg0) => PipelineStrings.Format(nameof (PhaseNamesMustBeUnique), arg0);

    public static string PhaseNamesMustBeUnique(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (PhaseNamesMustBeUnique), culture, arg0);

    public static string StagePhaseNamesMustBeUnique(object arg0, object arg1) => PipelineStrings.Format(nameof (StagePhaseNamesMustBeUnique), arg0, arg1);

    public static string StagePhaseNamesMustBeUnique(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StagePhaseNamesMustBeUnique), culture, arg0, arg1);

    public static string PhaseTargetRequired(object arg0) => PipelineStrings.Format(nameof (PhaseTargetRequired), arg0);

    public static string PhaseTargetRequired(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (PhaseTargetRequired), culture, arg0);

    public static string StageVariableGroupNotSupported(object arg0, object arg1) => PipelineStrings.Format(nameof (StageVariableGroupNotSupported), arg0, arg1);

    public static string StageVariableGroupNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (StageVariableGroupNotSupported), culture, arg0, arg1);
    }

    public static string PhaseVariableGroupNotSupported(object arg0, object arg1) => PipelineStrings.Format(nameof (PhaseVariableGroupNotSupported), arg0, arg1);

    public static string PhaseVariableGroupNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (PhaseVariableGroupNotSupported), culture, arg0, arg1);
    }

    public static string PipelineNotValid() => PipelineStrings.Get(nameof (PipelineNotValid));

    public static string PipelineNotValid(CultureInfo culture) => PipelineStrings.Get(nameof (PipelineNotValid), culture);

    public static string PipelineNotValidWithErrors(object arg0) => PipelineStrings.Format(nameof (PipelineNotValidWithErrors), arg0);

    public static string PipelineNotValidWithErrors(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (PipelineNotValidWithErrors), culture, arg0);

    public static string PipelineNotValidNoStartingPhase() => PipelineStrings.Get(nameof (PipelineNotValidNoStartingPhase));

    public static string PipelineNotValidNoStartingPhase(CultureInfo culture) => PipelineStrings.Get(nameof (PipelineNotValidNoStartingPhase), culture);

    public static string PipelineNotValidNoStartingStage() => PipelineStrings.Get(nameof (PipelineNotValidNoStartingStage));

    public static string PipelineNotValidNoStartingStage(CultureInfo culture) => PipelineStrings.Get(nameof (PipelineNotValidNoStartingStage), culture);

    public static string StageNotValidNoStartingPhase(object arg0) => PipelineStrings.Format(nameof (StageNotValidNoStartingPhase), arg0);

    public static string StageNotValidNoStartingPhase(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (StageNotValidNoStartingPhase), culture, arg0);

    public static string QueueNotDefined() => PipelineStrings.Get(nameof (QueueNotDefined));

    public static string QueueNotDefined(CultureInfo culture) => PipelineStrings.Get(nameof (QueueNotDefined), culture);

    public static string QueueNotFound(object arg0) => PipelineStrings.Format(nameof (QueueNotFound), arg0);

    public static string QueueNotFound(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (QueueNotFound), culture, arg0);

    public static string QueueNotFoundByName(object arg0) => PipelineStrings.Format(nameof (QueueNotFoundByName), arg0);

    public static string QueueNotFoundByName(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (QueueNotFoundByName), culture, arg0);

    public static string RegexFailed(object arg0, object arg1) => PipelineStrings.Format(nameof (RegexFailed), arg0, arg1);

    public static string RegexFailed(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (RegexFailed), culture, arg0, arg1);

    public static string SecureFileNotFound(object arg0) => PipelineStrings.Format(nameof (SecureFileNotFound), arg0);

    public static string SecureFileNotFound(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (SecureFileNotFound), culture, arg0);

    public static string SecureFileNotFoundForInput(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return PipelineStrings.Format(nameof (SecureFileNotFoundForInput), arg0, arg1, arg2, arg3);
    }

    public static string SecureFileNotFoundForInput(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (SecureFileNotFoundForInput), culture, arg0, arg1, arg2, arg3);
    }

    public static string ServiceEndpointNotFound(object arg0) => PipelineStrings.Format(nameof (ServiceEndpointNotFound), arg0);

    public static string ServiceEndpointNotFound(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (ServiceEndpointNotFound), culture, arg0);

    public static string ServiceEndpointNotFoundForInput(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return PipelineStrings.Format(nameof (ServiceEndpointNotFoundForInput), arg0, arg1, arg2, arg3);
    }

    public static string ServiceEndpointNotFoundForInput(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (ServiceEndpointNotFoundForInput), culture, arg0, arg1, arg2, arg3);
    }

    public static string ServiceEndpointDisabled(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return PipelineStrings.Format(nameof (ServiceEndpointDisabled), arg0, arg1, arg2, arg3);
    }

    public static string ServiceEndpointDisabled(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (ServiceEndpointDisabled), culture, arg0, arg1, arg2, arg3);
    }

    public static string StepConditionIsNotValid(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return PipelineStrings.Format(nameof (StepConditionIsNotValid), arg0, arg1, arg2, arg3);
    }

    public static string StepConditionIsNotValid(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (StepConditionIsNotValid), culture, arg0, arg1, arg2, arg3);
    }

    public static string StepInputEndpointAuthSchemeMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7)
    {
      return PipelineStrings.Format(nameof (StepInputEndpointAuthSchemeMismatch), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string StepInputEndpointAuthSchemeMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (StepInputEndpointAuthSchemeMismatch), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    public static string StepInputEndpointTypeMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5)
    {
      return PipelineStrings.Format(nameof (StepInputEndpointTypeMismatch), arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string StepInputEndpointTypeMismatch(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (StepInputEndpointTypeMismatch), culture, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string StepNameInvalid(object arg0, object arg1) => PipelineStrings.Format(nameof (StepNameInvalid), arg0, arg1);

    public static string StepNameInvalid(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StepNameInvalid), culture, arg0, arg1);

    public static string StepNamesMustBeUnique(object arg0, object arg1) => PipelineStrings.Format(nameof (StepNamesMustBeUnique), arg0, arg1);

    public static string StepNamesMustBeUnique(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StepNamesMustBeUnique), culture, arg0, arg1);

    public static string StepNotSupported() => PipelineStrings.Get(nameof (StepNotSupported));

    public static string StepNotSupported(CultureInfo culture) => PipelineStrings.Get(nameof (StepNotSupported), culture);

    public static string StepTaskInputInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5)
    {
      return PipelineStrings.Format(nameof (StepTaskInputInvalid), arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string StepTaskInputInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (StepTaskInputInvalid), culture, arg0, arg1, arg2, arg3, arg4, arg5);
    }

    public static string StepTaskReferenceInvalid(object arg0, object arg1) => PipelineStrings.Format(nameof (StepTaskReferenceInvalid), arg0, arg1);

    public static string StepTaskReferenceInvalid(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StepTaskReferenceInvalid), culture, arg0, arg1);

    public static string StepActionReferenceInvalid(object arg0, object arg1) => PipelineStrings.Format(nameof (StepActionReferenceInvalid), arg0, arg1);

    public static string StepActionReferenceInvalid(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StepActionReferenceInvalid), culture, arg0, arg1);

    public static string TaskInvalidForGivenTarget(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return PipelineStrings.Format(nameof (TaskInvalidForGivenTarget), arg0, arg1, arg2, arg3);
    }

    public static string TaskInvalidForGivenTarget(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (TaskInvalidForGivenTarget), culture, arg0, arg1, arg2, arg3);
    }

    public static string TaskUsingRestrictedNodeVersion(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return PipelineStrings.Format(nameof (TaskUsingRestrictedNodeVersion), arg0, arg1, arg2, arg3, arg4);
    }

    public static string TaskUsingRestrictedNodeVersion(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (TaskUsingRestrictedNodeVersion), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string TaskMissing(object arg0, object arg1, object arg2, object arg3) => PipelineStrings.Format(nameof (TaskMissing), arg0, arg1, arg2, arg3);

    public static string TaskMissing(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (TaskMissing), culture, arg0, arg1, arg2, arg3);
    }

    public static string TaskVersionMissing(object arg0, object arg1, object arg2, object arg3) => PipelineStrings.Format(nameof (TaskVersionMissing), arg0, arg1, arg2, arg3);

    public static string TaskVersionMissing(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (TaskVersionMissing), culture, arg0, arg1, arg2, arg3);
    }

    public static string TaskStepReferenceInvalid(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (TaskStepReferenceInvalid), arg0, arg1, arg2);

    public static string TaskStepReferenceInvalid(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (TaskStepReferenceInvalid), culture, arg0, arg1, arg2);
    }

    public static string ActionStepReferenceInvalid(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (ActionStepReferenceInvalid), arg0, arg1, arg2);

    public static string ActionStepReferenceInvalid(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (ActionStepReferenceInvalid), culture, arg0, arg1, arg2);
    }

    public static string TaskTemplateNotSupported(object arg0, object arg1) => PipelineStrings.Format(nameof (TaskTemplateNotSupported), arg0, arg1);

    public static string TaskTemplateNotSupported(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (TaskTemplateNotSupported), culture, arg0, arg1);

    public static string TemplateStoreNotProvided(object arg0, object arg1) => PipelineStrings.Format(nameof (TemplateStoreNotProvided), arg0, arg1);

    public static string TemplateStoreNotProvided(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (TemplateStoreNotProvided), culture, arg0, arg1);

    public static string UnsupportedTargetType(object arg0, object arg1) => PipelineStrings.Format(nameof (UnsupportedTargetType), arg0, arg1);

    public static string UnsupportedTargetType(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (UnsupportedTargetType), culture, arg0, arg1);

    public static string RepositoryNotSpecified() => PipelineStrings.Get(nameof (RepositoryNotSpecified));

    public static string RepositoryNotSpecified(CultureInfo culture) => PipelineStrings.Get(nameof (RepositoryNotSpecified), culture);

    public static string RepositoryResourceNotFound(object arg0) => PipelineStrings.Format(nameof (RepositoryResourceNotFound), arg0);

    public static string RepositoryResourceNotFound(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (RepositoryResourceNotFound), culture, arg0);

    public static string RepositoryResourceNotFoundExplicit(object arg0, object arg1) => PipelineStrings.Format(nameof (RepositoryResourceNotFoundExplicit), arg0, arg1);

    public static string RepositoryResourceNotFoundExplicit(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (RepositoryResourceNotFoundExplicit), culture, arg0, arg1);
    }

    public static string RepositoryResourceEndpointFoundExplicit(object arg0, object arg1) => PipelineStrings.Format(nameof (RepositoryResourceEndpointFoundExplicit), arg0, arg1);

    public static string RepositoryResourceEndpointFoundExplicit(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (RepositoryResourceEndpointFoundExplicit), culture, arg0, arg1);
    }

    public static string PoolResourceNotFoundExplicit(object arg0, object arg1) => PipelineStrings.Format(nameof (PoolResourceNotFoundExplicit), arg0, arg1);

    public static string PoolResourceNotFoundExplicit(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (PoolResourceNotFoundExplicit), culture, arg0, arg1);
    }

    public static string VariableGroupNotFound(object arg0) => PipelineStrings.Format(nameof (VariableGroupNotFound), arg0);

    public static string VariableGroupNotFound(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (VariableGroupNotFound), culture, arg0);

    public static string VariableGroupNotFoundForPhase(object arg0, object arg1) => PipelineStrings.Format(nameof (VariableGroupNotFoundForPhase), arg0, arg1);

    public static string VariableGroupNotFoundForPhase(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (VariableGroupNotFoundForPhase), culture, arg0, arg1);
    }

    public static string VariableGroupNotFoundForStage(object arg0, object arg1) => PipelineStrings.Format(nameof (VariableGroupNotFoundForStage), arg0, arg1);

    public static string VariableGroupNotFoundForStage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (VariableGroupNotFoundForStage), culture, arg0, arg1);
    }

    public static string JobNameWhenNoNameIsProvided(object arg0) => PipelineStrings.Format(nameof (JobNameWhenNoNameIsProvided), arg0);

    public static string JobNameWhenNoNameIsProvided(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (JobNameWhenNoNameIsProvided), culture, arg0);

    public static string StageNameWhenNoNameIsProvided(object arg0) => PipelineStrings.Format(nameof (StageNameWhenNoNameIsProvided), arg0);

    public static string StageNameWhenNoNameIsProvided(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (StageNameWhenNoNameIsProvided), culture, arg0);

    public static string InvalidAbsoluteRollingValue() => PipelineStrings.Get(nameof (InvalidAbsoluteRollingValue));

    public static string InvalidAbsoluteRollingValue(CultureInfo culture) => PipelineStrings.Get(nameof (InvalidAbsoluteRollingValue), culture);

    public static string InvalidPercentageRollingValue() => PipelineStrings.Get(nameof (InvalidPercentageRollingValue));

    public static string InvalidPercentageRollingValue(CultureInfo culture) => PipelineStrings.Get(nameof (InvalidPercentageRollingValue), culture);

    public static string InvalidRollingOption(object arg0) => PipelineStrings.Format(nameof (InvalidRollingOption), arg0);

    public static string InvalidRollingOption(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (InvalidRollingOption), culture, arg0);

    public static string EnvironmentNotFound(object arg0, object arg1) => PipelineStrings.Format(nameof (EnvironmentNotFound), arg0, arg1);

    public static string EnvironmentNotFound(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (EnvironmentNotFound), culture, arg0, arg1);

    public static string EnvironmentRequired(object arg0) => PipelineStrings.Format(nameof (EnvironmentRequired), arg0);

    public static string EnvironmentRequired(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (EnvironmentRequired), culture, arg0);

    public static string EnvironmentResourceNotFound(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (EnvironmentResourceNotFound), arg0, arg1, arg2);

    public static string EnvironmentResourceNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (EnvironmentResourceNotFound), culture, arg0, arg1, arg2);
    }

    public static string StageNamesMustBeUnique(object arg0) => PipelineStrings.Format(nameof (StageNamesMustBeUnique), arg0);

    public static string StageNamesMustBeUnique(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (StageNamesMustBeUnique), culture, arg0);

    public static string ServiceConnectionUsedInVariableGroupNotValid(object arg0, object arg1) => PipelineStrings.Format(nameof (ServiceConnectionUsedInVariableGroupNotValid), arg0, arg1);

    public static string ServiceConnectionUsedInVariableGroupNotValid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (ServiceConnectionUsedInVariableGroupNotValid), culture, arg0, arg1);
    }

    public static string TaskInvalidForServerTarget(object arg0, object arg1) => PipelineStrings.Format(nameof (TaskInvalidForServerTarget), arg0, arg1);

    public static string TaskInvalidForServerTarget(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (TaskInvalidForServerTarget), culture, arg0, arg1);

    public static string TimeoutExceedsMMSMaximum(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (TimeoutExceedsMMSMaximum), arg0, arg1, arg2);

    public static string TimeoutExceedsMMSMaximum(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (TimeoutExceedsMMSMaximum), culture, arg0, arg1, arg2);
    }

    public static string CancelTimeoutExceedsMMSMaximum(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (CancelTimeoutExceedsMMSMaximum), arg0, arg1, arg2);

    public static string CancelTimeoutExceedsMMSMaximum(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (CancelTimeoutExceedsMMSMaximum), culture, arg0, arg1, arg2);
    }

    public static string TaskOverride(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (TaskOverride), arg0, arg1, arg2);

    public static string TaskOverride(object arg0, object arg1, object arg2, CultureInfo culture) => PipelineStrings.Format(nameof (TaskOverride), culture, arg0, arg1, arg2);

    public static string FailedInsertStepToPhase(object arg0) => PipelineStrings.Format(nameof (FailedInsertStepToPhase), arg0);

    public static string FailedInsertStepToPhase(object arg0, CultureInfo culture) => PipelineStrings.Format(nameof (FailedInsertStepToPhase), culture, arg0);

    public static string TaskBuildConfigException(object arg0, object arg1, object arg2) => PipelineStrings.Format(nameof (TaskBuildConfigException), arg0, arg1, arg2);

    public static string TaskBuildConfigException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return PipelineStrings.Format(nameof (TaskBuildConfigException), culture, arg0, arg1, arg2);
    }

    public static string StageGroupInvalid(object arg0, object arg1) => PipelineStrings.Format(nameof (StageGroupInvalid), arg0, arg1);

    public static string StageGroupInvalid(object arg0, object arg1, CultureInfo culture) => PipelineStrings.Format(nameof (StageGroupInvalid), culture, arg0, arg1);
  }
}
