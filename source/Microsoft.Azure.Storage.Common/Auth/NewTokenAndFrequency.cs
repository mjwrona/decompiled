// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Auth.NewTokenAndFrequency
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;

namespace Microsoft.Azure.Storage.Auth
{
  public struct NewTokenAndFrequency
  {
    public NewTokenAndFrequency(string newToken, TimeSpan? newFrequency = null)
      : this()
    {
      this.Token = newToken;
      this.Frequency = newFrequency;
    }

    public string Token { get; set; }

    public TimeSpan? Frequency { get; set; }
  }
}
