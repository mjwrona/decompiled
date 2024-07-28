// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.NullFilter`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class NullFilter<TColumn> : IFilter<TColumn> where TColumn : IColumn
  {
    public static NullFilter<TColumn> Instance = new NullFilter<TColumn>();

    private NullFilter()
    {
    }

    public bool IsNull => true;

    public StringBuilder CreateFilter(StringBuilder builder) => builder;

    public bool IsMatch(ITableEntityWithColumns entity) => true;
  }
}
