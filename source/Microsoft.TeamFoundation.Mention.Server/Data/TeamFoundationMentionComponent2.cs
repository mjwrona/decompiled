// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent2
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Mention.Server.Data
{
  internal class TeamFoundationMentionComponent2 : TeamFoundationMentionComponent
  {
    public override IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentions(
      IEnumerable<MentionSource> sources)
    {
      try
      {
        this.TraceEnter(101211, nameof (GetMentions));
        this.PrepareStoredProcedure("prc_GetMentions");
        this.BindStringTable("@sourceIds", sources.Select<MentionSource, string>((System.Func<MentionSource, string>) (x => x.SourceId)));
        this.BindStringTable("@sourceType", sources.Select<MentionSource, string>((System.Func<MentionSource, string>) (x => x.SourceType)).Distinct<string>());
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.TeamFoundation.Mention.Server.Mention>((ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>) new TeamFoundationMentionComponent2.MentionBinder());
        return (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) resultCollection.GetCurrent<Microsoft.TeamFoundation.Mention.Server.Mention>().Items ?? Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
      }
      catch (Exception ex)
      {
        this.TraceException(101910, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101219, nameof (GetMentions));
      }
    }

    internal class MentionBinder : ObjectBinder<Microsoft.TeamFoundation.Mention.Server.Mention>
    {
      internal SqlColumnBinder sourceId = new SqlColumnBinder("SourceId");
      internal SqlColumnBinder sourceType = new SqlColumnBinder("SourceType");
      internal SqlColumnBinder rawText = new SqlColumnBinder("RawText");
      internal SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
      internal SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactType");
      internal SqlColumnBinder mentionAction = new SqlColumnBinder("MentionAction");
      internal SqlColumnBinder targetId = new SqlColumnBinder("TargetId");

      protected override Microsoft.TeamFoundation.Mention.Server.Mention Bind()
      {
        MentionSourceContext mentionSourceContext = new MentionSourceContext()
        {
          Type = this.sourceType.GetString((IDataReader) this.Reader, false),
          Identifier = this.sourceId.GetString((IDataReader) this.Reader, false)
        };
        return new Microsoft.TeamFoundation.Mention.Server.Mention()
        {
          Source = (IMentionSourceContext) mentionSourceContext,
          ArtifactId = this.artifactId.GetString((IDataReader) this.Reader, false),
          RawText = this.rawText.GetString((IDataReader) this.Reader, false),
          MentionAction = this.mentionAction.GetString((IDataReader) this.Reader, false),
          ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false),
          TargetId = this.targetId.GetString((IDataReader) this.Reader, false)
        };
      }
    }
  }
}
