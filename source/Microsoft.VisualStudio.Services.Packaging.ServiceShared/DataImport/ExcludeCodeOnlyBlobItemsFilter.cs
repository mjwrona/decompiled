// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataImport.ExcludeCodeOnlyBlobItemsFilter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.ItemStore.AzureStorage;
using Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataImport
{
  public class ExcludeCodeOnlyBlobItemsFilter : IPartitionKeyFilter
  {
    public IEnumerable<string> Filter(IEnumerable<string> partitionKeys)
    {
      List<string> stringList = new List<string>();
      string str = TableKeyUtility.RowKeyPrefixFromLocator(new Locator(new string[1]
      {
        "CodeOnlyDeploymentsBlobs"
      }));
      foreach (string partitionKey in partitionKeys)
      {
        if (!partitionKey.StartsWith(str))
          stringList.Add(partitionKey);
      }
      return (IEnumerable<string>) stringList;
    }
  }
}
