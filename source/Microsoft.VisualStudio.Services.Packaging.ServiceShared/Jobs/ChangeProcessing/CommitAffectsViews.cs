// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.CommitAffectsViews
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public abstract record CommitAffectsViews()
  {
    public sealed class LocalOnly : CommitAffectsViews, IEquatable<
    #nullable disable
    CommitAffectsViews.LocalOnly>
    {
      public static readonly 
      #nullable enable
      CommitAffectsViews.LocalOnly Instance = new CommitAffectsViews.LocalOnly();

      [CompilerGenerated]
      protected override Type EqualityContract => typeof (CommitAffectsViews.LocalOnly);

      [CompilerGenerated]
      public override string ToString()
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(nameof (LocalOnly));
        builder.Append(" { ");
        if (this.PrintMembers(builder))
          builder.Append(' ');
        builder.Append('}');
        return builder.ToString();
      }

      [CompilerGenerated]
      protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);

      [CompilerGenerated]
      public static bool operator !=(
        CommitAffectsViews.LocalOnly? left,
        CommitAffectsViews.LocalOnly? right)
      {
        return !(left == right);
      }

      [CompilerGenerated]
      public static bool operator ==(
        CommitAffectsViews.LocalOnly? left,
        CommitAffectsViews.LocalOnly? right)
      {
        if ((object) left == (object) right)
          return true;
        return (object) left != null && left.Equals(right);
      }

      [CompilerGenerated]
      public override int GetHashCode() => base.GetHashCode();

      [CompilerGenerated]
      public override bool Equals(object? obj) => this.Equals(obj as CommitAffectsViews.LocalOnly);

      [CompilerGenerated]
      public override sealed bool Equals(CommitAffectsViews? other) => this.Equals((object) other);

      [CompilerGenerated]
      public bool Equals(CommitAffectsViews.LocalOnly? other) => (object) this == (object) other || base.Equals((CommitAffectsViews) other);

      [CompilerGenerated]
      public override CommitAffectsViews \u003CClone\u003E\u0024() => (CommitAffectsViews) new CommitAffectsViews.LocalOnly(this);
    }

    public sealed class All : CommitAffectsViews, IEquatable<
    #nullable disable
    CommitAffectsViews.All>
    {
      public static readonly 
      #nullable enable
      CommitAffectsViews.All Instance = new CommitAffectsViews.All();

      [CompilerGenerated]
      protected override Type EqualityContract => typeof (CommitAffectsViews.All);

      [CompilerGenerated]
      public override string ToString()
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(nameof (All));
        builder.Append(" { ");
        if (this.PrintMembers(builder))
          builder.Append(' ');
        builder.Append('}');
        return builder.ToString();
      }

      [CompilerGenerated]
      protected override bool PrintMembers(StringBuilder builder) => base.PrintMembers(builder);

      [CompilerGenerated]
      public static bool operator !=(CommitAffectsViews.All? left, CommitAffectsViews.All? right) => !(left == right);

      [CompilerGenerated]
      public static bool operator ==(CommitAffectsViews.All? left, CommitAffectsViews.All? right)
      {
        if ((object) left == (object) right)
          return true;
        return (object) left != null && left.Equals(right);
      }

      [CompilerGenerated]
      public override int GetHashCode() => base.GetHashCode();

      [CompilerGenerated]
      public override bool Equals(object? obj) => this.Equals(obj as CommitAffectsViews.All);

      [CompilerGenerated]
      public override sealed bool Equals(CommitAffectsViews? other) => this.Equals((object) other);

      [CompilerGenerated]
      public bool Equals(CommitAffectsViews.All? other) => (object) this == (object) other || base.Equals((CommitAffectsViews) other);

      [CompilerGenerated]
      public override CommitAffectsViews \u003CClone\u003E\u0024() => (CommitAffectsViews) new CommitAffectsViews.All(this);
    }

    public sealed class Specific : CommitAffectsViews, IEquatable<
    #nullable disable
    CommitAffectsViews.Specific>
    {
      public Specific(Guid ViewId)
      {
        // ISSUE: reference to a compiler-generated field
        this.\u003CViewId\u003Ek__BackingField = ViewId;
        // ISSUE: explicit constructor call
        base.\u002Ector();
      }

      [CompilerGenerated]
      protected override 
      #nullable enable
      Type EqualityContract => typeof (CommitAffectsViews.Specific);

      public Guid ViewId { get; init; }

      [CompilerGenerated]
      public override string ToString()
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(nameof (Specific));
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
        builder.Append("ViewId = ");
        builder.Append(this.ViewId.ToString());
        return true;
      }

      [CompilerGenerated]
      public static bool operator !=(
        CommitAffectsViews.Specific? left,
        CommitAffectsViews.Specific? right)
      {
        return !(left == right);
      }

      [CompilerGenerated]
      public static bool operator ==(
        CommitAffectsViews.Specific? left,
        CommitAffectsViews.Specific? right)
      {
        if ((object) left == (object) right)
          return true;
        return (object) left != null && left.Equals(right);
      }

      [CompilerGenerated]
      public override int GetHashCode() => base.GetHashCode() * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(this.\u003CViewId\u003Ek__BackingField);

      [CompilerGenerated]
      public override bool Equals(object? obj) => this.Equals(obj as CommitAffectsViews.Specific);

      [CompilerGenerated]
      public override sealed bool Equals(CommitAffectsViews? other) => this.Equals((object) other);

      [CompilerGenerated]
      public bool Equals(CommitAffectsViews.Specific? other)
      {
        if ((object) this == (object) other)
          return true;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return base.Equals((CommitAffectsViews) other) && EqualityComparer<Guid>.Default.Equals(this.\u003CViewId\u003Ek__BackingField, other.\u003CViewId\u003Ek__BackingField);
      }

      [CompilerGenerated]
      public override CommitAffectsViews \u003CClone\u003E\u0024() => (CommitAffectsViews) new CommitAffectsViews.Specific(this);

      [CompilerGenerated]
      public void Deconstruct(out Guid ViewId) => ViewId = this.ViewId;
    }
  }
}
