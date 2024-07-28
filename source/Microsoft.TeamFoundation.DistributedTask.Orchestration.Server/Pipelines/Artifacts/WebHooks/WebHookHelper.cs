// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks.WebHookHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks
{
  public static class WebHookHelper
  {
    public static Guid GetWebHookIdFromPayloadUrl(
      IVssRequestContext requestContext,
      string payloadUrl)
    {
      try
      {
        Uri result1;
        if (!Uri.TryCreate(payloadUrl, UriKind.Absolute, out result1))
        {
          Guid result2;
          if (Guid.TryParse(payloadUrl, out result2))
            return result2;
        }
        else
        {
          string[] strArray = result1.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped).Split('/');
          return Guid.Parse(strArray[strArray.Length - 1]);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(10016124, "WebHook", "Cannot find webHookId from payloadUrl {0}. Exception: {1}", (object) payloadUrl, (object) ex);
      }
      return Guid.Empty;
    }

    public static void InitializeWebHookSecret(
      IVssRequestContext requestContext,
      string drawerName,
      string key,
      string secret)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(drawerName, nameof (drawerName));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      ITeamFoundationStrongBoxService strongBoxService = requestContext != null ? requestContext.GetService<ITeamFoundationStrongBoxService>() : throw new ArgumentNullException(nameof (requestContext));
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid drawerId = strongBoxService.UnlockDrawer(requestContext1, drawerName, false);
      if (drawerId == Guid.Empty)
        drawerId = strongBoxService.CreateDrawer(requestContext1, drawerName);
      strongBoxService.AddString(requestContext1, drawerId, key, secret);
    }

    public static void RemoveWebHookSecret(
      IVssRequestContext requestContext,
      string drawerName,
      string key)
    {
      ITeamFoundationStrongBoxService strongBoxService = requestContext != null ? requestContext.GetService<ITeamFoundationStrongBoxService>() : throw new ArgumentNullException(nameof (requestContext));
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid drawerId = strongBoxService.UnlockDrawer(requestContext1, drawerName, false);
      if (drawerId == Guid.Empty)
        return;
      strongBoxService.DeleteItem(requestContext1, drawerId, key);
    }

    public static bool ValidateWebHookPayload(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      string eventPayload,
      WebHook webHook,
      IArtifactType artifactType)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (webHook == null)
        throw new ArgumentNullException(nameof (webHook));
      if (artifactType == null)
        throw new ArgumentNullException(nameof (artifactType));
      ServiceEndpoint endpoint = (ServiceEndpoint) null;
      if (artifactType.IsIncomingWebHookArtifactType())
        endpoint = DistributedTaskEndpointServiceHelper.GetServiceEndpoint(requestContext.Elevate(), webHook.ConnectionId);
      if (artifactType.IsIncomingWebHookArtifactType() && endpoint == null)
        throw new WebHookException(string.Format("Service connection {0} for {1} does not exist.", (object) webHook.ConnectionId, (object) webHook.UniqueArtifactIdentifier));
      string payloadHashHeaderNamer = WebHookHelper.GetPayloadHashHeaderNamer(requestContext, artifactType, endpoint);
      if (string.IsNullOrEmpty(payloadHashHeaderNamer))
        return true;
      string payloadVerification = WebHookHelper.GetSecretForPayloadVerification(requestContext, artifactType, webHook, endpoint);
      string headerValue = string.Empty;
      if (requestMessage == null || !WebHookHelper.TryGetHeaderValue(requestMessage, payloadHashHeaderNamer, out headerValue))
        throw new WebHookException("Properties.Resources.RequiredHeaderSignatureNotFound");
      if (headerValue.StartsWith("sha1=", StringComparison.OrdinalIgnoreCase))
        headerValue = headerValue.Substring("sha1=".Length);
      if (string.IsNullOrEmpty(eventPayload))
        throw new WebHookException("Properties.Resources.EmptyWebHookEventPayload");
      return WebHookHelper.DoesPayloadHashMatches(eventPayload, payloadVerification, headerValue);
    }

    private static string GetPayloadHashHeaderNamer(
      IVssRequestContext requestContext,
      IArtifactType artifactType,
      ServiceEndpoint endpoint)
    {
      string payloadHashHeaderNamer = (string) null;
      if (!artifactType.IsIncomingWebHookArtifactType())
        payloadHashHeaderNamer = artifactType.ArtifactTriggerConfiguration?.PayloadHashHeaderName;
      else if (endpoint != null && endpoint.Authorization?.Parameters != null)
        endpoint.Authorization.Parameters.TryGetValue(WebhookPropertyNames.Header, out payloadHashHeaderNamer);
      return payloadHashHeaderNamer;
    }

    private static string GetSecretForPayloadVerification(
      IVssRequestContext requestContext,
      IArtifactType artifactType,
      WebHook webHook,
      ServiceEndpoint endpoint)
    {
      string payloadVerification = (string) null;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      if (artifactType.IsIncomingWebHookArtifactType())
      {
        if (endpoint != null && endpoint.Authorization?.Parameters != null)
          endpoint.Authorization.Parameters.TryGetValue(WebhookPropertyNames.Secret, out payloadVerification);
      }
      else
      {
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(vssRequestContext, "WebHookDrawer", false);
        if (drawerId == Guid.Empty)
        {
          requestContext.Trace(10016129, TraceLevel.Error, "WebHook", "Cannot find strongbox drawer {0}", "WebHookDrawer");
          throw new WebHookException("Properties.Resources.CannotConfirmPayloadHasSecret");
        }
        try
        {
          payloadVerification = service.GetString(vssRequestContext, drawerId, webHook.WebHookId.ToString());
        }
        catch (StrongBoxItemNotFoundException ex)
        {
          throw new WebHookException(TaskResources.CannotCalculateWebHookSignature());
        }
      }
      return payloadVerification;
    }

    private static bool DoesPayloadHashMatches(
      string eventPayload,
      string secret,
      string expectedHash)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(eventPayload);
      byte[] hash;
      using (HMACSHA1 hmacshA1 = new HMACSHA1(Encoding.UTF8.GetBytes(secret)))
        hash = hmacshA1.ComputeHash(bytes);
      byte[] byteArray = HexConverter.ToByteArray(expectedHash);
      return SecureCompare.TimeInvariantEquals(hash, byteArray);
    }

    private static bool TryGetHeaderValue(
      HttpRequestMessage requestMessage,
      string headerName,
      out string headerValue)
    {
      headerValue = string.Empty;
      bool headerValue1 = false;
      IEnumerable<string> values;
      if (requestMessage != null && requestMessage.Headers != null && requestMessage.Headers.TryGetValues(headerName, out values))
      {
        headerValue = values.FirstOrDefault<string>();
        headerValue1 = true;
      }
      return headerValue1;
    }

    public static string ToDebugString(this IDictionary<string, string> parameters)
    {
      string debugString = string.Empty;
      if (parameters != null && parameters.Any<KeyValuePair<string, string>>())
        debugString = string.Join<KeyValuePair<string, string>>(";", (IEnumerable<KeyValuePair<string, string>>) parameters);
      return debugString;
    }
  }
}
