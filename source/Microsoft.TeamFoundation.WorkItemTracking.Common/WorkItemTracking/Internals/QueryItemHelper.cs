// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.QueryItemHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class QueryItemHelper
  {
    private static readonly char[] InvalidNameChars = new char[3]
    {
      '«',
      '»',
      '⁄'
    };

    private static void AddTextNode(XmlElement node, string name, string text)
    {
      XmlElement element = node.OwnerDocument.CreateElement(name);
      XmlText textNode = node.OwnerDocument.CreateTextNode(text);
      element.AppendChild((XmlNode) textNode);
      node.AppendChild((XmlNode) element);
    }

    private static XmlElement CreateQueryItemActionNode(XmlElement node, string action, Guid id)
    {
      XmlElement element = node.OwnerDocument.CreateElement(action);
      element.SetAttribute("QueryID", id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
      node.AppendChild((XmlNode) element);
      return element;
    }

    public static void Create(
      XmlElement node,
      Guid id,
      Guid parentId,
      string name,
      string queryText,
      string ownerIdentifier,
      string ownerType)
    {
      QueryItemHelper.UpdateInternal(QueryItemHelper.CreateQueryItemActionNode(node, "InsertQueryItem", id), parentId, name, queryText, ownerIdentifier, ownerType);
    }

    public static void Update(
      XmlElement node,
      Guid id,
      Guid parentId,
      string name,
      string queryText,
      string ownerIdentifier,
      string ownerType)
    {
      QueryItemHelper.UpdateInternal(QueryItemHelper.CreateQueryItemActionNode(node, "UpdateQueryItem", id), parentId, name, queryText, ownerIdentifier, ownerType);
    }

    private static void UpdateInternal(
      XmlElement actionNode,
      Guid parentId,
      string name,
      string queryText,
      string ownerIdentifier,
      string ownerType)
    {
      if (parentId != Guid.Empty)
        actionNode.SetAttribute("QueryParentID", parentId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(name))
        QueryItemHelper.AddTextNode(actionNode, "Name", name);
      if (!string.IsNullOrEmpty(queryText))
        QueryItemHelper.AddTextNode(actionNode, "QueryText", queryText);
      if (ownerIdentifier == null)
        return;
      QueryItemHelper.AddTextNode(actionNode, "OwnerIdentifier", ownerIdentifier);
      QueryItemHelper.AddTextNode(actionNode, "OwnerType", ownerType);
    }

    public static void Delete(XmlElement node, Guid id) => QueryItemHelper.CreateQueryItemActionNode(node, "DeleteQueryItem", id);

    public static string CheckNameIsValid(string name)
    {
      TeamFoundationTrace.Verbose("--> CheckNameIsValid()");
      TeamFoundationTrace.Verbose("Name: {0}", (object) name);
      name = name != null ? name.Trim() : throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw new ArgumentException(InternalsResourceStrings.Get("QueryHierarchyNameCannotBeEmpty"));
      if (name.Length > (int) byte.MaxValue)
        throw new ArgumentException(InternalsResourceStrings.Format("QueryNameTooLong", (object) (int) byte.MaxValue));
      if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        throw new ArgumentException(InternalsResourceStrings.Get("ErrorInvalidQueryItemName"));
      name = name.IndexOfAny(QueryItemHelper.InvalidNameChars) < 0 ? name.Trim() : throw new ArgumentException(InternalsResourceStrings.Get("ErrorInvalidQueryItemName"));
      TeamFoundationTrace.Verbose("<-- CheckNameIsValid()");
      TeamFoundationTrace.Verbose("Name: {0}", (object) name);
      return name;
    }

    public static void ValidateWiql(WiqlAdapter wiqlAdapter, string queryText)
    {
      TeamFoundationTrace.Verbose("--> ValidateWiql()");
      TeamFoundationTrace.Verbose("WIQL: {0}", (object) queryText);
      NodeSelect syntax = Parser.ParseSyntax(queryText);
      if (wiqlAdapter != null)
        wiqlAdapter.Context = (IDictionary) new Hashtable()
        {
          {
            (object) "project",
            (object) string.Empty
          }
        };
      WiqlAdapter wiqlAdapter1 = wiqlAdapter;
      syntax.Bind((IExternal) wiqlAdapter1, (NodeTableName) null, (NodeFieldName) null);
      TeamFoundationTrace.Verbose("<-- ValidateWiql()");
    }

    public static XmlNodeList GetQueryTextNodes(XmlElement node) => node.SelectNodes("//QueryText");
  }
}
