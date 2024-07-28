// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.GpgSignatureLengthValidator
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class GpgSignatureLengthValidator : IValidator<long>
  {
    public static OrgLevelRegistrySettingDefinition<long> MaxSignatureSize = OrgLevelRegistrySettingDefinition.Create<long>((RegistryQuery) "/Configuration/Packaging/PyPI/Ingestion/MaxSignatureSize", 2048L);
    private readonly IRegistryService registryService;
    public const long MaxSignatureSizeDefault = 2048;
    public const string MaxSignatureSizeRegistryPath = "/Configuration/Packaging/PyPI/Ingestion/MaxSignatureSize";

    public GpgSignatureLengthValidator(IRegistryService registryService) => this.registryService = registryService;

    public void Validate(long streamLength)
    {
      long num = this.registryService.GetValue<long>((RegistryQuery) "/Configuration/Packaging/PyPI/Ingestion/MaxSignatureSize", 2048L);
      if (streamLength > num)
        throw new InvalidPackageException(Resources.Error_InvalidSignatureTooLong((object) num));
    }
  }
}
