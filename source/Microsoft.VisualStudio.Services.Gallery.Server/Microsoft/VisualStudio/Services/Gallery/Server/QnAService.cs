// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.QnAService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class QnAService : IQnAService, IVssFrameworkService
  {
    private const string s_area = "gallery";
    private const string s_layer = "qnaservice";
    private readonly IQnAUtils _qnAUtils;
    private readonly IMailNotification _mailNotification;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public QnAService()
    {
      this._qnAUtils = (IQnAUtils) new QnAUtils("gallery", "qnaservice");
      this._mailNotification = (IMailNotification) new MailNotification();
    }

    internal QnAService(IQnAUtils qnAUtilsObject, IMailNotification _mailNotificationObject)
    {
      this._qnAUtils = qnAUtilsObject;
      this._mailNotification = _mailNotificationObject;
    }

    public Question CreateQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Question question)
    {
      Stopwatch.StartNew();
      question = this.PreChecksForModifyQnAItem(requestContext, (QnAItem) question) as Question;
      PublishedExtension publishedExtension = this._qnAUtils.ValidateAndGetPublishedExtension(requestContext, publisherName, extensionName);
      ExtensionQnAItem extensionQnAItem = (ExtensionQnAItem) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent1 qnAcomponent1)
          extensionQnAItem = qnAcomponent1.CreateQnAItem(publishedExtension.ExtensionId, question.User.Id, question.Text, publishedExtension.Versions[0].Version, DateTime.UtcNow, -1L, isQuestion: true);
      }
      if (extensionQnAItem == null)
        throw new QnAUnhandledException(GalleryResources.QnAUnhandledExceptionMessage((object) "creating", (object) "Question".ToLower(CultureInfo.CurrentCulture)));
      Question qnAitem = this._qnAUtils.ConvertToQnAItem(requestContext, extensionQnAItem) as Question;
      qnAitem.Status |= QnAItemStatus.UserEditable;
      this._mailNotification.SendMailNotificationToPublisher(requestContext, publishedExtension, (MailNotificationEventData) this.PopulateNewQnAItemNotificationToPublisherEventData(requestContext, publishedExtension, (QnAItem) qnAitem, qnAitem.User.DisplayName));
      return qnAitem;
    }

    public Response CreateResponse(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      Response response)
    {
      Stopwatch.StartNew();
      response = this.PreChecksForModifyQnAItem(requestContext, (QnAItem) response) as Response;
      ArgumentUtility.CheckForOutOfRange(questionId, nameof (questionId), 1L, 9223372036854775806L);
      PublishedExtension publishedExtension = this._qnAUtils.ValidateAndGetPublishedExtension(requestContext, publisherName, extensionName);
      ExtensionQnAItem extensionQnAitem = (ExtensionQnAItem) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent1 qnAcomponent1)
          extensionQnAitem = qnAcomponent1.GetQnAItem(publishedExtension.ExtensionId, -1L, questionId);
      }
      if (extensionQnAitem == null)
        throw new QnAItemDoesNotExistException(GalleryResources.QnAQuestionDoesNotExistException((object) questionId));
      bool isPublisherCreated = this._qnAUtils.IsUserPublisherWithRequiredPermissions(requestContext, publishedExtension);
      if (this._qnAUtils.GetAuthenticatedIdentity(requestContext).Id != extensionQnAitem.UserId && !isPublisherCreated)
        throw new QnAOperationForbidden(GalleryResources.QnACreateResponseAccessDenied());
      ExtensionQnAItem extensionQnAItem = (ExtensionQnAItem) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent1 qnAcomponent1)
          extensionQnAItem = qnAcomponent1.CreateQnAItem(publishedExtension.ExtensionId, response.User.Id, response.Text, publishedExtension.Versions[0].Version, DateTime.UtcNow, questionId, isPublisherCreated);
      }
      if (extensionQnAItem == null)
        throw new QnAUnhandledException(GalleryResources.QnAUnhandledExceptionMessage((object) "creating", (object) "Response".ToLower(CultureInfo.CurrentCulture)));
      Response qnAitem = this._qnAUtils.ConvertToQnAItem(requestContext, extensionQnAItem) as Response;
      qnAitem.Status |= QnAItemStatus.UserEditable;
      if (isPublisherCreated)
      {
        QnAMailNotificationEvent userData = this.PopulateNewResponseNotificationEventToUserData(requestContext, publishedExtension, (QnAItem) qnAitem, publishedExtension.Publisher.DisplayName);
        this._mailNotification.SendMailNotificationToUser(requestContext, extensionQnAitem.UserId, (MailNotificationEventData) userData);
      }
      else
      {
        QnAMailNotificationEvent publisherEventData = this.PopulateNewQnAItemNotificationToPublisherEventData(requestContext, publishedExtension, (QnAItem) qnAitem, qnAitem.User.DisplayName);
        this._mailNotification.SendMailNotificationToPublisher(requestContext, publishedExtension, (MailNotificationEventData) publisherEventData);
      }
      return qnAitem;
    }

    public QuestionsResult GetQuestionsList(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int count = 10,
      int? page = null,
      DateTime? afterDate = null)
    {
      Stopwatch.StartNew();
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0, 2147483646);
      if (page.HasValue)
      {
        ArgumentUtility.CheckForOutOfRange(page.Value, nameof (page), 0, 2147483646);
        ArgumentUtility.CheckForOutOfRange((long) count * (long) page.Value, "offset", 0L, 2147483646L);
      }
      PublishedExtension publishedExtension = this._qnAUtils.ValidateAndGetPublishedExtension(requestContext, publisherName, extensionName);
      ExtensionQnAItemQuestions qnAitemQuestions = (ExtensionQnAItemQuestions) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
        qnAitemQuestions = component is QnAComponent3 qnAcomponent3 ? qnAcomponent3.GetQnAItemList(publishedExtension.ExtensionId, count, page, afterDate) : (component as QnAComponent1).GetQnAItemList(publishedExtension.ExtensionId, count, page);
      bool flag = false;
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this._qnAUtils.GetAuthenticatedIdentity(requestContext);
      if (authenticatedIdentity != null)
        flag = this._qnAUtils.IsUserPublisherWithRequiredPermissions(requestContext, publishedExtension);
      QuestionsResult questionsList = (QuestionsResult) null;
      if (qnAitemQuestions != null)
      {
        questionsList = new QuestionsResult()
        {
          HasMoreQuestions = qnAitemQuestions.HasMoreQuestions,
          Questions = new List<Question>()
        };
        foreach (ExtensionQnAItem question1 in qnAitemQuestions.QuestionList)
        {
          Question question = this._qnAUtils.ConvertToQnAItem(requestContext, question1) as Question;
          if (authenticatedIdentity != null && question.User.Id.Equals(authenticatedIdentity.Id))
            question.Status |= QnAItemStatus.UserEditable;
          foreach (ExtensionQnAItem extensionQnAItem in qnAitemQuestions.ResponseList.FindAll((Predicate<ExtensionQnAItem>) (x => x.ParentId == question.Id)))
          {
            Response qnAitem = this._qnAUtils.ConvertToQnAItem(requestContext, extensionQnAItem) as Response;
            if (authenticatedIdentity != null)
            {
              if (qnAitem.Status.HasFlag((Enum) QnAItemStatus.PublisherCreated))
              {
                if (flag)
                  qnAitem.Status |= QnAItemStatus.UserEditable;
              }
              else if (question.User.Id.Equals(authenticatedIdentity.Id))
                qnAitem.Status |= QnAItemStatus.UserEditable;
            }
            question.Responses.Add(qnAitem);
          }
          questionsList.Questions.Add(question);
        }
      }
      return questionsList;
    }

    public Question UpdateQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      Question question)
    {
      Stopwatch.StartNew();
      ArgumentUtility.CheckForNull<Question>(question, nameof (question));
      question = this.PreChecksForModifyQnAItem(requestContext, (QnAItem) question) as Question;
      ArgumentUtility.CheckForOutOfRange(questionId, nameof (questionId), 1L, 9223372036854775806L);
      PublishedExtension publishedExtension = this._qnAUtils.ValidateAndGetPublishedExtension(requestContext, publisherName, extensionName);
      ExtensionQnAItem extensionQnAitem = (ExtensionQnAItem) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent1 qnAcomponent1)
          extensionQnAitem = qnAcomponent1.GetQnAItem(publishedExtension.ExtensionId, -1L, questionId);
      }
      if (extensionQnAitem == null)
        throw new QnAItemDoesNotExistException(GalleryResources.QnAQuestionDoesNotExistException((object) questionId));
      if (this._qnAUtils.GetAuthenticatedIdentity(requestContext).Id != extensionQnAitem.UserId)
        throw new QnAOperationForbidden(GalleryResources.QnAUpdateQuestionAccessDenied());
      ExtensionQnAItem extensionQnAItem = (ExtensionQnAItem) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent1 qnAcomponent1)
          extensionQnAItem = qnAcomponent1.UpdateQnAItem(publishedExtension.ExtensionId, -1L, questionId, question.User.Id, question.Text, publishedExtension.Versions[0].Version, DateTime.UtcNow);
      }
      if (extensionQnAItem == null)
        throw new QnAUnhandledException(GalleryResources.QnAUnhandledExceptionMessage((object) "updating", (object) "Question".ToLower(CultureInfo.CurrentCulture)));
      Question qnAitem = this._qnAUtils.ConvertToQnAItem(requestContext, extensionQnAItem) as Question;
      qnAitem.Status |= QnAItemStatus.UserEditable;
      this._mailNotification.SendMailNotificationToPublisher(requestContext, publishedExtension, (MailNotificationEventData) this.PopulateUpdatedQnAItemNotificationToPublisherEventData(requestContext, publishedExtension, (QnAItem) qnAitem, qnAitem.User.DisplayName));
      return qnAitem;
    }

    public Response UpdateResponse(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      long responseId,
      Response response)
    {
      Stopwatch.StartNew();
      ArgumentUtility.CheckForNull<Response>(response, nameof (response));
      response = this.PreChecksForModifyQnAItem(requestContext, (QnAItem) response) as Response;
      ArgumentUtility.CheckForOutOfRange(questionId, nameof (questionId), 1L, 9223372036854775806L);
      ArgumentUtility.CheckForOutOfRange(responseId, nameof (responseId), 1L, 9223372036854775806L);
      PublishedExtension publishedExtension = this._qnAUtils.ValidateAndGetPublishedExtension(requestContext, publisherName, extensionName);
      ExtensionQnAItem extensionQnAitem1 = (ExtensionQnAItem) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent1 qnAcomponent1)
          extensionQnAitem1 = qnAcomponent1.GetQnAItem(publishedExtension.ExtensionId, questionId, responseId);
      }
      if (extensionQnAitem1 == null)
        throw new QnAItemDoesNotExistException(GalleryResources.QnAResponseDoesNotExistException((object) responseId));
      bool flag = this._qnAUtils.IsUserPublisherWithRequiredPermissions(requestContext, publishedExtension);
      if (this._qnAUtils.GetAuthenticatedIdentity(requestContext).Id != extensionQnAitem1.UserId && (!extensionQnAitem1.IsPublisherCreated || !flag))
        throw new QnAOperationForbidden(GalleryResources.QnAUpdateResponseAccessDenied());
      ExtensionQnAItem extensionQnAItem = (ExtensionQnAItem) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent1 qnAcomponent1)
          extensionQnAItem = qnAcomponent1.UpdateQnAItem(publishedExtension.ExtensionId, questionId, responseId, response.User.Id, response.Text, publishedExtension.Versions[0].Version, DateTime.UtcNow);
      }
      if (extensionQnAItem == null)
        throw new QnAUnhandledException(GalleryResources.QnAUnhandledExceptionMessage((object) "updating", (object) "Response".ToLower(CultureInfo.CurrentCulture)));
      Response qnAitem = this._qnAUtils.ConvertToQnAItem(requestContext, extensionQnAItem) as Response;
      qnAitem.Status |= QnAItemStatus.UserEditable;
      if (flag)
      {
        ExtensionQnAItem extensionQnAitem2 = (ExtensionQnAItem) null;
        using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
        {
          if (component is QnAComponent1 qnAcomponent1)
            extensionQnAitem2 = qnAcomponent1.GetQnAItem(publishedExtension.ExtensionId, -1L, questionId);
        }
        if (extensionQnAitem2 == null)
          throw new QnAItemDoesNotExistException(GalleryResources.QnAQuestionDoesNotExistException((object) questionId));
        MailNotificationEventData userEventData = (MailNotificationEventData) this.PopulateUpdatedResponseNotificationToUserEventData(requestContext, publishedExtension, (QnAItem) qnAitem, publishedExtension.Publisher.DisplayName);
        this._mailNotification.SendMailNotificationToUser(requestContext, extensionQnAitem2.UserId, userEventData);
      }
      else
      {
        MailNotificationEventData publisherEventData = (MailNotificationEventData) this.PopulateUpdatedQnAItemNotificationToPublisherEventData(requestContext, publishedExtension, (QnAItem) qnAitem, qnAitem.User.DisplayName);
        this._mailNotification.SendMailNotificationToPublisher(requestContext, publishedExtension, publisherEventData);
      }
      return qnAitem;
    }

    public Concern ReportQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      Concern concern)
    {
      Stopwatch.StartNew();
      ArgumentUtility.CheckForNull<Concern>(concern, nameof (concern));
      ArgumentUtility.CheckForNull<string>(concern.Text, "Text");
      ArgumentUtility.CheckForOutOfRange(questionId, nameof (questionId), 1L, 9223372036854775806L);
      PublishedExtension publishedExtension = this._qnAUtils.ValidateAndGetPublishedExtension(requestContext, publisherName, extensionName);
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this._qnAUtils.GetAuthenticatedIdentity(requestContext);
      if (authenticatedIdentity == null)
        throw new AnonymousQnASubmissionException(GalleryResources.CannotSubmitQnAAnonymously());
      concern.CreatedDate = DateTime.UtcNow;
      concern.UpdatedDate = concern.CreatedDate;
      if (concern.User == null)
        concern.User = new UserIdentityRef();
      concern.User.Id = authenticatedIdentity.Id;
      concern.User.DisplayName = authenticatedIdentity.DisplayName;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent2 qnAcomponent2)
          qnAcomponent2.ReportQnAItem(publishedExtension.ExtensionId, -1L, questionId, concern.User.Id, ConcernSource.QnA, concern.Category, concern.Text, concern.CreatedDate);
      }
      concern.Id = questionId;
      return concern;
    }

    public void DeleteQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      bool hardDelete)
    {
      this.DeleteQnAItem(requestContext, publisherName, extensionName, questionId, new long?(), hardDelete);
      requestContext.GetService<IGalleryAuditLogService>().LogAuditEntry(requestContext, nameof (DeleteQuestion), questionId.ToString((IFormatProvider) CultureInfo.InvariantCulture), "QnA");
    }

    public void DeleteResponse(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      long responseId,
      bool hardDelete)
    {
      this.DeleteQnAItem(requestContext, publisherName, extensionName, questionId, new long?(responseId), hardDelete);
      requestContext.GetService<IGalleryAuditLogService>().LogAuditEntry(requestContext, nameof (DeleteResponse), responseId.ToString((IFormatProvider) CultureInfo.InvariantCulture), "QnA");
    }

    public int DeleteAllQuestionsAndResponsesForExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      bool hardDelete)
    {
      if (this._qnAUtils.GetAuthenticatedIdentity(requestContext) == null)
        throw new AnonymousQnASubmissionException(GalleryResources.CannotDeleteQnAAnonymously());
      GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
      int num1 = 0;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent2 qnAcomponent2)
        {
          Guid extensionId1 = extensionId;
          long? parentId = new long?();
          long? itemId = new long?();
          int num2 = hardDelete ? 1 : 0;
          num1 = qnAcomponent2.DeleteQnAItems(extensionId1, parentId, itemId, num2 != 0);
        }
      }
      return num1;
    }

    public IEnumerable<ExtensionQnAItem> GetQnAItemsByUserId(
      IVssRequestContext requestContext,
      Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      IEnumerable<ExtensionQnAItem> qnAitemsByUserId = (IEnumerable<ExtensionQnAItem>) null;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent5 qnAcomponent5)
          qnAitemsByUserId = qnAcomponent5.GetQnAItemsByUserId(userId);
      }
      return qnAitemsByUserId;
    }

    public int AnonymizeQnAItems(IVssRequestContext requestContext, Guid userId)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      int num = 0;
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (component is QnAComponent5 qnAcomponent5)
          num = qnAcomponent5.AnonymizeQnAItems(userId);
      }
      return num;
    }

    public void PublishReCaptchaTokenCI(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData)
    {
      this._qnAUtils.PublishReCaptchaTokenCIForQnA(requestContext, ciData);
    }

    private void DeleteQnAItem(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      long? responseId,
      bool hardDelete)
    {
      Stopwatch.StartNew();
      ArgumentUtility.CheckForOutOfRange(questionId, nameof (questionId), 1L, 9223372036854775806L);
      if (responseId.HasValue)
        ArgumentUtility.CheckForOutOfRange(responseId.Value, nameof (responseId), 1L, 9223372036854775806L);
      PublishedExtension publishedExtension = this._qnAUtils.ValidateAndGetPublishedExtension(requestContext, publisherName, extensionName);
      if (this._qnAUtils.GetAuthenticatedIdentity(requestContext) == null)
        throw new AnonymousQnASubmissionException(GalleryResources.CannotDeleteQnAAnonymously());
      GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
      using (QnAComponent component = requestContext.CreateComponent<QnAComponent>())
      {
        if (!(component is QnAComponent2 qnAcomponent2))
          return;
        qnAcomponent2.DeleteQnAItems(publishedExtension.ExtensionId, new long?(questionId), responseId, hardDelete);
      }
    }

    private QnAItem PreChecksForModifyQnAItem(IVssRequestContext requestContext, QnAItem qnAItem)
    {
      ArgumentUtility.CheckForNull<QnAItem>(qnAItem, nameof (qnAItem));
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = this._qnAUtils.GetAuthenticatedIdentity(requestContext);
      if (authenticatedIdentity == null)
        throw new AnonymousQnASubmissionException(GalleryResources.CannotSubmitQnAAnonymously());
      ArgumentUtility.CheckStringForNullOrWhiteSpace(qnAItem.Text, "Text");
      ArgumentUtility.CheckStringLength(qnAItem.Text, "Text", 2048, 1);
      if (qnAItem.User == null)
        qnAItem.User = new UserIdentityRef();
      qnAItem.User.Id = authenticatedIdentity.Id;
      return this._qnAUtils.SanitizeItem(qnAItem);
    }

    private QnAMailNotificationEvent PopulateNewQnAItemNotificationToPublisherEventData(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      QnAItem item,
      string userDisplayName)
    {
      QnAMailNotificationEvent publisherEventData = this.PopulateNotificationEventCommonData(requestContext, extension, item, userDisplayName);
      string str = item.GetType() == typeof (Question) ? "Question".ToLower(CultureInfo.CurrentCulture) : "Response".ToLower(CultureInfo.CurrentCulture);
      string publisherSubject = GalleryResources.QnANewItemNotificationMailToPublisherSubject((object) str, (object) extension.DisplayName);
      string publisherHeaderNote = GalleryResources.QnANewItemNotificationMailToPublisherHeaderNote((object) str, (object) extension.DisplayName);
      string publisherIntroNote = GalleryResources.QnANewItemNotificationMailToPublisherIntroNote((object) str, (object) publisherEventData.GetAnchorText(nameof (extension), GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName)));
      publisherEventData.Subject = publisherSubject;
      publisherEventData.HeaderNote = publisherHeaderNote;
      publisherEventData.IntroductionNote = publisherIntroNote;
      return publisherEventData;
    }

    private QnAMailNotificationEvent PopulateNewResponseNotificationEventToUserData(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      QnAItem item,
      string userDisplayName)
    {
      QnAMailNotificationEvent userData = this.PopulateNotificationEventCommonData(requestContext, extension, item, userDisplayName);
      string userSubject = GalleryResources.QnANewResponseNotificationMailToUserSubject((object) extension.Publisher.DisplayName);
      string userIntroNote = GalleryResources.QnANewResponseNotificationMailToUserIntroNote((object) userData.GetAnchorText(extension.DisplayName, GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName)));
      userData.Subject = userSubject;
      userData.HeaderNote = userSubject;
      userData.IntroductionNote = userIntroNote;
      return userData;
    }

    private QnAMailNotificationEvent PopulateUpdatedQnAItemNotificationToPublisherEventData(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      QnAItem item,
      string userDisplayName)
    {
      QnAMailNotificationEvent publisherEventData = this.PopulateNotificationEventCommonData(requestContext, extension, item, userDisplayName);
      string displayName = extension.DisplayName;
      string str1 = item.GetType() == typeof (Question) ? "Question".ToLower(CultureInfo.CurrentCulture) : "Response".ToLower(CultureInfo.CurrentCulture);
      string publisherSubject = GalleryResources.QnAUpdatedItemNotificationMailToPublisherSubject((object) str1, (object) displayName);
      string str2 = GalleryResources.QnAUpdatedItemNotificationMailHeaderNote((object) str1, (object) displayName);
      string publisherIntroNote = GalleryResources.QnAUpdatedItemNotificationMailToPublisherIntroNote((object) str1, (object) publisherEventData.GetAnchorText(nameof (extension), GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName)));
      publisherEventData.Subject = publisherSubject;
      publisherEventData.HeaderNote = str2;
      publisherEventData.IntroductionNote = publisherIntroNote;
      return publisherEventData;
    }

    private QnAMailNotificationEvent PopulateUpdatedResponseNotificationToUserEventData(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      QnAItem item,
      string userDisplayName)
    {
      QnAMailNotificationEvent userEventData = this.PopulateNotificationEventCommonData(requestContext, extension, item, userDisplayName);
      string displayName = extension.DisplayName;
      string str1 = item.GetType() == typeof (Question) ? "Question".ToLower(CultureInfo.CurrentCulture) : "Response".ToLower(CultureInfo.CurrentCulture);
      string userSubject = GalleryResources.QnAUpdatedResponseNotificationMailToUserSubject((object) extension.Publisher.DisplayName);
      string str2 = displayName;
      string str3 = GalleryResources.QnAUpdatedItemNotificationMailHeaderNote((object) str1, (object) str2);
      string userIntroNote = GalleryResources.QnAUpdatedResponseNotificationMailToUserIntroNote((object) userEventData.GetAnchorText(displayName, GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName)));
      userEventData.Subject = userSubject;
      userEventData.HeaderNote = str3;
      userEventData.IntroductionNote = userIntroNote;
      return userEventData;
    }

    private QnAMailNotificationEvent PopulateNotificationEventCommonData(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      QnAItem item,
      string userDisplayName)
    {
      QnAMailNotificationEvent notificationEvent = new QnAMailNotificationEvent(requestContext);
      string str = "#qna";
      notificationEvent.CreatedDate = item.CreatedDate.ToString("dd-MMM-yyyy", (IFormatProvider) CultureInfo.InvariantCulture);
      notificationEvent.UserDisplayName = userDisplayName;
      notificationEvent.NotificationContent = item.Text;
      notificationEvent.ActionButtonText = GalleryResources.RespondOnMarketplaceButtonText();
      notificationEvent.ActionButtonUrl = GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName) + str;
      return notificationEvent;
    }
  }
}
