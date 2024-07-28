// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.SupportedValueHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  public abstract class SupportedValueHelper
  {
    protected readonly IVssRequestContext m_requestContext;
    private string[] m_supported;
    private string[] m_additionalValues;

    public SupportedValueHelper(IVssRequestContext requestContext = null) => this.m_requestContext = requestContext;

    public abstract RegistryQuery RegistryQuery { get; }

    public abstract string DefaultValues { get; }

    public virtual string Separator => ";";

    public virtual RegistryQuery? AdditionalRegistryQuery => new RegistryQuery?();

    protected virtual IEnumerable<string> RestrictedValues => (IEnumerable<string>) Array.Empty<string>();

    public IEnumerable<string> SupportedValues
    {
      get
      {
        if (this.m_supported == null)
          this.m_supported = this.GetSupportedValues();
        return (IEnumerable<string>) this.m_supported;
      }
    }

    public IEnumerable<string> AdditionalValues
    {
      get
      {
        if (this.m_additionalValues == null)
          this.m_additionalValues = this.GetAdditionalValues();
        return (IEnumerable<string>) this.m_additionalValues;
      }
    }

    public void CheckIsSupported(string value)
    {
      if (this.IsSupported(value))
        return;
      this.Throw(value);
    }

    public abstract void Throw(string value);

    public bool IsSupported(string value) => this.SupportedValues.Any<string>((Func<string, bool>) (x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase))) || this.IsAdditionalValue(value);

    public bool IsAdditionalValue(string value) => this.AdditionalValues.Any<string>((Func<string, bool>) (x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase)));

    private string[] GetSupportedValues() => this.NormalizeValues(this.m_requestContext != null ? this.m_requestContext.GetService<IVssRegistryService>().GetValue(this.m_requestContext, this.RegistryQuery, this.DefaultValues) : this.DefaultValues);

    private string[] NormalizeValues(string values)
    {
      if (string.IsNullOrEmpty(values))
        return Array.Empty<string>();
      return ((IEnumerable<string>) values.Split(new string[1]
      {
        this.Separator
      }, StringSplitOptions.RemoveEmptyEntries)).Except<string>(this.RestrictedValues, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToArray<string>();
    }

    private string[] GetAdditionalValues()
    {
      RegistryQuery? additionalRegistryQuery = this.AdditionalRegistryQuery;
      return this.NormalizeValues(this.m_requestContext == null || !additionalRegistryQuery.HasValue ? string.Empty : this.m_requestContext.GetService<IVssRegistryService>().GetValue(this.m_requestContext, additionalRegistryQuery.Value, string.Empty));
    }
  }
}
