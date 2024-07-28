// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.XslEmailTemplateTransform
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.EmailNotification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class XslEmailTemplateTransform
  {
    private readonly string m_searchPath;
    private readonly string m_xslFileName;
    internal static JsonSerializer s_serializer = new NotificationTranformJsonFormatter().CreateJsonSerializer();
    private static readonly ConcurrentDictionary<string, XslCompiledTransform> xsltCache = new ConcurrentDictionary<string, XslCompiledTransform>();
    private const string s_Area = "Notifications";
    private const string s_Layer = "XslTemplateTransform";

    public XslEmailTemplateTransform(string xslSearchPath, string fileName)
    {
      this.m_searchPath = xslSearchPath;
      this.m_xslFileName = fileName;
    }

    public NotificationTransformResult Transform(
      IVssRequestContext requestContext,
      NotificationTransformContext transformContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<NotificationTransformContext>(transformContext, nameof (transformContext));
      string error = (string) null;
      XmlDocumentFieldContainer xmlContainer;
      if (!XslEmailTemplateTransform.GetXmlContainer(transformContext.EventFieldContainer, out xmlContainer))
        return (NotificationTransformResult) null;
      foreach (KeyValuePair<string, string> systemInput in transformContext.SystemInputs)
      {
        if (!string.IsNullOrEmpty(systemInput.Value))
          xmlContainer.AddOrUpdateNode(systemInput.Key, systemInput.Value);
      }
      string emailContent;
      if (!XslEmailTemplateTransform.TryFormat(this.m_searchPath, requestContext.ServiceHost.GetCulture(requestContext).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture), this.m_xslFileName, (XmlNode) xmlContainer.Document, out emailContent, xslFileNotFoundHandler: (Action) (() => error = CoreRes.NotificationXslNotFound((object) transformContext.EventType))))
        throw new EmailNotificationOperationFailException(CoreRes.NotificationEmailFormatError((object) error, (object) transformContext.EventFieldContainer.GetDocumentString()));
      return new NotificationTransformResult()
      {
        Content = emailContent,
        Properties = new JObject()
        {
          {
            "emailSubject",
            (JToken) this.GetEmailTitle(requestContext, transformContext.EventType, xmlContainer.Document)
          }
        }
      };
    }

    public static bool TryFormat(
      string searchPath,
      string lcidPath,
      string xslFileName,
      XmlNode eventData,
      out string emailContent,
      Action<Exception> formatExceptionHandler = null,
      Action xslFileNotFoundHandler = null)
    {
      emailContent = (string) null;
      bool flag = false;
      string str = Path.Combine(searchPath, lcidPath, xslFileName);
      if (!File.Exists(str))
        str = Path.Combine(searchPath, xslFileName);
      try
      {
        XslCompiledTransform transform;
        if (!XslEmailTemplateTransform.xsltCache.TryGetValue(str, out transform))
        {
          if (File.Exists(str))
          {
            transform = new XslCompiledTransform();
            transform.Load(str);
            XslEmailTemplateTransform.xsltCache.TryAdd(str, transform);
          }
          else if (xslFileNotFoundHandler != null)
            xslFileNotFoundHandler();
        }
        if (transform != null)
        {
          emailContent = XslEmailTemplateTransform.TransformEvent(eventData, transform);
          flag = true;
        }
      }
      catch (Exception ex)
      {
        if (formatExceptionHandler == null)
          throw;
        else
          formatExceptionHandler(ex);
      }
      return flag;
    }

    private static bool GetXmlContainer(
      IFieldContainer fieldContainer,
      out XmlDocumentFieldContainer xmlContainer)
    {
      ref XmlDocumentFieldContainer local = ref xmlContainer;
      if (!(fieldContainer is XmlDocumentFieldContainer documentFieldContainer))
        documentFieldContainer = fieldContainer.GetDynamicFieldContainer(DynamicFieldContainerType.Xml) as XmlDocumentFieldContainer;
      local = documentFieldContainer;
      return xmlContainer != null;
    }

    private static string TransformEvent(XmlNode eventData, XslCompiledTransform transform)
    {
      XmlWriterSettings settings = transform.OutputSettings.Clone();
      settings.CheckCharacters = false;
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlWriter results = XmlWriter.Create((TextWriter) output, settings))
        {
          transform.Transform((IXPathNavigable) eventData, (XsltArgumentList) null, results);
          results.Flush();
          return output.ToString();
        }
      }
    }

    internal string GetEmailTitle(
      IVssRequestContext requestContext,
      string eventType,
      XmlDocument eventDocument = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
      string emailTitle = eventType;
      if (eventDocument != null)
      {
        XPathNodeIterator xpathNodeIterator = (XPathNodeIterator) eventDocument.CreateNavigator().Evaluate(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/*[local-name() = '{0}'][1]/*[local-name() = 'Title']", (object) eventType));
        if (xpathNodeIterator != null && xpathNodeIterator.MoveNext())
          emailTitle = xpathNodeIterator.Current.Value;
      }
      else
        requestContext.Trace(1002500, TraceLevel.Error, "Notifications", "XslTemplateTransform", "Unable to get email title for event of type {0}", (object) eventType);
      return emailTitle;
    }
  }
}
