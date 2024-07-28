// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AccountResourceRequest
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AccountResourceRequest
  {
    public string Location { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public Dictionary<string, string> Properties { get; set; }

    public AccountResourceRequestOperationType OperationType { get; set; }

    public string Upn { get; set; }

    public string AccountName { get; set; }

    public string RequestSource { get; set; }
  }
}
