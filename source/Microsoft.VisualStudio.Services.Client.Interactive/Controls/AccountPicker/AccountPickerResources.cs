// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.AccountPicker.AccountPickerResources
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Client.Controls.AccountPicker
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class AccountPickerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal AccountPickerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (AccountPickerResources.resourceMan == null)
          AccountPickerResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.Client.Controls.AccountPicker.AccountPickerResources", typeof (AccountPickerResources).Assembly);
        return AccountPickerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => AccountPickerResources.resourceCulture;
      set => AccountPickerResources.resourceCulture = value;
    }

    internal static string AddNewAccount => AccountPickerResources.ResourceManager.GetString(nameof (AddNewAccount), AccountPickerResources.resourceCulture);

    internal static string CancelButton => AccountPickerResources.ResourceManager.GetString(nameof (CancelButton), AccountPickerResources.resourceCulture);

    internal static string DialogHeader => AccountPickerResources.ResourceManager.GetString(nameof (DialogHeader), AccountPickerResources.resourceCulture);

    internal static string DialogTitle => AccountPickerResources.ResourceManager.GetString(nameof (DialogTitle), AccountPickerResources.resourceCulture);

    internal static string OkButton => AccountPickerResources.ResourceManager.GetString(nameof (OkButton), AccountPickerResources.resourceCulture);
  }
}
