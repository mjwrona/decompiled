// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.IAccount
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Admin;
using System.Security;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public interface IAccount
  {
    AccountType AccountType { get; set; }

    string UserName { get; }

    string Domain { get; }

    string FullName { get; set; }

    string ProvidedName { get; set; }

    bool IsLocalAccount { get; }

    SecureString Password { get; set; }

    string SidAsString { get; }

    SecurityIdentifier SecurityIdentifier { get; }
  }
}
