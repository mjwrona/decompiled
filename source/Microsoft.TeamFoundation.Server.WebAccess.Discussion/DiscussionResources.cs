// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Discussion.DiscussionResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Discussion, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3C57FAFE-4971-4BBB-A484-416136CA3D02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Discussion.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Discussion
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class DiscussionResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal DiscussionResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (DiscussionResources.resourceMan == null)
          DiscussionResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Discussion.Resources.DiscussionResources", typeof (DiscussionResources).Assembly);
        return DiscussionResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => DiscussionResources.resourceCulture;
      set => DiscussionResources.resourceCulture = value;
    }

    public static string MarkdownPaste => DiscussionResources.ResourceManager.GetString(nameof (MarkdownPaste), DiscussionResources.resourceCulture);

    public static string MarkdownPasteHTML => DiscussionResources.ResourceManager.GetString(nameof (MarkdownPasteHTML), DiscussionResources.resourceCulture);

    public static string MarkdownPasteText => DiscussionResources.ResourceManager.GetString(nameof (MarkdownPasteText), DiscussionResources.resourceCulture);
  }
}
