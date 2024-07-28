// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.NewKeyParameters
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System.Collections.Generic;

namespace Microsoft.Azure.KeyVault.Models
{
  public class NewKeyParameters
  {
    public string Kty { get; set; }

    public string CurveName { get; set; }

    public int? KeySize { get; set; }

    public IList<string> KeyOps { get; set; }

    public KeyAttributes Attributes { get; set; }

    public IDictionary<string, string> Tags { get; set; }
  }
}
