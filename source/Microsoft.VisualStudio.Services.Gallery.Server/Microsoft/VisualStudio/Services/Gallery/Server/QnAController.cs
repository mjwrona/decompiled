// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.QnAController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "qna")]
  public class QnAController : TfsApiController
  {
    private bool isRequestFromChinaRegion;

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AnonymousQnASubmissionException>(HttpStatusCode.Unauthorized);
      exceptionMap.AddStatusCode<QnAItemDoesNotExistException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<QnAOperationForbidden>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<QnAItemAlreadyReportedException>(HttpStatusCode.Conflict);
    }

    [HttpGet]
    [ClientLocationId("C010D03D-812C-4ADE-AE07-C1862475EDA5")]
    public QuestionsResult GetQuestions(
      string publisherName,
      string extensionName,
      [FromUri] int count = 10,
      [FromUri] int? page = null,
      [FromUri] DateTime? afterDate = null)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      return this.TfsRequestContext.GetService<IQnAService>().GetQuestionsList(this.TfsRequestContext, publisherName, extensionName, count, page, afterDate);
    }

    [HttpPost]
    [ClientLocationId("6D1D9741-ECA8-4701-A3A5-235AFC82DFA4")]
    public Question CreateQuestion(string publisherName, string extensionName, [FromBody] Question question)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      ArgumentUtility.CheckForNull<Question>(question, nameof (question));
      IQnAService service = this.TfsRequestContext.GetService<IQnAService>();
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInQnA"))
      {
        int num = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, question.ReCaptchaToken) ? 1 : 0;
        this.isRequestFromChinaRegion = GalleryServerUtil.IsRequestFromChinaRegion(this.TfsRequestContext);
        Dictionary<string, object> ciData = new Dictionary<string, object>()
        {
          {
            CustomerIntelligenceProperty.Action,
            (object) "ReCaptchaValidation"
          },
          {
            "UserAgent",
            (object) this.TfsRequestContext.UserAgent
          },
          {
            "PublisherName",
            (object) publisherName
          },
          {
            "ExtensionName",
            (object) extensionName
          },
          {
            "QuestionId",
            (object) question.Id
          },
          {
            "Source",
            (object) "Q&A"
          },
          {
            "Scenario",
            (object) "CreateScenario"
          },
          {
            "ReCaptchaToken",
            (object) question.ReCaptchaToken
          },
          {
            "IsRequestFromChinaRegion",
            (object) this.isRequestFromChinaRegion
          }
        };
        if (num != 0)
        {
          ciData.Add("FeatureValidation", (object) "Valid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
        }
        else
        {
          ciData.Add("FeatureValidation", (object) "Invalid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
        }
      }
      return service.CreateQuestion(this.TfsRequestContext, publisherName, extensionName, question);
    }

    [HttpPatch]
    [ClientLocationId("6D1D9741-ECA8-4701-A3A5-235AFC82DFA4")]
    public Question UpdateQuestion(
      string publisherName,
      string extensionName,
      long questionId,
      [FromBody] Question question)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      ArgumentUtility.CheckForNull<Question>(question, nameof (question));
      IQnAService service = this.TfsRequestContext.GetService<IQnAService>();
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInQnA"))
      {
        int num = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, question.ReCaptchaToken) ? 1 : 0;
        this.isRequestFromChinaRegion = GalleryServerUtil.IsRequestFromChinaRegion(this.TfsRequestContext);
        Dictionary<string, object> ciData = new Dictionary<string, object>()
        {
          {
            CustomerIntelligenceProperty.Action,
            (object) "ReCaptchaValidation"
          },
          {
            "UserAgent",
            (object) this.TfsRequestContext.UserAgent
          },
          {
            "PublisherName",
            (object) publisherName
          },
          {
            "ExtensionName",
            (object) extensionName
          },
          {
            "QuestionId",
            (object) questionId
          },
          {
            "Source",
            (object) "Q&A"
          },
          {
            "Scenario",
            (object) "UpdateScenario"
          },
          {
            "ReCaptchaToken",
            (object) question.ReCaptchaToken
          },
          {
            "IsRequestFromChinaRegion",
            (object) this.isRequestFromChinaRegion
          }
        };
        if (num != 0)
        {
          ciData.Add("FeatureValidation", (object) "Valid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
        }
        else
        {
          ciData.Add("FeatureValidation", (object) "Invalid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
        }
      }
      return service.UpdateQuestion(this.TfsRequestContext, publisherName, extensionName, questionId, question);
    }

    [HttpDelete]
    [ClientLocationId("6D1D9741-ECA8-4701-A3A5-235AFC82DFA4")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteQuestion(
      string publisherName,
      string extensionName,
      long questionId)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      this.TfsRequestContext.GetService<IQnAService>().DeleteQuestion(this.TfsRequestContext, publisherName, extensionName, questionId, false);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPost]
    [ClientLocationId("7F8AE5E0-46B0-438F-B2E8-13E8513517BD")]
    public Response CreateResponse(
      string publisherName,
      string extensionName,
      long questionId,
      [FromBody] Response response)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      ArgumentUtility.CheckForNull<Response>(response, nameof (response));
      IQnAService service = this.TfsRequestContext.GetService<IQnAService>();
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInQnA"))
      {
        int num = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, response.ReCaptchaToken) ? 1 : 0;
        this.isRequestFromChinaRegion = GalleryServerUtil.IsRequestFromChinaRegion(this.TfsRequestContext);
        Dictionary<string, object> ciData = new Dictionary<string, object>()
        {
          {
            CustomerIntelligenceProperty.Action,
            (object) "ReCaptchaValidation"
          },
          {
            "UserAgent",
            (object) this.TfsRequestContext.UserAgent
          },
          {
            "PublisherName",
            (object) publisherName
          },
          {
            "ExtensionName",
            (object) extensionName
          },
          {
            "QuestionId",
            (object) questionId
          },
          {
            "Source",
            (object) "Q&AResponse"
          },
          {
            "Scenario",
            (object) "CreateScenario"
          },
          {
            "ReCaptchaToken",
            (object) response.ReCaptchaToken
          },
          {
            "IsRequestFromChinaRegion",
            (object) this.isRequestFromChinaRegion
          }
        };
        if (num != 0)
        {
          ciData.Add("FeatureValidation", (object) "Valid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
        }
        else
        {
          ciData.Add("FeatureValidation", (object) "Invalid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
        }
      }
      return service.CreateResponse(this.TfsRequestContext, publisherName, extensionName, questionId, response);
    }

    [HttpPatch]
    [ClientLocationId("7F8AE5E0-46B0-438F-B2E8-13E8513517BD")]
    public Response UpdateResponse(
      string publisherName,
      string extensionName,
      long questionId,
      long responseId,
      [FromBody] Response response)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      ArgumentUtility.CheckForNull<Response>(response, nameof (response));
      IQnAService service = this.TfsRequestContext.GetService<IQnAService>();
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaInQnA"))
      {
        int num = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, response.ReCaptchaToken) ? 1 : 0;
        this.isRequestFromChinaRegion = GalleryServerUtil.IsRequestFromChinaRegion(this.TfsRequestContext);
        Dictionary<string, object> ciData = new Dictionary<string, object>()
        {
          {
            CustomerIntelligenceProperty.Action,
            (object) "ReCaptchaValidation"
          },
          {
            "UserAgent",
            (object) this.TfsRequestContext.UserAgent
          },
          {
            "PublisherName",
            (object) publisherName
          },
          {
            "ExtensionName",
            (object) extensionName
          },
          {
            "QuestionId",
            (object) questionId
          },
          {
            "ResponseId",
            (object) responseId
          },
          {
            "Source",
            (object) "Q&A"
          },
          {
            "Scenario",
            (object) "UpdateScenario"
          },
          {
            "ReCaptchaToken",
            (object) response.ReCaptchaToken
          },
          {
            "IsRequestFromChinaRegion",
            (object) this.isRequestFromChinaRegion
          }
        };
        if (num != 0)
        {
          ciData.Add("FeatureValidation", (object) "Valid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
        }
        else
        {
          ciData.Add("FeatureValidation", (object) "Invalid");
          service.PublishReCaptchaTokenCI(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
        }
      }
      return service.UpdateResponse(this.TfsRequestContext, publisherName, extensionName, questionId, responseId, response);
    }

    [HttpDelete]
    [ClientLocationId("7F8AE5E0-46B0-438F-B2E8-13E8513517BD")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteResponse(
      string publisherName,
      string extensionName,
      long questionId,
      long responseId)
    {
      ArgumentUtility.CheckForNull<string>(publisherName, nameof (publisherName));
      ArgumentUtility.CheckForNull<string>(extensionName, nameof (extensionName));
      this.TfsRequestContext.GetService<IQnAService>().DeleteResponse(this.TfsRequestContext, publisherName, extensionName, questionId, responseId, false);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPost]
    [ClientLocationId("784910CD-254A-494D-898B-0728549B2F10")]
    public Concern ReportQuestion(
      string pubName,
      string extName,
      long questionId,
      [FromBody] Concern concern)
    {
      ArgumentUtility.CheckForNull<string>(pubName, nameof (pubName));
      ArgumentUtility.CheckForNull<string>(extName, nameof (extName));
      ArgumentUtility.CheckForNull<Concern>(concern, nameof (concern));
      return this.TfsRequestContext.GetService<IQnAService>().ReportQuestion(this.TfsRequestContext, pubName, extName, questionId, concern);
    }
  }
}
