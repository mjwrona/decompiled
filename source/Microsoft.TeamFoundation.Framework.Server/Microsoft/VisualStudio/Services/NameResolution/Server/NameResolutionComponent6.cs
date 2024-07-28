// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  public class NameResolutionComponent6 : NameResolutionComponent5
  {
    public override List<NameResolutionEntry> RemoveResolutionEntries(Guid value, string @namespace = null)
    {
      this.PrepareStoredProcedure("prc_RemoveNameResolutionEntriesByValue", ReplicationType.Synchronous);
      this.BindGuid("@value", value);
      this.BindString("@namespace", this.TranslateNamespaceToDatabase(@namespace), 256, true, SqlDbType.NVarChar);
      return this.ExecuteQueryReturningNameResolutionEntries();
    }

    public override List<NameResolutionEntry> RemoveResolutionEntries(
      IEnumerable<NameResolutionEntry> entries)
    {
      this.PrepareStoredProcedure("prc_RemoveNameResolutionEntries", ReplicationType.Synchronous);
      this.BindNameResolutionTable("@entries", (IEnumerable<NameResolutionEntry>) entries.Select<NameResolutionEntry, NameResolutionEntry>(new System.Func<NameResolutionEntry, NameResolutionEntry>(((NameResolutionComponent) this).TranslateEntryToDatabase)).ToArray<NameResolutionEntry>());
      return this.ExecuteQueryReturningNameResolutionEntries();
    }
  }
}
