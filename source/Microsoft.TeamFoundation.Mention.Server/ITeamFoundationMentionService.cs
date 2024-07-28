// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.ITeamFoundationMentionService
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Mention.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationMentionService))]
  public interface ITeamFoundationMentionService : IVssFrameworkService
  {
    IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> ProcessMentionCandidates(
      IVssRequestContext requestContext,
      IEnumerable<MentionCandidate> candidates,
      out int totalNumberOfMentionsFound,
      out int mentionProcessingErrors,
      out string firstErrorMessage);

    IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> ProcessMentionCandidates(
      IVssRequestContext requestContext,
      IEnumerable<MentionCandidate> candidates);

    IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentions(
      IVssRequestContext requestContext,
      IEnumerable<MentionSource> sources);

    void SaveMentions(IVssRequestContext requestContext, IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions);

    IEnumerable<MentionRecord> UpdateMentions(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions);

    void DeleteMentionsByCommentIds(IVssRequestContext requestContext, ISet<int> commentIds);

    void DeleteMentionsBySourceIds(IVssRequestContext requestContext, ISet<string> soruceIds);

    IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsByCommentIds(
      IVssRequestContext requestContext,
      ISet<int> commentIds);

    IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsBySourceIds(
      IVssRequestContext requestContext,
      ISet<string> sourceIds);

    IEnumerable<LastMentionedInfo> GetRecentMentionsForTargets(
      IVssRequestContext requestContext,
      Guid? sourceProjectId,
      string sourceType,
      int limit,
      IEnumerable<MentionTarget> targets);

    IEnumerable<LastMentionedInfo> GetRecentMentionsForCurrentUser(
      IVssRequestContext requestContext,
      Guid? sourceProjectId,
      string sourceType,
      int limit);

    void DeleteMentions(
      IVssRequestContext requestContext,
      IEnumerable<MentionSourceExtended> sources);

    int CleanupMentions(IVssRequestContext requestContext, DateTime currentTime);
  }
}
