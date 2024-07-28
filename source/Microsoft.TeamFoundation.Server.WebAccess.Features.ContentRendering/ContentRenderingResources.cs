// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering.ContentRenderingResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78AC3D55-06D3-4434-8BC6-2E2C9E46022A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class ContentRenderingResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ContentRenderingResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (ContentRenderingResources.resourceMan == null)
          ContentRenderingResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering.ContentRenderingResources", typeof (ContentRenderingResources).Assembly);
        return ContentRenderingResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => ContentRenderingResources.resourceCulture;
      set => ContentRenderingResources.resourceCulture = value;
    }

    public static string AttachmentDocumentation => ContentRenderingResources.ResourceManager.GetString(nameof (AttachmentDocumentation), ContentRenderingResources.resourceCulture);

    public static string AttachmentHelpText => ContentRenderingResources.ResourceManager.GetString(nameof (AttachmentHelpText), ContentRenderingResources.resourceCulture);

    public static string AttachmentInstructions => ContentRenderingResources.ResourceManager.GetString(nameof (AttachmentInstructions), ContentRenderingResources.resourceCulture);

    public static string AttachmentInstructionsLink => ContentRenderingResources.ResourceManager.GetString(nameof (AttachmentInstructionsLink), ContentRenderingResources.resourceCulture);

    public static string MarkdownBold => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownBold), ContentRenderingResources.resourceCulture);

    public static string MarkdownBulletedList => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownBulletedList), ContentRenderingResources.resourceCulture);

    public static string MarkdownCode => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownCode), ContentRenderingResources.resourceCulture);

    public static string MarkdownDocumentation => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownDocumentation), ContentRenderingResources.resourceCulture);

    public static string MarkdownHeader => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownHeader), ContentRenderingResources.resourceCulture);

    public static string MarkdownHeaderN => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownHeaderN), ContentRenderingResources.resourceCulture);

    public static string MarkdownInstructions => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownInstructions), ContentRenderingResources.resourceCulture);

    public static string MarkdownItalic => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownItalic), ContentRenderingResources.resourceCulture);

    public static string MarkdownLink => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownLink), ContentRenderingResources.resourceCulture);

    public static string MarkdownNumberedList => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownNumberedList), ContentRenderingResources.resourceCulture);

    public static string MarkdownTaskList => ContentRenderingResources.ResourceManager.GetString(nameof (MarkdownTaskList), ContentRenderingResources.resourceCulture);

    public static string ToolbarOverflowAriaLabel => ContentRenderingResources.ResourceManager.GetString(nameof (ToolbarOverflowAriaLabel), ContentRenderingResources.resourceCulture);
  }
}
