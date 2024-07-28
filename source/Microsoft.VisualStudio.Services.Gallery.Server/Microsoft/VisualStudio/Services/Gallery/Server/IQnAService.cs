// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IQnAService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (QnAService))]
  public interface IQnAService : IVssFrameworkService
  {
    Question CreateQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Question question);

    Response CreateResponse(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      Response response);

    QuestionsResult GetQuestionsList(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      int count = 10,
      int? page = null,
      DateTime? afterDate = null);

    Question UpdateQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      Question question);

    Response UpdateResponse(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      long responseId,
      Response response);

    Concern ReportQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      Concern concern);

    void DeleteQuestion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      bool hardDelete);

    void DeleteResponse(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      long questionId,
      long responseId,
      bool hardDelete);

    int DeleteAllQuestionsAndResponsesForExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      bool hardDelete);

    IEnumerable<ExtensionQnAItem> GetQnAItemsByUserId(
      IVssRequestContext requestContext,
      Guid userId);

    int AnonymizeQnAItems(IVssRequestContext requestContext, Guid userId);

    void PublishReCaptchaTokenCI(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData);
  }
}
