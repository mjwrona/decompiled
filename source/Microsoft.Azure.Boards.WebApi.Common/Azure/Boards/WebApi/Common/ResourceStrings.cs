// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.ResourceStrings
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  internal static class ResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ResourceStrings), typeof (ResourceStrings).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ResourceStrings.s_resMgr;

    private static string Get(string resourceName) => ResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.Get(resourceName) : ResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetInt(resourceName) : (int) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetBool(resourceName) : (bool) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ResourceStrings.Get(resourceName, culture);
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

    public static string ClassificationNodeUrlCreationFailed() => ResourceStrings.Get(nameof (ClassificationNodeUrlCreationFailed));

    public static string ClassificationNodeUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (ClassificationNodeUrlCreationFailed), culture);

    public static string EnterpriseWithUrlNotFound(object arg0) => ResourceStrings.Format(nameof (EnterpriseWithUrlNotFound), arg0);

    public static string EnterpriseWithUrlNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (EnterpriseWithUrlNotFound), culture, arg0);

    public static string FieldUrlCreationFailed() => ResourceStrings.Get(nameof (FieldUrlCreationFailed));

    public static string FieldUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (FieldUrlCreationFailed), culture);

    public static string InvalidOrMissingResourceId() => ResourceStrings.Get(nameof (InvalidOrMissingResourceId));

    public static string InvalidOrMissingResourceId(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidOrMissingResourceId), culture);

    public static string InvalidParameterType(object arg0, object arg1) => ResourceStrings.Format(nameof (InvalidParameterType), arg0, arg1);

    public static string InvalidParameterType(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidParameterType), culture, arg0, arg1);

    public static string InvalidRelationResourceSize() => ResourceStrings.Get(nameof (InvalidRelationResourceSize));

    public static string InvalidRelationResourceSize(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidRelationResourceSize), culture);

    public static string InvalidRelationUrl() => ResourceStrings.Get(nameof (InvalidRelationUrl));

    public static string InvalidRelationUrl(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidRelationUrl), culture);

    public static string InvalidWiql() => ResourceStrings.Get(nameof (InvalidWiql));

    public static string InvalidWiql(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidWiql), culture);

    public static string LocationServiceException() => ResourceStrings.Get(nameof (LocationServiceException));

    public static string LocationServiceException(CultureInfo culture) => ResourceStrings.Get(nameof (LocationServiceException), culture);

    public static string NullOrEmptyParameter(object arg0) => ResourceStrings.Format(nameof (NullOrEmptyParameter), arg0);

    public static string NullOrEmptyParameter(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (NullOrEmptyParameter), culture, arg0);

    public static string WorkItemAttachmentUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemAttachmentUrlCreationFailed));

    public static string WorkItemAttachmentUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemAttachmentUrlCreationFailed), culture);

    public static string WorkItemIconUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemIconUrlCreationFailed));

    public static string WorkItemIconUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemIconUrlCreationFailed), culture);

    public static string WorkItemRevisionUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemRevisionUrlCreationFailed));

    public static string WorkItemRevisionUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemRevisionUrlCreationFailed), culture);

    public static string WorkItemsHubUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemsHubUrlCreationFailed));

    public static string WorkItemsHubUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemsHubUrlCreationFailed), culture);

    public static string WorkItemUpdatesUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemUpdatesUrlCreationFailed));

    public static string WorkItemUpdatesUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemUpdatesUrlCreationFailed), culture);

    public static string WorkItemUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemUrlCreationFailed));

    public static string WorkItemUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemUrlCreationFailed), culture);

    public static string WorkItemUrlNotWellFormed(object arg0) => ResourceStrings.Format(nameof (WorkItemUrlNotWellFormed), arg0);

    public static string WorkItemUrlNotWellFormed(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemUrlNotWellFormed), culture, arg0);

    public static string WorkItemCommentUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemCommentUrlCreationFailed));

    public static string WorkItemCommentUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemCommentUrlCreationFailed), culture);

    public static string WorkItemCommentResponseUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemCommentResponseUrlCreationFailed));

    public static string WorkItemCommentResponseUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemCommentResponseUrlCreationFailed), culture);

    public static string WorkItemCommentVersionUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemCommentVersionUrlCreationFailed));

    public static string WorkItemCommentVersionUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemCommentVersionUrlCreationFailed), culture);

    public static string TestWorkItemsDeletionError() => ResourceStrings.Get(nameof (TestWorkItemsDeletionError));

    public static string TestWorkItemsDeletionError(CultureInfo culture) => ResourceStrings.Get(nameof (TestWorkItemsDeletionError), culture);

    public static string IdentityRefNotAcceptedException() => ResourceStrings.Get(nameof (IdentityRefNotAcceptedException));

    public static string IdentityRefNotAcceptedException(CultureInfo culture) => ResourceStrings.Get(nameof (IdentityRefNotAcceptedException), culture);

    public static string InvalidBatchWorkItemUpdateJson(object arg0) => ResourceStrings.Format(nameof (InvalidBatchWorkItemUpdateJson), arg0);

    public static string InvalidBatchWorkItemUpdateJson(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidBatchWorkItemUpdateJson), culture, arg0);

    public static string RelationAlreadyExists() => ResourceStrings.Get(nameof (RelationAlreadyExists));

    public static string RelationAlreadyExists(CultureInfo culture) => ResourceStrings.Get(nameof (RelationAlreadyExists), culture);

    public static string UnexpectedException() => ResourceStrings.Get(nameof (UnexpectedException));

    public static string UnexpectedException(CultureInfo culture) => ResourceStrings.Get(nameof (UnexpectedException), culture);

    public static string MissingPatchDocument() => ResourceStrings.Get(nameof (MissingPatchDocument));

    public static string MissingPatchDocument(CultureInfo culture) => ResourceStrings.Get(nameof (MissingPatchDocument), culture);

    public static string ExpandParameterConflict() => ResourceStrings.Get(nameof (ExpandParameterConflict));

    public static string ExpandParameterConflict(CultureInfo culture) => ResourceStrings.Get(nameof (ExpandParameterConflict), culture);

    public static string InvalidAsOfParameter() => ResourceStrings.Get(nameof (InvalidAsOfParameter));

    public static string InvalidAsOfParameter(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidAsOfParameter), culture);

    public static string WorkItemNotFoundAtTime(object arg0, object arg1) => ResourceStrings.Format(nameof (WorkItemNotFoundAtTime), arg0, arg1);

    public static string WorkItemNotFoundAtTime(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemNotFoundAtTime), culture, arg0, arg1);

    public static string QueryParameterOutOfRangeWithRangeValues(
      object arg0,
      object arg1,
      object arg2)
    {
      return ResourceStrings.Format(nameof (QueryParameterOutOfRangeWithRangeValues), arg0, arg1, arg2);
    }

    public static string QueryParameterOutOfRangeWithRangeValues(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (QueryParameterOutOfRangeWithRangeValues), culture, arg0, arg1, arg2);
    }

    public static string ProjectNotFound() => ResourceStrings.Get(nameof (ProjectNotFound));

    public static string ProjectNotFound(CultureInfo culture) => ResourceStrings.Get(nameof (ProjectNotFound), culture);

    public static string ProcessServiceDeploymentTemplateUpdateBlocked() => ResourceStrings.Get(nameof (ProcessServiceDeploymentTemplateUpdateBlocked));

    public static string ProcessServiceDeploymentTemplateUpdateBlocked(CultureInfo culture) => ResourceStrings.Get(nameof (ProcessServiceDeploymentTemplateUpdateBlocked), culture);

    public static string ProcessServiceInvalidTemplate() => ResourceStrings.Get(nameof (ProcessServiceInvalidTemplate));

    public static string ProcessServiceInvalidTemplate(CultureInfo culture) => ResourceStrings.Get(nameof (ProcessServiceInvalidTemplate), culture);

    public static string ProcessServiceMultipleUpdateBlocked(object arg0) => ResourceStrings.Format(nameof (ProcessServiceMultipleUpdateBlocked), arg0);

    public static string ProcessServiceMultipleUpdateBlocked(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ProcessServiceMultipleUpdateBlocked), culture, arg0);

    public static string ProcessServiceUpdateBlocked(object arg0) => ResourceStrings.Format(nameof (ProcessServiceUpdateBlocked), arg0);

    public static string ProcessServiceUpdateBlocked(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ProcessServiceUpdateBlocked), culture, arg0);

    public static string ProcessServiceUpdateBlockedDeleting(object arg0) => ResourceStrings.Format(nameof (ProcessServiceUpdateBlockedDeleting), arg0);

    public static string ProcessServiceUpdateBlockedDeleting(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ProcessServiceUpdateBlockedDeleting), culture, arg0);

    public static string PromoteProcessStarted(object arg0, object arg1) => ResourceStrings.Format(nameof (PromoteProcessStarted), arg0, arg1);

    public static string PromoteProcessStarted(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (PromoteProcessStarted), culture, arg0, arg1);

    public static string ErrorContactingRemoteServer() => ResourceStrings.Get(nameof (ErrorContactingRemoteServer));

    public static string ErrorContactingRemoteServer(CultureInfo culture) => ResourceStrings.Get(nameof (ErrorContactingRemoteServer), culture);

    public static string InvalidRelationComment() => ResourceStrings.Get(nameof (InvalidRelationComment));

    public static string InvalidRelationComment(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidRelationComment), culture);

    public static string InvalidRelationName() => ResourceStrings.Get(nameof (InvalidRelationName));

    public static string InvalidRelationName(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidRelationName), culture);
  }
}
