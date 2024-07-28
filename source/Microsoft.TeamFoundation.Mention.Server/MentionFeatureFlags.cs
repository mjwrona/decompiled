// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.MentionFeatureFlags
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

namespace Microsoft.TeamFoundation.Mention.Server
{
  public static class MentionFeatureFlags
  {
    public static string UseNotificationPipeline = "VisualStudio.Mentions.UseNotificationPipeline";
    public static string AllowGroupMentions = "VisualStudio.Mentions.AllowGroupMentions";
    public static string ShowGroupMentions = "VisualStudio.Mentions.ShowGroupMentions";
    public static string PopulateStorageKeysInMentions = "VisualStudio.Mentions.PopulateStorageKeysInMentions";
    public static string UseStorageKeyInMentions = "VisualStudio.Mentions.UseStorageKeyInMentions";
    public static string MentionRemoveEmails = "VisualStudio.Mentions.RemoveEmails";
    public static string EnableMentionFormatMismatchCleanupJob = "VisualStudio.Mentions.EnableMentionFormatMismatchCleanupJob";
    public static string SuppressMentionEmails = "VisualStudio.Mentions.SuppressMentionEmails";
    public static string GitCommitMessageWITTransition = "VisualStudio.Mentions.GitCommitMessageWITTransition";
    public static string EmailsLocaleFixDisabled = "VisualStudio.Mentions.EmailsLocaleFixDisabled";
  }
}
