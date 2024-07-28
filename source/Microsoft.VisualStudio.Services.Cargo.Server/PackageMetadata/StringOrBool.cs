// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata.StringOrBool
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata
{
  public record StringOrBool()
  {
    public sealed class String : StringOrBool, IEquatable<
    #nullable disable
    StringOrBool.String>
    {
      public String(
      #nullable enable
      string Value)
      {
        // ISSUE: reference to a compiler-generated field
        this.\u003CValue\u003Ek__BackingField = Value;
        // ISSUE: explicit constructor call
        base.\u002Ector();
      }

      [CompilerGenerated]
      protected override Type EqualityContract => typeof (StringOrBool.String);

      public string Value { get; init; }

      [CompilerGenerated]
      public override string ToString()
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(nameof (String));
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
        builder.Append("Value = ");
        builder.Append((object) this.Value);
        return true;
      }

      [CompilerGenerated]
      public static bool operator !=(StringOrBool.String? left, StringOrBool.String? right) => !(left == right);

      [CompilerGenerated]
      public static bool operator ==(StringOrBool.String? left, StringOrBool.String? right)
      {
        if ((object) left == (object) right)
          return true;
        return (object) left != null && left.Equals(right);
      }

      [CompilerGenerated]
      public override int GetHashCode() => base.GetHashCode() * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.\u003CValue\u003Ek__BackingField);

      [CompilerGenerated]
      public override bool Equals(object? obj) => this.Equals(obj as StringOrBool.String);

      [CompilerGenerated]
      public override sealed bool Equals(StringOrBool? other) => this.Equals((object) other);

      [CompilerGenerated]
      public bool Equals(StringOrBool.String? other)
      {
        if ((object) this == (object) other)
          return true;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return base.Equals((StringOrBool) other) && EqualityComparer<string>.Default.Equals(this.\u003CValue\u003Ek__BackingField, other.\u003CValue\u003Ek__BackingField);
      }

      [CompilerGenerated]
      public override StringOrBool \u003CClone\u003E\u0024() => (StringOrBool) new StringOrBool.String(this);

      [CompilerGenerated]
      public void Deconstruct(out string Value) => Value = this.Value;
    }

    public sealed class Bool : StringOrBool, IEquatable<
    #nullable disable
    StringOrBool.Bool>
    {
      public Bool(bool Value)
      {
        // ISSUE: reference to a compiler-generated field
        this.\u003CValue\u003Ek__BackingField = Value;
        // ISSUE: explicit constructor call
        base.\u002Ector();
      }

      [CompilerGenerated]
      protected override 
      #nullable enable
      Type EqualityContract => typeof (StringOrBool.Bool);

      public bool Value { get; init; }

      [CompilerGenerated]
      public override string ToString()
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(nameof (Bool));
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
        builder.Append("Value = ");
        builder.Append(this.Value.ToString());
        return true;
      }

      [CompilerGenerated]
      public static bool operator !=(StringOrBool.Bool? left, StringOrBool.Bool? right) => !(left == right);

      [CompilerGenerated]
      public static bool operator ==(StringOrBool.Bool? left, StringOrBool.Bool? right)
      {
        if ((object) left == (object) right)
          return true;
        return (object) left != null && left.Equals(right);
      }

      [CompilerGenerated]
      public override int GetHashCode() => base.GetHashCode() * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.\u003CValue\u003Ek__BackingField);

      [CompilerGenerated]
      public override bool Equals(object? obj) => this.Equals(obj as StringOrBool.Bool);

      [CompilerGenerated]
      public override sealed bool Equals(StringOrBool? other) => this.Equals((object) other);

      [CompilerGenerated]
      public bool Equals(StringOrBool.Bool? other)
      {
        if ((object) this == (object) other)
          return true;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return base.Equals((StringOrBool) other) && EqualityComparer<bool>.Default.Equals(this.\u003CValue\u003Ek__BackingField, other.\u003CValue\u003Ek__BackingField);
      }

      [CompilerGenerated]
      public override StringOrBool \u003CClone\u003E\u0024() => (StringOrBool) new StringOrBool.Bool(this);

      [CompilerGenerated]
      public void Deconstruct(out bool Value) => Value = this.Value;
    }
  }
}
