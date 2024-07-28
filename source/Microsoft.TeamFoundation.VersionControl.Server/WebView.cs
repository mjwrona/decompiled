// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WebView
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Globalization;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WebView
  {
    internal static bool IsWebViewQuery(
      VersionControlRequestContext versionControlRequestContext)
    {
      string str = HttpContext.Current.Request.Params["webView"];
      bool result = false;
      if (!string.IsNullOrEmpty(str) && !bool.TryParse(str, out result))
        result = false;
      return result;
    }

    internal static void ConditionAndSerializeShelvedItem(
      VersionControlRequestContext versionControlRequestContext,
      bool isWebView,
      XmlWriter writer,
      string shelvesetName,
      string shelvesetOwner,
      PendingChange shelvedItem)
    {
      shelvedItem.CreationDate = TimeZone.CurrentTimeZone.ToLocalTime(shelvedItem.CreationDate);
      if (isWebView)
        WebView.GenerateShelvedItemWebView(versionControlRequestContext, writer, shelvesetName, shelvesetOwner, shelvedItem);
      else
        new XmlSerializer(typeof (PendingChange)).Serialize(writer, (object) shelvedItem);
    }

    private static void GenerateShelvedItemWebView(
      VersionControlRequestContext versionControlRequestContext,
      XmlWriter writer,
      string shelvesetName,
      string shelvesetOwner,
      PendingChange shelvedItem)
    {
      writer.WriteStartElement("PendingChange");
      writer.WriteStartAttribute("title");
      writer.WriteString(VersionControlPath.GetFileName(shelvedItem.ServerItem));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("date");
      writer.WriteString(shelvedItem.CreationDate.ToString());
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("item");
      writer.WriteString(shelvedItem.ServerItem);
      writer.WriteEndAttribute();
      TeamFoundationLinkingService service = versionControlRequestContext.RequestContext.GetService<TeamFoundationLinkingService>();
      VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new ShelvesetUri(shelvesetName, shelvesetOwner, UriType.Extended);
      writer.WriteStartAttribute("ssurl");
      writer.WriteString(service.GetArtifactUrlExternal(versionControlRequestContext.RequestContext, controlIntegrationUri.ArtifactId));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("ss");
      writer.WriteString(WorkspaceSpec.Combine(shelvesetName, shelvesetOwner));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("chg");
      writer.WriteString(Changeset.ChangeTypeToString(shelvedItem.ChangeType));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("ver");
      writer.WriteString(shelvedItem.Version.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      writer.WriteEndAttribute();
      if (shelvedItem.SourceServerItem != null)
      {
        writer.WriteStartAttribute("srcitem");
        writer.WriteString(shelvedItem.SourceServerItem);
        writer.WriteEndAttribute();
        writer.WriteStartAttribute("svrfm");
        writer.WriteString(shelvedItem.SourceVersionFrom.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        writer.WriteEndAttribute();
      }
      writer.WriteStartAttribute("type");
      writer.WriteString(shelvedItem.ItemType.ToString());
      writer.WriteEndAttribute();
      writer.WriteEndElement();
    }

    internal static void ConditionAndSerializeItem(
      VersionControlRequestContext versionControlRequestContext,
      bool isWebView,
      XmlWriter writer,
      Item item)
    {
      item.CheckinDate = TimeZone.CurrentTimeZone.ToLocalTime(item.CheckinDate);
      item.TimeZone = !TimeZone.CurrentTimeZone.IsDaylightSavingTime(item.CheckinDate) ? TimeZone.CurrentTimeZone.StandardName : TimeZone.CurrentTimeZone.DaylightName;
      item.TimeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset(item.CheckinDate);
      if (isWebView)
        WebView.GenerateItemWebView(versionControlRequestContext, writer, item);
      else
        new XmlSerializer(typeof (Item)).Serialize(writer, (object) item);
    }

    internal static void ConditionAndSerializeItem(
      VersionControlRequestContext versionControlRequestContext,
      XmlWriter writer,
      Item item)
    {
      WebView.ConditionAndSerializeItem(versionControlRequestContext, WebView.IsWebViewQuery(versionControlRequestContext), writer, item);
    }

    private static void GenerateItemWebView(
      VersionControlRequestContext versionControlRequestContext,
      XmlWriter writer,
      Item item)
    {
      writer.WriteStartElement("Item");
      writer.WriteStartAttribute("title");
      writer.WriteString(VersionControlPath.GetFileName(item.ServerItem));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("date");
      writer.WriteString(item.CheckinDate.ToString());
      writer.WriteEndAttribute();
      writer.WriteStartAttribute(nameof (item));
      writer.WriteString(item.ServerItem);
      writer.WriteEndAttribute();
      TeamFoundationLinkingService service = versionControlRequestContext.RequestContext.GetService<TeamFoundationLinkingService>();
      VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new ChangesetUri(item.ChangesetId, UriType.Extended);
      writer.WriteStartAttribute("csurl");
      writer.WriteString(service.GetArtifactUrlExternal(versionControlRequestContext.RequestContext, controlIntegrationUri.ArtifactId));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("cs");
      writer.WriteString(item.ChangesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("len");
      writer.WriteString(item.FileLength.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("tz");
      writer.WriteString(item.TimeZone.ToString());
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("type");
      writer.WriteString(item.ItemType.ToString());
      writer.WriteEndAttribute();
      writer.WriteStartAttribute("tzo");
      writer.WriteString(item.TimeZoneOffset.ToString());
      writer.WriteEndAttribute();
      writer.WriteEndElement();
    }
  }
}
