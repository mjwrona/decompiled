// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Auth.StorageAccountKey
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

namespace Microsoft.Azure.Storage.Auth
{
  internal struct StorageAccountKey
  {
    internal string KeyName;
    internal byte[] KeyValue;

    public StorageAccountKey(string keyName, byte[] keyValue)
    {
      this.KeyName = keyName;
      this.KeyValue = keyValue;
    }
  }
}
