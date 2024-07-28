// Decompiled with JetBrains decompiler
// Type: Nest.DocumentPath`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DocumentPath<T> : IEquatable<DocumentPath<T>>, IDocumentPath where T : class
  {
    public DocumentPath(T document)
      : this(Nest.Id.From<T>(document))
    {
      this.Document = document;
    }

    public DocumentPath(Nest.Id id)
    {
      this.Self.Id = id;
      this.Self.Index = (IndexName) typeof (T);
    }

    internal T Document { get; set; }

    internal IDocumentPath Self => (IDocumentPath) this;

    Nest.Id IDocumentPath.Id { get; set; }

    IndexName IDocumentPath.Index { get; set; }

    public bool Equals(DocumentPath<T> other)
    {
      IDocumentPath documentPath = (IDocumentPath) other;
      IDocumentPath self = this.Self;
      if (!self.Index.NullOrEquals<IndexName>(documentPath.Index) || !self.Id.NullOrEquals<Nest.Id>(documentPath.Id))
        return false;
      // ISSUE: variable of a boxed type
      __Boxed<T> document = (object) this.Document;
      return document == null || document.Equals((object) other.Document);
    }

    public static DocumentPath<T> Id(Nest.Id id) => new DocumentPath<T>(id);

    public static DocumentPath<T> Id(T @object) => new DocumentPath<T>(@object);

    public static implicit operator DocumentPath<T>(T @object) => (object) @object != null ? new DocumentPath<T>(@object) : (DocumentPath<T>) null;

    public static implicit operator DocumentPath<T>(Nest.Id id) => !(id == (Nest.Id) null) ? new DocumentPath<T>(id) : (DocumentPath<T>) null;

    public static implicit operator DocumentPath<T>(long id) => new DocumentPath<T>((Nest.Id) id);

    public static implicit operator DocumentPath<T>(string id) => !id.IsNullOrEmpty() ? new DocumentPath<T>((Nest.Id) id) : (DocumentPath<T>) null;

    public static implicit operator DocumentPath<T>(Guid id) => new DocumentPath<T>((Nest.Id) id);

    public DocumentPath<T> Index(IndexName index)
    {
      if (index == (IndexName) null)
        return this;
      this.Self.Index = index;
      return this;
    }

    public override int GetHashCode()
    {
      IndexName index = this.Self.Index;
      int num = ((object) index != null ? index.GetHashCode() : 0) * 397;
      Nest.Id id = this.Self.Id;
      int hashCode = (object) id != null ? id.GetHashCode() : 0;
      return num ^ hashCode;
    }

    public override bool Equals(object obj)
    {
      DocumentPath<T> other = obj as DocumentPath<T>;
      return (object) other != null && this.Equals(other);
    }

    public static bool operator ==(DocumentPath<T> x, DocumentPath<T> y) => object.Equals((object) x, (object) y);

    public static bool operator !=(DocumentPath<T> x, DocumentPath<T> y) => !object.Equals((object) x, (object) y);
  }
}
