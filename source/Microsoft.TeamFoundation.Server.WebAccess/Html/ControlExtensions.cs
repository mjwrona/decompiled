// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Html.ControlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Html
{
  public static class ControlExtensions
  {
    private const string IsExpandedSettingKey = "isExpanded";
    private const string SizeSettingKey = "size";
    public const string LeftHubSplitterKey = "LeftHubSplitter";
    public const string RightHubSplitterKey = "RightHubSplitter";
    public const string WebUIStateSettingsKey = "Web/UIState";
    private const string SearchBoxAdapterCssClassKey = "SearchBoxCssClass";
    private const string WorkItemsSearchAdapterCssClass = "search-adapter-work-items";
    private const string CodeSearchAdapterCssClass = "search-adapter-search";

    public static TControl ControlId<TControl>(this TControl control, string value) where TControl : ControlBase
    {
      control.ControlId = value;
      return control;
    }

    public static MvcHtmlString Control<TControl>(this HtmlHelper htmlHelper, TControl controlType) where TControl : ControlBase
    {
      ArgumentUtility.CheckForNull<TControl>(controlType, "control");
      return controlType.ToHtml(htmlHelper);
    }

    public static MvcHtmlString Control<TControl>(
      this HtmlHelper htmlHelper,
      TControl controlType,
      object htmlAttributes)
      where TControl : ControlBase
    {
      ArgumentUtility.CheckForNull<TControl>(controlType, "control");
      return controlType.ToHtml(htmlHelper, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes));
    }

    public static MvcHtmlString Control<TControl>(
      this HtmlHelper htmlHelper,
      TControl controlType,
      IDictionary<string, object> htmlAttributes)
      where TControl : ControlBase
    {
      ArgumentUtility.CheckForNull<TControl>(controlType, "control");
      return controlType.ToHtml(htmlHelper, htmlAttributes);
    }

    public static MvcHtmlString MenuBar(this HtmlHelper htmlHelper, IEnumerable<MenuItem> menuItems) => ControlExtensions.MenuBar(htmlHelper, menuItems, (IDictionary<string, object>) null);

    public static MvcHtmlString MenuBar(
      this HtmlHelper htmlHelper,
      IEnumerable<MenuItem> menuItems,
      object htmlAttributes)
    {
      return ControlExtensions.MenuBar(htmlHelper, menuItems, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes));
    }

    public static MvcHtmlString MenuBar(
      this HtmlHelper htmlHelper,
      IEnumerable<MenuItem> menuItems,
      IDictionary<string, object> htmlAttributes)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (MenuBar));
      try
      {
        Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuBar controlType = new Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuBar();
        if (menuItems != null)
        {
          foreach (MenuItem menuItem in menuItems)
            controlType.ChildItems.Add(menuItem);
        }
        return ControlExtensions.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuBar>(htmlHelper, controlType, htmlAttributes);
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (MenuBar));
      }
    }

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      string filterName,
      IEnumerable<string> filterItems)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter(filterName, filterItems));
    }

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      string filterName,
      IEnumerable<string> filterItems,
      object htmlAttributes)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter(filterName, filterItems), htmlAttributes);
    }

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      string filterName,
      IEnumerable<string> filterItems,
      IDictionary<string, object> htmlAttributes)
    {
      return ControlExtensions.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(htmlHelper, new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter(filterName, filterItems), htmlAttributes);
    }

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      string filterName,
      IEnumerable<PivotFilterItem> filterItems)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter(filterName, filterItems));
    }

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      string filterName,
      IEnumerable<PivotFilterItem> filterItems,
      object htmlAttributes)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter(filterName, filterItems), htmlAttributes);
    }

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      string filterName,
      IEnumerable<PivotFilterItem> filterItems,
      IDictionary<string, object> htmlAttributes)
    {
      return ControlExtensions.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(htmlHelper, new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter(filterName, filterItems), htmlAttributes);
    }

    public static MvcHtmlString PivotFilter(this HtmlHelper htmlHelper, Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter filter) => htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(filter);

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter filter,
      object htmlAttributes)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(filter, htmlAttributes);
    }

    public static MvcHtmlString PivotFilter(
      this HtmlHelper htmlHelper,
      Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter filter,
      IDictionary<string, object> htmlAttributes)
    {
      return ControlExtensions.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotFilter>(htmlHelper, filter, htmlAttributes);
    }

    public static MvcHtmlString PivotViews(
      this HtmlHelper htmlHelper,
      IEnumerable<string> views,
      string contributionId = null,
      bool autoEnhance = true)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews(views, contributionId, autoEnhance));
    }

    public static MvcHtmlString PivotViews(
      this HtmlHelper htmlHelper,
      IEnumerable<string> views,
      object htmlAttributes,
      string contributionId = null,
      bool autoEnhance = true)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews(views, contributionId, autoEnhance), htmlAttributes);
    }

    public static MvcHtmlString PivotViews(
      this HtmlHelper htmlHelper,
      IEnumerable<string> views,
      IDictionary<string, object> htmlAttributes,
      string contributionId = null,
      bool autoEnhance = true)
    {
      return ControlExtensions.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews>(htmlHelper, new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews(views, contributionId, autoEnhance), htmlAttributes);
    }

    public static MvcHtmlString PivotViews(
      this HtmlHelper htmlHelper,
      IEnumerable<PivotView> views,
      string contributionId = null,
      bool autoEnhance = true)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews(views, contributionId, autoEnhance));
    }

    public static MvcHtmlString PivotViews(
      this HtmlHelper htmlHelper,
      IEnumerable<PivotView> views,
      object htmlAttributes,
      string contributionId = null,
      bool autoEnhance = true)
    {
      return htmlHelper.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews>(new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews(views, contributionId, autoEnhance), htmlAttributes);
    }

    public static MvcHtmlString PivotViews(
      this HtmlHelper htmlHelper,
      IEnumerable<PivotView> views,
      IDictionary<string, object> htmlAttributes,
      string contributionId = null,
      bool autoEnhance = true)
    {
      return ControlExtensions.Control<Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews>(htmlHelper, new Microsoft.TeamFoundation.Server.WebAccess.Controls.PivotViews(views, contributionId, autoEnhance), htmlAttributes);
    }

    public static MvcHtmlString BeginTag(
      this HtmlHelper htmlHelper,
      string tagName,
      IDictionary<string, object> htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder(tagName);
      tagBuilder.MergeAttributes<string, object>(htmlAttributes);
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.StartTag));
    }

    public static MvcHtmlString EndTag(this HtmlHelper htmlHelper, string tagName) => MvcHtmlString.Create("</" + tagName + ">");

    public static MvcHtmlString CreateFullScreenMenuBar(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext)
    {
      return htmlHelper.MenuBar((IEnumerable<MenuItem>) new MenuItem[1]
      {
        htmlHelper.GetFullScreenMenuItem()
      }, (object) new{ @class = "wit-full-screen-menubar" });
    }

    public static MenuItem GetFullScreenMenuItem(this HtmlHelper htmlHelper) => new MenuItem()
    {
      CommandId = "fullscreen-toggle",
      Icon = "icon-enter-full-screen"
    };

    public static IDisposable Tag(
      this HtmlHelper htmlHelper,
      string tagName,
      IDictionary<string, object> htmlAttributes)
    {
      htmlHelper.ViewContext.Writer.Write(htmlHelper.BeginTag(tagName, htmlAttributes).ToHtmlString());
      return (IDisposable) new ControlExtensions.ActionDisposable((Action) (() => htmlHelper.ViewContext.Writer.Write((object) htmlHelper.EndTag(tagName))));
    }

    public static MvcHtmlString LabelFor(
      this HtmlHelper htmlHelper,
      string labelText,
      string targetId)
    {
      if (string.IsNullOrEmpty(labelText))
        return MvcHtmlString.Empty;
      TagBuilder tagBuilder = new TagBuilder("label");
      if (!string.IsNullOrEmpty(targetId))
        tagBuilder.Attributes.Add("for", targetId);
      int length = labelText.IndexOf('&');
      if (length >= 0 && length + 1 < labelText.Length)
      {
        string str = labelText.Substring(length + 1, 1);
        tagBuilder.Attributes.Add("accesskey", str);
        StringBuilder stringBuilder = new StringBuilder();
        if (length > 0)
          stringBuilder.Append(labelText.Substring(0, length).HtmlEncode());
        stringBuilder.Append("<u>");
        stringBuilder.Append(str.HtmlEncode());
        stringBuilder.Append("</u>");
        if (length + 2 < labelText.Length)
          stringBuilder.Append(labelText.Substring(length + 2));
        tagBuilder.InnerHtml = stringBuilder.ToString();
      }
      else
        tagBuilder.SetInnerText(labelText);
      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    public static void CacheStatefulSplitterSettings(this HtmlHelper htmlHelper, string splitterId = "LeftHubSplitter")
    {
    }

    public static string StatefulSplitterClasses(
      this HtmlHelper htmlHelper,
      string splitterId = "LeftHubSplitter",
      bool includeToggle = true)
    {
      StringBuilder stringBuilder = new StringBuilder("stateful ");
      if (includeToggle)
        stringBuilder.Append("toggle-button-enabled toggle-button-hotkey-enabled ");
      if (!htmlHelper.GetStatefulSplitterSetting(splitterId).Expanded.Value)
        stringBuilder.Append("collapsed");
      string str;
      if (htmlHelper.ViewData["HubSplitterClasses"] is Dictionary<string, string> dictionary && dictionary.TryGetValue(splitterId, out str))
        stringBuilder.Append(" " + str);
      return stringBuilder.ToString();
    }

    public static void AddSplitterClasses(
      this ViewDataDictionary viewData,
      string splitterId,
      string classes)
    {
      if (!(viewData["HubSplitterClasses"] is Dictionary<string, string> dictionary))
      {
        dictionary = new Dictionary<string, string>();
        viewData["HubSplitterClasses"] = (object) dictionary;
      }
      dictionary[splitterId] = classes;
    }

    public static MvcHtmlString StatefulSplitterOptions(
      this HtmlHelper htmlHelper,
      string splitterId = "LeftHubSplitter")
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      JsObject data = (JsObject) null;
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__40.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ControlExtensions.\u003C\u003Eo__40.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SplitterOptions", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__40.\u003C\u003Ep__0.Target((CallSite) ControlExtensions.\u003C\u003Eo__40.\u003C\u003Ep__0, htmlHelper.ViewBag) is Dictionary<string, JsObject> dictionary)
        dictionary.TryGetValue(splitterId, out data);
      if (data == null)
        data = new JsObject();
      data["settingPath"] = (object) htmlHelper.GetStatefulSplitterSettingPath(tfsWebContext, splitterId);
      SplitterSettings statefulSplitterSetting = htmlHelper.GetStatefulSplitterSetting(splitterId);
      int? size = statefulSplitterSetting.Size;
      if (size.HasValue)
      {
        JsObject jsObject = data;
        size = statefulSplitterSetting.Size;
        // ISSUE: variable of a boxed type
        __Boxed<int> local = (ValueType) size.Value;
        jsObject["initialSize"] = (object) local;
      }
      return htmlHelper.JsonIsland((object) data, (object) new
      {
        @class = "options"
      });
    }

    public static MvcHtmlString StatefulSplitterFixedSideStyles(
      this HtmlHelper htmlHelper,
      string splitterId = "LeftHubSplitter")
    {
      return htmlHelper.StatefulSplitterStyles(false, splitterId);
    }

    public static MvcHtmlString StatefulSplitterFillSideStyles(
      this HtmlHelper htmlHelper,
      string splitterId = "LeftHubSplitter")
    {
      return htmlHelper.StatefulSplitterStyles(true, splitterId);
    }

    public static void PreventDefaultMainLandmark(this HtmlHelper htmlHelper) => htmlHelper.ViewContext.TfsWebContext().TfsRequestContext.Items["UseDefaultMainLandmark"] = (object) false;

    public static MvcHtmlString DefaultMainLandmark(this HtmlHelper htmlHelper)
    {
      bool flag = true;
      return htmlHelper.ViewContext.TfsWebContext().TfsRequestContext.Items.TryGetValue<bool>("UseDefaultMainLandmark", out flag) && !flag ? MvcHtmlString.Empty : MvcHtmlString.Create("role=\"main\"");
    }

    public static bool IsCurrentController(this HtmlHelper htmlHelper, string targetController) => ControlExtensions.AreEqualUnderIgnoreCase(htmlHelper.ViewContext.TfsWebContext().NavigationContext.CurrentController, targetController);

    private static MvcHtmlString StatefulSplitterStyles(
      this HtmlHelper htmlHelper,
      bool forFillSide,
      string splitterId = "LeftHubSplitter")
    {
      if (splitterId != "LeftHubSplitter" && splitterId != "RightHubSplitter")
        return MvcHtmlString.Empty;
      htmlHelper.ViewContext.TfsWebContext();
      SplitterSettings statefulSplitterSetting = htmlHelper.GetStatefulSplitterSetting(splitterId);
      bool flag = htmlHelper.CheckIfNoSplit(splitterId);
      int? size;
      int num1;
      if (statefulSplitterSetting.Expanded.Value)
      {
        size = statefulSplitterSetting.Size;
        if (size.HasValue)
        {
          size = statefulSplitterSetting.Size;
          num1 = size.Value < 0 ? 1 : 0;
          goto label_6;
        }
      }
      num1 = 1;
label_6:
      int num2 = flag ? 1 : 0;
      if ((num1 | num2) != 0)
        return MvcHtmlString.Empty;
      if (forFillSide)
      {
        string str = splitterId == "LeftHubSplitter" ? "left" : "right";
        size = statefulSplitterSetting.Size;
        // ISSUE: variable of a boxed type
        __Boxed<int> local = (ValueType) size.Value;
        return MvcHtmlString.Create(string.Format("style=\"{0}: {1}px;\"", (object) str, (object) local));
      }
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      size = statefulSplitterSetting.Size;
      // ISSUE: variable of a boxed type
      __Boxed<int> local1 = (ValueType) size.Value;
      return MvcHtmlString.Create(string.Format((IFormatProvider) invariantCulture, "style=\"width: {0}px;\"", (object) local1));
    }

    private static bool CheckIfNoSplit(this HtmlHelper htmlHelper, string splitterId)
    {
      string str;
      return htmlHelper.ViewData["HubSplitterClasses"] is Dictionary<string, string> dictionary && dictionary.TryGetValue(splitterId, out str) && str.Contains("no-split");
    }

    public static SplitterSettings GetStatefulSplitterSetting(
      this HtmlHelper htmlHelper,
      string splitterId)
    {
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      string splitterSettingPath = htmlHelper.GetStatefulSplitterSettingPath(tfsWebContext, splitterId);
      bool defaultExpandState = true;
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__48.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ControlExtensions.\u003C\u003Eo__48.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SplitterDefaultExpandStates", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__48.\u003C\u003Ep__0.Target((CallSite) ControlExtensions.\u003C\u003Eo__48.\u003C\u003Ep__0, htmlHelper.ViewBag) is Dictionary<string, bool> dictionary)
        dictionary.TryGetValue(splitterId, out defaultExpandState);
      return ControlExtensions.GetStatefulSplitterSetting(tfsWebContext.TfsRequestContext, splitterSettingPath, defaultExpandState);
    }

    public static SplitterSettings GetStatefulSplitterSetting(
      IVssRequestContext requestContext,
      string settingPath,
      bool defaultExpandState)
    {
      SplitterSettings statefulSplitterSetting = requestContext.GetService<ISettingsService>().GetValue<SplitterSettings>(requestContext, SettingsUserScope.User, settingPath, (SplitterSettings) null) ?? new SplitterSettings();
      if (!statefulSplitterSetting.Expanded.HasValue)
        statefulSplitterSetting.Expanded = new bool?(defaultExpandState);
      return statefulSplitterSetting;
    }

    private static string GetStatefulSplitterSettingPath(
      this HtmlHelper htmlHelper,
      TfsWebContext tfsWebContext,
      string splitterId)
    {
      ArgumentUtility.CheckForNull<TfsWebContext>(tfsWebContext, nameof (tfsWebContext));
      ArgumentUtility.CheckStringForNullOrEmpty(splitterId, nameof (splitterId));
      string str1 = (string) null;
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__50.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ControlExtensions.\u003C\u003Eo__50.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SplitterSettingsPrefix", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__50.\u003C\u003Ep__0.Target((CallSite) ControlExtensions.\u003C\u003Eo__50.\u003C\u003Ep__0, htmlHelper.ViewBag) is Dictionary<string, string> dictionary)
        dictionary.TryGetValue(splitterId, out str1);
      string str2 = splitterId;
      if (!string.IsNullOrEmpty(str1))
        str2 = str1 + "/" + str2;
      return string.Format("{0}/{1}/{2}", (object) "Web/UIState", (object) tfsWebContext.NavigationContext.CurrentController, (object) str2);
    }

    public static void ConfigureLeftHubSplitter(
      this ControllerBase controller,
      string collapsedLabel,
      string settingsPrefix = null,
      bool collapsedByDefault = false,
      bool hidePane = false,
      string toggleButtonCollapsedTooltip = null,
      string toggleButtonExpandedTooltip = null)
    {
      controller.ConfigureStatefulSplitter("LeftHubSplitter", collapsedLabel, settingsPrefix, collapsedByDefault, hidePane, toggleButtonCollapsedTooltip, toggleButtonExpandedTooltip);
    }

    public static void ConfigureRightHubSplitter(
      this ControllerBase controller,
      string collapsedLabel = "",
      string settingsPrefix = null,
      bool collapsedByDefault = false,
      bool hidePane = false,
      string toggleButtonCollapsedTooltip = null,
      string toggleButtonExpandedTooltip = null)
    {
      controller.ConfigureStatefulSplitter("RightHubSplitter", collapsedLabel, settingsPrefix, collapsedByDefault, hidePane, toggleButtonCollapsedTooltip, toggleButtonExpandedTooltip);
    }

    public static void ConfigureStatefulSplitter(
      this ControllerBase controller,
      string splitterId,
      string collapsedLabel,
      string settingsPrefix = null,
      bool collapsedByDefault = false,
      bool hidePane = false,
      string toggleButtonCollapsedTooltip = null,
      string toggleButtonExpandedTooltip = null)
    {
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SplitterDefaultExpandStates", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!(ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__0.Target((CallSite) ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__0, controller.ViewBag) is Dictionary<string, bool> dictionary1))
      {
        dictionary1 = new Dictionary<string, bool>();
        // ISSUE: reference to a compiler-generated field
        if (ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, Dictionary<string, bool>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SplitterDefaultExpandStates", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__1.Target((CallSite) ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__1, controller.ViewBag, dictionary1);
      }
      dictionary1[splitterId] = !collapsedByDefault;
      if (hidePane)
        controller.ViewData.AddSplitterClasses(splitterId, "no-split");
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SplitterOptions", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!(ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__2.Target((CallSite) ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__2, controller.ViewBag) is Dictionary<string, JsObject> dictionary2))
      {
        dictionary2 = new Dictionary<string, JsObject>();
        // ISSUE: reference to a compiler-generated field
        if (ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, Dictionary<string, JsObject>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SplitterOptions", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__3.Target((CallSite) ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__3, controller.ViewBag, dictionary2);
      }
      Dictionary<string, JsObject> dictionary3 = dictionary2;
      string key = splitterId;
      JsObject jsObject = new JsObject();
      jsObject.Add(nameof (collapsedLabel), (object) collapsedLabel);
      dictionary3[key] = jsObject;
      if (!string.IsNullOrEmpty(toggleButtonCollapsedTooltip))
        dictionary2[splitterId].Add(nameof (toggleButtonCollapsedTooltip), (object) toggleButtonCollapsedTooltip);
      if (!string.IsNullOrEmpty(toggleButtonExpandedTooltip))
        dictionary2[splitterId].Add(nameof (toggleButtonExpandedTooltip), (object) toggleButtonExpandedTooltip);
      if (string.IsNullOrEmpty(settingsPrefix))
        return;
      // ISSUE: reference to a compiler-generated field
      if (ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SplitterSettingsPrefix", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!(ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__4.Target((CallSite) ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__4, controller.ViewBag) is Dictionary<string, string> dictionary4))
      {
        dictionary4 = new Dictionary<string, string>();
        // ISSUE: reference to a compiler-generated field
        if (ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, Dictionary<string, string>, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "SplitterSettingsPrefix", typeof (ControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj = ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__5.Target((CallSite) ControlExtensions.\u003C\u003Eo__53.\u003C\u003Ep__5, controller.ViewBag, dictionary4);
      }
      dictionary4[splitterId] = settingsPrefix;
    }

    public static void ConfigureSearchBox(this ControllerBase controller, string adapterCssClass) => controller.ViewData["SearchBoxCssClass"] = (object) adapterCssClass;

    public static MvcHtmlString SearchBox(this HtmlHelper htmlHelper, string cssClass)
    {
      string cssClass1 = htmlHelper.ViewData["SearchBoxCssClass"] as string;
      TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      if (string.IsNullOrEmpty(cssClass1))
        cssClass1 = tfsWebContext.NavigationContext.TopMostLevel != NavigationContextLevels.Application && tfsWebContext.NavigationContext.TopMostLevel != NavigationContextLevels.Collection || !ControlExtensions.IsCodeSearchEnabled(tfsWebContext) ? "search-adapter-work-items" : "search-adapter-search";
      TagBuilder tagBuilder = new TagBuilder("div").AddClass("search-box").AddClass(cssClass).AddClass(cssClass1);
      string currentController = htmlHelper.ViewContext.TfsWebContext().NavigationContext.CurrentController;
      if ((ControlExtensions.IsCodeSearchEnabled(tfsWebContext) || ControlExtensions.IsWorkItemSearchEnabled(tfsWebContext.TfsRequestContext)) && !ControlExtensions.AreEqualUnderIgnoreCase(currentController, "search") || ControlExtensions.AreEqualUnderIgnoreCase(currentController, "Git") || ControlExtensions.AreEqualUnderIgnoreCase(currentController, "versionControl"))
        tagBuilder.AddCssClass("hidden");
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString SearchPageSearchBox(this HtmlHelper htmlHelper) => htmlHelper.SearchBox("header-search");

    public static bool IsContributionAvailable(this HtmlHelper htmlHelper, string contributionId) => ExtensionContributionUtility.IsContributionAvailable(htmlHelper.ViewContext.TfsWebContext().TfsRequestContext, contributionId);

    private static bool AreEqualUnderIgnoreCase(string left, string right) => string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

    private static bool IsWorkItemSearchEnabled(IVssRequestContext requestContext)
    {
      bool flag1 = requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? ExtensionContributionUtility.IsContributionAvailable(requestContext, "ms.vss-workitem-searchonprem.workitem-entity-type") : ExtensionContributionUtility.IsContributionAvailable(requestContext, "ms.vss-workitem-search.workitem-entity-type");
      bool flag2 = requestContext.IsFeatureEnabled("WebAccess.Search.WorkItem.Feature.Toggle");
      if (requestContext.IsFeatureEnabled("WebAccess.Search.WorkItem"))
        return true;
      if (!flag1)
        return false;
      return flag2 && ControlExtensions.IsWorkItemSearchToggleOn(requestContext) || !flag2;
    }

    private static bool IsWorkItemSearchToggleOn(IVssRequestContext requestContext)
    {
      IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
      return !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? service.IsFeatureEnabled(requestContext, "ms.vss-workitem-search.enable-workitem-search") : service.IsFeatureEnabled(requestContext, "ms.vss-workitem-searchonprem.enable-workitem-search");
    }

    private static bool IsCodeSearchEnabled(TfsWebContext tfsWebContext)
    {
      if (tfsWebContext.TfsRequestContext.IsStakeholder())
        return false;
      return tfsWebContext.IsFeatureAvailable("WebAccess.SearchShell") || ExtensionContributionUtility.IsContributionAvailable(tfsWebContext.TfsRequestContext, "ms.vss-code-search.code-entity-type");
    }

    public static void RenderGrid(this HtmlHelper htmlHelper) => htmlHelper.RenderPartial("Root", "ResponsiveGrid", (object) null);

    public static void RenderInGrid(
      this HtmlHelper htmlHelper,
      int sectionNumber,
      int rows,
      int columns,
      bool adjustHeight,
      string area,
      string viewName,
      object model)
    {
      ResponsiveGridCellModel viewModel = new ResponsiveGridCellModel()
      {
        SectionNumber = sectionNumber,
        Rows = rows,
        Columns = columns,
        AdjustHeight = adjustHeight,
        Area = area,
        ViewName = viewName,
        Model = model
      };
      htmlHelper.RenderPartial("Root", "ResponsiveGridCell", (object) viewModel);
    }

    public static void RenderInGrid(
      this HtmlHelper htmlHelper,
      int sectionNumber,
      int rows,
      int columns,
      bool adjustHeight,
      string controlString)
    {
      ResponsiveGridCellModel viewModel = new ResponsiveGridCellModel()
      {
        SectionNumber = sectionNumber,
        Rows = rows,
        Columns = columns,
        AdjustHeight = adjustHeight,
        ControlString = controlString
      };
      htmlHelper.RenderPartial("Root", "ResponsiveGridCell", (object) viewModel);
    }

    public static void DismissableNotifications(
      this HtmlHelper htmlHelper,
      IEnumerable<NotificationMessageModel> messages,
      string className = null,
      bool closeable = true,
      bool showIcon = false,
      bool clientDismissable = false)
    {
      htmlHelper.DismissableNotifications(messages, className, closeable, showIcon, clientDismissable, false);
    }

    public static void DismissableNotifications(
      this HtmlHelper htmlHelper,
      IEnumerable<NotificationMessageModel> messages,
      string className,
      bool closeable,
      bool showIcon,
      bool clientDismissable,
      bool hidden)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (DismissableNotifications));
      try
      {
        if (messages == null)
          return;
        TextWriter writer = htmlHelper.ViewContext.Writer;
        string cssClass = "tfs-host-notifications " + (className ?? string.Empty);
        foreach (NotificationMessageModel message in messages)
        {
          TagBuilder tagBuilder = new TagBuilder("div").AddClass(cssClass);
          ControlExtensions.NotificationMessageOptions data = new ControlExtensions.NotificationMessageOptions()
          {
            Message = message,
            Closeable = closeable,
            ShowIcon = showIcon,
            ClientDismissable = clientDismissable,
            Hidden = hidden
          };
          tagBuilder.InnerHtml = htmlHelper.DataContractJsonIsland<ControlExtensions.NotificationMessageOptions>(data, (object) new
          {
            @class = "options"
          }).ToHtmlString();
          writer.Write((object) tagBuilder);
        }
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (DismissableNotifications));
      }
    }

    private class ActionDisposable : IDisposable
    {
      private Action m_action;
      private bool m_disposed;

      public ActionDisposable(Action action) => this.m_action = action;

      public void Dispose()
      {
        if (this.m_disposed)
          return;
        if (this.m_action != null)
          this.m_action();
        this.m_disposed = true;
      }
    }

    [DataContract]
    private class NotificationMessageOptions
    {
      [DataMember(Name = "message")]
      public NotificationMessageModel Message { get; set; }

      [DataMember(Name = "closeable")]
      public bool Closeable { get; set; }

      [DataMember(Name = "showIcon")]
      public bool ShowIcon { get; set; }

      [DataMember(Name = "clientDismissable")]
      public bool ClientDismissable { get; set; }

      [DataMember(Name = "hidden")]
      public bool Hidden { get; set; }
    }
  }
}
