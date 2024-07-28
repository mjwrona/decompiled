// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.SubscriptionTemplate
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  [DataContract]
  internal class SubscriptionTemplate
  {
    private static List<SubscriptionTemplate> s_basicTemplates;
    private static List<SubscriptionTemplate> s_chatRoomTemplates;
    private static List<SubscriptionTemplate> s_defaultCustomTemplates;

    public static IEnumerable<SubscriptionTemplate> GetBasicTemplates(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext)
    {
      if (SubscriptionTemplate.s_basicTemplates == null)
        SubscriptionTemplate.s_basicTemplates = SubscriptionTemplate.ReadTemplates(AlertsServerResources.BasicSubscriptionTemplates);
      List<SubscriptionTemplate> list = SubscriptionTemplate.ApplyTemplateAdapters(tfsRequestContext, tfsWebContext, (IEnumerable<SubscriptionTemplate>) SubscriptionTemplate.s_basicTemplates).ToList<SubscriptionTemplate>();
      foreach (SubscriptionTemplate subscriptionTemplate in list)
      {
        if (string.IsNullOrEmpty(subscriptionTemplate.BasicTemplateTag))
          subscriptionTemplate.BasicTemplateTag = subscriptionTemplate.EventTypeName;
      }
      return (IEnumerable<SubscriptionTemplate>) list;
    }

    public static IEnumerable<SubscriptionTemplate> GetChatRoomTemplates(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext)
    {
      if (SubscriptionTemplate.s_chatRoomTemplates == null)
        SubscriptionTemplate.s_chatRoomTemplates = SubscriptionTemplate.ReadTemplates(AlertsServerResources.ChatSubscriptionTemplates);
      List<SubscriptionTemplate> list = SubscriptionTemplate.ApplyTemplateAdapters(tfsRequestContext, tfsWebContext, (IEnumerable<SubscriptionTemplate>) SubscriptionTemplate.s_chatRoomTemplates).ToList<SubscriptionTemplate>();
      foreach (SubscriptionTemplate subscriptionTemplate in list)
      {
        if (string.IsNullOrEmpty(subscriptionTemplate.BasicTemplateTag))
          subscriptionTemplate.BasicTemplateTag = subscriptionTemplate.EventTypeName;
      }
      return (IEnumerable<SubscriptionTemplate>) list;
    }

    public static IEnumerable<SubscriptionTemplate> GetCustomTemplates(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext)
    {
      CachedRegistryService service = tfsRequestContext.GetService<CachedRegistryService>();
      string templatesXml = (string) null;
      RegistryQuery registryQuery;
      if (!string.IsNullOrEmpty(tfsWebContext.NavigationContext.Project))
      {
        CachedRegistryService registryService = service;
        IVssRequestContext requestContext = tfsRequestContext;
        registryQuery = (RegistryQuery) ("/Configuration/Alerts/Templates/Project/" + tfsWebContext.CurrentProjectGuid.ToString());
        ref RegistryQuery local = ref registryQuery;
        templatesXml = registryService.GetValue(requestContext, in local);
      }
      if (string.IsNullOrEmpty(templatesXml))
      {
        CachedRegistryService registryService = service;
        IVssRequestContext requestContext = tfsRequestContext;
        registryQuery = (RegistryQuery) "/Configuration/Alerts/Templates/Collection";
        ref RegistryQuery local = ref registryQuery;
        templatesXml = registryService.GetValue(requestContext, in local);
      }
      List<SubscriptionTemplate> templates;
      if (string.IsNullOrEmpty(templatesXml))
      {
        if (SubscriptionTemplate.s_defaultCustomTemplates == null)
          SubscriptionTemplate.s_defaultCustomTemplates = SubscriptionTemplate.ReadTemplates(AlertsServerResources.CustomSubscriptionTemplates);
        templates = SubscriptionTemplate.s_defaultCustomTemplates;
      }
      else
        templates = SubscriptionTemplate.ReadTemplates(templatesXml);
      return SubscriptionTemplate.ApplyTemplateAdapters(tfsRequestContext, tfsWebContext, (IEnumerable<SubscriptionTemplate>) templates);
    }

    private static IEnumerable<SubscriptionTemplate> ApplyTemplateAdapters(
      IVssRequestContext tfsRequestContext,
      TfsWebContext tfsWebContext,
      IEnumerable<SubscriptionTemplate> templates)
    {
      IEnumerable<SubscriptionTemplate> templates1 = templates.Select<SubscriptionTemplate, SubscriptionTemplate>((Func<SubscriptionTemplate, SubscriptionTemplate>) (t => new SubscriptionTemplate(TfsSubscriptionAdapter.CreateAdapter(tfsRequestContext, tfsWebContext, t.SubscriptionType), tfsWebContext, t)));
      IEnumerable<SubscriptionTemplate> templates2 = SubscriptionTemplate.FilterVersionControlTemplates(tfsWebContext, templates1);
      return SubscriptionTemplate.FilterTemplatesByLicense(tfsWebContext, templates2);
    }

    private static IEnumerable<SubscriptionTemplate> FilterTemplatesByLicense(
      TfsWebContext tfsWebContext,
      IEnumerable<SubscriptionTemplate> templates)
    {
      return templates.Where<SubscriptionTemplate>((Func<SubscriptionTemplate, bool>) (template => template.IsLicensed));
    }

    private static IEnumerable<SubscriptionTemplate> FilterVersionControlTemplates(
      TfsWebContext tfsWebContext,
      IEnumerable<SubscriptionTemplate> templates)
    {
      return tfsWebContext.NavigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Project) ? templates.Where<SubscriptionTemplate>((Func<SubscriptionTemplate, bool>) (template => SubscriptionTemplate.IsSupportedVCTemplate(tfsWebContext, template))) : templates;
    }

    private static bool IsSupportedVCTemplate(
      TfsWebContext tfsWebContext,
      SubscriptionTemplate template)
    {
      if (template.SubscriptionType == SubscriptionType.GitPushEvent || template.SubscriptionType == SubscriptionType.GitPullRequestEvent)
        return AlertsExtensions.DoesProjectUseGitVersionControl(tfsWebContext);
      return template.SubscriptionType != SubscriptionType.CheckinEvent && template.SubscriptionType != SubscriptionType.CodeReviewChangedEvent || AlertsExtensions.DoesProjectUseVCVersionControl(tfsWebContext);
    }

    private static List<SubscriptionTemplate> ReadTemplates(string templatesXml)
    {
      templatesXml = templatesXml.Trim();
      List<SubscriptionTemplate> subscriptionTemplateList = new List<SubscriptionTemplate>();
      XmlReader reader = XmlReader.Create((TextReader) new StringReader(templatesXml), new XmlReaderSettings()
      {
        IgnoreComments = true,
        IgnoreWhitespace = true,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      });
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(reader);
      foreach (XmlElement xmlElement in xmlDocument.SelectNodes("/SubscriptionTemplates/SubscriptionTemplate").Cast<XmlElement>())
      {
        SubscriptionTemplate subscriptionTemplate = new SubscriptionTemplate()
        {
          TemplateName = xmlElement.GetAttribute("TemplateName"),
          EventTypeName = xmlElement.GetAttribute("EventType")
        };
        subscriptionTemplate.SubscriptionType = (SubscriptionType) Enum.Parse(typeof (SubscriptionType), subscriptionTemplate.EventTypeName, true);
        subscriptionTemplate.ShowInQuickAlerts = string.Equals("True", xmlElement.GetAttribute("ShowInQuickAlerts"), StringComparison.OrdinalIgnoreCase);
        subscriptionTemplate.BasicTemplateTag = xmlElement.GetAttribute("BasicTemplateTag");
        subscriptionTemplate.Matcher = "XPathMatcher";
        subscriptionTemplate.ConditionString = string.Empty;
        if (xmlElement.FirstChild != null)
          subscriptionTemplate.ConditionString = xmlElement.FirstChild.InnerText.Trim();
        subscriptionTemplate.DefaultName = xmlElement.GetAttribute("DefaultName");
        if (string.IsNullOrEmpty(subscriptionTemplate.DefaultName))
          subscriptionTemplate.DefaultName = subscriptionTemplate.TemplateName;
        subscriptionTemplateList.Add(subscriptionTemplate);
      }
      return subscriptionTemplateList;
    }

    private SubscriptionTemplate()
    {
    }

    private SubscriptionTemplate(
      TfsSubscriptionAdapter adapter,
      TfsWebContext tfsWebContext,
      SubscriptionTemplate copyFrom)
    {
      this.ConditionString = copyFrom.ConditionString;
      this.EventTypeName = copyFrom.EventTypeName;
      this.SubscriptionType = copyFrom.SubscriptionType;
      this.ShowInQuickAlerts = copyFrom.ShowInQuickAlerts;
      this.BasicTemplateTag = copyFrom.BasicTemplateTag;
      this.Matcher = copyFrom.Matcher;
      this.IsLicensed = adapter.IsLicensed(tfsWebContext);
      string str1 = AlertsServerResources.ResourceManager.GetString(copyFrom.DefaultName, AlertsServerResources.Culture);
      this.DefaultName = str1 == null ? copyFrom.DefaultName : str1;
      string str2 = AlertsServerResources.ResourceManager.GetString(copyFrom.TemplateName, AlertsServerResources.Culture);
      this.TemplateName = str2 == null ? copyFrom.TemplateName : str2;
      if (tfsWebContext.NavigationContext.Levels.HasFlag((Enum) NavigationContextLevels.Project))
        this.ConditionString = this.ConditionString.Replace("'@Project'", "'" + tfsWebContext.Project.Name + "'").Replace("'@ProjectUri'", "'" + tfsWebContext.Project.Uri + "'");
      try
      {
        this.Filter = adapter.ParseCondition(tfsWebContext.TfsRequestContext, this.ConditionString, this.Matcher);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (SubscriptionTemplate), ex);
        this.Filter = new ExpressionFilterModel();
        this.ParseError = ex.ToString();
      }
    }

    protected string ConditionString { get; set; }

    [DataMember(Name = "parseError", EmitDefaultValue = false)]
    public string ParseError { get; set; }

    [DataMember(Name = "defaultName")]
    public string DefaultName { get; set; }

    [DataMember(Name = "eventTypeName")]
    public string EventTypeName { get; set; }

    [DataMember(Name = "templateName", EmitDefaultValue = false)]
    public string TemplateName { get; set; }

    [DataMember(Name = "subscriptionType")]
    public SubscriptionType SubscriptionType { get; set; }

    [DataMember(Name = "filter")]
    public ExpressionFilterModel Filter { get; set; }

    [DataMember(Name = "showInQuickAlerts")]
    public bool ShowInQuickAlerts { get; set; }

    [DataMember(Name = "basicTemplateTag", EmitDefaultValue = false)]
    public string BasicTemplateTag { get; set; }

    [DataMember(Name = "matcher")]
    public string Matcher { get; set; }

    public bool IsLicensed { get; private set; }
  }
}
