// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.TypeCastException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class TypeCastException : ExpressionException
  {
    private readonly string m_message;

    internal TypeCastException(Type fromType, Type toType)
      : base((ISecretMasker) null, string.Empty)
    {
      this.FromType = fromType;
      this.ToType = toType;
      this.m_message = ExpressionResources.TypeCastErrorNoValue((object) fromType.Name, (object) toType.Name);
    }

    internal TypeCastException(
      ISecretMasker secretMasker,
      object value,
      ValueKind fromKind,
      ValueKind toKind)
      : base((ISecretMasker) null, string.Empty)
    {
      this.Value = value;
      this.FromKind = new ValueKind?(fromKind);
      this.ToKind = new ValueKind?(toKind);
      this.m_message = ExpressionResources.TypeCastError((object) fromKind, (object) toKind, (object) ExpressionUtil.FormatValue(secretMasker, value, fromKind));
    }

    internal TypeCastException(
      ISecretMasker secretMasker,
      object value,
      ValueKind fromKind,
      Type toType)
      : base((ISecretMasker) null, string.Empty)
    {
      this.Value = value;
      this.FromKind = new ValueKind?(fromKind);
      this.ToType = toType;
      this.m_message = ExpressionResources.TypeCastError((object) fromKind, (object) toType, (object) ExpressionUtil.FormatValue(secretMasker, value, fromKind));
    }

    internal TypeCastException(
      ISecretMasker secretMasker,
      object value,
      ValueKind fromKind,
      Type toType,
      string error)
      : base((ISecretMasker) null, string.Empty)
    {
      this.Value = value;
      this.FromKind = new ValueKind?(fromKind);
      this.ToType = toType;
      this.m_message = ExpressionResources.TypeCastErrorWithError((object) fromKind, (object) toType, (object) ExpressionUtil.FormatValue(secretMasker, value, fromKind), secretMasker != null ? (object) secretMasker.MaskSecrets(error) : (object) error);
    }

    public override string Message => this.m_message;

    internal object Value { get; }

    internal ValueKind? FromKind { get; }

    internal Type FromType { get; }

    internal ValueKind? ToKind { get; }

    internal Type ToType { get; }
  }
}
