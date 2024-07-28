// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuItem
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public class MenuItem : MenuBase
  {
    public const string ClassCore = "menu-item";
    public const string ClassText = "text";
    public const string ClassHtml = "html";
    public const string ClassIcon = "icon";
    public const string MenuItemClassIcon = "menu-item-icon";
    public const string ClassDrop = "drop";
    public const string ClassDisabled = "disabled";
    public const string ClassSelected = "selected";
    public const string ClassSeparator = "separator";
    public const string ClassHidden = "invisible";

    public MenuItem()
      : base("li")
    {
      this.ShowText = true;
      this.CoreClass = "menu-item";
    }

    public MenuItem(string text)
      : this()
    {
      this.Text = text;
    }

    public MenuItem(string text, string action, string actionArgs)
      : this()
    {
      this.Text = text;
      this.Action = action;
      this.ActionArguments = (object) actionArgs;
    }

    public MenuItem(string text, string commandId)
      : this()
    {
      this.Text = text;
      this.CommandId = commandId;
    }

    public string CommandId { get; set; }

    public string Text { get; set; }

    public string TextClass { get; set; }

    public string Title { get; set; }

    public string Icon { get; set; }

    public string Href { get; set; }

    public bool Encoded { get; set; }

    public bool Hidden { get; set; }

    public bool Disabled { get; set; }

    public bool Selected { get; set; }

    public string SelectedCssClass { get; set; }

    public bool Separator { get; set; }

    public bool IsLabel { get; set; }

    public bool ShowText { get; set; }

    public bool HideDrop { get; set; }

    public string Action { get; set; }

    public string AriaLabel { get; set; }

    public object ActionArguments { get; set; }

    public bool? ShowIcon { get; set; }

    public bool? IdIsAction { get; set; }

    public bool? ClickOpensSubMenu { get; set; }

    public object ChildMenuOptions { get; set; }

    public object ExtraOptions { get; set; }

    public MenuItemPinningOptions PinningOptions { get; set; }

    public Menu ChildMenu { get; private set; }

    protected override MenuItemCollection FillChildItems()
    {
      this.ChildMenu = new Menu((MenuBase) this);
      return this.ChildMenu.ChildItems;
    }

    internal JsObject ToJson()
    {
      JsObject json = new JsObject();
      if (!string.IsNullOrEmpty(this.CssClass))
        json["cssClass"] = (object) this.CssClass;
      if (!string.IsNullOrEmpty(this.CommandId))
        json["id"] = (object) this.CommandId;
      if (!string.IsNullOrEmpty(this.Text))
        json["text"] = (object) this.Text;
      if (!string.IsNullOrEmpty(this.TextClass))
        json["textClass"] = (object) this.TextClass;
      if (!string.IsNullOrEmpty(this.Title))
        json["title"] = (object) this.Title;
      if (this.Hidden)
        json["hidden"] = (object) true;
      if (this.ShowText)
        json["showText"] = (object) true;
      if (this.Encoded)
        json["encoded"] = (object) true;
      bool? nullable;
      if (this.IdIsAction.HasValue)
      {
        JsObject jsObject = json;
        nullable = this.IdIsAction;
        // ISSUE: variable of a boxed type
        __Boxed<bool> local = (ValueType) nullable.Value;
        jsObject["idIsAction"] = (object) local;
      }
      nullable = this.ClickOpensSubMenu;
      if (nullable.HasValue)
      {
        JsObject jsObject = json;
        nullable = this.ClickOpensSubMenu;
        // ISSUE: variable of a boxed type
        __Boxed<bool> local = (ValueType) nullable.Value;
        jsObject["clickOpensSubMenu"] = (object) local;
      }
      if (!string.IsNullOrEmpty(this.Href))
        json["href"] = (object) this.Href;
      if (!string.IsNullOrEmpty(this.AriaLabel))
        json["ariaLabel"] = (object) this.AriaLabel;
      if (this.Separator || this.IsLabel)
      {
        json["separator"] = (object) true;
        if (this.IsLabel)
          json["isLabel"] = (object) true;
      }
      else
      {
        if (!string.IsNullOrEmpty(this.Icon))
          json["icon"] = (object) this.Icon;
        nullable = this.ShowIcon;
        if (nullable.HasValue)
        {
          JsObject jsObject = json;
          nullable = this.ShowIcon;
          // ISSUE: variable of a boxed type
          __Boxed<bool> local = (ValueType) nullable.Value;
          jsObject["showIcon"] = (object) local;
        }
        if (this.HideDrop)
          json["hideDrop"] = (object) true;
        if (this.Disabled)
          json["disabled"] = (object) true;
        if (this.Selected)
          json["selected"] = (object) true;
        if (this.PinningOptions != null)
          json["pinningOptions"] = (object) new
          {
            isPinned = this.PinningOptions.IsPinned,
            hidePin = this.PinningOptions.HidePin
          };
        if (!string.IsNullOrEmpty(this.Action))
          json["action"] = (object) this.Action;
        if (this.ActionArguments != null)
          json["arguments"] = this.ActionArguments;
        if (this.ExtraOptions != null)
          json["extraOptions"] = this.ExtraOptions;
        if (this.HasChildren)
        {
          json["childItems"] = (object) this.ChildItems.Select<MenuItem, JsObject>((Func<MenuItem, JsObject>) (item => item.ToJson()));
          if (this.ChildMenuOptions != null)
            json["childOptions"] = this.ChildMenuOptions;
        }
      }
      return json;
    }

    public override MvcHtmlString ToHtml(
      HtmlHelper htmlHelper,
      IDictionary<string, object> htmlAttributes)
    {
      if (string.IsNullOrEmpty(this.Href))
        return base.ToHtml(htmlHelper, htmlAttributes);
      TagBuilder tagBuilder = new TagBuilder(this.TagName);
      tagBuilder.AddCssClass("menu-item-container");
      TagBuilder tag = new TagBuilder("a");
      this.PopulateRootTag(tag, htmlAttributes);
      tag.MergeAttribute("href", this.Href);
      if (!string.IsNullOrEmpty(this.AriaLabel))
        tag.MergeAttribute("aria-label", this.AriaLabel);
      StringBuilder contents = new StringBuilder();
      this.WriteHtmlContents(htmlHelper, tag, contents);
      tag.InnerHtml = contents.ToString();
      tagBuilder.InnerHtml = tag.ToString();
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    protected override void WriteHtmlContents(
      HtmlHelper htmlHelper,
      TagBuilder tag,
      StringBuilder contents)
    {
      if (this.Hidden)
        tag.AddCssClass("invisible");
      if (!string.IsNullOrEmpty(this.CommandId))
        tag.MergeAttribute("command", this.CommandId);
      if (this.Separator)
      {
        tag.AddCssClass("menu-item-separator");
        tag.MergeAttribute("role", "separator");
        TagBuilder tagBuilder = new TagBuilder("div");
        if (this.ShowText && !string.IsNullOrEmpty(this.Text))
        {
          tagBuilder.AddClass("text-separator");
          if (this.Encoded)
            tagBuilder.InnerHtml = this.Text;
          else
            tagBuilder.SetInnerText(this.Text);
        }
        else
          tagBuilder.AddClass("separator");
        contents.AppendLine(tagBuilder.ToString());
      }
      else
      {
        if (this.Disabled)
          tag.AddCssClass("disabled");
        if (this.Selected)
          tag.AddCssClass(!string.IsNullOrEmpty(this.SelectedCssClass) ? this.SelectedCssClass : "selected");
        bool flag1 = !string.IsNullOrEmpty(this.Icon);
        bool? showIcon = this.ShowIcon;
        if (showIcon.HasValue)
        {
          showIcon = this.ShowIcon;
          flag1 = showIcon.Value;
        }
        else if (this.Owner != null)
          flag1 = this.Owner.ShowIcons;
        if (flag1)
        {
          TagBuilder tagBuilder = new TagBuilder("div");
          if (this.Parent.IsBowtieMenu)
            tagBuilder.AddCssClass("menu-item-icon");
          else
            tagBuilder.AddCssClass("icon");
          if (!string.IsNullOrEmpty(this.Icon))
          {
            tagBuilder.AddCssClass(this.Icon);
            tag.Data("icon", (object) this.Icon);
          }
          if (!string.IsNullOrEmpty(this.Title))
            tagBuilder.MergeAttribute("title", this.Title);
          contents.Append(tagBuilder.ToString());
        }
        if (this.ShowText)
        {
          TagBuilder tagBuilder = new TagBuilder("span");
          tagBuilder.AddCssClass(this.Encoded ? "html" : "text");
          tagBuilder.MergeAttribute("title", this.Title);
          if (!string.IsNullOrEmpty(this.TextClass))
            tagBuilder.AddCssClass(this.TextClass);
          if (this.Encoded)
            tagBuilder.InnerHtml = this.Text;
          else
            tagBuilder.SetInnerText(this.Text);
          contents.Append(tagBuilder.ToString());
        }
        if (!this.ShowText & flag1)
        {
          tag.AddCssClass("icon-only");
          if (!string.IsNullOrEmpty(this.Title))
            tag.MergeAttribute("title", this.Title);
        }
        if (this.PinningOptions != null)
        {
          if (!this.PinningOptions.HidePin)
            tag.AddClass("pin-visible");
          TagBuilder tagBuilder = new TagBuilder("i");
          tagBuilder.AddClass("pin").AddClass("bowtie-icon");
          tagBuilder.MergeAttribute("role", "button");
          tagBuilder.MergeAttribute("aria-label", PlatformResources.MenuItemUnpinButtonLabel);
          if (this.PinningOptions.IsPinned)
            tagBuilder.AddClass("bowtie-pin-pinned");
          contents.Append(tagBuilder.ToString());
        }
        if (this.HasChildren)
        {
          if (!this.HideDrop)
          {
            TagBuilder tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("drop");
            contents.Append(tagBuilder.ToString());
            tag.AddCssClass("drop-visible");
          }
          tag.MergeAttribute("aria-haspopup", "true");
          tag.MergeAttribute("aria-expanded", "false");
          bool flag2 = true;
          if (this.Parent != null)
            flag2 = this.Parent.RenderGrandchildren;
          if (flag2)
            contents.AppendLine(this.ChildMenu.ToHtml(htmlHelper).ToHtmlString());
        }
        if (!string.IsNullOrEmpty(this.Href) || string.IsNullOrEmpty(this.AriaLabel))
          return;
        tag.MergeAttribute("aria-label", this.AriaLabel);
      }
    }
  }
}
