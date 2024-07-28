// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.IResponseExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  public static class IResponseExtensions
  {
    public static string SerializeRequestAndResponse(this IResponse queryResponse)
    {
      try
      {
        return FormattableString.Invariant(FormattableStringFactory.Create("ResponseStatus: {0}{1}", (object) queryResponse.ApiCall.HttpStatusCode, (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Request: {0}{1}", (object) queryResponse.ApiCall.Uri, (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("{0}", queryResponse.ApiCall.RequestBodyInBytes == null ? (object) string.Empty : (object) Encoding.UTF8.GetString(queryResponse.ApiCall.RequestBodyInBytes).CompactJson()));
      }
      catch (Exception ex)
      {
        return FormattableString.Invariant(FormattableStringFactory.Create("{0} failed with exception [{1}].", (object) nameof (SerializeRequestAndResponse), (object) ex));
      }
    }
  }
}
