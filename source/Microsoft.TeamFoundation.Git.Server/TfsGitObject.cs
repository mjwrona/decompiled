// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitObject
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class TfsGitObject
  {
    private long m_length = -1;

    internal TfsGitObject(ICachedGitObjectSet objectSet, Sha1Id objectId)
    {
      this.ObjectSet = objectSet;
      this.ObjectId = objectId;
    }

    internal ICachedGitObjectSet ObjectSet { get; }

    public abstract GitObjectType ObjectType { get; }

    internal abstract GitPackObjectType PackType { get; }

    public Sha1Id ObjectId { get; private set; }

    internal abstract IEnumerable<Sha1Id> ReferencedObjectIds { get; }

    public long GetLength()
    {
      if (-1L == this.m_length)
      {
        using (Stream contentAndCheckType = this.ObjectSet.GetContentAndCheckType(this.ObjectId, this.ObjectType))
          this.SetLength(contentAndCheckType.Length);
      }
      return this.m_length;
    }

    private void SetLength(long length)
    {
      if (-1L == this.m_length)
        this.m_length = length;
      else if (this.m_length != length)
        throw new InvalidOperationException();
    }

    public Stream GetContent()
    {
      Stream stream = this.ObjectSet.GetContentAndCheckType(this.ObjectId, this.ObjectType);
      try
      {
        this.SetLength(stream.Length);
        Stream content = stream;
        stream = (Stream) null;
        return content;
      }
      finally
      {
        stream?.Dispose();
      }
    }

    public override string ToString() => this.ObjectId.ToString();
  }
}
