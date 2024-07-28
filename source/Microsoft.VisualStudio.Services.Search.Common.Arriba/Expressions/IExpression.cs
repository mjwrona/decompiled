// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.IExpression
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions
{
  public interface IExpression : IEnumerable<IExpression>, IEnumerable
  {
    IExpression[] Children { get; set; }
  }
}
