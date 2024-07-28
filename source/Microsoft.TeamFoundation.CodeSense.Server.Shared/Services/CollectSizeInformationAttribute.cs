// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.CollectSizeInformationAttribute
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  public sealed class CollectSizeInformationAttribute : Attribute
  {
    public CollectSizeInformationAttribute(string counterUri, string counterBaseUri)
    {
      this.CounterUri = counterUri;
      this.CounterBaseUri = counterBaseUri;
    }

    public string CounterUri { get; private set; }

    public string CounterBaseUri { get; private set; }
  }
}
