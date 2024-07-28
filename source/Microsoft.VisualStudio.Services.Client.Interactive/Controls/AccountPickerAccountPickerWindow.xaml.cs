// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.AccountPicker.AccountPickerWindow
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.AccountManagement;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.VisualStudio.Services.Client.Controls.AccountPicker
{
  public partial class AccountPickerWindow : Window, IComponentConnector
  {
    internal TextBlock Header;
    internal ComboBox AccountComboBox;
    internal Button OkButton;
    internal Button CancelButton;
    private bool _contentLoaded;

    public List<IAccountPickerItem> Accounts { get; set; }

    public CachedAccount SelectedAccount => (CachedAccount) this.AccountComboBox.SelectedItem;

    internal AccountPickerWindow(IEnumerable<IAccountCacheItem> cachedTokens)
    {
      this.Accounts = new List<IAccountPickerItem>();
      foreach (IAccountCacheItem token in (IEnumerable<IAccountCacheItem>) cachedTokens.OrderBy<IAccountCacheItem, string>((Func<IAccountCacheItem, string>) (x => x.Username)))
        this.Accounts.Add((IAccountPickerItem) new CachedAccount(token));
      this.InitializeComponent();
      this.ApplyResources();
      this.DataContext = (object) this;
      if (!this.Accounts.Any<IAccountPickerItem>())
        return;
      this.AccountComboBox.SelectedIndex = 0;
    }

    private void ApplyResources()
    {
      this.Title = AccountPickerResources.DialogTitle;
      this.Header.Text = AccountPickerResources.DialogHeader;
      this.OkButton.Content = (object) AccountPickerResources.OkButton;
      this.CancelButton.Content = (object) AccountPickerResources.CancelButton;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e) => this.DialogResult = new bool?(true);

    private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.VisualStudio.Services.Client.Interactive;V19.0.0.0;component/controls/accountpicker/accountpickerwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Header = (TextBlock) target;
          break;
        case 2:
          this.AccountComboBox = (ComboBox) target;
          break;
        case 3:
          this.OkButton = (Button) target;
          this.OkButton.Click += new RoutedEventHandler(this.OkButton_Click);
          break;
        case 4:
          this.CancelButton = (Button) target;
          this.CancelButton.Click += new RoutedEventHandler(this.CancelButton_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
