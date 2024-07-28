// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions.CodeEditor.CodeEditorResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AFF996E-AF68-479F-B1B4-F4F26C62129C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions.CodeEditor
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class CodeEditorResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CodeEditorResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (CodeEditorResources.resourceMan == null)
          CodeEditorResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions.CodeEditor.CodeEditorResources", typeof (CodeEditorResources).Assembly);
        return CodeEditorResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => CodeEditorResources.resourceCulture;
      set => CodeEditorResources.resourceCulture = value;
    }

    internal static string ContextMenuToggleCodeFolding => CodeEditorResources.ResourceManager.GetString(nameof (ContextMenuToggleCodeFolding), CodeEditorResources.resourceCulture);

    internal static string ContextMenuToggleMinimap => CodeEditorResources.ResourceManager.GetString(nameof (ContextMenuToggleMinimap), CodeEditorResources.resourceCulture);

    internal static string ContextMenuToggleWhiteSpace => CodeEditorResources.ResourceManager.GetString(nameof (ContextMenuToggleWhiteSpace), CodeEditorResources.resourceCulture);

    internal static string ContextMenuToggleWordWrap => CodeEditorResources.ResourceManager.GetString(nameof (ContextMenuToggleWordWrap), CodeEditorResources.resourceCulture);

    internal static string CopiedLineLinkTitle => CodeEditorResources.ResourceManager.GetString(nameof (CopiedLineLinkTitle), CodeEditorResources.resourceCulture);

    internal static string CopyLineLinkTitle => CodeEditorResources.ResourceManager.GetString(nameof (CopyLineLinkTitle), CodeEditorResources.resourceCulture);

    internal static string FailedToGetContentWithError => CodeEditorResources.ResourceManager.GetString(nameof (FailedToGetContentWithError), CodeEditorResources.resourceCulture);

    internal static string IEVersionError => CodeEditorResources.ResourceManager.GetString(nameof (IEVersionError), CodeEditorResources.resourceCulture);

    internal static string Loading => CodeEditorResources.ResourceManager.GetString(nameof (Loading), CodeEditorResources.resourceCulture);

    internal static string TruncatedDiffMessage => CodeEditorResources.ResourceManager.GetString(nameof (TruncatedDiffMessage), CodeEditorResources.resourceCulture);

    internal static string TruncatedEditorContent => CodeEditorResources.ResourceManager.GetString(nameof (TruncatedEditorContent), CodeEditorResources.resourceCulture);
  }
}
