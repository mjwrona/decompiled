// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.DescriptionContentTypeValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class DescriptionContentTypeValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private const string MarkdownContentType = "text/markdown";
    private readonly string[] allowedDescriptionContentTypes = new string[3]
    {
      "text/plain",
      "text/x-rst",
      "text/markdown"
    };
    private readonly string[] allowedCharsets = new string[1]
    {
      "UTF-8"
    };
    private readonly string[] allowedMarkdownVariants = new string[2]
    {
      "GFM",
      "CommonMark"
    };

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      if (request.ProtocolSpecificInfo.Metadata.DescriptionContentType == null)
        return Task.FromResult<NullResult>((NullResult) null);
      MediaTypeHeaderValue parsedValue;
      if (!MediaTypeHeaderValue.TryParse(request.ProtocolSpecificInfo.Metadata.DescriptionContentType, out parsedValue))
        throw new InvalidPackageException(Resources.Error_InvalidDescriptionContentType());
      if (!((IEnumerable<string>) this.allowedDescriptionContentTypes).Contains<string>(parsedValue.MediaType))
        throw new InvalidPackageException(Resources.Error_UnsupportedDescriptionContentType((object) parsedValue.MediaType, (object) string.Join(",", this.allowedDescriptionContentTypes)));
      ICollection<NameValueHeaderValue> parameters = parsedValue.Parameters;
      Dictionary<string, string> dictionary = (parameters != null ? parameters.ToDictionary<NameValueHeaderValue, string, string>((Func<NameValueHeaderValue, string>) (x => x.Name), (Func<NameValueHeaderValue, string>) (x => x.Value)) : (Dictionary<string, string>) null) ?? new Dictionary<string, string>();
      if (dictionary.ContainsKey("charset") && !((IEnumerable<string>) this.allowedCharsets).Contains<string>(dictionary["charset"]))
        throw new InvalidPackageException(Resources.Error_UnsupportedDescriptionContentTypeCharset((object) dictionary["charset"], (object) string.Join(",", this.allowedCharsets)));
      if (parsedValue.MediaType.Equals("text/markdown") && dictionary.ContainsKey("variant") && !((IEnumerable<string>) this.allowedMarkdownVariants).Contains<string>(dictionary["variant"]))
        throw new InvalidPackageException(Resources.Error_UnsupportedMarkdownVariant((object) dictionary["variant"], (object) string.Join(",", this.allowedMarkdownVariants)));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
