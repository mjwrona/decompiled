// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssJsonCollectionWrapper`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  public sealed class VssJsonCollectionWrapper<T> : VssJsonCollectionWrapperBase
  {
    public VssJsonCollectionWrapper()
    {
    }

    public VssJsonCollectionWrapper(IEnumerable source)
      : base(source)
    {
    }

    [DataMember]
    public T Value
    {
      get => (T) this.BaseValue;
      private set => this.BaseValue = (IEnumerable) (object) value;
    }
  }
}
