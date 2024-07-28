// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.RegistrationSDKHelper
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  internal static class RegistrationSDKHelper
  {
    internal const int TemplateMaxLength = 200;

    internal static void ValidateRegistration(RegistrationDescription registration)
    {
      switch (registration)
      {
        case WindowsTemplateRegistrationDescription registration1:
          registration1.SetWnsType();
          break;
        case MpnsTemplateRegistrationDescription registration2:
          registration2.SetMpnsType();
          break;
      }
      registration.Validate(true, ApiVersion.Four);
    }

    private static void SetMpnsType(
      this MpnsTemplateRegistrationDescription registration)
    {
      if (registration == null || registration.IsJsonObjectPayLoad())
        return;
      if (registration.MpnsHeaders != null && registration.MpnsHeaders.ContainsKey("X-NotificationClass"))
      {
        int num = int.Parse(registration.MpnsHeaders["X-NotificationClass"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (num >= 3 && num <= 10 || num >= 13 && num <= 20 || num >= 23 && num <= 31)
          return;
      }
      if (!registration.IsXmlPayLoad())
        return;
      if (registration.MpnsHeaders == null)
        registration.MpnsHeaders = new MpnsHeaderCollection();
      switch (RegistrationSDKHelper.DetectMpnsTemplateRegistationType((string) registration.BodyTemplate, SRClient.NotSupportedXMLFormatAsBodyTemplateForMpns))
      {
        case MpnsTemplateBodyType.Toast:
          RegistrationSDKHelper.AddOrUpdateHeader((SortedDictionary<string, string>) registration.MpnsHeaders, "X-WindowsPhone-Target", "toast");
          RegistrationSDKHelper.AddOrUpdateHeader((SortedDictionary<string, string>) registration.MpnsHeaders, "X-NotificationClass", "2");
          break;
        case MpnsTemplateBodyType.Tile:
          RegistrationSDKHelper.AddOrUpdateHeader((SortedDictionary<string, string>) registration.MpnsHeaders, "X-WindowsPhone-Target", "token");
          RegistrationSDKHelper.AddOrUpdateHeader((SortedDictionary<string, string>) registration.MpnsHeaders, "X-NotificationClass", "1");
          break;
      }
    }

    private static void SetWnsType(
      this WindowsTemplateRegistrationDescription registration)
    {
      if (registration == null || registration.IsJsonObjectPayLoad() || !registration.IsXmlPayLoad())
        return;
      if (registration.WnsHeaders == null)
        registration.WnsHeaders = new WnsHeaderCollection();
      if (registration.WnsHeaders.ContainsKey("X-WNS-Type"))
      {
        if (registration.WnsHeaders["X-WNS-Type"].Equals("wns/raw", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            using (XmlReader reader = XmlReader.Create((TextReader) new StringReader((string) registration.BodyTemplate)))
            {
              new XmlDocument().Load(reader);
              return;
            }
          }
          catch (XmlException ex)
          {
            throw new ArgumentException(SRClient.NotSupportedXMLFormatAsBodyTemplate);
          }
        }
      }
      switch (RegistrationSDKHelper.DetectWindowsTemplateRegistationType((string) registration.BodyTemplate, SRClient.NotSupportedXMLFormatAsBodyTemplate))
      {
        case WindowsTemplateBodyType.Toast:
          RegistrationSDKHelper.AddOrUpdateHeader((SortedDictionary<string, string>) registration.WnsHeaders, "X-WNS-Type", "wns/toast");
          break;
        case WindowsTemplateBodyType.Tile:
          RegistrationSDKHelper.AddOrUpdateHeader((SortedDictionary<string, string>) registration.WnsHeaders, "X-WNS-Type", "wns/tile");
          break;
        case WindowsTemplateBodyType.Badge:
          RegistrationSDKHelper.AddOrUpdateHeader((SortedDictionary<string, string>) registration.WnsHeaders, "X-WNS-Type", "wns/badge");
          break;
      }
    }

    private static void AddOrUpdateHeader(
      SortedDictionary<string, string> headers,
      string key,
      string value)
    {
      if (!headers.ContainsKey(key))
        headers.Add(key, value);
      else
        headers[key] = value;
    }

    public static WindowsTemplateBodyType DetectWindowsTemplateRegistationType(
      string body,
      string errorMsg)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(body)))
      {
        try
        {
          xmlDocument.Load(reader);
        }
        catch (XmlException ex)
        {
          throw new ArgumentException(errorMsg);
        }
        XmlNode xmlNode = xmlDocument.FirstChild;
        while (xmlNode != null && xmlNode.NodeType != XmlNodeType.Element)
          xmlNode = xmlNode.NextSibling;
        if (xmlNode == null)
          throw new ArgumentException(errorMsg);
        WindowsTemplateBodyType result;
        if (xmlNode == null || !Enum.TryParse<WindowsTemplateBodyType>(xmlNode.Name, true, out result))
          throw new ArgumentException(errorMsg);
        return result;
      }
    }

    public static MpnsTemplateBodyType DetectMpnsTemplateRegistationType(
      string body,
      string errorMsg)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(body)))
      {
        try
        {
          xmlDocument.Load(reader);
        }
        catch (XmlException ex)
        {
          throw new ArgumentException(errorMsg);
        }
        XmlNode xmlNode = xmlDocument.FirstChild;
        while (xmlNode != null && xmlNode.NodeType != XmlNodeType.Element)
          xmlNode = xmlNode.NextSibling;
        if (xmlNode == null)
          throw new ArgumentException(errorMsg);
        if (!xmlNode.NamespaceURI.Equals("WPNotification", StringComparison.OrdinalIgnoreCase) || !xmlNode.LocalName.Equals("Notification", StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException(errorMsg);
        XmlNode firstChild = xmlNode.FirstChild;
        MpnsTemplateBodyType result;
        if (firstChild == null || !Enum.TryParse<MpnsTemplateBodyType>(firstChild.LocalName, true, out result))
          throw new ArgumentException(errorMsg);
        return result;
      }
    }

    public static string AddDeclarationToXml(string content)
    {
      XmlDocument xmlDocument = new XmlDocument();
      using (XmlReader reader = XmlReader.Create((TextReader) new StringReader(content)))
      {
        xmlDocument.Load(reader);
        if (xmlDocument.FirstChild.NodeType != XmlNodeType.XmlDeclaration)
        {
          XmlNode xmlDeclaration = (XmlNode) xmlDocument.CreateXmlDeclaration("1.0", "utf-16", (string) null);
          XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
          xmlDocument.InsertBefore(xmlDeclaration, documentElement);
        }
        return xmlDocument.InnerXml;
      }
    }
  }
}
