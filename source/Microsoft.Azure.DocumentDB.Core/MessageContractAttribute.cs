// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.MessageContractAttribute
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Net.Security;

namespace Microsoft.Azure.Documents
{
  internal sealed class MessageContractAttribute : Attribute
  {
    public bool HasProtectionLevel => false;

    public bool IsWrapped
    {
      get => false;
      set
      {
      }
    }

    public ProtectionLevel ProtectionLevel
    {
      get => ProtectionLevel.None;
      set
      {
      }
    }

    public string WrapperName
    {
      get => (string) null;
      set
      {
      }
    }

    public string WrapperNamespace
    {
      get => (string) null;
      set
      {
      }
    }
  }
}
