// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.TableItemExceptionMapper
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage
{
  public class TableItemExceptionMapper : IEnumeratorExceptionMapper
  {
    private const string EntityTooLargeString = "EntityTooLarge";

    private TableItemExceptionMapper()
    {
    }

    public static IEnumeratorExceptionMapper Instance => (IEnumeratorExceptionMapper) new TableItemExceptionMapper();

    public bool TryMapException(Exception ex, out Exception mappedException)
    {
      bool flag = false;
      if (ex is ExpandedTableStorageException storageException)
      {
        RequestResult requestInformation = storageException.RequestInformation;
        if (requestInformation != null)
        {
          if (requestInformation.HttpStatusCode == 413)
            flag = true;
          if (requestInformation.ExtendedErrorInformation?.ErrorCode == "EntityTooLarge")
            flag = true;
        }
      }
      if (flag)
      {
        mappedException = (Exception) new ItemTooBigException(ex.Message);
        return true;
      }
      mappedException = (Exception) null;
      return false;
    }
  }
}
