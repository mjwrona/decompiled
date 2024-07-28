// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.FilterUserColumnValue
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.VisualStudio.Services.Content.Server.Azure;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage
{
  public class FilterUserColumnValue : StringColumnValue<UserColumn>
  {
    private readonly UserColumn column;

    public FilterUserColumnValue(UserColumn column, string filterValue)
      : base(filterValue)
    {
      this.column = column;
    }

    public override UserColumn Column => this.column;
  }
}
