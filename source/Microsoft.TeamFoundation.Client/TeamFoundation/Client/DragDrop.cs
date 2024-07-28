// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.DragDrop
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Windows;
using System.Windows.Input;

namespace Microsoft.TeamFoundation.Client
{
  public class DragDrop
  {
    protected static DragDrop m_instance;
    public static readonly DependencyProperty DragSourceFormatProperty = DependencyProperty.RegisterAttached("DragSourceFormat", typeof (string), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(DragDrop.DragSourceFormatChanged)));
    public static readonly DependencyProperty DropTargetFormatsProperty = DependencyProperty.RegisterAttached("DropTargetFormats", typeof (string), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(DragDrop.DropTargetFormatsChanged)));
    public static readonly DependencyProperty DragDropTemplateProperty = DependencyProperty.RegisterAttached("DragDropTemplate", typeof (DataTemplate), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DragSourceMoveCommandProperty = DependencyProperty.RegisterAttached("DragSourceMoveCommand", typeof (ICommand), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DropTargetMoveCommandProperty = DependencyProperty.RegisterAttached("DropTargetMoveCommand", typeof (ICommand), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    private static readonly DependencyPropertyKey DropSourceFormatPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DropSourceFormat", typeof (string), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty DropSourceFormatProperty = DragDrop.DropSourceFormatPropertyKey.DependencyProperty;
    public static readonly DependencyProperty DropTargetTemplateProperty = DependencyProperty.RegisterAttached("DropTargetTemplate", typeof (DataTemplate), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((object) null, new PropertyChangedCallback(DragDrop.DropTargetTemplateChanged)));
    public static readonly DependencyProperty ContinueDragEventHandlingProperty = DependencyProperty.RegisterAttached("ContinueDragEventHandling", typeof (bool), typeof (DragDrop), (PropertyMetadata) new UIPropertyMetadata((object) false));

    public static string GetDragSourceFormat(DependencyObject obj) => (string) obj.GetValue(DragDrop.DragSourceFormatProperty);

    public static void SetDragSourceFormat(DependencyObject obj, string value) => obj.SetValue(DragDrop.DragSourceFormatProperty, (object) value);

    public static string GetDropTargetFormats(DependencyObject obj) => (string) obj.GetValue(DragDrop.DropTargetFormatsProperty);

    public static void SetDropTargetFormats(DependencyObject obj, string value) => obj.SetValue(DragDrop.DropTargetFormatsProperty, (object) value);

    public static DataTemplate GetDragDropTemplate(DependencyObject obj) => (DataTemplate) obj.GetValue(DragDrop.DragDropTemplateProperty);

    public static void SetDragDropTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(DragDrop.DragDropTemplateProperty, (object) value);

    public static ICommand GetDragSourceMoveCommand(DependencyObject obj) => (ICommand) obj.GetValue(DragDrop.DragSourceMoveCommandProperty);

    public static void SetDragSourceMoveCommand(DependencyObject obj, ICommand value) => obj.SetValue(DragDrop.DragSourceMoveCommandProperty, (object) value);

    public static ICommand GetDropTargetMoveCommand(DependencyObject obj) => (ICommand) obj.GetValue(DragDrop.DropTargetMoveCommandProperty);

    public static void SetDropTargetMoveCommand(DependencyObject obj, ICommand value) => obj.SetValue(DragDrop.DropTargetMoveCommandProperty, (object) value);

    public static string GetDropSourceFormat(DependencyObject obj) => (string) obj.GetValue(DragDrop.DropSourceFormatProperty);

    protected static void SetDropSourceFormat(DependencyObject obj, string value) => obj.SetValue(DragDrop.DropSourceFormatPropertyKey, (object) value);

    public static DataTemplate GetDropTargetTemplate(DependencyObject obj) => (DataTemplate) obj.GetValue(DragDrop.DropTargetTemplateProperty);

    public static void SetDropTargetTemplate(DependencyObject obj, DataTemplate value) => obj.SetValue(DragDrop.DropTargetTemplateProperty, (object) value);

    public static bool GetContinueDragEventHandling(DependencyObject obj) => (bool) obj.GetValue(DragDrop.ContinueDragEventHandlingProperty);

    public static void SetContinueDragEventHandling(DependencyObject obj, bool value) => obj.SetValue(DragDrop.ContinueDragEventHandlingProperty, (object) value);

    private static void DragSourceFormatChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      if (DragDrop.m_instance == null)
        return;
      DragDrop.m_instance.DragSourceFormatChangedInternal(obj, e);
    }

    protected virtual void DragSourceFormatChangedInternal(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
    }

    private static void DropTargetFormatsChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      if (DragDrop.m_instance == null)
        return;
      DragDrop.m_instance.DropTargetFormatsChangedInternal(obj, e);
    }

    protected virtual void DropTargetFormatsChangedInternal(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
    }

    private static void DropTargetTemplateChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      if (DragDrop.m_instance == null)
        return;
      DragDrop.m_instance.DropTargetTemplateChangedInternal(obj, e);
    }

    protected virtual void DropTargetTemplateChangedInternal(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
    }
  }
}
