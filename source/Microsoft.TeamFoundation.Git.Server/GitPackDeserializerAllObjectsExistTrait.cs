// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackDeserializerAllObjectsExistTrait
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Telemetry;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class GitPackDeserializerAllObjectsExistTrait : IGitPackDeserializerTrait
  {
    private readonly IVssRequestContext m_rc;
    private readonly ushort m_packIntId;
    private readonly HashSet<ObjectIdAndGitPackIndexEntry> m_expectedEntries;
    private readonly bool m_allowExtraEntries;
    private const string c_layer = "GitPackDeserializerAllObjectsExistTrait";

    public GitPackDeserializerAllObjectsExistTrait(
      IVssRequestContext requestContext,
      ushort packIntId,
      IEnumerable<ObjectIdAndGitPackIndexEntry> expectedEntries,
      bool allowExtraEntries)
    {
      this.m_rc = requestContext;
      this.m_packIntId = packIntId;
      this.m_expectedEntries = new HashSet<ObjectIdAndGitPackIndexEntry>(expectedEntries);
      this.m_allowExtraEntries = allowExtraEntries;
    }

    public void AddToDeserializer(GitPackDeserializer deserializer)
    {
      deserializer.DeserializationComplete += (GitPackDeserializer.DeserializationCompleteHandler) ((packHash, packStream, packLength) =>
      {
        if (this.m_expectedEntries.Count != 0)
          throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Possible corruption: {0} missing objects in pack. First missing: {1}", (object) this.m_expectedEntries.Count, (object) this.m_expectedEntries.First<ObjectIdAndGitPackIndexEntry>())));
      });
      deserializer.ObjectInfo += (GitPackDeserializer.ObjectInfoHandler) ((progress, objectId, objectType, objectLength, offsetInPack, lengthInPack) =>
      {
        if (this.m_rc.IsTracing(1013956, TraceLevel.Info, GitServerUtils.TraceArea, nameof (GitPackDeserializerAllObjectsExistTrait)))
        {
          int num1 = 1000;
          int num2 = Math.Max(progress.TotalObjects / num1, 1);
          if (progress.ObjectsEnumerated % num2 == 0)
            this.m_rc.ReportProgress(88, "GitPackDeserializer.Walking", nameof (GitPackDeserializerAllObjectsExistTrait), false, string.Format("Enumerated {0} out of {1} total objects.", (object) progress.ObjectsEnumerated, (object) progress.TotalObjects));
        }
        ObjectIdAndGitPackIndexEntry gitPackIndexEntry = new ObjectIdAndGitPackIndexEntry(objectId, objectType, new TfsGitObjectLocation(this.m_packIntId, offsetInPack, lengthInPack));
        if (this.m_expectedEntries.Remove(gitPackIndexEntry))
          return;
        if (!this.m_allowExtraEntries)
          throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Possible corruption: {0} not expected in pack.", (object) gitPackIndexEntry)));
        if (deserializer.BaseObjects.TryLookupObjectType(objectId) == GitObjectType.Bad)
          throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Possible corruption: An extra entry: {0} found in pack, but objectId is not in the index", (object) gitPackIndexEntry)));
      });
    }
  }
}
