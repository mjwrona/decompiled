// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.IColumnValue`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public interface IColumnValue<out T> where T : IColumn
  {
    T Column { get; }

    IValue Value { get; }

    StringBuilder CreateFilterCondition(StringBuilder builder, ComparisonOperator op);
  }
}
