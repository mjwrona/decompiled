// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssJsonCollectionWrapperBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  public abstract class VssJsonCollectionWrapperBase : ISecuredObject
  {
    private IEnumerable _value;

    protected VssJsonCollectionWrapperBase()
    {
    }

    public VssJsonCollectionWrapperBase(IEnumerable source)
    {
      this.Count = source != null ? (!(source is ICollection) ? source.Cast<object>().Count<object>() : ((ICollection) source).Count) : 0;
      this._value = source;
    }

    [DataMember(Order = 0)]
    public int Count { get; private set; }

    protected IEnumerable BaseValue
    {
      get => this._value;
      set => this._value = value;
    }

    Guid ISecuredObject.NamespaceId => throw new NotImplementedException();

    int ISecuredObject.RequiredPermissions => throw new NotImplementedException();

    string ISecuredObject.GetToken() => throw new NotImplementedException();
  }
}
