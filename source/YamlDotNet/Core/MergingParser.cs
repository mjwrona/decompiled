// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.MergingParser
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
  public sealed class MergingParser : IParser
  {
    private readonly MergingParser.ParsingEventCollection _events;
    private readonly IParser _innerParser;
    private IEnumerator<LinkedListNode<ParsingEvent>> _iterator;
    private bool _merged;

    public MergingParser(IParser innerParser)
    {
      this._events = new MergingParser.ParsingEventCollection();
      this._merged = false;
      this._iterator = this._events.GetEnumerator();
      this._innerParser = innerParser;
    }

    public ParsingEvent Current => this._iterator.Current?.Value;

    public bool MoveNext()
    {
      if (!this._merged)
      {
        this.Merge();
        this._events.CleanMarked();
        this._iterator = this._events.GetEnumerator();
        this._merged = true;
      }
      return this._iterator.MoveNext();
    }

    private void Merge()
    {
      while (this._innerParser.MoveNext())
        this._events.Add(this._innerParser.Current);
      foreach (LinkedListNode<ParsingEvent> node in this._events)
      {
        if (this.IsMergeToken(node))
        {
          this._events.MarkDeleted(node);
          if (!this.HandleMerge(node.Next))
            throw new SemanticErrorException(node.Value.Start, node.Value.End, "Unrecognized merge key pattern");
        }
      }
    }

    private bool HandleMerge(LinkedListNode<ParsingEvent> node)
    {
      if (node == null)
        return false;
      if (node.Value is AnchorAlias)
        return this.HandleAnchorAlias(node);
      return node.Value is SequenceStart && this.HandleSequence(node);
    }

    private bool IsMergeToken(LinkedListNode<ParsingEvent> node) => node.Value is Scalar scalar && scalar.Value == "<<";

    private bool HandleAnchorAlias(LinkedListNode<ParsingEvent> node)
    {
      if (node == null || !(node.Value is AnchorAlias))
        return false;
      IEnumerable<ParsingEvent> mappingEvents = this.GetMappingEvents(((AnchorAlias) node.Value).Value);
      this._events.AddAfter(node, mappingEvents);
      this._events.MarkDeleted(node);
      return true;
    }

    private bool HandleSequence(LinkedListNode<ParsingEvent> node)
    {
      if (node == null || !(node.Value is SequenceStart))
        return false;
      this._events.MarkDeleted(node);
      LinkedListNode<ParsingEvent> next;
      for (; node != null; node = next)
      {
        if (node.Value is SequenceEnd)
        {
          this._events.MarkDeleted(node);
          return true;
        }
        next = node.Next;
        this.HandleMerge(next);
      }
      return true;
    }

    private IEnumerable<ParsingEvent> GetMappingEvents(string anchor)
    {
      MergingParser.ParsingEventCloner cloner = new MergingParser.ParsingEventCloner();
      int nesting = 0;
      return this._events.FromAnchor(anchor).Select<LinkedListNode<ParsingEvent>, ParsingEvent>((Func<LinkedListNode<ParsingEvent>, ParsingEvent>) (e => e.Value)).TakeWhile<ParsingEvent>((Func<ParsingEvent, bool>) (e => (nesting += e.NestingIncrease) >= 0)).Select<ParsingEvent, ParsingEvent>((Func<ParsingEvent, ParsingEvent>) (e => cloner.Clone(e)));
    }

    private sealed class ParsingEventCollection : 
      IEnumerable<LinkedListNode<ParsingEvent>>,
      IEnumerable
    {
      private readonly LinkedList<ParsingEvent> _events;
      private readonly HashSet<LinkedListNode<ParsingEvent>> _deleted;
      private readonly Dictionary<string, LinkedListNode<ParsingEvent>> _references;

      public ParsingEventCollection()
      {
        this._events = new LinkedList<ParsingEvent>();
        this._deleted = new HashSet<LinkedListNode<ParsingEvent>>();
        this._references = new Dictionary<string, LinkedListNode<ParsingEvent>>();
      }

      public void AddAfter(LinkedListNode<ParsingEvent> node, IEnumerable<ParsingEvent> items)
      {
        foreach (ParsingEvent parsingEvent in items)
          node = this._events.AddAfter(node, parsingEvent);
      }

      public void Add(ParsingEvent item)
      {
        LinkedListNode<ParsingEvent> node = this._events.AddLast(item);
        this.AddReference(item, node);
      }

      public void MarkDeleted(LinkedListNode<ParsingEvent> node) => this._deleted.Add(node);

      public void CleanMarked()
      {
        foreach (LinkedListNode<ParsingEvent> node in this._deleted)
          this._events.Remove(node);
      }

      public IEnumerable<LinkedListNode<ParsingEvent>> FromAnchor(string anchor)
      {
        IEnumerator<LinkedListNode<ParsingEvent>> iterator = this.GetEnumerator(this._references[anchor].Next);
        while (iterator.MoveNext())
          yield return iterator.Current;
      }

      public IEnumerator<LinkedListNode<ParsingEvent>> GetEnumerator() => this.GetEnumerator(this._events.First);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      private IEnumerator<LinkedListNode<ParsingEvent>> GetEnumerator(
        LinkedListNode<ParsingEvent> node)
      {
        for (; node != null; node = node.Next)
          yield return node;
      }

      private void AddReference(ParsingEvent item, LinkedListNode<ParsingEvent> node)
      {
        if (!(item is MappingStart))
          return;
        string anchor = ((NodeEvent) item).Anchor;
        if (string.IsNullOrEmpty(anchor))
          return;
        this._references[anchor] = node;
      }
    }

    private sealed class ParsingEventCloner : IParsingEventVisitor
    {
      private ParsingEvent clonedEvent;

      public ParsingEvent Clone(ParsingEvent e)
      {
        e.Accept((IParsingEventVisitor) this);
        return this.clonedEvent;
      }

      void IParsingEventVisitor.Visit(AnchorAlias e) => this.clonedEvent = (ParsingEvent) new AnchorAlias(e.Value, e.Start, e.End);

      void IParsingEventVisitor.Visit(StreamStart e) => throw new NotSupportedException();

      void IParsingEventVisitor.Visit(StreamEnd e) => throw new NotSupportedException();

      void IParsingEventVisitor.Visit(DocumentStart e) => throw new NotSupportedException();

      void IParsingEventVisitor.Visit(DocumentEnd e) => throw new NotSupportedException();

      void IParsingEventVisitor.Visit(Scalar e) => this.clonedEvent = (ParsingEvent) new Scalar((string) null, e.Tag, e.Value, e.Style, e.IsPlainImplicit, e.IsQuotedImplicit, e.Start, e.End);

      void IParsingEventVisitor.Visit(SequenceStart e) => this.clonedEvent = (ParsingEvent) new SequenceStart((string) null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);

      void IParsingEventVisitor.Visit(SequenceEnd e) => this.clonedEvent = (ParsingEvent) new SequenceEnd(e.Start, e.End);

      void IParsingEventVisitor.Visit(MappingStart e) => this.clonedEvent = (ParsingEvent) new MappingStart((string) null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);

      void IParsingEventVisitor.Visit(MappingEnd e) => this.clonedEvent = (ParsingEvent) new MappingEnd(e.Start, e.End);

      void IParsingEventVisitor.Visit(Comment e) => throw new NotSupportedException();
    }
  }
}
