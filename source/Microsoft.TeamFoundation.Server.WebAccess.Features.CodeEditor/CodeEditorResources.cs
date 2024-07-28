// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor.CodeEditorResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC93D3E8-2C8E-445B-91D5-3622EF15D37B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class CodeEditorResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CodeEditorResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (CodeEditorResources.resourceMan == null)
          CodeEditorResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor.CodeEditorResources", typeof (CodeEditorResources).Assembly);
        return CodeEditorResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => CodeEditorResources.resourceCulture;
      set => CodeEditorResources.resourceCulture = value;
    }

    public static string ContextMenuToggleMinimap => CodeEditorResources.ResourceManager.GetString(nameof (ContextMenuToggleMinimap), CodeEditorResources.resourceCulture);

    public static string ContextMenuToggleWhiteSpace => CodeEditorResources.ResourceManager.GetString(nameof (ContextMenuToggleWhiteSpace), CodeEditorResources.resourceCulture);

    public static string ContextMenuToggleWordWrap => CodeEditorResources.ResourceManager.GetString(nameof (ContextMenuToggleWordWrap), CodeEditorResources.resourceCulture);

    public static string CopiedLineLinkTitle => CodeEditorResources.ResourceManager.GetString(nameof (CopiedLineLinkTitle), CodeEditorResources.resourceCulture);

    public static string CopyLineLinkTitle => CodeEditorResources.ResourceManager.GetString(nameof (CopyLineLinkTitle), CodeEditorResources.resourceCulture);

    public static string EscapeToLoseFocus => CodeEditorResources.ResourceManager.GetString(nameof (EscapeToLoseFocus), CodeEditorResources.resourceCulture);

    public static string FailedToGetContent => CodeEditorResources.ResourceManager.GetString(nameof (FailedToGetContent), CodeEditorResources.resourceCulture);

    public static string IEVersionError => CodeEditorResources.ResourceManager.GetString(nameof (IEVersionError), CodeEditorResources.resourceCulture);

    public static string Loading => CodeEditorResources.ResourceManager.GetString(nameof (Loading), CodeEditorResources.resourceCulture);

    public static string TruncatedDiffMessage => CodeEditorResources.ResourceManager.GetString(nameof (TruncatedDiffMessage), CodeEditorResources.resourceCulture);

    public static string TruncatedEditorContent => CodeEditorResources.ResourceManager.GetString(nameof (TruncatedEditorContent), CodeEditorResources.resourceCulture);
  }
}
