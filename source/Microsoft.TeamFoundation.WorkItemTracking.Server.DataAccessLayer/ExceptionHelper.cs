// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExceptionHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class ExceptionHelper
  {
    internal static Dictionary<int, string> SqlErrorResourceNames;

    internal static string GetErrorMessage(
      string sqlMessage,
      int errorNumber,
      params object[] messageParameters)
    {
      string resourceName;
      if (!ExceptionHelper.SqlErrorResourceNames.TryGetValue(errorNumber, out resourceName) || string.IsNullOrEmpty(resourceName))
        return ExceptionHelper.FormatSqlMessage(sqlMessage);
      string messageResource = ExceptionHelper.GetMessageResource(resourceName);
      if (messageParameters.Length == 0)
        messageParameters = ExceptionHelper.ExtractErrorParameterStrings(sqlMessage);
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, messageResource, messageParameters);
    }

    internal static string GetMessageResource(string resourceName)
    {
      string messageResource = DalResourceStrings.Get(resourceName);
      if (string.IsNullOrEmpty(messageResource))
        messageResource = InternalsResourceStrings.Get(resourceName);
      return messageResource;
    }

    internal static string FormatSqlMessage(string message)
    {
      message = message.Replace('\a', ',');
      message = message.Replace('\t', ';');
      if (message.StartsWith("%error=\"", StringComparison.Ordinal))
      {
        message = message.Replace("%error=\"", "Error ");
        message = message.Replace("\";%:", ": ");
      }
      message = message.Replace(";%", string.Empty);
      message = message.Replace("%", string.Empty);
      return message;
    }

    internal static object[] ExtractStrings(SqlError sqlError, params object[] stringNames)
    {
      List<object> objectList = new List<object>(stringNames.Length);
      if (sqlError != null && stringNames.Length != 0)
      {
        foreach (string stringName in stringNames)
          objectList.Add((object) TeamFoundationServiceException.ExtractString(sqlError, stringName));
      }
      return objectList.ToArray();
    }

    internal static object[] ExtractErrorParameterStrings(string sqlErrorMessage)
    {
      List<object> objectList1 = new List<object>();
      if (sqlErrorMessage != null)
      {
        int currentIndex = 0;
        KeyValuePair<string, object>? errorParameterString;
        while ((errorParameterString = ExceptionHelper.ExtractErrorParameterString(sqlErrorMessage, ref currentIndex)).HasValue)
        {
          KeyValuePair<string, object> keyValuePair = errorParameterString.Value;
          if (!keyValuePair.Key.Equals("error", StringComparison.OrdinalIgnoreCase))
          {
            List<object> objectList2 = objectList1;
            keyValuePair = errorParameterString.Value;
            object obj = keyValuePair.Value;
            objectList2.Add(obj);
          }
        }
      }
      return objectList1.ToArray();
    }

    private static KeyValuePair<string, object>? ExtractErrorParameterString(
      string message,
      ref int currentIndex)
    {
      int num1 = message.IndexOf("%", currentIndex, StringComparison.Ordinal);
      if (num1 == -1)
        return new KeyValuePair<string, object>?();
      int num2 = message.IndexOf("=", num1 + "%".Length, StringComparison.Ordinal);
      if (num2 == -1)
        return new KeyValuePair<string, object>?();
      int num3;
      for (int index = message.IndexOf(";%", num2 + "=".Length, StringComparison.Ordinal); index != -1; index = num3)
      {
        int num4;
        int num5;
        if ((num3 = message.IndexOf(";%", index + ";%".Length, StringComparison.Ordinal)) == -1 || (num4 = message.IndexOf("=", index + ";%".Length, StringComparison.Ordinal)) != -1 && num4 < num3 && (num5 = message.IndexOf("%", index + ";%".Length, StringComparison.Ordinal)) != -1 && num5 < num4)
        {
          currentIndex = index + ";%".Length;
          string str = message.Substring(num2 + "=".Length, index - (num2 + "=".Length));
          if (str.StartsWith("\"", StringComparison.Ordinal) && str.EndsWith("\"", StringComparison.Ordinal))
            str = str.Trim('"');
          return new KeyValuePair<string, object>?(new KeyValuePair<string, object>(message.Substring(num1 + "%".Length, num2 - (num1 + "%".Length)), (object) str));
        }
      }
      return new KeyValuePair<string, object>?();
    }

    internal static XmlNode TranslateWorkItemDestroyAuthorizeException(
      IVssRequestContext context,
      int errorNumber,
      SqlException se,
      SqlError sqlError)
    {
      if (context.GetClientVersion() < 3)
        throw new LegacyValidationException(se.Message, 600031);
      string[] unauthorizedWorkItemIds = TeamFoundationServiceException.ExtractString(sqlError, "DestroyFailures").Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries);
      return ExceptionHelper.TranslateWorkItemDestroyAuthorizeException<string>(errorNumber, (IEnumerable<string>) unauthorizedWorkItemIds);
    }

    internal static XmlNode TranslateWorkItemDestroyAuthorizeException<T>(
      int errorNumber,
      IEnumerable<T> unauthorizedWorkItemIds)
      where T : IConvertible
    {
      XmlDocument doc = new XmlDocument();
      XmlNode details = (XmlNode) null;
      ExceptionHelper.PrepareDetailsElement(errorNumber, out doc, out details);
      foreach (T unauthorizedWorkItemId in unauthorizedWorkItemIds)
      {
        XmlNode node1 = doc.CreateNode(XmlNodeType.Element, "DestroyWorkItem", (string) null);
        XmlNode node2 = node1;
        ref T local = ref unauthorizedWorkItemId;
        T obj = default (T);
        if ((object) obj == null)
        {
          obj = local;
          local = ref obj;
        }
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        string str = local.ToString((IFormatProvider) invariantCulture);
        TFCommonUtil.AddXmlAttribute(node2, "WorkItemID", str);
        details.AppendChild(node1);
      }
      return details;
    }

    internal static XmlNode GenerateXmlDetails(
      IVssRequestContext context,
      string message,
      int errorNumber,
      int linkTypeId,
      int sourceId,
      int targetId)
    {
      string elemName = string.Empty;
      switch (errorNumber)
      {
        case 600270:
        case 600271:
        case 600272:
        case 600273:
        case 600276:
        case 600278:
        case 600279:
          elemName = "InsertWorkItemLink";
          break;
        case 600274:
          elemName = "UpdateWorkItemLink";
          break;
        case 600275:
          elemName = "DeleteWorkItemLink";
          break;
      }
      if (context.GetClientVersion() < 3)
      {
        if (errorNumber == 600274 || errorNumber == 600275)
          throw new LegacyValidationException(DalResourceStrings.Get("CannotEditLinkFromOlderClient"), 600171);
        throw new LegacyValidationException(message, 600031);
      }
      XmlDocument doc;
      XmlNode details;
      ExceptionHelper.PrepareDetailsElement(errorNumber, out doc, out details);
      if (errorNumber != 600280)
      {
        XmlNode linkErrorNode = ExceptionHelper.CreateLinkErrorNode(doc, elemName, sourceId.ToString(), targetId.ToString(), linkTypeId.ToString());
        details.AppendChild(linkErrorNode);
      }
      return details;
    }

    internal static XmlNode TranslateLinkException(
      IVssRequestContext context,
      int errorNumber,
      SqlException se,
      SqlError sqlError)
    {
      string elemName1 = string.Empty;
      switch (errorNumber)
      {
        case 600270:
        case 600271:
        case 600272:
        case 600273:
        case 600278:
        case 600279:
          elemName1 = "InsertWorkItemLink";
          break;
        case 600274:
          elemName1 = "UpdateWorkItemLink";
          break;
        case 600275:
          elemName1 = "DeleteWorkItemLink";
          break;
      }
      if (se.Errors.Count != 1)
        return (XmlNode) null;
      if (context.GetClientVersion() < 3)
      {
        if (errorNumber == 600274 || errorNumber == 600275)
          throw new LegacyValidationException(DalResourceStrings.Get("CannotEditLinkFromOlderClient"), 600171);
        throw new LegacyValidationException(se.Message, 600031);
      }
      XmlDocument doc;
      XmlNode details;
      ExceptionHelper.PrepareDetailsElement(errorNumber, out doc, out details);
      if (errorNumber == 600276 || errorNumber == 600280)
      {
        string str1 = TeamFoundationServiceException.ExtractString(sqlError, "LinkFailures");
        char[] separator1 = new char[1]{ '\a' };
        foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
        {
          char[] separator2 = new char[1]{ '\t' };
          string[] strArray = str2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
          string sourceID = strArray[0];
          string targetID = strArray[1];
          string linkType = strArray[2];
          string s = strArray[3];
          string elemName2 = "InsertWorkItemLink";
          if (int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture) == 2)
            elemName2 = "UpdateWorkItemLink";
          else if (int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture) == 3)
            elemName2 = "DeleteWorkItemLink";
          XmlNode linkErrorNode = ExceptionHelper.CreateLinkErrorNode(doc, elemName2, sourceID, targetID, linkType);
          details.AppendChild(linkErrorNode);
        }
      }
      else
      {
        string sourceID = TeamFoundationServiceException.ExtractString(sqlError, "SourceID");
        string targetID = TeamFoundationServiceException.ExtractString(sqlError, "TargetID");
        string linkType = TeamFoundationServiceException.ExtractString(sqlError, "LinkType");
        XmlNode linkErrorNode = ExceptionHelper.CreateLinkErrorNode(doc, elemName1, sourceID, targetID, linkType);
        details.AppendChild(linkErrorNode);
      }
      return details;
    }

    internal static XmlNode CreateLinkErrorNode(
      XmlDocument doc,
      LinkUpdateType action,
      int sourceID,
      int targetID,
      int linkType)
    {
      string elemName = string.Empty;
      switch (action)
      {
        case LinkUpdateType.Add:
          elemName = "InsertWorkItemLink";
          break;
        case LinkUpdateType.Update:
          elemName = "UpdateWorkItemLink";
          break;
        case LinkUpdateType.Delete:
          elemName = "DeleteWorkItemLink";
          break;
      }
      return ExceptionHelper.CreateLinkErrorNode(doc, elemName, sourceID.ToString((IFormatProvider) CultureInfo.InvariantCulture), targetID.ToString((IFormatProvider) CultureInfo.InvariantCulture), linkType.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal static XmlNode CreateLinkErrorNode(
      XmlDocument doc,
      string elemName,
      string sourceID,
      string targetID,
      string linkType)
    {
      XmlNode node = doc.CreateNode(XmlNodeType.Element, elemName, (string) null);
      TFCommonUtil.AddXmlAttribute(node, "SourceID", sourceID);
      TFCommonUtil.AddXmlAttribute(node, "TargetID", targetID);
      TFCommonUtil.AddXmlAttribute(node, "LinkType", linkType);
      return node;
    }

    internal static XmlNode TranslateQueryItemException(
      IVssRequestContext context,
      int errorNumber,
      SqlException se,
      SqlError sqlError)
    {
      if (se.Errors.Count != 1)
        return (XmlNode) null;
      XmlDocument doc = new XmlDocument();
      XmlNode details = (XmlNode) null;
      ExceptionHelper.PrepareDetailsElement(errorNumber, out doc, out details);
      switch (errorNumber)
      {
        case 600290:
          string str1 = TeamFoundationServiceException.ExtractString(sqlError, "QueryItemNameConflicts");
          char[] separator1 = new char[1]{ '\a' };
          foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
          {
            char[] separator2 = new char[1]{ '\t' };
            string[] strArray = str2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
            string str3 = strArray[0];
            string str4 = strArray[1];
            string str5 = strArray[2];
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "QueryItemNameConflicts", (string) null);
            TFCommonUtil.AddXmlAttribute(node, "Name", str3);
            TFCommonUtil.AddXmlAttribute(node, "QueryID", str4);
            TFCommonUtil.AddXmlAttribute(node, "QueryParentID", str5);
            details.AppendChild(node);
          }
          break;
        case 600295:
          string str6 = TeamFoundationServiceException.ExtractString(sqlError, "QueryItemCircles");
          char[] separator3 = new char[1]{ '\a' };
          foreach (string str7 in str6.Split(separator3, StringSplitOptions.RemoveEmptyEntries))
          {
            XmlNode node = doc.CreateNode(XmlNodeType.Element, "QueryItemCircles", (string) null);
            TFCommonUtil.AddXmlAttribute(node, "QueryID", str7);
            details.AppendChild(node);
          }
          break;
        default:
          string name = TeamFoundationServiceException.ExtractString(sqlError, "Action");
          string str8 = TeamFoundationServiceException.ExtractString(sqlError, "ID");
          XmlNode node1 = doc.CreateNode(XmlNodeType.Element, name, (string) null);
          TFCommonUtil.AddXmlAttribute(node1, "QueryID", str8);
          if (errorNumber == 600292 || errorNumber == 600293)
          {
            string str9 = TeamFoundationServiceException.ExtractString(sqlError, "ParentID");
            TFCommonUtil.AddXmlAttribute(node1, "QueryParentID", str9);
          }
          details.AppendChild(node1);
          break;
      }
      return details;
    }

    internal static XmlNode TranslateFieldLimitExceededException(
      IVssRequestContext context,
      int errorNumber,
      SqlException se,
      SqlError sqlError)
    {
      string addFieldCount = TeamFoundationServiceException.ExtractString(sqlError, "FieldsAddedCount");
      string remainingCount = TeamFoundationServiceException.ExtractString(sqlError, "FieldsRemainingCount");
      return ExceptionHelper.TranslateFieldLimitExceededException(errorNumber, addFieldCount, remainingCount);
    }

    internal static XmlNode TranslateFieldLimitExceededException(
      int errorNumber,
      string addFieldCount,
      string remainingCount)
    {
      XmlDocument doc = new XmlDocument();
      XmlNode details = (XmlNode) null;
      ExceptionHelper.PrepareDetailsElement(errorNumber, out doc, out details);
      XmlNode node = doc.CreateNode(XmlNodeType.Element, "InsertField", (string) null);
      TFCommonUtil.AddXmlAttribute(node, "FieldsAddedCount", addFieldCount);
      TFCommonUtil.AddXmlAttribute(node, "FieldsRemainingCount", remainingCount);
      details.AppendChild(node);
      return details;
    }

    internal static void PrepareDetailsElement(
      int errorCode,
      out XmlDocument doc,
      out XmlNode details)
    {
      doc = new XmlDocument();
      details = doc.CreateNode(XmlNodeType.Element, nameof (details), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultdetail/03");
      TFCommonUtil.AddXmlAttribute(details, "id", errorCode.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
