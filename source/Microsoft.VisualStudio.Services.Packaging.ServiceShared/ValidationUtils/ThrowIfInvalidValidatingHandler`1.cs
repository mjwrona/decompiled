// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils.ThrowIfInvalidValidatingHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils
{
  public class ThrowIfInvalidValidatingHandler<T> : 
    IAsyncHandler<T>,
    IAsyncHandler<T, NullResult>,
    IHaveInputType<T>,
    IHaveOutputType<NullResult>
  {
    private readonly IConverter<T, Exception> innerValidatingConverter;

    public ThrowIfInvalidValidatingHandler(IConverter<T, Exception> innerValidatingConverter) => this.innerValidatingConverter = innerValidatingConverter;

    public Task<NullResult> Handle(T valueToValidate)
    {
      Exception exception = this.innerValidatingConverter.Convert(valueToValidate);
      if (exception != null)
        throw exception;
      return NullResult.NullTask;
    }
  }
}
