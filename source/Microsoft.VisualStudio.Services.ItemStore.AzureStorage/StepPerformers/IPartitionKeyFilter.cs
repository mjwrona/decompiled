// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers.IPartitionKeyFilter
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers
{
  public interface IPartitionKeyFilter
  {
    IEnumerable<string> Filter(IEnumerable<string> partitionKeys);
  }
}
