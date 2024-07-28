// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TrackingKeyBase
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public abstract class TrackingKeyBase : ITrackingKey
  {
    public abstract TimeTrackingKeyType Type { get; }

    protected TrackingKeyBase(string raw, bool allowEmpty) => this.Raw = allowEmpty || !string.IsNullOrEmpty(raw) ? raw : throw new ArgumentException("The time tracking key is empty.");

    public string Raw { get; private set; }

    public virtual string RegistryPath => string.Format("{0}/{1}", (object) this.Type, (object) this.Raw);

    public override bool Equals(object obj) => obj is TrackingKeyBase trackingKeyBase && this.Type == trackingKeyBase.Type && this.Raw == trackingKeyBase.Raw;

    public override int GetHashCode() => (548224262 * -1521134295 + this.Type.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Raw);
  }
}
