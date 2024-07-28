// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContentProviderHelper
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  public static class ContentProviderHelper
  {
    private static RegistryQuery cdnTimeoutQuery = new RegistryQuery("/Configuration/WebAccess/CDN/TimeoutSeconds");
    private const int cdnTimeoutDefault = 10;
    private static readonly Random s_randomGenerator = new Random();
    private const string c_scriptsFirstStrategy = "scriptsFirst";
    private const string c_stylesFirstStrategy = "stylesFirst";

    public static void GenerateContent(IVssRequestContext requestContext, TextWriter writer)
    {
      int maxPriorityToInline = 20;
      IWebDiagnosticsService service1 = requestContext.GetService<IWebDiagnosticsService>();
      using (PerformanceTimer.StartMeasure(requestContext, "ProcessContent"))
      {
        IContentService service2 = requestContext.GetService<IContentService>();
        IEnumerable<ContributedContent> source = service2.QueryContent(requestContext);
        string preferredLocation = service1.IsCdnEnabled(requestContext) ? "Cdn" : "Local";
        string fileType = service2.GetRequestContentType(requestContext);
        string scriptType = service2.GetRequestScriptType(requestContext);
        string styleType = service2.GetRequestStyleType(requestContext);
        bool flag1 = service1.IsCdnEnabled(requestContext);
        bool flag2 = service1.IsBundlingEnabled(requestContext);
        bool flag3 = requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.CdnInitialScriptCheck");
        bool flag4 = !requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.DisableCdnTimeoutCheck") & flag1 & flag2;
        bool useIntegrity = requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.SubresourceIntegrity");
        bool flag5 = new AadAuthenticationSessionTokenConfiguration().IssueAadAuthenticationCookieEnabled(requestContext, new Guid?());
        string nonce = (string) null;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Security.ContentSecurityPolicy") || requestContext.IsFeatureEnabled("VisualStudio.Services.Security.ContentSecurityPolicy.ReportOnly"))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          nonce = vssRequestContext.GetService<IContentSecurityPolicyNonceManagementService>().GetNonceValue(vssRequestContext, HttpContextFactory.Current);
        }
        using (XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings()
        {
          ConformanceLevel = ConformanceLevel.Fragment
        }))
        {
          JArray jarray1 = new JArray((object) source.Where<ContributedContent>((Func<ContributedContent, bool>) (o => o.ModuleNamespaces != null)).SelectMany<ContributedContent, string>((Func<ContributedContent, IEnumerable<string>>) (s => s.ModuleNamespaces)).Distinct<string>());
          JArray jarray2 = new JArray((object) source.Where<ContributedContent>((Func<ContributedContent, bool>) (o => o.LoaderPriority >= LoaderPriority.Deferred && o.ModuleNamespaces != null)).SelectMany<ContributedContent, string>((Func<ContributedContent, IEnumerable<string>>) (s => s.ModuleNamespaces)).Distinct<string>());
          xmlWriter.WriteStartElement("meta");
          xmlWriter.WriteAttributeString("name", "referrer");
          xmlWriter.WriteAttributeString("content", "strict-origin-when-cross-origin");
          xmlWriter.WriteEndElement();
          xmlWriter.WriteStartElement("script");
          xmlWriter.WriteAttributeString("data-description", "contentInit");
          ContentProviderHelper.AddNonce(nonce, xmlWriter);
          xmlWriter.WriteRaw(string.Format("window.$DEBUG = {0};", service1.IsDebugContentEnabled(requestContext) ? (object) "true" : (object) "false"));
          xmlWriter.WriteString(string.Format("window.__scriptType = {0};", (object) JsonConvert.ToString(scriptType)));
          xmlWriter.WriteString(string.Format("window.__moduleNamespaces = {0};", (object) JsonConvert.SerializeObject((object) jarray1)));
          xmlWriter.WriteString(string.Format("window.__asyncModules = {0};", (object) JsonConvert.SerializeObject((object) jarray2)));
          xmlWriter.WriteString(string.Format("window.__extensionsStaticRoot = {0};", (object) JsonConvert.SerializeObject((object) StaticResources.Extensions.GetLocation(string.Empty, requestContext))));
          xmlWriter.WriteString(string.Format("window.__extensionsLocalStaticRoot = {0};", (object) JsonConvert.SerializeObject((object) StaticResources.Extensions.GetLocalLocation(string.Empty, requestContext))));
          xmlWriter.WriteString(string.Format("window.__useAadToken = {0};", flag5 ? (object) "true" : (object) "false"));
          string str1 = ContentProviderHelper.s_randomGenerator.Next(0, 10) < 5 ? "scriptsFirst" : "stylesFirst";
          xmlWriter.WriteRaw("window.resourceLoadStrategy = '" + str1 + "';");
          xmlWriter.WriteRaw("var preTraceServiceStartErrors = []; var onPreTraceServiceError = function (event) { ");
          xmlWriter.WriteRaw("preTraceServiceStartErrors.push(event) }; window.addEventListener('error', onPreTraceServiceError);");
          xmlWriter.WriteRaw("var previousResourceLoadCompletion = window.performance.now(); var resourceLoadTimes = {}; window.logResourceLoad = function (id) {");
          xmlWriter.WriteRaw("var now = window.performance.now(); resourceLoadTimes[id] = { current: now, previous: previousResourceLoadCompletion }; ");
          xmlWriter.WriteRaw("previousResourceLoadCompletion = now; }; ");
          xmlWriter.WriteEndElement();
          if (flag1 & flag2)
          {
            string str2 = "";
            if (requestContext.ExecutionEnvironment.IsSslOnly)
              str2 = ";secure";
            xmlWriter.WriteStartElement("script");
            xmlWriter.WriteAttributeString("data-description", "disableCdn");
            ContentProviderHelper.AddNonce(nonce, xmlWriter);
            xmlWriter.WriteRaw("function cdnReload(fromTimeout) { \n");
            xmlWriter.WriteRaw(string.Format("document.cookie = '{0}={1};max-age={2};path=/{3}'; \n", (object) "TFS-CDN", (object) "disabled", (object) 28800, (object) str2));
            xmlWriter.WriteRaw(string.Format("document.cookie = '{0}={1};path=/{2}'; \n", (object) "TFS-CDNTRACE", (object) "report", (object) str2));
            xmlWriter.WriteRaw(string.Format("if(fromTimeout){{document.cookie = '{0}={1};path=/{2}'; }}\n", (object) "TFS-CDNTIMEOUT", (object) "report", (object) str2));
            xmlWriter.WriteRaw("document.location.reload(true); \n");
            xmlWriter.WriteRaw("}");
            if (flag4)
            {
              int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in ContentProviderHelper.cdnTimeoutQuery, true, 10);
              xmlWriter.WriteRaw(string.Format("\nvar cdnTimeout = setTimeout(function () {{ cdnReload(true); }}, {0});", (object) (num * 1000)));
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("script");
            xmlWriter.WriteAttributeString("data-description", "trackLoadedBundles");
            ContentProviderHelper.AddNonce(nonce, xmlWriter);
            xmlWriter.WriteRaw(" var loadedScripts = {}; document.addEventListener('scriptLoaded', function (e) { if (e && e.detail) { loadedScripts[e.detail.id] = true;} });");
            xmlWriter.WriteEndElement();
          }
          List<ContentSource> list1 = source.SelectMany<ContributedContent, ContentSource>((Func<ContributedContent, IEnumerable<ContentSource>>) (c => c.GetContentSources(requestContext, styleType + fileType, "text/css", preferredLocation))).ToList<ContentSource>();
          List<ContentSource> list2 = source.SelectMany<ContributedContent, ContentSource>((Func<ContributedContent, IEnumerable<ContentSource>>) (c => c.GetContentSources(requestContext, scriptType + fileType, "text/javascript", preferredLocation))).ToList<ContentSource>();
          int[] contentPriorities1 = ContentProviderHelper.GetContentPriorities(list1);
          int[] contentPriorities2 = ContentProviderHelper.GetContentPriorities(list2);
          int lastIndex1 = list1.FindLastIndex((Predicate<ContentSource>) (c => c.Priority <= maxPriorityToInline));
          int lastIndex2 = list2.FindLastIndex((Predicate<ContentSource>) (c => c.Priority <= maxPriorityToInline));
          if (str1 == "scriptsFirst")
          {
            ContentProviderHelper.EmitInlinedScripts(useIntegrity, nonce, xmlWriter, list2, lastIndex2);
            ContentProviderHelper.EmitInlinedStylesheets(useIntegrity, nonce, xmlWriter, list1, lastIndex1);
          }
          else
          {
            ContentProviderHelper.EmitInlinedStylesheets(useIntegrity, nonce, xmlWriter, list1, lastIndex1);
            ContentProviderHelper.EmitInlinedScripts(useIntegrity, nonce, xmlWriter, list2, lastIndex2);
          }
          if (flag1)
          {
            xmlWriter.WriteStartElement("script");
            xmlWriter.WriteAttributeString("data-description", "cdnFallback");
            ContentProviderHelper.AddNonce(nonce, xmlWriter);
            xmlWriter.WriteRaw("if (!window.LWL) { cdnReload(false); }");
            xmlWriter.WriteEndElement();
            if (flag4)
            {
              xmlWriter.WriteStartElement("script");
              xmlWriter.WriteAttributeString("data-description", "clearCdnTimeout");
              ContentProviderHelper.AddNonce(nonce, xmlWriter);
              xmlWriter.WriteRaw("window.clearTimeout(cdnTimeout);");
              xmlWriter.WriteEndElement();
            }
          }
          xmlWriter.WriteStartElement("script");
          xmlWriter.WriteAttributeString("data-description", "fireLoadEvents");
          ContentProviderHelper.AddNonce(nonce, xmlWriter);
          xmlWriter.WriteRaw("var contentElements = document.querySelectorAll(\"[data-loadingcontent=true]\"); ");
          xmlWriter.WriteRaw("for (var i=0; i<contentElements.length; i++) { var loadTimes = resourceLoadTimes[contentElements[i].getAttribute('data-clientid')]; ");
          xmlWriter.WriteRaw("window.__contentLoaded(contentElements[i], ");
          xmlWriter.WriteRaw(flag3 & flag1 & flag2 ? "contentElements[i].tagName.toUpperCase() === 'SCRIPT' ? loadedScripts[contentElements[i].getAttribute('data-sourcecontribution')] || false : true," : "true, ");
          xmlWriter.WriteRaw("loadTimes && loadTimes.current, loadTimes && loadTimes.previous); }");
          xmlWriter.WriteEndElement();
          for (int index = lastIndex1 + 1; index < list1.Count; ++index)
            ContentProviderHelper.EmitPreloadLinkTag(list1[index], contentPriorities1[index], useIntegrity, xmlWriter);
          for (int index = lastIndex2 + 1; index < list2.Count; ++index)
            ContentProviderHelper.EmitPreloadLinkTag(list2[index], contentPriorities2[index], useIntegrity, xmlWriter);
          if (lastIndex2 + 1 >= list2.Count)
            return;
          xmlWriter.WriteStartElement("script");
          xmlWriter.WriteAttributeString("data-description", "delayLoadScripts");
          ContentProviderHelper.AddNonce(nonce, xmlWriter);
          for (int index = lastIndex1 + 1; index < list1.Count; ++index)
            ContentProviderHelper.EmitLoadScriptStatement(list1[index], contentPriorities1[index], useIntegrity, xmlWriter);
          for (int index = lastIndex2 + 1; index < list2.Count; ++index)
            ContentProviderHelper.EmitLoadScriptStatement(list2[index], contentPriorities2[index], useIntegrity, xmlWriter);
          xmlWriter.WriteEndElement();
        }
      }
    }

    private static void EmitInlinedScripts(
      bool useIntegrity,
      string nonce,
      XmlWriter xmlWriter,
      List<ContentSource> scriptSources,
      int lastScriptIndexToInline)
    {
      for (int index = 0; index <= lastScriptIndexToInline; ++index)
      {
        ContentSource scriptSource = scriptSources[index];
        xmlWriter.WriteStartElement("script");
        xmlWriter.WriteAttributeString("src", scriptSource.Url);
        ContentProviderHelper.AddNonce(nonce, xmlWriter);
        xmlWriter.WriteAttributeString("data-contentlength", scriptSource.ContentLength.ToString());
        xmlWriter.WriteAttributeString("data-clientid", scriptSource.ClientId);
        xmlWriter.WriteAttributeString("data-sourcecontribution", scriptSource.ContributionId ?? string.Empty);
        xmlWriter.WriteAttributeString("data-loadingcontent", "true");
        if (useIntegrity && scriptSource.Integrity != null)
        {
          xmlWriter.WriteAttributeString("integrity", scriptSource.Integrity);
          xmlWriter.WriteAttributeString("crossorigin", "anonymous");
        }
        if (scriptSource.Priority >= 100)
          xmlWriter.WriteAttributeString("defer", "");
        xmlWriter.WriteString(string.Empty);
        xmlWriter.WriteEndElement();
        ContentProviderHelper.WriteResourceLoadedTimestampScript(xmlWriter, nonce, scriptSource.ClientId);
      }
    }

    private static void EmitInlinedStylesheets(
      bool useIntegrity,
      string nonce,
      XmlWriter xmlWriter,
      List<ContentSource> styleSources,
      int lastStyleIndexToInline)
    {
      for (int index = 0; index <= lastStyleIndexToInline; ++index)
      {
        ContentSource styleSource = styleSources[index];
        xmlWriter.WriteStartElement("link");
        xmlWriter.WriteAttributeString("href", styleSource.Url);
        xmlWriter.WriteAttributeString("rel", "stylesheet");
        xmlWriter.WriteAttributeString("crossorigin", "anonymous");
        xmlWriter.WriteAttributeString("data-contentlength", styleSource.ContentLength.ToString());
        xmlWriter.WriteAttributeString("data-clientid", styleSource.ClientId);
        xmlWriter.WriteAttributeString("data-sourcecontribution", styleSource.ContributionId ?? string.Empty);
        xmlWriter.WriteAttributeString("data-loadingcontent", "true");
        if (useIntegrity && styleSource.Integrity != null)
          xmlWriter.WriteAttributeString("integrity", styleSource.Integrity);
        xmlWriter.WriteEndElement();
        ContentProviderHelper.WriteResourceLoadedTimestampScript(xmlWriter, nonce, styleSource.ClientId);
      }
    }

    private static void WriteResourceLoadedTimestampScript(
      XmlWriter xmlWriter,
      string nonce,
      string clientId)
    {
      xmlWriter.WriteStartElement("script");
      xmlWriter.WriteAttributeString("data-description", "logResourceLoad");
      ContentProviderHelper.AddNonce(nonce, xmlWriter);
      xmlWriter.WriteRaw("window.logResourceLoad('" + clientId + "');");
      xmlWriter.WriteEndElement();
    }

    private static int[] GetContentPriorities(List<ContentSource> sources)
    {
      int[] contentPriorities = new int[sources.Count];
      int num = int.MaxValue;
      for (int index = sources.Count - 1; index >= 0; --index)
      {
        if (sources[index].Priority < num)
          num = sources[index].Priority;
        contentPriorities[index] = num;
      }
      return contentPriorities;
    }

    private static void EmitPreloadLinkTag(
      ContentSource contentSource,
      int priority,
      bool useIntegrity,
      XmlWriter xmlWriter)
    {
      if (priority >= 30)
        return;
      bool flag1 = string.Equals(contentSource.ContentType, "text/css");
      bool flag2 = useIntegrity && contentSource.Integrity != null;
      xmlWriter.WriteStartElement("link");
      xmlWriter.WriteAttributeString("rel", "preload");
      xmlWriter.WriteAttributeString("as", flag1 ? "style" : "script");
      xmlWriter.WriteAttributeString("href", contentSource.Url);
      if (flag2)
        xmlWriter.WriteAttributeString("integrity", contentSource.Integrity);
      if (flag1 | flag2)
        xmlWriter.WriteAttributeString("crossorigin", "anonymous");
      xmlWriter.WriteEndElement();
    }

    private static void EmitLoadScriptStatement(
      ContentSource contentSource,
      int priority,
      bool useIntegrity,
      XmlWriter xmlWriter)
    {
      string str = (string) null;
      if (useIntegrity && contentSource.Integrity != null)
        str = ", integrity:'" + contentSource.Integrity + "'";
      xmlWriter.WriteRaw(string.Format("window.__loadScript({{ url:'{0}', clientId: '{1}', contributionId:'{2}', contentType:'{3}', contentLength:{4}, priority:{5}{6} }});" + Environment.NewLine, (object) contentSource.Url, (object) contentSource.ClientId, (object) contentSource.ContributionId, (object) contentSource.ContentType, (object) contentSource.ContentLength, (object) priority, (object) str));
    }

    private static void AddNonce(string nonce, XmlWriter xmlWriter)
    {
      if (string.IsNullOrEmpty(nonce))
        return;
      xmlWriter.WriteAttributeString(nameof (nonce), nonce);
    }
  }
}
