// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.DfpjFilteredFeedChange
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public abstract record DfpjFilteredFeedChange()
  {
    public class FeedToDelete : 
      DfpjFilteredFeedChange,
      IEquatable<
      #nullable disable
      DfpjFilteredFeedChange.FeedToDelete>
    {
      public FeedToDelete(Microsoft.VisualStudio.Services.Feed.WebApi.Feed Feed, long ContinuationToken)
      {
        // ISSUE: reference to a compiler-generated field
        this.\u003CFeed\u003Ek__BackingField = Feed;
        // ISSUE: reference to a compiler-generated field
        this.\u003CContinuationToken\u003Ek__BackingField = ContinuationToken;
        // ISSUE: explicit constructor call
        base.\u002Ector();
      }

      [CompilerGenerated]
      protected override 
      #nullable enable
      Type EqualityContract => typeof (DfpjFilteredFeedChange.FeedToDelete);

      public 
      #nullable disable
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed Feed { get; init; }

      public long ContinuationToken { get; init; }

      [CompilerGenerated]
      public override 
      #nullable enable
      string ToString()
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(nameof (FeedToDelete));
        builder.Append(" { ");
        if (this.PrintMembers(builder))
          builder.Append(' ');
        builder.Append('}');
        return builder.ToString();
      }

      [CompilerGenerated]
      protected override bool PrintMembers(StringBuilder builder)
      {
        if (base.PrintMembers(builder))
          builder.Append(", ");
        builder.Append("Feed = ");
        builder.Append((object) this.Feed);
        builder.Append(", ContinuationToken = ");
        builder.Append(this.ContinuationToken.ToString());
        return true;
      }

      [CompilerGenerated]
      public static bool operator !=(
        DfpjFilteredFeedChange.FeedToDelete? left,
        DfpjFilteredFeedChange.FeedToDelete? right)
      {
        return !(left == right);
      }

      [CompilerGenerated]
      public static bool operator ==(
        DfpjFilteredFeedChange.FeedToDelete? left,
        DfpjFilteredFeedChange.FeedToDelete? right)
      {
        if ((object) left == (object) right)
          return true;
        return (object) left != null && left.Equals(right);
      }

      [CompilerGenerated]
      public override int GetHashCode() => (base.GetHashCode() * -1521134295 + EqualityComparer<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>.Default.GetHashCode(this.\u003CFeed\u003Ek__BackingField)) * -1521134295 + EqualityComparer<long>.Default.GetHashCode(this.\u003CContinuationToken\u003Ek__BackingField);

      [CompilerGenerated]
      public override bool Equals(object? obj) => this.Equals(obj as DfpjFilteredFeedChange.FeedToDelete);

      [CompilerGenerated]
      public override sealed bool Equals(DfpjFilteredFeedChange? other) => this.Equals((object) other);

      [CompilerGenerated]
      public virtual bool Equals(DfpjFilteredFeedChange.FeedToDelete? other)
      {
        if ((object) this == (object) other)
          return true;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return base.Equals((DfpjFilteredFeedChange) other) && EqualityComparer<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>.Default.Equals(this.\u003CFeed\u003Ek__BackingField, other.\u003CFeed\u003Ek__BackingField) && EqualityComparer<long>.Default.Equals(this.\u003CContinuationToken\u003Ek__BackingField, other.\u003CContinuationToken\u003Ek__BackingField);
      }

      [CompilerGenerated]
      public override DfpjFilteredFeedChange \u003CClone\u003E\u0024() => (DfpjFilteredFeedChange) new DfpjFilteredFeedChange.FeedToDelete(this);

      [CompilerGenerated]
      protected FeedToDelete(DfpjFilteredFeedChange.FeedToDelete original)
        : base((DfpjFilteredFeedChange) original)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.\u003CFeed\u003Ek__BackingField = original.\u003CFeed\u003Ek__BackingField;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.\u003CContinuationToken\u003Ek__BackingField = original.\u003CContinuationToken\u003Ek__BackingField;
      }

      [CompilerGenerated]
      public void Deconstruct(out 
      #nullable disable
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed Feed, out long ContinuationToken)
      {
        Feed = this.Feed;
        ContinuationToken = this.ContinuationToken;
      }
    }

    public class LastContinuationToken : 
      DfpjFilteredFeedChange,
      IEquatable<DfpjFilteredFeedChange.LastContinuationToken>
    {
      public LastContinuationToken(long ContinuationToken)
      {
        // ISSUE: reference to a compiler-generated field
        this.\u003CContinuationToken\u003Ek__BackingField = ContinuationToken;
        // ISSUE: explicit constructor call
        base.\u002Ector();
      }

      [CompilerGenerated]
      protected override 
      #nullable enable
      Type EqualityContract => typeof (DfpjFilteredFeedChange.LastContinuationToken);

      public long ContinuationToken { get; init; }

      [CompilerGenerated]
      public override string ToString()
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(nameof (LastContinuationToken));
        builder.Append(" { ");
        if (this.PrintMembers(builder))
          builder.Append(' ');
        builder.Append('}');
        return builder.ToString();
      }

      [CompilerGenerated]
      protected override bool PrintMembers(StringBuilder builder)
      {
        if (base.PrintMembers(builder))
          builder.Append(", ");
        builder.Append("ContinuationToken = ");
        builder.Append(this.ContinuationToken.ToString());
        return true;
      }

      [CompilerGenerated]
      public static bool operator !=(
        DfpjFilteredFeedChange.LastContinuationToken? left,
        DfpjFilteredFeedChange.LastContinuationToken? right)
      {
        return !(left == right);
      }

      [CompilerGenerated]
      public static bool operator ==(
        DfpjFilteredFeedChange.LastContinuationToken? left,
        DfpjFilteredFeedChange.LastContinuationToken? right)
      {
        if ((object) left == (object) right)
          return true;
        return (object) left != null && left.Equals(right);
      }

      [CompilerGenerated]
      public override int GetHashCode() => base.GetHashCode() * -1521134295 + EqualityComparer<long>.Default.GetHashCode(this.\u003CContinuationToken\u003Ek__BackingField);

      [CompilerGenerated]
      public override bool Equals(object? obj) => this.Equals(obj as DfpjFilteredFeedChange.LastContinuationToken);

      [CompilerGenerated]
      public override sealed bool Equals(DfpjFilteredFeedChange? other) => this.Equals((object) other);

      [CompilerGenerated]
      public virtual bool Equals(DfpjFilteredFeedChange.LastContinuationToken? other)
      {
        if ((object) this == (object) other)
          return true;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return base.Equals((DfpjFilteredFeedChange) other) && EqualityComparer<long>.Default.Equals(this.\u003CContinuationToken\u003Ek__BackingField, other.\u003CContinuationToken\u003Ek__BackingField);
      }

      [CompilerGenerated]
      public override DfpjFilteredFeedChange \u003CClone\u003E\u0024() => (DfpjFilteredFeedChange) new DfpjFilteredFeedChange.LastContinuationToken(this);

      [CompilerGenerated]
      protected LastContinuationToken(
        DfpjFilteredFeedChange.LastContinuationToken original)
        : base((DfpjFilteredFeedChange) original)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.\u003CContinuationToken\u003Ek__BackingField = original.\u003CContinuationToken\u003Ek__BackingField;
      }

      [CompilerGenerated]
      public void Deconstruct(out long ContinuationToken) => ContinuationToken = this.ContinuationToken;
    }
  }
}
