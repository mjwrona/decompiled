// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.EventTagParserHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class EventTagParserHandler : ITagParserHandler
  {
    void ITagParserHandler.OnReferencedObject(ObjectIdAndType o)
    {
      EventTagParserHandler.ReferencedObjectEvent referencedObject = this.ReferencedObject;
      if (referencedObject == null)
        return;
      referencedObject(o);
    }

    void ITagParserHandler.OnName(string name)
    {
      EventTagParserHandler.NameEvent name1 = this.Name;
      if (name1 == null)
        return;
      name1(name);
    }

    void ITagParserHandler.OnTagger(IdentityAndDate tagger)
    {
      EventTagParserHandler.TaggerEvent tagger1 = this.Tagger;
      if (tagger1 == null)
        return;
      tagger1(tagger);
    }

    void ITagParserHandler.OnComment(string comment)
    {
      EventTagParserHandler.CommentEvent comment1 = this.Comment;
      if (comment1 == null)
        return;
      comment1(comment);
    }

    public event EventTagParserHandler.ReferencedObjectEvent ReferencedObject;

    public event EventTagParserHandler.NameEvent Name;

    public event EventTagParserHandler.TaggerEvent Tagger;

    public event EventTagParserHandler.CommentEvent Comment;

    public delegate void ReferencedObjectEvent(ObjectIdAndType o);

    public delegate void NameEvent(string name);

    public delegate void TaggerEvent(IdentityAndDate tagger);

    public delegate void CommentEvent(string comment);
  }
}
