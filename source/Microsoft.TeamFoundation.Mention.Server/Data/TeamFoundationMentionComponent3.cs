// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.Data.TeamFoundationMentionComponent3
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
  internal class TeamFoundationMentionComponent3 : TeamFoundationMentionComponent2
  {
    public override void DeleteMentions(IEnumerable<MentionSourceExtended> sources)
    {
      try
      {
        this.TraceEnter(101223, nameof (DeleteMentions));
        this.PrepareStoredProcedure("prc_DeleteMentions");
        this.BindStringTable("@sourceIdsToDelete", sources.Select<MentionSourceExtended, string>((System.Func<MentionSourceExtended, string>) (x => x.SourceId)));
        this.BindString("@sourceType", sources.Select<MentionSourceExtended, string>((System.Func<MentionSourceExtended, string>) (source => source.SourceType)).First<string>(), 200, false, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(101225, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(101224, nameof (DeleteMentions));
      }
    }
  }
}
