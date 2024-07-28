// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.PIIPropertyProcessor
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class PIIPropertyProcessor : IPiiPropertyProcessor
  {
    private const string NotHashedPropertySuffix = ".NotHashed";
    private const string Key = "959069c9-9e93-4fa1-bf16-3f8120d7db0c";
    private static readonly ThreadLocal<HashAlgorithm> Encrypter = new ThreadLocal<HashAlgorithm>(new Func<HashAlgorithm>(PIIPropertyProcessor.CreateEncryptor));

    public string BuildRawPropertyName(string propertyName) => propertyName + ".NotHashed";

    public bool CanAddRawValue(IEventProcessorContext eventProcessorContext) => eventProcessorContext.HostTelemetrySession.CanCollectPrivateInformation;

    public object ConvertToRawValue(object value)
    {
      TelemetryHashedProperty telemetryHashedProperty = value as TelemetryHashedProperty;
      telemetryHashedProperty.RequiresArgumentNotNull<TelemetryHashedProperty>("hashedProperty");
      return telemetryHashedProperty.RawValue;
    }

    public string ConvertToHashedValue(object value)
    {
      TelemetryHashedProperty telemetryHashedProperty = value as TelemetryHashedProperty;
      telemetryHashedProperty.RequiresArgumentNotNull<TelemetryHashedProperty>("hashedProperty");
      return this.HashPropertyValue(telemetryHashedProperty.StringValue);
    }

    public Type TypeOfPiiProperty() => typeof (TelemetryPiiProperty);

    public Type TypeOfHashedProperty() => typeof (TelemetryHashedProperty);

    private static HashAlgorithm CreateEncryptor() => (HashAlgorithm) new HMACSHA256(Encoding.UTF8.GetBytes("959069c9-9e93-4fa1-bf16-3f8120d7db0c"));

    private string HashPropertyValue(string value) => BitConverter.ToString(PIIPropertyProcessor.Encrypter.Value.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", string.Empty);
  }
}
