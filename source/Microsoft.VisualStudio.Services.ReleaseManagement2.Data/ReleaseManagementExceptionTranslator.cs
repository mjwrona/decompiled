// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.ReleaseManagementExceptionTranslator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  internal static class ReleaseManagementExceptionTranslator
  {
    private static readonly Dictionary<int, SqlExceptionFactory> TranslatedExceptions = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        700000,
        new SqlExceptionFactory(typeof (GenericDatabaseUpdateErrorException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700004,
        new SqlExceptionFactory(typeof (ReleaseDefinitionNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700055,
        new SqlExceptionFactory(typeof (ReleaseDefinitionDisabledException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700005,
        new SqlExceptionFactory(typeof (ReleaseDefinitionAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700003,
        new SqlExceptionFactory(typeof (ReleaseManagementObjectNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700006,
        new SqlExceptionFactory(typeof (PlanIdAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700007,
        new SqlExceptionFactory(typeof (ArtifactAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700008,
        new SqlExceptionFactory(typeof (ReleaseAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700009,
        new SqlExceptionFactory(typeof (ArtifactDefinitionDoesNotExistException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700010,
        new SqlExceptionFactory(typeof (ApprovalUpdateException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700011,
        new SqlExceptionFactory(typeof (ReleaseNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700012,
        new SqlExceptionFactory(typeof (ReleaseEnvironmentNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700013,
        new SqlExceptionFactory(typeof (QueueAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700014,
        new SqlExceptionFactory(typeof (QueueNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700015,
        new SqlExceptionFactory(typeof (ReleaseDefinitionEnvironmentTemplateAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700016,
        new SqlExceptionFactory(typeof (ReleaseDefinitionEnvironmentTemplateNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700017,
        new SqlExceptionFactory(typeof (ReleaseDeletionNotAllowedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700041,
        new SqlExceptionFactory(typeof (ReleaseDeletionNotAllowedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700018,
        new SqlExceptionFactory(typeof (InvalidReleaseStatusUpdateException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700019,
        new SqlExceptionFactory(typeof (ReleaseStepNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700020,
        new SqlExceptionFactory(typeof (ReleasesNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700021,
        new SqlExceptionFactory(typeof (DraftReleaseCannotBeStartedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700022,
        new SqlExceptionFactory(typeof (ReleaseDefinitionRevisionAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700023,
        new SqlExceptionFactory(typeof (InvalidReleaseEnvironmentStatusUpdateException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700024,
        new SqlExceptionFactory(typeof (QueueReleaseNotAllowedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700025,
        new SqlExceptionFactory(typeof (DuplicateStepsInsertionException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700026,
        new SqlExceptionFactory(typeof (InvalidDataException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700027,
        new SqlExceptionFactory(typeof (ReleaseNotActiveException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700028,
        new SqlExceptionFactory(typeof (ReleaseEnvironmentNotActiveException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700029,
        new SqlExceptionFactory(typeof (ReleaseDeletionNotAllowedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700030,
        new SqlExceptionFactory(typeof (ReleaseDefinitionAlreadyUpdatedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700031,
        new SqlExceptionFactory(typeof (ReleaseStepNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700032,
        new SqlExceptionFactory(typeof (ReleaseDefinitionEnvironmentNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700033,
        new SqlExceptionFactory(typeof (ReleaseDefinitionHistoryUpdateException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700034,
        new SqlExceptionFactory(typeof (DeploymentStatusAlreadyUpdatedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700035,
        new SqlExceptionFactory(typeof (DeploymentOperationStatusAlreadyUpdatedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700036,
        new SqlExceptionFactory(typeof (ManualInterventionNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700037,
        new SqlExceptionFactory(typeof (ManualInterventionAlreadyUpdatedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700039,
        new SqlExceptionFactory(typeof (FolderNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700038,
        new SqlExceptionFactory(typeof (FolderExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700040,
        new SqlExceptionFactory(typeof (FolderParentNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700042,
        new SqlExceptionFactory(typeof (DeploymentUpdateNotAllowedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700043,
        new SqlExceptionFactory(typeof (ReleaseDefinitionDeletionNotAllowedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700044,
        new SqlExceptionFactory(typeof (DeletedReleaseDefinitionNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700045,
        new SqlExceptionFactory(typeof (DeploymentResourceAlreadyExistsException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700046,
        new SqlExceptionFactory(typeof (DeploymentResourceNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700047,
        new SqlExceptionFactory(typeof (DeletedDefinitionEnvironmentTemplateNotFoundException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700048,
        new SqlExceptionFactory(typeof (NewDeploymentAlreadyStartedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700049,
        new SqlExceptionFactory(typeof (InvalidReleaseEnvironmentStepStatusException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700050,
        new SqlExceptionFactory(typeof (GateUpdateFailedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      },
      {
        700054,
        new SqlExceptionFactory(typeof (ReleaseDefinitionSnapshotRevisionNotMatchedException), new Func<IVssRequestContext, int, SqlException, SqlError, Exception>(ReleaseManagementExceptionTranslator.CreateException))
      }
    };

    internal static IDictionary<int, SqlExceptionFactory> GetTranslatedExceptions() => (IDictionary<int, SqlExceptionFactory>) ReleaseManagementExceptionTranslator.TranslatedExceptions;

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is the one-stop shop for all custom exceptions")]
    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "This is the one-stop shop for all custom exceptions")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is the one-stop shop for all custom exceptions")]
    private static Exception CreateException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      switch (errorNumber)
      {
        case 700000:
          ReleaseManagementExceptionTranslator.TraceException(requestContext, 1999990, sqlException);
          exception = (Exception) new GenericDatabaseUpdateErrorException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GenericDatabaseUpdateError));
          break;
        case 700001:
          ReleaseManagementExceptionTranslator.TraceException(requestContext, 1999991, sqlException);
          exception = (Exception) new TransactionRequiredException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TransactionRequiredError));
          break;
        case 700003:
          exception = (Exception) new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ObjectDoesNotExist, (object) sqlError.ExtractString("objectType"), (object) sqlError.ExtractInt("ID")));
          break;
        case 700004:
          exception = (Exception) new ReleaseDefinitionNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionNotFound, (object) sqlError.ExtractInt("definitionId")));
          break;
        case 700005:
          exception = (Exception) new ReleaseDefinitionAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionAlreadyExists, (object) sqlError.ExtractString("definitionName")));
          break;
        case 700006:
          exception = (Exception) new PlanIdAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PlanIdAlreadyExists, (object) sqlError.ExtractString("planId")));
          break;
        case 700007:
          exception = (Exception) new ArtifactAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactDefinitionAlreadyExists, (object) sqlError.ExtractString("Name")));
          break;
        case 700008:
          exception = (Exception) new ReleaseAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseWithGivenNameAlreadyExists, (object) sqlError.ExtractString("releaseName")));
          break;
        case 700009:
          exception = (Exception) new ArtifactDefinitionDoesNotExistException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactDefinitionDoesNotExist, (object) sqlError.ExtractString("ID")));
          break;
        case 700010:
          exception = (Exception) new ApprovalUpdateException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidApprovalUpdate, (object) sqlError.ExtractInt("stepId")));
          break;
        case 700011:
          exception = (Exception) new ReleaseNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseNotFound, (object) sqlError.ExtractInt("releaseId")));
          break;
        case 700012:
          exception = (Exception) new ReleaseEnvironmentNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseEnvironmentNotFound, (object) sqlError.ExtractInt("releaseId"), (object) sqlError.ExtractInt("releaseEnvironmentId")));
          break;
        case 700013:
          exception = (Exception) new QueueAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.QueueAlreadyExists, (object) sqlError.ExtractString("name")));
          break;
        case 700014:
          exception = (Exception) new QueueNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.QueueNotFound, (object) sqlError.ExtractString("Id")));
          break;
        case 700015:
          exception = (Exception) new ReleaseDefinitionEnvironmentTemplateAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DefinitionEnvironmentTemplateAlreadyExists, (object) sqlError.ExtractString("templateName")));
          break;
        case 700016:
          exception = (Exception) new ReleaseDefinitionEnvironmentTemplateNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DefinitionEnvironmentTemplateNotFound, (object) sqlError.ExtractString("templateId")));
          break;
        case 700017:
          exception = (Exception) new ReleaseDeletionNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeletionNotAllowedDueToCurrentlyActiveOnEnvironments, (object) sqlError.ExtractString("releaseName"), (object) sqlError.ExtractString("environments")));
          break;
        case 700018:
          exception = (Exception) new InvalidReleaseStatusUpdateException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReleaseStatusUpdate, (object) sqlError.ExtractInt("releaseId")));
          break;
        case 700019:
          exception = (Exception) new ReleaseStepNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseStepNotFound, (object) sqlError.ExtractInt("releaseStepId")));
          break;
        case 700020:
          exception = (Exception) new ReleasesNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleasesNotFound, (object) sqlError.ExtractString("ID")));
          break;
        case 700021:
          exception = (Exception) new DraftReleaseCannotBeStartedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DraftReleaseCannotBeStarted, (object) sqlError.ExtractInt("releaseId")));
          break;
        case 700022:
          exception = (Exception) new ReleaseDefinitionRevisionAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionRevisionAlreadyExists, (object) sqlError.ExtractString("definitionId"), (object) sqlError.ExtractString("definitionRevision")));
          break;
        case 700023:
          exception = (Exception) new InvalidReleaseEnvironmentStatusUpdateException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReleaseEnvironmentStatusUpdate, (object) sqlError.ExtractInt("releaseEnvironmentId"), (object) sqlError.ExtractInt("releaseId")));
          break;
        case 700024:
          exception = (Exception) new QueueReleaseNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.QueueReleaseNotAllowed, (object) sqlError.ExtractInt("releaseId")));
          break;
        case 700025:
          exception = (Exception) new DuplicateStepsInsertionException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DuplicateStepsInsertion, (object) sqlError.ExtractString("releaseId"), (object) sqlError.ExtractString("releaseEnvironmentId"), (object) sqlError.ExtractString("rank"), (object) sqlError.ExtractString("stepType"), (object) sqlError.ExtractString("trialNumber")));
          break;
        case 700026:
          exception = (Exception) new InvalidDataException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidData));
          break;
        case 700027:
          exception = (Exception) new ReleaseNotActiveException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseNotInActiveState));
          break;
        case 700028:
          exception = (Exception) new ReleaseEnvironmentNotActiveException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseEnvironmentNotInActiveState));
          break;
        case 700029:
          exception = (Exception) new ReleaseDeletionNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeletionNotAllowedForRetainedReleases, (object) sqlError.ExtractString("releaseName")));
          break;
        case 700030:
          exception = (Exception) new ReleaseDefinitionAlreadyUpdatedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DefinitionAlreadyUpdated));
          break;
        case 700031:
          exception = (Exception) new ReleaseStepNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseStepByDefinitionEnvironmentIdNotFound, (object) sqlError.ExtractString("releaseId"), (object) sqlError.ExtractString("definitionEnvironmentId"), (object) sqlError.ExtractString("trialNumber")));
          break;
        case 700032:
          exception = (Exception) new ReleaseDefinitionEnvironmentNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DefinitionEnvironmentNotFound, (object) sqlError.ExtractString("definitionEnvironmentId")));
          break;
        case 700033:
          exception = (Exception) new ReleaseDefinitionHistoryUpdateException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionSnapshotFileIdUpdateFailed, (object) sqlError.ExtractString("updatedRowsCount")));
          break;
        case 700034:
          DeploymentStatus deploymentStatus1 = (DeploymentStatus) sqlError.ExtractInt("currentState");
          exception = (Exception) new DeploymentStatusAlreadyUpdatedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentStatusAlreadyUpdated, (object) (DeploymentStatus) sqlError.ExtractInt("newState"), (object) deploymentStatus1));
          break;
        case 700035:
          DeploymentOperationStatus deploymentOperationStatus1 = (DeploymentOperationStatus) sqlError.ExtractInt("newState");
          exception = (Exception) new DeploymentOperationStatusAlreadyUpdatedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentOperationStatusAlreadyUpdated, (object) sqlError.ExtractInt("attempt"), (object) sqlError.ExtractInt("environmentId"), (object) sqlError.ExtractInt("releaseId"), (object) deploymentOperationStatus1));
          break;
        case 700036:
          exception = (Exception) new ManualInterventionNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ManualInterventionNotFound, (object) sqlError.ExtractString("manualInterventionId")));
          break;
        case 700037:
          exception = (Exception) new ManualInterventionAlreadyUpdatedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CannotUpdateManualIntervention, (object) sqlError.ExtractString("manualInterventionId")));
          break;
        case 700038:
          exception = (Exception) new FolderExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FolderExists, (object) sqlError.ExtractString("folderPath")));
          break;
        case 700039:
          exception = (Exception) new FolderNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FolderNotFound, (object) sqlError.ExtractString("folderPath")));
          break;
        case 700040:
          exception = (Exception) new FolderParentNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FolderParentNotFound, (object) sqlError.ExtractString("folderPath")));
          break;
        case 700041:
          exception = (Exception) new ReleaseDeletionNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDeletionNotAllowedDueToPendingOnEnvironments, (object) sqlError.ExtractString("releaseName"), (object) sqlError.ExtractString("environments")));
          break;
        case 700042:
          DeploymentStatus deploymentStatus2 = (DeploymentStatus) sqlError.ExtractInt("currentDeploymentState");
          DeploymentOperationStatus deploymentOperationStatus2 = (DeploymentOperationStatus) sqlError.ExtractInt("currentState");
          DeploymentOperationStatus deploymentOperationStatus3 = (DeploymentOperationStatus) sqlError.ExtractInt("newState");
          int num1 = sqlError.ExtractInt("attempt");
          int num2 = sqlError.ExtractInt("environmentId");
          int num3 = sqlError.ExtractInt("releaseId");
          exception = (Exception) new DeploymentUpdateNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentUpdateNotAllowedException, (object) num1, (object) num2, (object) num3, (object) deploymentOperationStatus3, (object) deploymentOperationStatus2, (object) deploymentStatus2));
          exception.Data[(object) "currentDeploymentStatus"] = (object) deploymentStatus2;
          exception.Data[(object) "currentOperationStatus"] = (object) deploymentOperationStatus2;
          exception.Data[(object) "desiredOperationStatus"] = (object) deploymentOperationStatus3;
          exception.Data[(object) "attempt"] = (object) num1;
          exception.Data[(object) "environmentId"] = (object) num2;
          exception.Data[(object) "releaseId"] = (object) num3;
          break;
        case 700043:
          exception = (Exception) new ReleaseDefinitionDeletionNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionDeletionNotAllowed, (object) sqlError.ExtractString("releaseDefinitionName")));
          break;
        case 700044:
          exception = (Exception) new DeletedReleaseDefinitionNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeletedReleaseDefinitionNotFound, (object) sqlError.ExtractInt("definitionId")));
          break;
        case 700045:
          exception = (Exception) new DeploymentResourceAlreadyExistsException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentResourceAlreadyExists, (object) sqlError.ExtractString("resourceIdentifier")));
          break;
        case 700046:
          exception = (Exception) new DeploymentResourceNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentResourceNotFound, (object) sqlError.ExtractInt("resourceId")));
          break;
        case 700047:
          exception = (Exception) new DeletedDefinitionEnvironmentTemplateNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeletedDefinitionEnvironmentTemplateNotFound, (object) sqlError.ExtractString("templateId")));
          break;
        case 700048:
          exception = (Exception) new NewDeploymentAlreadyStartedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NewDeploymentAlreadyStarted, (object) sqlError.ExtractInt("definitionEnvironmentId"), (object) sqlError.ExtractInt("releaseId"), (object) sqlError.ExtractInt("latestDeploymentReleaseId")));
          break;
        case 700049:
          ReleaseEnvironmentStepStatus environmentStepStatus1 = (ReleaseEnvironmentStepStatus) sqlError.ExtractInt("currentStepState");
          ReleaseEnvironmentStepStatus environmentStepStatus2 = (ReleaseEnvironmentStepStatus) sqlError.ExtractInt("expectedStepState");
          exception = (Exception) new InvalidReleaseEnvironmentStepStatusException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReleaseEnvironmentStatus, (object) sqlError.ExtractInt("stepId"), (object) sqlError.ExtractInt("releaseId"), (object) environmentStepStatus2, (object) environmentStepStatus1));
          break;
        case 700050:
          string newlyIgnoredGatesJson = sqlError.ExtractString("exitingIgnoredGates");
          string existingIgnoredGatesJson = sqlError.ExtractString("afterGatesIgnored");
          GateStatus gateStatus = (GateStatus) sqlError.ExtractInt("gateStatus");
          int gateStepId = sqlError.ExtractInt("gateStepId");
          exception = (Exception) IgnoreGatesUpdateExceptionExtension.GetException(sqlError.ExtractInt("releaseId"), gateStepId, gateStatus, existingIgnoredGatesJson, newlyIgnoredGatesJson);
          break;
        case 700054:
          exception = (Exception) new ReleaseDefinitionSnapshotRevisionNotMatchedException(Resources.ReleaseDefinitionSnapshotRevisionNotMatched);
          break;
        case 700055:
          exception = (Exception) new ReleaseDefinitionDisabledException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionDisabled, (object) sqlError.ExtractInt("definitionId")));
          break;
      }
      return exception;
    }

    private static void TraceException(
      IVssRequestContext requestContext,
      int tracePoint,
      SqlException sqlException)
    {
      if (requestContext == null)
        return;
      requestContext.TraceException(tracePoint, "ReleaseManagementService", "Service", (Exception) sqlException);
    }
  }
}
