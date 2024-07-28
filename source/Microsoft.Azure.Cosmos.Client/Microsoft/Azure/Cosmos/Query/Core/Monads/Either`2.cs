// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Monads.Either`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Monads
{
  internal readonly struct Either<TLeft, TRight>
  {
    private readonly TLeft left;
    private readonly TRight right;

    private Either(TLeft left, TRight right, bool isLeft)
    {
      this.left = left;
      this.right = right;
      this.IsLeft = isLeft;
    }

    public bool IsLeft { get; }

    public bool IsRight => !this.IsLeft;

    public void Match(Action<TLeft> onLeft, Action<TRight> onRight)
    {
      if (this.IsLeft)
        onLeft(this.left);
      else
        onRight(this.right);
    }

    public TResult Match<TResult>(Func<TLeft, TResult> onLeft, Func<TRight, TResult> onRight) => !this.IsLeft ? onRight(this.right) : onLeft(this.left);

    public TLeft FromLeft(TLeft defaultValue) => !this.IsLeft ? defaultValue : this.left;

    public TRight FromRight(TRight defaultValue) => !this.IsRight ? defaultValue : this.right;

    public override bool Equals(object obj)
    {
      if ((ValueType) this == obj)
        return true;
      return obj is Either<TLeft, TRight> other && this.Equals(other);
    }

    public bool Equals(Either<TLeft, TRight> other) => this.IsLeft == other.IsLeft && (!this.IsLeft ? this.right.Equals((object) other.right) : this.left.Equals((object) other.left));

    public override int GetHashCode()
    {
      int num = 0 ^ this.IsLeft.GetHashCode();
      return !this.IsLeft ? num ^ this.right.GetHashCode() : num ^ this.left.GetHashCode();
    }

    public static implicit operator Either<TLeft, TRight>(TLeft left) => new Either<TLeft, TRight>(left, default (TRight), true);

    public static implicit operator Either<TLeft, TRight>(TRight right) => new Either<TLeft, TRight>(default (TLeft), right, false);
  }
}
