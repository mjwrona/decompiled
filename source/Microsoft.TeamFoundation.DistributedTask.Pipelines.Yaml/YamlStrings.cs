// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.YamlStrings
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal static class YamlStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (YamlStrings), typeof (YamlStrings).GetTypeInfo().Assembly);

    public static ResourceManager Manager => YamlStrings.s_resMgr;

    private static string Get(string resourceName) => YamlStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? YamlStrings.Get(resourceName) : YamlStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) YamlStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? YamlStrings.GetInt(resourceName) : (int) YamlStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) YamlStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? YamlStrings.GetBool(resourceName) : (bool) YamlStrings.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => YamlStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = YamlStrings.Get(resourceName, culture);
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

    public static string AnchorsNotSupported(object arg0) => YamlStrings.Format(nameof (AnchorsNotSupported), arg0);

    public static string AnchorsNotSupported(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (AnchorsNotSupported), culture, arg0);

    public static string ExpectedAtLeastOnePair() => YamlStrings.Get(nameof (ExpectedAtLeastOnePair));

    public static string ExpectedAtLeastOnePair(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedAtLeastOnePair), culture);

    public static string ExpectedBoolean(object arg0) => YamlStrings.Format(nameof (ExpectedBoolean), arg0);

    public static string ExpectedBoolean(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (ExpectedBoolean), culture, arg0);

    public static string ExpectedDocumentEnd() => YamlStrings.Get(nameof (ExpectedDocumentEnd));

    public static string ExpectedDocumentEnd(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedDocumentEnd), culture);

    public static string ExpectedDocumentStart() => YamlStrings.Get(nameof (ExpectedDocumentStart));

    public static string ExpectedDocumentStart(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedDocumentStart), culture);

    public static string ExpectedEndOfParseEvents() => YamlStrings.Get(nameof (ExpectedEndOfParseEvents));

    public static string ExpectedEndOfParseEvents(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedEndOfParseEvents), culture);

    public static string ExpectedExpression() => YamlStrings.Get(nameof (ExpectedExpression));

    public static string ExpectedExpression(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedExpression), culture);

    public static string ExpectedInteger(object arg0) => YamlStrings.Format(nameof (ExpectedInteger), arg0);

    public static string ExpectedInteger(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (ExpectedInteger), culture, arg0);

    public static string ExpectedNParametersFollowingDirective(
      object arg0,
      object arg1,
      object arg2)
    {
      return YamlStrings.Format(nameof (ExpectedNParametersFollowingDirective), arg0, arg1, arg2);
    }

    public static string ExpectedNParametersFollowingDirective(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return YamlStrings.Format(nameof (ExpectedNParametersFollowingDirective), culture, arg0, arg1, arg2);
    }

    public static string ExpectedMapping() => YamlStrings.Get(nameof (ExpectedMapping));

    public static string ExpectedMapping(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedMapping), culture);

    public static string ExpectedNonEmptyString() => YamlStrings.Get(nameof (ExpectedNonEmptyString));

    public static string ExpectedNonEmptyString(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedNonEmptyString), culture);

    public static string ExpectedParseEvent() => YamlStrings.Get(nameof (ExpectedParseEvent));

    public static string ExpectedParseEvent(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedParseEvent), culture);

    public static string ExpectedScalar() => YamlStrings.Get(nameof (ExpectedScalar));

    public static string ExpectedScalar(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedScalar), culture);

    public static string ExpectedScalarSequenceOrMapping() => YamlStrings.Get(nameof (ExpectedScalarSequenceOrMapping));

    public static string ExpectedScalarSequenceOrMapping(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedScalarSequenceOrMapping), culture);

    public static string ExpectedSequence() => YamlStrings.Get(nameof (ExpectedSequence));

    public static string ExpectedSequence(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedSequence), culture);

    public static string ExpectedStreamEnd() => YamlStrings.Get(nameof (ExpectedStreamEnd));

    public static string ExpectedStreamEnd(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedStreamEnd), culture);

    public static string ExpectedStreamStart() => YamlStrings.Get(nameof (ExpectedStreamStart));

    public static string ExpectedStreamStart(CultureInfo culture) => YamlStrings.Get(nameof (ExpectedStreamStart), culture);

    public static string InvalidEachParameter1(object arg0) => YamlStrings.Format(nameof (InvalidEachParameter1), arg0);

    public static string InvalidEachParameter1(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (InvalidEachParameter1), culture, arg0);

    public static string InvalidIdentifier(object arg0) => YamlStrings.Format(nameof (InvalidIdentifier), arg0);

    public static string InvalidIdentifier(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (InvalidIdentifier), culture, arg0);

    public static string InvalidIdentifierReserved(object arg0) => YamlStrings.Format(nameof (InvalidIdentifierReserved), arg0);

    public static string InvalidIdentifierReserved(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (InvalidIdentifierReserved), culture, arg0);

    public static string InvalidNameAndVersion(object arg0) => YamlStrings.Format(nameof (InvalidNameAndVersion), arg0);

    public static string InvalidNameAndVersion(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (InvalidNameAndVersion), culture, arg0);

    public static string InvalidTaskReference(object arg0) => YamlStrings.Format(nameof (InvalidTaskReference), arg0);

    public static string InvalidTaskReference(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (InvalidTaskReference), culture, arg0);

    public static string InvalidTemplateParameter(object arg0, object arg1) => YamlStrings.Format(nameof (InvalidTemplateParameter), arg0, arg1);

    public static string InvalidTemplateParameter(object arg0, object arg1, CultureInfo culture) => YamlStrings.Format(nameof (InvalidTemplateParameter), culture, arg0, arg1);

    public static string InvalidTemplateParameterType(object arg0, object arg1, object arg2) => YamlStrings.Format(nameof (InvalidTemplateParameterType), arg0, arg1, arg2);

    public static string InvalidTemplateParameterType(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return YamlStrings.Format(nameof (InvalidTemplateParameterType), culture, arg0, arg1, arg2);
    }

    public static string InvalidTemplateParameterValue(object arg0, object arg1) => YamlStrings.Format(nameof (InvalidTemplateParameterValue), arg0, arg1);

    public static string InvalidTemplateParameterValue(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return YamlStrings.Format(nameof (InvalidTemplateParameterValue), culture, arg0, arg1);
    }

    public static string LineColumn(object arg0, object arg1) => YamlStrings.Format(nameof (LineColumn), arg0, arg1);

    public static string LineColumn(object arg0, object arg1, CultureInfo culture) => YamlStrings.Format(nameof (LineColumn), culture, arg0, arg1);

    public static string MaxFilesExceeded(object arg0) => YamlStrings.Format(nameof (MaxFilesExceeded), arg0);

    public static string MaxFilesExceeded(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (MaxFilesExceeded), culture, arg0);

    public static string MaxFileSizeExceeded(object arg0) => YamlStrings.Format(nameof (MaxFileSizeExceeded), arg0);

    public static string MaxFileSizeExceeded(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (MaxFileSizeExceeded), culture, arg0);

    public static string MaxObjectDepthExceeded() => YamlStrings.Get(nameof (MaxObjectDepthExceeded));

    public static string MaxObjectDepthExceeded(CultureInfo culture) => YamlStrings.Get(nameof (MaxObjectDepthExceeded), culture);

    public static string MaxObjectSizeExceeded() => YamlStrings.Get(nameof (MaxObjectSizeExceeded));

    public static string MaxObjectSizeExceeded(CultureInfo culture) => YamlStrings.Get(nameof (MaxObjectSizeExceeded), culture);

    public static string MaxParseEventsExceeded() => YamlStrings.Get(nameof (MaxParseEventsExceeded));

    public static string MaxParseEventsExceeded(CultureInfo culture) => YamlStrings.Get(nameof (MaxParseEventsExceeded), culture);

    public static string MaxTemplateBuilderEventsExceeded() => YamlStrings.Get(nameof (MaxTemplateBuilderEventsExceeded));

    public static string MaxTemplateBuilderEventsExceeded(CultureInfo culture) => YamlStrings.Get(nameof (MaxTemplateBuilderEventsExceeded), culture);

    public static string MissingTemplateParameterValue(object arg0) => YamlStrings.Format(nameof (MissingTemplateParameterValue), arg0);

    public static string MissingTemplateParameterValue(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (MissingTemplateParameterValue), culture, arg0);

    public static string DuplicateTemplateParameter(object arg0) => YamlStrings.Format(nameof (DuplicateTemplateParameter), arg0);

    public static string DuplicateTemplateParameter(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (DuplicateTemplateParameter), culture, arg0);

    public static string UnexpectedParsingEventType(object arg0) => YamlStrings.Format(nameof (UnexpectedParsingEventType), arg0);

    public static string UnexpectedParsingEventType(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (UnexpectedParsingEventType), culture, arg0);

    public static string UnexpectedTemplateParameter(object arg0) => YamlStrings.Format(nameof (UnexpectedTemplateParameter), arg0);

    public static string UnexpectedTemplateParameter(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (UnexpectedTemplateParameter), culture, arg0);

    public static string UnexpectedTemplateType(object arg0) => YamlStrings.Format(nameof (UnexpectedTemplateType), arg0);

    public static string UnexpectedTemplateType(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (UnexpectedTemplateType), culture, arg0);

    public static string UnexpectedValue(object arg0) => YamlStrings.Format(nameof (UnexpectedValue), arg0);

    public static string UnexpectedValue(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (UnexpectedValue), culture, arg0);

    public static string UnexpectedValueWithoutContent() => YamlStrings.Get(nameof (UnexpectedValueWithoutContent));

    public static string UnexpectedValueWithoutContent(CultureInfo culture) => YamlStrings.Get(nameof (UnexpectedValueWithoutContent), culture);

    public static string ValueAlreadyDefined(object arg0) => YamlStrings.Format(nameof (ValueAlreadyDefined), arg0);

    public static string ValueAlreadyDefined(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (ValueAlreadyDefined), culture, arg0);

    public static string ValueMutuallyExclusive(object arg0, object arg1) => YamlStrings.Format(nameof (ValueMutuallyExclusive), arg0, arg1);

    public static string ValueMutuallyExclusive(object arg0, object arg1, CultureInfo culture) => YamlStrings.Format(nameof (ValueMutuallyExclusive), culture, arg0, arg1);

    public static string ExpressionNotAllowed() => YamlStrings.Get(nameof (ExpressionNotAllowed));

    public static string ExpressionNotAllowed(CultureInfo culture) => YamlStrings.Get(nameof (ExpressionNotAllowed), culture);

    public static string DirectiveNotAllowedInline(object arg0) => YamlStrings.Format(nameof (DirectiveNotAllowedInline), arg0);

    public static string DirectiveNotAllowedInline(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (DirectiveNotAllowedInline), culture, arg0);

    public static string ExpressionNotClosed() => YamlStrings.Get(nameof (ExpressionNotClosed));

    public static string ExpressionNotClosed(CultureInfo culture) => YamlStrings.Get(nameof (ExpressionNotClosed), culture);

    public static string DirectiveNotAllowed(object arg0) => YamlStrings.Format(nameof (DirectiveNotAllowed), arg0);

    public static string DirectiveNotAllowed(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (DirectiveNotAllowed), culture, arg0);

    public static string ExpectedJArrayOrJObectResult(object arg0) => YamlStrings.Format(nameof (ExpectedJArrayOrJObectResult), arg0);

    public static string ExpectedJArrayOrJObectResult(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (ExpectedJArrayOrJObectResult), culture, arg0);

    public static string LoadingFile(object arg0) => YamlStrings.Format(nameof (LoadingFile), arg0);

    public static string LoadingFile(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (LoadingFile), culture, arg0);

    public static string CompiledYamlDocument() => YamlStrings.Get(nameof (CompiledYamlDocument));

    public static string CompiledYamlDocument(CultureInfo culture) => YamlStrings.Get(nameof (CompiledYamlDocument), culture);

    public static string YamlDocument() => YamlStrings.Get(nameof (YamlDocument));

    public static string YamlDocument(CultureInfo culture) => YamlStrings.Get(nameof (YamlDocument), culture);

    public static string IdentifierAlreadyDefined(object arg0) => YamlStrings.Format(nameof (IdentifierAlreadyDefined), arg0);

    public static string IdentifierAlreadyDefined(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (IdentifierAlreadyDefined), culture, arg0);

    public static string UnexpectedSequenceStart() => YamlStrings.Get(nameof (UnexpectedSequenceStart));

    public static string UnexpectedSequenceStart(CultureInfo culture) => YamlStrings.Get(nameof (UnexpectedSequenceStart), culture);

    public static string UnexpectedMappingStart() => YamlStrings.Get(nameof (UnexpectedMappingStart));

    public static string UnexpectedMappingStart(CultureInfo culture) => YamlStrings.Get(nameof (UnexpectedMappingStart), culture);

    public static string TransformResultSequenceExpectedScalar() => YamlStrings.Get(nameof (TransformResultSequenceExpectedScalar));

    public static string TransformResultSequenceExpectedScalar(CultureInfo culture) => YamlStrings.Get(nameof (TransformResultSequenceExpectedScalar), culture);

    public static string TransformResultMappingExpectedScalar() => YamlStrings.Get(nameof (TransformResultMappingExpectedScalar));

    public static string TransformResultMappingExpectedScalar(CultureInfo culture) => YamlStrings.Get(nameof (TransformResultMappingExpectedScalar), culture);

    public static string ExpectedSequenceOrMappingActual(object arg0) => YamlStrings.Format(nameof (ExpectedSequenceOrMappingActual), arg0);

    public static string ExpectedSequenceOrMappingActual(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (ExpectedSequenceOrMappingActual), culture, arg0);

    public static string MaxTemplateEventsExceeded() => YamlStrings.Get(nameof (MaxTemplateEventsExceeded));

    public static string MaxTemplateEventsExceeded(CultureInfo culture) => YamlStrings.Get(nameof (MaxTemplateEventsExceeded), culture);

    public static string UnableToConvertToTemplateToken(object arg0) => YamlStrings.Format(nameof (UnableToConvertToTemplateToken), arg0);

    public static string UnableToConvertToTemplateToken(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (UnableToConvertToTemplateToken), culture, arg0);

    public static string UnableToDetermineOneOf() => YamlStrings.Get(nameof (UnableToDetermineOneOf));

    public static string UnableToDetermineOneOf(CultureInfo culture) => YamlStrings.Get(nameof (UnableToDetermineOneOf), culture);

    public static string InvalidDownloadTaskAlias() => YamlStrings.Get(nameof (InvalidDownloadTaskAlias));

    public static string InvalidDownloadTaskAlias(CultureInfo culture) => YamlStrings.Get(nameof (InvalidDownloadTaskAlias), culture);

    public static string CannotOverrideSystemVariable(object arg0) => YamlStrings.Format(nameof (CannotOverrideSystemVariable), arg0);

    public static string CannotOverrideSystemVariable(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (CannotOverrideSystemVariable), culture, arg0);

    public static string KeyAlreadyDefined(object arg0) => YamlStrings.Format(nameof (KeyAlreadyDefined), arg0);

    public static string KeyAlreadyDefined(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (KeyAlreadyDefined), culture, arg0);

    public static string KeyNotFound(object arg0) => YamlStrings.Format(nameof (KeyNotFound), arg0);

    public static string KeyNotFound(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (KeyNotFound), culture, arg0);

    public static string CronSyntaxError(object arg0) => YamlStrings.Format(nameof (CronSyntaxError), arg0);

    public static string CronSyntaxError(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (CronSyntaxError), culture, arg0);

    public static string InvalidPipelineScheduleBranchFilters() => YamlStrings.Get(nameof (InvalidPipelineScheduleBranchFilters));

    public static string InvalidPipelineScheduleBranchFilters(CultureInfo culture) => YamlStrings.Get(nameof (InvalidPipelineScheduleBranchFilters), culture);

    public static string CannotFindBuildResource(object arg0) => YamlStrings.Format(nameof (CannotFindBuildResource), arg0);

    public static string CannotFindBuildResource(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (CannotFindBuildResource), culture, arg0);

    public static string CannotFindTaskId(object arg0, object arg1) => YamlStrings.Format(nameof (CannotFindTaskId), arg0, arg1);

    public static string CannotFindTaskId(object arg0, object arg1, CultureInfo culture) => YamlStrings.Format(nameof (CannotFindTaskId), culture, arg0, arg1);

    public static string CannotFindTaskIdFromArtifactExtension(object arg0) => YamlStrings.Format(nameof (CannotFindTaskIdFromArtifactExtension), arg0);

    public static string CannotFindTaskIdFromArtifactExtension(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (CannotFindTaskIdFromArtifactExtension), culture, arg0);

    public static string EnvironmentNameKeyNotFound(object arg0) => YamlStrings.Format(nameof (EnvironmentNameKeyNotFound), arg0);

    public static string EnvironmentNameKeyNotFound(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (EnvironmentNameKeyNotFound), culture, arg0);

    public static string EnvironmentResourceNameConflict(object arg0, object arg1) => YamlStrings.Format(nameof (EnvironmentResourceNameConflict), arg0, arg1);

    public static string EnvironmentResourceNameConflict(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return YamlStrings.Format(nameof (EnvironmentResourceNameConflict), culture, arg0, arg1);
    }

    public static string ResourceTypeIsMandatoryWithTags() => YamlStrings.Get(nameof (ResourceTypeIsMandatoryWithTags));

    public static string ResourceTypeIsMandatoryWithTags(CultureInfo culture) => YamlStrings.Get(nameof (ResourceTypeIsMandatoryWithTags), culture);

    public static string CannotFindPackageResource(object arg0) => YamlStrings.Format(nameof (CannotFindPackageResource), arg0);

    public static string CannotFindPackageResource(object arg0, CultureInfo culture) => YamlStrings.Format(nameof (CannotFindPackageResource), culture, arg0);

    public static string CannotFindTaskIdForPackageResources(object arg0, object arg1) => YamlStrings.Format(nameof (CannotFindTaskIdForPackageResources), arg0, arg1);

    public static string CannotFindTaskIdForPackageResources(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return YamlStrings.Format(nameof (CannotFindTaskIdForPackageResources), culture, arg0, arg1);
    }

    public static string CannotFindTaskIdFromPackageArtifactExtension(object arg0) => YamlStrings.Format(nameof (CannotFindTaskIdFromPackageArtifactExtension), arg0);

    public static string CannotFindTaskIdFromPackageArtifactExtension(
      object arg0,
      CultureInfo culture)
    {
      return YamlStrings.Format(nameof (CannotFindTaskIdFromPackageArtifactExtension), culture, arg0);
    }

    public static string FailedToAddParameterForWebhook(object arg0, object arg1) => YamlStrings.Format(nameof (FailedToAddParameterForWebhook), arg0, arg1);

    public static string FailedToAddParameterForWebhook(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return YamlStrings.Format(nameof (FailedToAddParameterForWebhook), culture, arg0, arg1);
    }
  }
}
