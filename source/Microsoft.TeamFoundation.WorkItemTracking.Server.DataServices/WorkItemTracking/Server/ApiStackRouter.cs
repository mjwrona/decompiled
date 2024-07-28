// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ApiStackRouter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ApiStackRouter
  {
    private IVssRequestContext m_requestContext;
    private static readonly string[] s_teeUserAgentSubStrings = new string[4]
    {
      "Team Explorer Everywhere",
      "Team  Explorer  Everywhere",
      "TeamExplorer",
      "TFSSDKForJava"
    };

    public ApiStackRouter(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
    }

    public void Run(Action methodBlock) => this.LegacyExceptionDetour((Action) (() => methodBlock()));

    private void LegacyExceptionGuard(Action a) => a();

    public void LegacyExceptionDetour(Action a)
    {
      Exception e = (Exception) null;
      try
      {
        a();
      }
      catch (WorkItemTrackingTreeNodeNotFoundException ex)
      {
        e = !ex.GuidId.HasValue ? (!ex.IntId.HasValue ? (Exception) new LegacyValidationException(ServerResources.InvalidNodeNameException((object) ex.Name)) : (Exception) new LegacyValidationException(ServerResources.InvalidNodeIdException((object) ex.IntId.Value))) : (Exception) new LegacyValidationException(ServerResources.InvalidNodeGuidException((object) ex.GuidId.Value));
      }
      catch (WorkItemTrackingFieldDefinitionNotFoundException ex)
      {
        e = !ex.Id.HasValue ? (Exception) new LegacyValidationException(ServerResources.InvalidFieldName((object) ex.Name)) : (Exception) new LegacyValidationException(ServerResources.InvalidFieldId((object) ex.Id.Value));
      }
      catch (WorkItemLinkInvalidException ex)
      {
        e = (Exception) this.ConvertMetadataLinkException(ex);
      }
      catch (WorkItemTrackingUnauthorizedOperationException ex)
      {
        e = (Exception) new LegacyDeniedOrNotExist("ErrorDeniedOrNotExist");
      }
      catch (WorkItemRevisionMismatchException ex)
      {
        e = (Exception) new LegacyValidationException(InternalsResourceStrings.Get("ErrorWorkItemUpdateWorkItemMissingOrUpdated"), 600122);
      }
      catch (WorkItemTrackingAggregateException ex)
      {
        this.LegacyExceptionDetour((Action) (() =>
        {
          throw ex.LeadingException;
        }));
      }
      catch (WorkItemLinkUnauthorizedAccessException ex)
      {
        e = (Exception) new LegacySecurityException(InternalsResourceStrings.Get("ErrorNotANamespaceAdmin"), 600081);
      }
      catch (WorkItemLinkEndUnauthorizedAccessException ex)
      {
        e = (Exception) new LegacyWorkItemLinkException(this.m_requestContext, 600276, ex.LinkType, ex.SourceId, ex.TargetId);
      }
      catch (QueryItemNotFoundException ex)
      {
        e = (Exception) new LegacyDeniedOrNotExist();
      }
      catch (WorkItemTrackingQueryInvalidEverClausePredicateException ex)
      {
        e = (Exception) new ArgumentException(DalResourceStrings.Get("QueryInvalidLongTextOperator"), "queryXml");
      }
      catch (WorkItemTrackingQueryDepthTooLargeException ex)
      {
        e = (Exception) new LegacyValidationException(ex.Message, 602014);
      }
      catch (WorkItemTrackingQueryException ex)
      {
        e = (Exception) new LegacyValidationException(ex.Message, 600171);
      }
      catch (SyntaxException ex)
      {
        e = (Exception) new LegacyValidationException(ex.Message, 600171);
      }
      catch (WorkItemDestroyException ex)
      {
        e = this.m_requestContext.GetClientVersion() >= 3 ? (Exception) new LegacyWorkItemDestroyException(ex.UnauthorizedWorkItemIds) : (Exception) new LegacyDeniedOrNotExist();
      }
      catch (WorkItemUnauthorizedAccessException ex)
      {
        e = ex.AccessType != AccessType.Write ? (Exception) new LegacyValidationException(ex.Message, ex.ErrorCode == 0 ? 602001 : ex.ErrorCode) : (Exception) new LegacyValidationException(InternalsResourceStrings.Get("ErrorNoAreaWriteAccess"), 600171);
      }
      catch (WorkItemTargetTypeIsDisabledException ex)
      {
        e = (Exception) new LegacyValidationException(ex.Message, 600171);
      }
      catch (WorkItemTrackingServiceException ex)
      {
        e = (Exception) this.ConvertFieldInvalidException(ex);
      }
      catch (InvalidTagNameException ex)
      {
        e = (Exception) new LegacyValidationException(ex.Message);
      }
      catch (AccessCheckException ex)
      {
        e = (Exception) new LegacySecurityException(ex.Message, 600171, (Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        e = (Exception) new LegacyDeniedOrNotExist("ErrorDeniedOrNotExist");
      }
      catch (Exception ex)
      {
        e = ex;
      }
      if (e == null)
        return;
      ExceptionManager.ThrowProperSoapException(this.m_requestContext, e);
    }

    private LegacyValidationException ConvertFieldInvalidException(
      WorkItemTrackingServiceException e)
    {
      if (e.ErrorCode == 600171)
      {
        switch (e)
        {
          case WorkItemFieldInvalidException _:
          case RuleValidationException _:
            string str = string.Empty;
            switch (e)
            {
              case WorkItemFieldInvalidException _:
                str = ((WorkItemFieldInvalidException) e).ReferenceName;
                break;
              case RuleValidationException _:
                str = ((RuleValidationException) e).FieldReferenceName;
                break;
            }
            XmlDocument doc;
            XmlNode details;
            ExceptionHelper.PrepareDetailsElement(e.ErrorCode, out doc, out details);
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "FieldData", (string) null);
            TFCommonUtil.AddXmlAttribute(node, "FieldName", str);
            details.AppendChild(node);
            return new LegacyValidationException(e.Message, e.ErrorCode, e.InnerException, details);
        }
      }
      return new LegacyValidationException(e.Message, e.ErrorCode == 0 ? 602001 : e.ErrorCode);
    }

    private LegacyValidationException ConvertMetadataLinkException(WorkItemLinkInvalidException e)
    {
      string linkName = e.LinkName;
      string message = e.Message;
      if (string.IsNullOrEmpty(linkName))
        linkName = e.LinkTypeId.ToString();
      if (e.ErrorCode != 600269 && e.ErrorCode != 600270 && e.ErrorCode != 600271)
        return new LegacyValidationException(message, e.ErrorCode);
      if (e.ErrorCode == 600269)
        message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkCannotUseDisabledType"), (object) linkName);
      XmlDocument doc;
      XmlNode details;
      ExceptionHelper.PrepareDetailsElement(e.ErrorCode, out doc, out details);
      XmlNode linkErrorNode = ExceptionHelper.CreateLinkErrorNode(doc, LinkUpdateType.Add, e.SourceId, e.TargetId, e.LinkTypeId);
      details.AppendChild(linkErrorNode);
      return new LegacyValidationException(message, e.ErrorCode, (Exception) null, details);
    }

    public static void ValidateSimplePostRequestOrigin(
      TeamFoundationHttpHandler httpHandler,
      IVssRequestContext requestContext,
      HttpRequestBase request)
    {
      if (!StringComparer.Ordinal.Equals(request.HttpMethod, "POST"))
        return;
      string x = request.ContentType;
      if (x != null)
      {
        int length = x.IndexOf(';');
        if (length >= 0)
          x = x.Substring(0, length);
      }
      if ((string.IsNullOrEmpty(x) || StringComparer.OrdinalIgnoreCase.Equals(x, "application/x-www-form-urlencoded") || StringComparer.OrdinalIgnoreCase.Equals(x, "multipart/form-data") || StringComparer.OrdinalIgnoreCase.Equals(x, "text/plain")) && string.IsNullOrWhiteSpace(request.Headers["X-TFS-Version"]) && !((IEnumerable<string>) ApiStackRouter.s_teeUserAgentSubStrings).Any<string>((Func<string, bool>) (s => request.UserAgent.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)) && !ApiStackRouter.IsUserAgentWhitelisted(requestContext, request.UserAgent))
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
    }

    private static bool IsUserAgentWhitelisted(IVssRequestContext requestContext, string userAgent)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string str = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/WebServices/UserAgentWhitelist", "");
      if (string.IsNullOrEmpty(str))
        return false;
      return ((IEnumerable<string>) str.Split(';')).Any<string>((Func<string, bool>) (v => userAgent.IndexOf(v, StringComparison.OrdinalIgnoreCase) >= 0));
    }
  }
}
