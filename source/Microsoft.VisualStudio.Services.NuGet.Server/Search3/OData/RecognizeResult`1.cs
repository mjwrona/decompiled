// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData.RecognizeResult`1
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData
{
  public class RecognizeResult<T>
  {
    public bool Succeeded { get; }

    public T Result { get; }

    public ImmutableList<Exception> Errors { get; }

    public RecognizeResult(bool succeeded, T result)
    {
      this.Succeeded = succeeded;
      this.Result = result;
      this.Errors = ImmutableList<Exception>.Empty;
      if (succeeded)
        return;
      this.Errors = this.Errors.Add(new Exception("unspecified error"));
    }

    public RecognizeResult(T result)
      : this(true, result)
    {
    }

    public RecognizeResult(Exception ex)
      : this((IEnumerable<Exception>) ImmutableList.Create<Exception>(ex))
    {
    }

    public RecognizeResult(IEnumerable<Exception> errors)
    {
      this.Succeeded = false;
      this.Result = default (T);
      this.Errors = errors.ToImmutableList<Exception>();
    }
  }
}
