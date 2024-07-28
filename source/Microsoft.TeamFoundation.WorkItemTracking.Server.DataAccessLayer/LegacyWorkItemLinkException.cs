// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyWorkItemLinkException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacyWorkItemLinkException : LegacyValidationException
  {
    public LegacyWorkItemLinkException(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
      : base(LegacyWorkItemLinkException.FormatMessage(requestContext, errorNumber, ex, sqlError), errorNumber, (Exception) null, ExceptionHelper.TranslateLinkException(requestContext, errorNumber, ex, sqlError))
    {
    }

    public LegacyWorkItemLinkException(
      IVssRequestContext requestContext,
      int errorNumber,
      int linkTypeId,
      int sourceId,
      int targetId)
      : base(LegacyWorkItemLinkException.FormatMessage(requestContext, errorNumber, linkTypeId, sourceId, targetId), errorNumber, (Exception) null, (XmlNode) null)
    {
      this.Details = ExceptionHelper.GenerateXmlDetails(requestContext, this.Message, errorNumber, linkTypeId, sourceId, targetId);
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      int errorNumber,
      int linkTypeId,
      int sourceId,
      int targetId)
    {
      MDWorkItemLinkType linkType;
      requestContext.GetService<WorkItemTrackingLinkService>().TryGetLinkTypeById(requestContext, linkTypeId, out linkType);
      object obj = (object) linkType;
      string str = linkTypeId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (linkType != null)
      {
        obj = linkType.ForwardId == linkTypeId ? (object) linkType.ForwardEndName : (object) linkType.ReverseEndName;
        str = linkType.ReverseId == linkTypeId ? linkType.ForwardEndName : linkType.ReverseEndName;
      }
      return ExceptionHelper.GetErrorMessage((string) null, errorNumber, obj, (object) sourceId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) targetId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) str);
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException ex,
      SqlError sqlError)
    {
      if (errorNumber != 600276 && errorNumber != 600280)
      {
        int num1 = TeamFoundationServiceException.ExtractInt(sqlError, "SourceID");
        int num2 = TeamFoundationServiceException.ExtractInt(sqlError, "TargetID");
        int id = TeamFoundationServiceException.ExtractInt(sqlError, "LinkType");
        if (id > -1)
        {
          MDWorkItemLinkType linkType;
          requestContext.GetService<WorkItemTrackingLinkService>().TryGetLinkTypeById(requestContext, id, out linkType);
          object obj = (object) id;
          string str = id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          if (linkType != null)
          {
            obj = linkType.ForwardId == id ? (object) linkType.ForwardEndName : (object) linkType.ReverseEndName;
            str = linkType.ReverseId == id ? linkType.ForwardEndName : linkType.ReverseEndName;
          }
          return ExceptionHelper.GetErrorMessage(ex.Message, errorNumber, obj, (object) num1.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) num2.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) str);
        }
      }
      return ExceptionHelper.GetErrorMessage(ex.Message, errorNumber);
    }
  }
}
