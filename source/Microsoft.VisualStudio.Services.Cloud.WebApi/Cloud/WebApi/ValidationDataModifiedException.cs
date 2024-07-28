// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ValidationDataModifiedException
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [Serializable]
  public class ValidationDataModifiedException : DataImportException
  {
    public ValidationDataModifiedException(string message)
      : base(message)
    {
    }

    public ValidationDataModifiedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ValidationDataModifiedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
