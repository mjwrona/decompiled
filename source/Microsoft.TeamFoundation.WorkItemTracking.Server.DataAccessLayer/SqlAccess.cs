// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SqlAccess
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal abstract class SqlAccess : WorkItemTrackingResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected SqlAccess() => this.InitializeInternal();

    public override void Dispose()
    {
      if (this.SqlBatchExemptionLock != null)
      {
        this.SqlBatchExemptionLock.Dispose();
        this.SqlBatchExemptionLock = (IDisposable) null;
      }
      base.Dispose();
    }

    private void InitializeInternal()
    {
      this.ContainerErrorCode = 50000;
      this.NeedHandleCustomException = true;
    }

    internal bool NeedHandleCustomException { set; get; }

    internal IDisposable SqlBatchExemptionLock { set; private get; }

    public void ExecuteBatchPayload(
      string sqlBatch,
      List<SqlParameter> parameterList,
      out Payload resultPayload)
    {
      this.ExecuteBatchPayload(sqlBatch, parameterList, int.MaxValue, (PayloadConverter) null, out resultPayload);
    }

    public void ExecuteBatchPayload(
      string sqlBatch,
      List<SqlParameter> parameterList,
      int inMemoryTableCount,
      PayloadConverter payloadConverter,
      out Payload resultPayload)
    {
      this.ExecuteBatchPayload(sqlBatch, parameterList, inMemoryTableCount, payloadConverter, out resultPayload, int.MaxValue, (IDictionary<int, IPayloadTableSchema>) null);
    }

    public virtual void ExecuteBatchPayload(
      string sqlBatch,
      List<SqlParameter> parameterList,
      int inMemoryTableCount,
      PayloadConverter payloadConverter,
      out Payload resultPayload,
      int expectedTableCount,
      IDictionary<int, IPayloadTableSchema> schemas)
    {
      Payload payload = new Payload(schemas, payloadConverter, inMemoryTableCount);
      resultPayload = payload;
      this.RequestContext.TraceBlock(900282, 900284, "DataAccessLayer", nameof (SqlAccess), nameof (ExecuteBatchPayload), (Action) (() =>
      {
        this.ExecSqlBatch<Payload>(this.RequestContext, sqlBatch, parameterList, (ExecuteUnknownCallback<Payload>) (reader =>
        {
          if (!payload.Fill(reader, expectedTableCount))
          {
            payload.SqlExceptionHandler = new Payload.SqlExceptionCallback(((TeamFoundationSqlResourceComponent) this).MapException);
            payload.SqlTypeExeptionHandler = new Payload.SqlTypeExceptionCallback(((TeamFoundationSqlResourceComponent) this).TranslateException);
            payload.SqlAccess = this;
          }
          return payload;
        }));
        this.RequestContext.Trace(900289, TraceLevel.Info, "DataAccessLayer", nameof (SqlAccess), "Resulting Payload contains {0} tables.", (object) payload.TableCount);
      }));
    }

    public virtual TResult ExecSqlBatch<TResult>(
      IVssRequestContext requestContext,
      string sql,
      List<SqlParameter> parameterList,
      ExecuteUnknownCallback<TResult> execUnknownCallback)
    {
      if (execUnknownCallback == null)
        execUnknownCallback = (ExecuteUnknownCallback<TResult>) (r => default (TResult));
      using (SqlCommand command = this.PrepareSqlBatch(sql.Length))
      {
        if (requestContext.IsCanceled)
          throw new LegacyCancelledByUserException(DalResourceStrings.Get("CancelledByUser"), 602006);
        this.AddStatement(sql, parameterList.Count);
        this.AddParameters(command, parameterList);
        return this.ExecuteUnknown<TResult>(execUnknownCallback);
      }
    }

    protected override object ExecuteUnknown(SqlDataReader reader, object param) => param is ExecuteUnknownCallback<object> executeUnknownCallback ? executeUnknownCallback((IDataReader) reader) : (object) null;

    public TResult ExecuteUnknown<TResult>(ExecuteUnknownCallback<TResult> callback) => (TResult) this.ExecuteUnknown((object) (ExecuteUnknownCallback<object>) (reader => (object) callback(reader)));

    protected virtual void AddParameters(SqlCommand command, List<SqlParameter> parameterList)
    {
      this.RequestContext.Trace(900288, TraceLevel.Verbose, "DataAccessLayer", nameof (SqlAccess), command.CommandText + " - Parameters: ");
      foreach (SqlParameter parameter in parameterList)
      {
        this.RequestContext.Trace(900288, TraceLevel.Verbose, "DataAccessLayer", nameof (SqlAccess), "{0}:{1}", (object) parameter.ParameterName, parameter.Value);
        command.Parameters.Add(parameter);
      }
    }

    protected override void TranslateException(SqlTypeException sqlTypeException)
    {
      this.RequestContext.Trace(900291, TraceLevel.Error, "DataAccessLayer", nameof (SqlAccess), "Exception: msg-'{0}', string-'{1}'.", (object) sqlTypeException.Message, (object) sqlTypeException.ToString());
      throw new LegacySqlDataTypeConversionException(sqlTypeException.Message, sqlTypeException.InnerException);
    }

    protected override bool HandleCustomException(SqlException ex)
    {
      if (!this.NeedHandleCustomException)
        return false;
      int num = 0;
      foreach (SqlError error in ex.Errors)
      {
        if (this.ContainerErrorCode != 0 && error.Number == this.ContainerErrorCode)
          num = TeamFoundationServiceException.ExtractInt(error, "error");
        else if (!error.IsInformational())
          num = error.Number;
      }
      bool flag = false;
      if (num == 600047)
      {
        if (this.RequestContext.GetAccessIntent("WorkItem") == AccessIntent.Read)
          throw new WorkItemTrackingElevateAccessIntentException((Exception) ex);
        this.RequestContext.Trace(900406, TraceLevel.Info, "DataAccessLayer", nameof (SqlAccess), "User is not in system. On best effort trying to pull down his membership info from BIS");
        try
        {
          SyncBase syncBase = new SyncBase(this.RequestContext);
          using (this.AcquireExemptionLock())
            flag = syncBase.SyncIdentity(this.RequestContext.UserContext);
        }
        catch (Exception ex1)
        {
          this.RequestContext.TraceException(900407, "DataAccessLayer", nameof (SqlAccess), ex1);
        }
      }
      this.NeedHandleCustomException = false;
      return flag;
    }

    internal SqlDataReader SqlDataReader => this.DataReader;

    static SqlAccess()
    {
      Dictionary<int, SqlAccess.SqlExceptionAndResourceFactory> source = new Dictionary<int, SqlAccess.SqlExceptionAndResourceFactory>();
      source.Add(600028, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "InvalidProjectNodeId"));
      source.Add(600032, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorInvalidQueryId"));
      source.Add(600029, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorInvalidQueryName"));
      source.Add(600030, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorInvalidQueryText"));
      source.Add(600036, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorNotUniqueStoredQuery"));
      source.Add(600019, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorAddConstantNotValidDomainAccount"));
      source.Add(600038, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorAuthorizeFieldChanges"));
      source.Add(600039, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorNoPermissionCacheOutofdate"));
      source.Add(600077, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorNoPermissionToModify"));
      source.Add(600083, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorNoPermissionToModify"));
      source.Add(600010, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(602205, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorNoPermissionToModifyField"));
      source.Add(600011, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600124, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorChangeFieldReportingTypeAlreadySet"));
      source.Add(600125, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600012, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkitemTypeException"));
      source.Add(600006, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600007, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorNoPermissionToModifyRules"));
      source.Add(600015, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkitemTypeException"));
      source.Add(600016, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600045, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorUserOrGroupNotFound"));
      source.Add(600047, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorUserOrGroupNotFound"));
      source.Add(600027, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600075, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorChangeWorkItemTypeAlreadyChanged"));
      source.Add(600076, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600080, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorChangeWorkItemTypeInvalidProjectID"));
      source.Add(600082, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorChangeActionAlreadyChanged"));
      source.Add(600060, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataFieldsOutOfRange"));
      source.Add(600061, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataInvalidFieldType"));
      source.Add(600062, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataInvalidFieldName"));
      source.Add(600140, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataInvalidFieldReferenceName"));
      source.Add(600063, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataUnimplementedReadRuleCondition"));
      source.Add(600064, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataRuleFlagsNonEditable"));
      source.Add(600065, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataRuleEnforcementIsNotImplemented"));
      source.Add(600066, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminDataDenyAdminNotAllowed"));
      source.Add(600069, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "BadAdminDataTypeCannotChangeAfterFieldHasBeenUsed"));
      source.Add(600123, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadAdminCannotAddRuleOnCoreFields"));
      source.Add(600074, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorLookupRuleTreeIdNotFound"));
      source.Add(600072, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorWorkItemUpdateForceRollback"));
      source.Add(600141, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorAddFileFileAlreadyExists"));
      source.Add(600151, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorChangeActionAlreadyChanged"));
      source.Add(600128, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "DeleteFieldFieldDoesNotExist"));
      source.Add(600129, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "DeleteFieldFieldInUse"));
      source.Add(600153, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorInvalidFieldSpecified"));
      source.Add(600154, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorAddSetInvalidParentIDOrConstID"));
      source.Add(600157, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(602014, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorQueryDepthTooLarge"));
      source.Add(602015, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600122, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorWorkItemUpdateWorkItemMissingOrUpdated"));
      source.Add(602204, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorAddFileFailedToFindFile"));
      source.Add(5074, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "IndexAssociatedWithFieldError"));
      source.Add(602001, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorUnexpected"));
      source.Add(602002, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorChangesRejected"));
      source.Add(602003, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorBadServiceConfig"));
      source.Add(602004, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorChangesRejected"));
      source.Add(602005, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorRequestNotCancelable"));
      source.Add(600149, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorAddFileFileAlreadyExists"));
      source.Add(600450, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorDuplicateWorkItemTypeletID"));
      source.Add(600451, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorDuplicateWorkItemTypeletName"));
      source.Add(600452, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorWorkItemTypeletDoesNotExist"));
      source.Add(-1, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600158, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorFieldNameInUse"));
      source.Add(600159, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorFieldReferenceNameInUse"));
      source.Add(600177, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorAddConstantStringNotSupportedByCollation"));
      source.Add(600178, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600160, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600161, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600162, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600163, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600164, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600165, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600166, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600167, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600168, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600169, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600170, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorMetadataCacheOutofdate"));
      source.Add(600250, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "LinkTypeNotFound"));
      source.Add(600251, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_InvalidRules"));
      source.Add(600252, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_InvalidRules"));
      source.Add(600253, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_InvalidTopology"));
      source.Add(600254, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ReverseNameInvalid"));
      source.Add(600255, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ReverseNameInvalid"));
      source.Add(600256, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ReferenceNameInUse"));
      source.Add(600257, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ForwardNameInUse"));
      source.Add(600258, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ReverseNameInUse"));
      source.Add(600259, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_NothingSpecified"));
      source.Add(600260, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ReverseNameInvalid"));
      source.Add(600261, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ReverseNameInvalid"));
      source.Add(600262, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ForwardNameInUse"));
      source.Add(600263, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ReverseNameInvalid"));
      source.Add(600264, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_EditDenied"));
      source.Add(600265, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_ModifyTopology"));
      source.Add(600266, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ImportWorkItemLinkTypeException_DeleteDenied"));
      source.Add(600267, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "AddLinkCannotLinkToSelf"));
      source.Add(600277, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "LinkAuthorizationFailed"));
      source.Add(600312, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "LinkAuthorizationFailedAreaIdChanged"));
      source.Add(600269, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "AddLinkDisabledType"));
      source.Add(600305, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorWorkItemTypeAlreadyExists"));
      source.Add(600306, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorGlobalListInUse"));
      source.Add(600307, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "WorkItemTypeCategoryTypeNotInProject"));
      source.Add(600308, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorDuplicateCatName"));
      source.Add(600309, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorDuplicateCatRefName"));
      source.Add(600310, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "ErrorWorkItemTypeInUse"));
      source.Add(600311, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "WorkItemTypeCategoryCannotDeleteDefaultType"));
      source.Add(600289, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "QueryHierarchyDeniedOrNotExist"));
      source.Add(600286, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600288, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600287, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600171, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException)));
      source.Add(600139, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyBatchSaveException), "ErrorWorkItemUpdateForceRollback"));
      source.Add(600031, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyDeniedOrNotExist), "ErrorDeniedOrNotExist"));
      source.Add(600081, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySecurityException), "ErrorNotANamespaceAdmin"));
      source.Add(600034, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySecurityException), "ErrorNotAProjectUser"));
      source.Add(600276, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "LinkAuthorizationFailed"));
      source.Add(600280, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "LinkAuthorizationFailedLinkLocked"));
      source.Add(600270, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "AddLinkCircularity"));
      source.Add(600271, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "AddLinkExtraParent"));
      source.Add(600272, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "AddLinkCircularity"));
      source.Add(600273, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "AddLinkAlreadyExists"));
      source.Add(600278, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "AddLinkMaxDepthExceeded"));
      source.Add(600279, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyValidationException), "AddLinkLimitExceeded"));
      source.Add(600274, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "LinkNotFoundForEdit"));
      source.Add(600275, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemLinkException), "LinkNotFoundForDelete"));
      source.Add(600291, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyDeniedOrNotExist"));
      source.Add(600292, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyParentDoesNotExist"));
      source.Add(600293, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyParentIsNotAFolder"));
      source.Add(600290, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyNameConflictsWithExistingItem"));
      source.Add(600295, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyCircularReference"));
      source.Add(600294, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyTypeMismatch"));
      source.Add(600296, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyItemAlreadyExists"));
      source.Add(600297, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyCannotMoveRootFolder"));
      source.Add(600298, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "QueryHierarchyCannotDeleteRootFolder"));
      source.Add(600299, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemGenericException)));
      source.Add(600314, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryItemException), "TooManyQueryItemUnderParent"));
      source.Add(600315, new SqlAccess.SqlExceptionAndResourceFactory(typeof (TooManyQueryItemsReturnedException), "TooManyQueryItemsReturned"));
      source.Add(600304, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyWorkItemDestroyException), "ErrorDeniedOrNotExist"));
      source.Add(600173, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyFieldLimitExceededException)));
      source.Add(480000, new SqlAccess.SqlExceptionAndResourceFactory(typeof (DateTimeShiftDetectedException)));
      source.Add(480001, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyTagsSaveFailedException)));
      source.Add(600174, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyQueryOptimizationFailedException)));
      source.Add(241, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlDataTypeConversionException), "DataTypeInvalid"));
      source.Add(242, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlDataTypeConversionException), "DataTypeInvalid"));
      source.Add(245, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlDataTypeConversionException), "DataTypeInvalid"));
      source.Add(8114, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlDataTypeConversionException), "DataTypeInvalid"));
      source.Add(8115, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlDataTypeConversionException), "DataTypeInvalid"));
      source.Add(8023, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlDataTypeConversionException), "DataTypeInvalid"));
      source.Add(701, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorDeadlockException), "ErrorSqlInsufficientMemory"));
      source.Add(1205, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorDeadlockException), "ErrorSqlErrorDeadlock"));
      source.Add(600172, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorDeadlockException), "ErrorSqlErrorDeadlock"));
      source.Add(2801, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorDeadlockException), "ErrorSqlErrorDeadlock"));
      source.Add(4413, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorViewBindingErrorException), "ErrorSqlErrorDeadlock"));
      source.Add(4501, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorViewBindingErrorException), "ErrorSqlErrorDeadlock"));
      source.Add(4502, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorViewBindingErrorException), "ErrorSqlErrorDeadlock"));
      source.Add(-2, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCommandTimeOutException), "ErrorSqlErrorTimeOutOrServerNotResponding"));
      source.Add(8645, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCommandTimeOutException), "ErrorSqlInsufficientMemory"));
      source.Add(18452, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(4061, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(4062, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(4063, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(4064, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18450, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18451, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18457, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18458, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18459, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18460, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18461, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18470, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18486, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18487, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(18488, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(1396, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlLoginFailedException), "ErrorSqlErrorLoginFailed"));
      source.Add(4060, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17142, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(2, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(52, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(53, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(64, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(233, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17197, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17830, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(18456, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(11001, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(10061, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(9105, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(58, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(1229, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(921, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(922, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(923, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(924, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(926, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(928, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(929, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(942, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(943, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(945, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(946, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(948, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(949, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(950, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(951, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(952, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(3908, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(9001, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17144, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17148, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17806, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17807, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17809, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17810, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17813, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17825, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17826, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17827, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17828, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17829, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17832, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17835, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(17836, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(605, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(10060, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(10064, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(10065, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(6005, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(51, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(109, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(232, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(1231, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(-2146892993, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(10053, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(3980, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(10054, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlCannotOpenDatabaseException), "ErrorCannotOpenDatabase"));
      source.Add(0, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyCancelledByUserException), "ErrorServerInternalUnknown"));
      source.Add(208, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyGeneralSqlException)));
      source.Add(2812, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyGeneralSqlException)));
      source.Add(207, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyGeneralSqlException)));
      source.Add(4012, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyGeneralSqlException)));
      source.Add(8623, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacyGeneralSqlException)));
      source.Add(266, new SqlAccess.SqlExceptionAndResourceFactory(typeof (DatabaseConfigurationException)));
      source.Add(137, new SqlAccess.SqlExceptionAndResourceFactory(typeof (DatabaseConfigurationException)));
      source.Add(9954, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorFullTextException)));
      source.Add(9955, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorFullTextException)));
      source.Add(9956, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorFullTextException)));
      source.Add(9957, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorFullTextException)));
      source.Add(9958, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorFullTextException)));
      for (int key = 7600; key <= 7700; ++key)
        source.Add(key, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorFullTextException)));
      for (int key = 30000; key <= 30100; ++key)
        source.Add(key, new SqlAccess.SqlExceptionAndResourceFactory(typeof (LegacySqlErrorFullTextException)));
      SqlAccess.s_sqlExceptionFactories = source.ToDictionary<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>, int, SqlExceptionFactory>((System.Func<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>, int>) (pair => pair.Key), (System.Func<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>, SqlExceptionFactory>) (pair => pair.Value.Factory));
      ExceptionHelper.SqlErrorResourceNames = source.Where<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>>((System.Func<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>, bool>) (pair => pair.Value.ResourceName != null)).ToDictionary<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>, int, string>((System.Func<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>, int>) (pair => pair.Key), (System.Func<KeyValuePair<int, SqlAccess.SqlExceptionAndResourceFactory>, string>) (pair => pair.Value.ResourceName));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) SqlAccess.s_sqlExceptionFactories;

    internal static SqlParameter CreateTableParameter<T>(
      string parameterName,
      WorkItemTrackingTableValueParameter<T> tvp)
    {
      object obj = (object) null;
      if (tvp != null && !tvp.IsNullOrEmpty)
        obj = (object) tvp;
      return new SqlParameter(parameterName, obj)
      {
        TypeName = tvp.TypeName,
        SqlDbType = SqlDbType.Structured
      };
    }

    internal static SqlParameter CreateNVarCharParameter(
      string parameterName,
      string parameterValue)
    {
      SqlParameter nvarCharParameter = parameterValue.Length > 4000 ? new SqlParameter(parameterName, SqlDbType.NVarChar) : new SqlParameter(parameterName, SqlDbType.NVarChar, 4000);
      nvarCharParameter.Value = (object) parameterValue;
      return nvarCharParameter;
    }

    private struct SqlExceptionAndResourceFactory
    {
      internal SqlExceptionFactory Factory;
      internal string ResourceName;

      internal SqlExceptionAndResourceFactory(Type exceptionType, string resourceName)
      {
        this.Factory = new SqlExceptionFactory(exceptionType, SqlAccess.SqlExceptionAndResourceFactory.GetFactoryMethod(exceptionType));
        this.ResourceName = resourceName;
      }

      internal SqlExceptionAndResourceFactory(Type exceptionType)
      {
        this.Factory = new SqlExceptionFactory(exceptionType, SqlAccess.SqlExceptionAndResourceFactory.GetFactoryMethod(exceptionType));
        this.ResourceName = (string) null;
      }

      private static Func<IVssRequestContext, int, SqlException, SqlError, Exception> GetFactoryMethod(
        Type exceptionType)
      {
        return (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((context, errorNumber, sqlException, sqlError) =>
        {
          Func<string> func = (Func<string>) (() => ExceptionHelper.GetErrorMessage(sqlException.Message, errorNumber));
          if (exceptionType == typeof (LegacyValidationException))
            return (Exception) new LegacyValidationException(func(), errorNumber);
          if (exceptionType == typeof (LegacyConfigurationException))
            return (Exception) new LegacyConfigurationException(func(), errorNumber);
          if (exceptionType == typeof (LegacySecurityException))
            return (Exception) new LegacySecurityException(func(), errorNumber);
          return exceptionType == typeof (LegacyDeniedOrNotExist) ? (Exception) new LegacyDeniedOrNotExist(func()) : new SqlExceptionFactory(exceptionType).Create(context, errorNumber, sqlException, sqlError);
        });
      }
    }
  }
}
