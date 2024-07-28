// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssCamelCasePropertyNamesPreserveEnumsContractResolver
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json.Serialization;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.WebApi
{
  internal class VssCamelCasePropertyNamesPreserveEnumsContractResolver : 
    CamelCasePropertyNamesContractResolver
  {
    protected override 
    #nullable disable
    JsonDictionaryContract CreateDictionaryContract(Type type)
    {
      JsonDictionaryContract dictionaryContract = base.CreateDictionaryContract(type);
      Type keyType = dictionaryContract.DictionaryKeyType;
      int num = keyType != (Type) null ? (keyType.IsEnum ? 1 : 0) : 0;
      dictionaryContract.DictionaryKeyResolver = num == 0 ? (Func<string, string>) (name => name) : (Func<string, string>) (name => ((int) Enum.Parse(keyType, name)).ToString());
      return dictionaryContract;
    }
  }
}
