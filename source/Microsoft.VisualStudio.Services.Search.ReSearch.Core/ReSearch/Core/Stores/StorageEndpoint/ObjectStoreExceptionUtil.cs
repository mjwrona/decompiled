// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.ObjectStoreExceptionUtil
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint
{
  internal static class ObjectStoreExceptionUtil
  {
    public static bool IsBlobAccessExceptionSuppressable(Exception ex)
    {
      bool flag = false;
      if (ex is ObjectStoreException && ex.InnerException != null)
      {
        string str = ex.InnerException.Data[(object) "Reason"] != null ? ex.InnerException.Data[(object) "Reason"].ToString() : (string) null;
        if (!string.IsNullOrWhiteSpace(str))
        {
          if (ex.InnerException is EndOfStreamException && str.Equals("BlobMagicReadFailure", StringComparison.OrdinalIgnoreCase))
            flag = true;
          else if (ex.InnerException is IOException && (str.Equals("BlobHeaderMismatch", StringComparison.OrdinalIgnoreCase) || str.Equals("BlobHashMismatch", StringComparison.OrdinalIgnoreCase)))
            flag = true;
        }
      }
      return flag;
    }

    public static string GetBlobAccessErrorLog(Exception ex)
    {
      string empty = string.Empty;
      foreach (object key in (IEnumerable) ex.InnerException.Data.Keys)
        empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "['{0}' : '{1}']\n", key, ex.InnerException.Data[key]);
      return empty;
    }

    public static class Constant
    {
      public const string ExceptionDataReasonKey = "Reason";
      public const string ExceptionDataBlobMagicReadFailure = "BlobMagicReadFailure";
      public const string ExceptionDataBlobHeaderMismatch = "BlobHeaderMismatch";
      public const string ExceptionDataBlobHashMismatch = "BlobHashMismatch";
      public const string ExceptionDataItemEncoderTypeMismatch = "ItemEncoderTypeMismatch";
      public const string ExceptionDataItemTypeMismatch = "ItemTypeMismatch";
    }
  }
}
