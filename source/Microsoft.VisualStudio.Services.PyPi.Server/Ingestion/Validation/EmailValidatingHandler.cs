// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.EmailValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class EmailValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    private static readonly TimeSpan ParseTimeout = TimeSpan.FromSeconds(1.0);

    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      if (request.IngestionDirection == IngestionDirection.PullFromUpstream)
        return Task.FromResult<NullResult>((NullResult) null);
      if (request.ProtocolSpecificInfo.Metadata.AuthorEmail != null)
        EmailValidatingHandler.ParseEmail(request.ProtocolSpecificInfo.Metadata.AuthorEmail);
      if (request.ProtocolSpecificInfo.Metadata.MaintainerEmail != null)
        EmailValidatingHandler.ParseEmail(request.ProtocolSpecificInfo.Metadata.MaintainerEmail);
      return Task.FromResult<NullResult>((NullResult) null);
    }

    private static EmailSpec ParseEmail(string input)
    {
      try
      {
        return EmailParser.ParseEmail(input, EmailValidatingHandler.ParseTimeout);
      }
      catch (OperationCanceledException ex)
      {
        throw new InvalidPackageException(Resources.Error_EmailParseTookTooLong((object) input), (Exception) ex);
      }
      catch (InvalidEmailException ex)
      {
        throw new InvalidPackageException(ex.Message, (Exception) ex);
      }
    }
  }
}
