// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.MessageBusCredentials
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal struct MessageBusCredentials : IEquatable<MessageBusCredentials>
  {
    public MessageBusCredentials(string sharedAccessKeyName, string sharedAccessKeyValue)
      : this()
    {
      this.SharedAccessKeyName = sharedAccessKeyName;
      this.SharedAccessKeyValue = sharedAccessKeyValue;
      this.HasValue = !string.IsNullOrEmpty(sharedAccessKeyName) && !string.IsNullOrEmpty(sharedAccessKeyValue);
    }

    public TokenProvider CreateTokenProvider() => TokenProvider.CreateSharedAccessSignatureTokenProvider(this.SharedAccessKeyName, this.SharedAccessKeyValue);

    public bool Equals(MessageBusCredentials other) => StringComparer.Ordinal.Equals(this.SharedAccessKeyName, other.SharedAccessKeyName) && StringComparer.Ordinal.Equals(this.SharedAccessKeyValue, other.SharedAccessKeyValue);

    public string SharedAccessKeyName { get; private set; }

    public string SharedAccessKeyValue { get; private set; }

    public bool HasValue { get; private set; }
  }
}
