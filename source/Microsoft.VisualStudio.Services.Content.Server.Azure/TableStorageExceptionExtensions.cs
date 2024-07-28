// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TableStorageExceptionExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class TableStorageExceptionExtensions
  {
    private static readonly Regex ErrorText = new Regex("Unexpected response code for operation : ([0-9]+)", RegexOptions.IgnoreCase);

    public static bool TryGetFailedOperationIndex(
      this StorageException exception,
      TableBatchOperationDescriptor batchToExecute,
      out int index)
    {
      index = -1;
      if (batchToExecute.Count == 1)
      {
        index = 0;
        return true;
      }
      if (exception.RequestInformation.ExtendedErrorInformation != null)
      {
        string errorMessage = exception.RequestInformation.ExtendedErrorInformation.ErrorMessage;
        if (errorMessage != null)
        {
          string[] strArray = errorMessage.Split(':');
          if (strArray.Length <= 1)
            return false;
          return int.TryParse(strArray[0], out index) || TableStorageExceptionExtensions.TryGetFailedOperationIndexFromMessageText(exception.Message, out index);
        }
      }
      return false;
    }

    private static bool TryGetFailedOperationIndexFromMessageText(string message, out int index)
    {
      index = -1;
      Match match = TableStorageExceptionExtensions.ErrorText.Match(message);
      return match.Success && int.TryParse(match.Groups[1].Captures[0].Value, out index);
    }
  }
}
