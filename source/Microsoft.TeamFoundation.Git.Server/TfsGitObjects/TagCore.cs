// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitObjects.TagCore
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.TfsGitObjects
{
  internal sealed class TagCore : IGitObjectCore
  {
    private TagCore() => this.EstimatedSize = CacheUtil.ObjectOverhead + ObjectIdAndType.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size;

    public ObjectIdAndType ReferencedObject { get; private set; }

    public string Name { get; private set; }

    public IdentityAndDate Tagger { get; private set; }

    public string Comment { get; private set; }

    public static TagCore Parse(Stream stream)
    {
      TagCore parent = new TagCore();
      TagCore.ParserHandler handler = new TagCore.ParserHandler(parent);
      TagParser.Parse(stream, (ITagParserHandler) handler);
      return parent;
    }

    public GitPackObjectType PackType => GitPackObjectType.Tag;

    public int EstimatedSize { get; private set; }

    private class ParserHandler : ITagParserHandler
    {
      private readonly TagCore m_parent;

      public ParserHandler(TagCore parent) => this.m_parent = parent;

      public void OnReferencedObject(ObjectIdAndType obj) => this.m_parent.ReferencedObject = obj;

      public void OnName(string name)
      {
        this.m_parent.Name = name;
        this.m_parent.EstimatedSize += CacheUtil.ObjectOverhead + name.Length * 2;
      }

      public void OnTagger(IdentityAndDate tagger)
      {
        this.m_parent.Tagger = tagger;
        this.m_parent.EstimatedSize += tagger.EstimatedSize;
      }

      public void OnComment(string comment)
      {
        this.m_parent.Comment = comment;
        this.m_parent.EstimatedSize += CacheUtil.ObjectOverhead + comment.Length * 2;
      }
    }
  }
}
