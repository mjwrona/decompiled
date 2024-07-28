// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.Serializers.ValidationCommandSerializer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using NuGet.Services.ServiceBus;
using NuGet.Services.Validation;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.Serializers
{
  public class ValidationCommandSerializer : 
    ICommandSerializer<PackageValidationMessageData, IBrokeredMessage>
  {
    private readonly IServiceBusMessageSerializer _validationRequestSerializer = (IServiceBusMessageSerializer) new ServiceBusMessageSerializer();

    public PackageValidationMessageData Deserialize(IBrokeredMessage message) => this._validationRequestSerializer.DeserializePackageValidationMessageData(message);

    public IBrokeredMessage Serialize(PackageValidationMessageData message) => this._validationRequestSerializer.SerializePackageValidationMessageData(message);
  }
}
