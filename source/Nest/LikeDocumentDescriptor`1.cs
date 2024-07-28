// Decompiled with JetBrains decompiler
// Type: Nest.LikeDocumentDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LikeDocumentDescriptor<TDocument> : 
    DescriptorBase<LikeDocumentDescriptor<TDocument>, ILikeDocument>,
    ILikeDocument
    where TDocument : class
  {
    public LikeDocumentDescriptor() => this.Self.Index = (IndexName) typeof (TDocument);

    object ILikeDocument.Document { get; set; }

    Nest.Fields ILikeDocument.Fields { get; set; }

    Nest.Id ILikeDocument.Id { get; set; }

    IndexName ILikeDocument.Index { get; set; }

    IPerFieldAnalyzer ILikeDocument.PerFieldAnalyzer { get; set; }

    Nest.Routing ILikeDocument.Routing { get; set; }

    public LikeDocumentDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<ILikeDocument, IndexName>) ((a, v) => a.Index = v));

    public LikeDocumentDescriptor<TDocument> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<ILikeDocument, Nest.Id>) ((a, v) => a.Id = v));

    public LikeDocumentDescriptor<TDocument> Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<ILikeDocument, Nest.Routing>) ((a, v) => a.Routing = v));

    public LikeDocumentDescriptor<TDocument> Fields(
      Func<FieldsDescriptor<TDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TDocument>, IPromise<Nest.Fields>>>(fields, (Action<ILikeDocument, Func<FieldsDescriptor<TDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<TDocument>())?.Value : (Nest.Fields) null));
    }

    public LikeDocumentDescriptor<TDocument> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ILikeDocument, Nest.Fields>) ((a, v) => a.Fields = v));

    public LikeDocumentDescriptor<TDocument> Document(TDocument document) => this.Assign<TDocument>(document, (Action<ILikeDocument, TDocument>) ((a, v) => a.Document = (object) v));

    public LikeDocumentDescriptor<TDocument> PerFieldAnalyzer(
      Func<PerFieldAnalyzerDescriptor<TDocument>, IPromise<IPerFieldAnalyzer>> analyzerSelector)
    {
      return this.Assign<Func<PerFieldAnalyzerDescriptor<TDocument>, IPromise<IPerFieldAnalyzer>>>(analyzerSelector, (Action<ILikeDocument, Func<PerFieldAnalyzerDescriptor<TDocument>, IPromise<IPerFieldAnalyzer>>>) ((a, v) => a.PerFieldAnalyzer = v != null ? v(new PerFieldAnalyzerDescriptor<TDocument>())?.Value : (IPerFieldAnalyzer) null));
    }
  }
}
