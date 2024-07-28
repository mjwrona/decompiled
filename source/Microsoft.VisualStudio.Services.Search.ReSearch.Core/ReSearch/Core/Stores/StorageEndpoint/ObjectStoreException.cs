// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.ObjectStoreException
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint
{
  [Serializable]
  public class ObjectStoreException : Exception
  {
    public ObjectStoreException()
    {
    }

    public ObjectStoreException(string message)
      : base(message)
    {
    }

    public ObjectStoreException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ObjectStoreException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override string Message
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(base.Message);
        stringBuilder.Append(" [");
        foreach (DictionaryEntry dictionaryEntry in this.Data)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0} = '{1}',", dictionaryEntry.Key, dictionaryEntry.Value);
        stringBuilder.Append("]");
        return stringBuilder.ToString();
      }
    }
  }
}
