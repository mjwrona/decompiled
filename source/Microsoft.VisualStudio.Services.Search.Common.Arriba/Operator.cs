// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba
{
  [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Operator", Justification = "Suppressing it as it will not confuse any search REST API users")]
  public enum Operator
  {
    Equals,
    NotEquals,
    StartsWith,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    In,
    Matches,
    MatchesExact,
    Near,
    Before,
    After,
  }
}
