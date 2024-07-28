// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.MentionResources
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Mention.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class MentionResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal MentionResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (MentionResources.resourceMan == null)
          MentionResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Mention.Server.MentionResources", typeof (MentionResources).Assembly);
        return MentionResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => MentionResources.resourceCulture;
      set => MentionResources.resourceCulture = value;
    }

    public static string ChangesetSourceText => MentionResources.ResourceManager.GetString(nameof (ChangesetSourceText), MentionResources.resourceCulture);

    public static string ChatMessageSourceText => MentionResources.ResourceManager.GetString(nameof (ChatMessageSourceText), MentionResources.resourceCulture);

    public static string CommentSourceText => MentionResources.ResourceManager.GetString(nameof (CommentSourceText), MentionResources.resourceCulture);

    public static string GitCommitSourceText => MentionResources.ResourceManager.GetString(nameof (GitCommitSourceText), MentionResources.resourceCulture);

    public static string GitLinkType => MentionResources.ResourceManager.GetString(nameof (GitLinkType), MentionResources.resourceCulture);

    public static string PullRequestDescriptionSourceText => MentionResources.ResourceManager.GetString(nameof (PullRequestDescriptionSourceText), MentionResources.resourceCulture);

    public static string PullRequestSourceText => MentionResources.ResourceManager.GetString(nameof (PullRequestSourceText), MentionResources.resourceCulture);

    public static string ShelvesetSourceText => MentionResources.ResourceManager.GetString(nameof (ShelvesetSourceText), MentionResources.resourceCulture);

    public static string WikiPageSourceText => MentionResources.ResourceManager.GetString(nameof (WikiPageSourceText), MentionResources.resourceCulture);

    public static string WorkItemSourceText => MentionResources.ResourceManager.GetString(nameof (WorkItemSourceText), MentionResources.resourceCulture);
  }
}
