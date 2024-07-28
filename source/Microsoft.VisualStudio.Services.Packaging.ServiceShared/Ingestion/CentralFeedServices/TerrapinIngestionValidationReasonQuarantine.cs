// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinIngestionValidationReasonQuarantine
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class TerrapinIngestionValidationReasonQuarantine : 
    TerrapinIngestionValidationReason,
    IEquatable<
    #nullable disable
    TerrapinIngestionValidationReasonQuarantine>
  {
    public TerrapinIngestionValidationReasonQuarantine(
      string Message,
      DateTime? QuarantinedUntilUtc = null)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CQuarantinedUntilUtc\u003Ek__BackingField = QuarantinedUntilUtc;
      // ISSUE: explicit constructor call
      base.\u002Ector("Quarantine", Message);
    }

    [CompilerGenerated]
    protected override 
    #nullable enable
    Type EqualityContract => typeof (TerrapinIngestionValidationReasonQuarantine);

    public DateTime? QuarantinedUntilUtc { get; init; }

    [CompilerGenerated]
    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append(nameof (TerrapinIngestionValidationReasonQuarantine));
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
      builder.Append("QuarantinedUntilUtc = ");
      builder.Append(this.QuarantinedUntilUtc.ToString());
      return true;
    }

    [CompilerGenerated]
    public static bool operator !=(
      TerrapinIngestionValidationReasonQuarantine? left,
      TerrapinIngestionValidationReasonQuarantine? right)
    {
      return !(left == right);
    }

    [CompilerGenerated]
    public static bool operator ==(
      TerrapinIngestionValidationReasonQuarantine? left,
      TerrapinIngestionValidationReasonQuarantine? right)
    {
      if ((object) left == (object) right)
        return true;
      return (object) left != null && left.Equals(right);
    }

    [CompilerGenerated]
    public override int GetHashCode() => base.GetHashCode() * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(this.\u003CQuarantinedUntilUtc\u003Ek__BackingField);

    [CompilerGenerated]
    public override bool Equals(object? obj) => this.Equals(obj as TerrapinIngestionValidationReasonQuarantine);

    [CompilerGenerated]
    public override sealed bool Equals(TerrapinIngestionValidationReason? other) => this.Equals((object) other);

    [CompilerGenerated]
    public virtual bool Equals(TerrapinIngestionValidationReasonQuarantine? other)
    {
      if ((object) this == (object) other)
        return true;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return base.Equals((TerrapinIngestionValidationReason) other) && EqualityComparer<DateTime?>.Default.Equals(this.\u003CQuarantinedUntilUtc\u003Ek__BackingField, other.\u003CQuarantinedUntilUtc\u003Ek__BackingField);
    }

    [CompilerGenerated]
    public override TerrapinIngestionValidationReason \u003CClone\u003E\u0024() => (TerrapinIngestionValidationReason) new TerrapinIngestionValidationReasonQuarantine(this);

    [CompilerGenerated]
    protected TerrapinIngestionValidationReasonQuarantine(
      TerrapinIngestionValidationReasonQuarantine original)
      : base((TerrapinIngestionValidationReason) original)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.\u003CQuarantinedUntilUtc\u003Ek__BackingField = original.\u003CQuarantinedUntilUtc\u003Ek__BackingField;
    }

    [CompilerGenerated]
    public void Deconstruct(out 
    #nullable disable
    string Message, out DateTime? QuarantinedUntilUtc)
    {
      Message = this.Message;
      QuarantinedUntilUtc = this.QuarantinedUntilUtc;
    }
  }
}
