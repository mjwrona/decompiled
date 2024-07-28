// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ErrorInReasonPhraseExceptionFilter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class ErrorInReasonPhraseExceptionFilter : LogExceptionFilterAttribute
  {
    public override sealed async Task OnExceptionAsync(
      HttpActionExecutedContext actionExecutedContext,
      CancellationToken cancellationToken)
    {
      ErrorInReasonPhraseExceptionFilter phraseExceptionFilter = this;
      phraseExceptionFilter.SetErrorResponseFromException(actionExecutedContext, cancellationToken);
      Exception exception = actionExecutedContext.Exception;
      IVssRequestContext tfsRequestContext = actionExecutedContext.ActionContext?.ControllerContext?.Controller is TfsApiController controller ? controller.TfsRequestContext : (IVssRequestContext) null;
      PackagingWarningMessage message;
      if (exception == null)
        message = (PackagingWarningMessage) null;
      else if (tfsRequestContext == null)
        message = (PackagingWarningMessage) null;
      else if (tfsRequestContext.IsFeatureEnabled("Packaging.DisableErrorsInReasonPhrase"))
        message = (PackagingWarningMessage) null;
      else if (!ErrorInReasonPhraseExceptionFilter.ShouldEmitWarning(exception))
      {
        message = (PackagingWarningMessage) null;
      }
      else
      {
        HttpStatusCode statusCode = actionExecutedContext.Response.StatusCode;
        Guid activityId = tfsRequestContext.ActivityId;
        bool premisesDeployment = tfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment;
        message = ErrorInReasonPhraseExceptionFilter.ComputeWarningMessage(exception, statusCode, activityId, premisesDeployment, actionExecutedContext.Response.ReasonPhrase);
        await phraseExceptionFilter.OnExceptionAsync(actionExecutedContext, message, cancellationToken);
        actionExecutedContext.Response.ReasonPhrase = message.StatusDescriptionMessage;
        message = (PackagingWarningMessage) null;
      }
    }

    protected virtual Task OnExceptionAsync(
      HttpActionExecutedContext actionExecutedContext,
      PackagingWarningMessage message,
      CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }

    public static bool ShouldEmitWarning(Exception ex) => !ex.Data.Contains((object) "OverridePackagingClientWarningEnabled") || (bool) ex.Data[(object) "OverridePackagingClientWarningEnabled"];

    public static PackagingWarningMessage ComputeWarningMessage(
      Exception exception,
      HttpStatusCode statusCode,
      Guid activityId,
      bool isOnPrem,
      string existingReasonPhrase)
    {
      string upperInvariant = activityId.ToString().ToUpperInvariant();
      string str1 = isOnPrem ? Resources.Warning_PackagingClientWarningTFSActivityIdSuffix((object) upperInvariant) : Resources.Warning_PackagingClientWarningVSTSActivityIdSuffix((object) upperInvariant);
      string str2 = Regex.Replace(UserFriendlyError.GetMessageFromException(exception, statusCode, activityId), "\\r\\n?|\\n", " ");
      string customWarningMessage = str2 + str1;
      string str3 = string.IsNullOrWhiteSpace(existingReasonPhrase) ? str2 : existingReasonPhrase + " - " + str2;
      int num1 = str3.Length + str1.Length - 512;
      if (num1 > 0)
        str3 = str3.Substring(0, str3.Length - num1);
      string statusDescriptionMessage = str3 + str1;
      int num2 = num1 > 0 ? 1 : 0;
      return new PackagingWarningMessage(customWarningMessage, statusDescriptionMessage, num2 != 0);
    }
  }
}
