// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Mention.MentionResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Mention, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EBF937EB-CE65-404A-9E2A-85B6514F6A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Mention.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Mention
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
          MentionResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Mention.Resources.MentionResources", typeof (MentionResources).Assembly);
        return MentionResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => MentionResources.resourceCulture;
      set => MentionResources.resourceCulture = value;
    }

    public static string AutocompleteLoading => MentionResources.ResourceManager.GetString(nameof (AutocompleteLoading), MentionResources.resourceCulture);

    public static string AutocompleteNoSuggestions => MentionResources.ResourceManager.GetString(nameof (AutocompleteNoSuggestions), MentionResources.resourceCulture);

    public static string AutocompleteSearchButtonText => MentionResources.ResourceManager.GetString(nameof (AutocompleteSearchButtonText), MentionResources.resourceCulture);

    public static string AutocompleteServerError => MentionResources.ResourceManager.GetString(nameof (AutocompleteServerError), MentionResources.resourceCulture);

    public static string AutocompleteSuggestionsPlural => MentionResources.ResourceManager.GetString(nameof (AutocompleteSuggestionsPlural), MentionResources.resourceCulture);

    public static string AutocompleteSuggestionsSingular => MentionResources.ResourceManager.GetString(nameof (AutocompleteSuggestionsSingular), MentionResources.resourceCulture);

    public static string AutocompleteTermsOverLimit => MentionResources.ResourceManager.GetString(nameof (AutocompleteTermsOverLimit), MentionResources.resourceCulture);

    public static string DiscussionMentionPersonTip => MentionResources.ResourceManager.GetString(nameof (DiscussionMentionPersonTip), MentionResources.resourceCulture);

    public static string DiscussionMentionWorkItemTip => MentionResources.ResourceManager.GetString(nameof (DiscussionMentionWorkItemTip), MentionResources.resourceCulture);

    public static string DiscussionShowMoreLinkText => MentionResources.ResourceManager.GetString(nameof (DiscussionShowMoreLinkText), MentionResources.resourceCulture);

    public static string DiscussionShowMoreRetrievalError => MentionResources.ResourceManager.GetString(nameof (DiscussionShowMoreRetrievalError), MentionResources.resourceCulture);

    public static string DiscussionStateTextFormat => MentionResources.ResourceManager.GetString(nameof (DiscussionStateTextFormat), MentionResources.resourceCulture);

    public static string DiscussionUserCommentedTextFormat => MentionResources.ResourceManager.GetString(nameof (DiscussionUserCommentedTextFormat), MentionResources.resourceCulture);

    public static string GitPullRequestMentionWorkItemDiscussionMessageFormat => MentionResources.ResourceManager.GetString(nameof (GitPullRequestMentionWorkItemDiscussionMessageFormat), MentionResources.resourceCulture);

    public static string MentionMaterializeErrorDialogTitle => MentionResources.ResourceManager.GetString(nameof (MentionMaterializeErrorDialogTitle), MentionResources.resourceCulture);

    public static string MentionMaterializeIdentityFailError => MentionResources.ResourceManager.GetString(nameof (MentionMaterializeIdentityFailError), MentionResources.resourceCulture);

    public static string MentionPR => MentionResources.ResourceManager.GetString(nameof (MentionPR), MentionResources.resourceCulture);

    public static string MentionSomeone => MentionResources.ResourceManager.GetString(nameof (MentionSomeone), MentionResources.resourceCulture);

    public static string MentionWorkItem => MentionResources.ResourceManager.GetString(nameof (MentionWorkItem), MentionResources.resourceCulture);

    public static string PullRequestText => MentionResources.ResourceManager.GetString(nameof (PullRequestText), MentionResources.resourceCulture);

    public static string WorkItemMentionOptionSelected => MentionResources.ResourceManager.GetString(nameof (WorkItemMentionOptionSelected), MentionResources.resourceCulture);
  }
}
