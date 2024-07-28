// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinIngestionValidationReasonQuarantinePending
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class TerrapinIngestionValidationReasonQuarantinePending : 
    TerrapinIngestionValidationReason,
    IEquatable<
    #nullable disable
    TerrapinIngestionValidationReasonQuarantinePending>
  {
    public TerrapinIngestionValidationReasonQuarantinePending(string Message)
      : base("QuarantinedPendingAssessment", Message)
    {
    }

    [CompilerGenerated]
    protected override 
    #nullable enable
    Type EqualityContract => typeof (TerrapinIngestionValidationReasonQuarantinePending);

    [CompilerGenerated]
    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      builder.Append(nameof (TerrapinIngestionValidationReasonQuarantinePending));
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
      TerrapinIngestionValidationReasonQuarantinePending? left,
      TerrapinIngestionValidationReasonQuarantinePending? right)
    {
      return !(left == right);
    }

    [CompilerGenerated]
    public static bool operator ==(
      TerrapinIngestionValidationReasonQuarantinePending? left,
      TerrapinIngestionValidationReasonQuarantinePending? right)
    {
      if ((object) left == (object) right)
        return true;
      return (object) left != null && left.Equals(right);
    }

    [CompilerGenerated]
    public override int GetHashCode() => base.GetHashCode();

    [CompilerGenerated]
    public override bool Equals(object? obj) => this.Equals(obj as TerrapinIngestionValidationReasonQuarantinePending);

    [CompilerGenerated]
    public override sealed bool Equals(TerrapinIngestionValidationReason? other) => this.Equals((object) other);

    [CompilerGenerated]
    public virtual bool Equals(
      TerrapinIngestionValidationReasonQuarantinePending? other)
    {
      return (object) this == (object) other || base.Equals((TerrapinIngestionValidationReason) other);
    }

    [CompilerGenerated]
    public override TerrapinIngestionValidationReason \u003CClone\u003E\u0024() => (TerrapinIngestionValidationReason) new TerrapinIngestionValidationReasonQuarantinePending(this);

    [CompilerGenerated]
    protected TerrapinIngestionValidationReasonQuarantinePending(
      TerrapinIngestionValidationReasonQuarantinePending original)
      : base((TerrapinIngestionValidationReason) original)
    {
    }

    [CompilerGenerated]
    public void Deconstruct(out 
    #nullable disable
    string Message) => Message = this.Message;
  }
}
