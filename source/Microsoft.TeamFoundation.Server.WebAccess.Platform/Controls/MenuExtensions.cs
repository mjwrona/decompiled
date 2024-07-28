// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.MenuExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public static class MenuExtensions
  {
    public static TControl CssClass<TControl>(this TControl control, string value) where TControl : ControlBase
    {
      control.CssClass = value;
      return control;
    }

    public static MenuItem AddMenuItem(this MenuBase menuBase)
    {
      MenuItem menuItem = new MenuItem();
      menuBase.ChildItems.Add(menuItem);
      return menuItem;
    }

    public static MenuItem AddSeparator(this MenuBase menuBase) => menuBase.AddMenuItem().Separator();

    public static MenuItem AddLabel(this MenuBase menuBase) => menuBase.AddMenuItem().Label();

    public static TMenuBase Scope<TMenuBase>(this TMenuBase menuBase, System.Action<TMenuBase> addAction) where TMenuBase : MenuBase => menuBase.Scope<TMenuBase>(true, addAction);

    public static TMenuBase Scope<TMenuBase>(
      this TMenuBase menuBase,
      bool condition,
      System.Action<TMenuBase> addAction)
      where TMenuBase : MenuBase
    {
      addAction(menuBase);
      return menuBase;
    }

    public static MenuItem Separator(this MenuItem menuItem)
    {
      menuItem.Separator = true;
      return menuItem;
    }

    public static MenuItem Label(this MenuItem menuItem)
    {
      menuItem.Separator();
      menuItem.IsLabel = true;
      return menuItem;
    }

    public static MenuItem ItemId(this MenuItem menuItem, string value)
    {
      menuItem.CommandId = value;
      menuItem.IdIsAction = new bool?(false);
      return menuItem;
    }

    public static MenuItem AriaLabel(this MenuItem menuItem, string value)
    {
      menuItem.AriaLabel = value;
      return menuItem;
    }

    public static MenuItem CommandId(this MenuItem menuItem, string value)
    {
      menuItem.CommandId = value;
      return menuItem;
    }

    public static MenuItem Text(this MenuItem menuItem, string text)
    {
      menuItem.Text = text;
      return menuItem;
    }

    public static MenuItem TextClass(this MenuItem menuItem, string textClass)
    {
      menuItem.TextClass = textClass;
      return menuItem;
    }

    public static MenuItem Html(this MenuItem menuItem, string html)
    {
      menuItem.Text = html;
      menuItem.Encoded = true;
      return menuItem;
    }

    public static MenuItem Title(this MenuItem menuItem, string value)
    {
      menuItem.Title = value;
      return menuItem;
    }

    public static MenuItem ShowText(this MenuItem menuItem) => menuItem.ShowText(true);

    public static MenuItem ShowText(this MenuItem menuItem, bool value)
    {
      menuItem.ShowText = value;
      return menuItem;
    }

    public static MenuItem Icon(this MenuItem menuItem, string icon)
    {
      menuItem.Icon = icon;
      return menuItem;
    }

    public static MenuItem ShowIcon(this MenuItem menuItem, bool value)
    {
      menuItem.ShowIcon = new bool?(value);
      return menuItem;
    }

    public static MenuItem HideDrop(this MenuItem menuItem) => menuItem.HideDrop(true);

    public static MenuItem HideDrop(this MenuItem menuItem, bool value)
    {
      menuItem.HideDrop = value;
      return menuItem;
    }

    public static MenuItem Hidden(this MenuItem menuItem, bool value)
    {
      menuItem.Hidden = value;
      return menuItem;
    }

    public static MenuItem Disabled(this MenuItem menuItem) => menuItem.Disabled(true);

    public static MenuItem Disabled(this MenuItem menuItem, bool value)
    {
      menuItem.Disabled = value;
      return menuItem;
    }

    public static MenuItem Selected(this MenuItem menuItem) => menuItem.Selected(true);

    public static MenuItem Selected(this MenuItem menuItem, bool value)
    {
      menuItem.Selected = value;
      return menuItem;
    }

    public static MenuItem IdIsAction(this MenuItem menuItem, bool value)
    {
      menuItem.IdIsAction = new bool?(value);
      return menuItem;
    }

    public static MenuItem ActionLink(this MenuItem menuItem, string value)
    {
      menuItem.Action = "navigate";
      menuItem.ActionArguments = (object) value;
      return menuItem;
    }

    public static MenuItem Action(
      this MenuItem menuItem,
      string actionName,
      object actionArguments)
    {
      menuItem.Action = actionName;
      menuItem.ActionArguments = actionArguments;
      return menuItem;
    }

    public static MenuItem ActionArgs(this MenuItem menuItem, object actionArguments)
    {
      menuItem.ActionArguments = actionArguments;
      return menuItem;
    }

    public static MenuItem AddChildMenuOptions(this MenuItem menuItem, object childOptions)
    {
      menuItem.ChildMenuOptions = childOptions;
      return menuItem;
    }

    public static MenuItem AddExtraOptions(this MenuItem menuItem, object extraOptions)
    {
      menuItem.ExtraOptions = extraOptions;
      return menuItem;
    }

    public static TMenuOwner ShowIcon<TMenuOwner>(this TMenuOwner menuOwner) where TMenuOwner : MenuOwner => menuOwner.ShowIcon<TMenuOwner>(true);

    public static TMenuOwner ShowIcon<TMenuOwner>(this TMenuOwner menuOwner, bool value) where TMenuOwner : MenuOwner
    {
      menuOwner.ShowIcons = value;
      return menuOwner;
    }

    public static TMenuOwner PopupAlign<TMenuOwner>(this TMenuOwner menuOwner, string value) where TMenuOwner : MenuOwner
    {
      menuOwner.PopupAlign = value;
      return menuOwner;
    }
  }
}
