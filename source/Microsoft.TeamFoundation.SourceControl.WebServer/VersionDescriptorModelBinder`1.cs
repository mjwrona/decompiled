// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.VersionDescriptorModelBinder`1
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public abstract class VersionDescriptorModelBinder<TVersionDescriptor> : IModelBinder
  {
    public string Prefix { get; set; }

    protected abstract string ExpectedServiceArea { get; }

    public VersionDescriptorModelBinder() => this.Prefix = "";

    protected string GetPrefix(ModelBindingContext bindingContext)
    {
      if (bindingContext.ModelName.StartsWith("base", StringComparison.OrdinalIgnoreCase))
        return "base";
      return bindingContext.ModelName.StartsWith("target", StringComparison.OrdinalIgnoreCase) ? "target" : "";
    }

    protected bool TryGetVersion(ModelBindingContext bindingContext, out string result)
    {
      ValueProviderResult valueProviderResult = this.GetValueProviderResult(bindingContext, "version");
      if (valueProviderResult?.RawValue is IList<string>)
        throw new ArgumentException(Resources.Get("AmbiguousVersion")).Expected(this.ExpectedServiceArea);
      result = valueProviderResult == null ? (string) null : (string) valueProviderResult.RawValue;
      return result != null;
    }

    protected bool TryGetVersionType<TVersionType>(
      ModelBindingContext bindingContext,
      out TVersionType result)
      where TVersionType : struct
    {
      string str = "versionType";
      return this.TryParseValueProvider<TVersionType>(this.GetValueProviderResult(bindingContext, str), str, out result);
    }

    protected bool TryGetVersionOptions<TVersionOption>(
      ModelBindingContext bindingContext,
      string paramName,
      out TVersionOption result)
      where TVersionOption : struct
    {
      return this.TryParseValueProvider<TVersionOption>(this.GetValueProviderResult(bindingContext, paramName), paramName, out result);
    }

    private ValueProviderResult GetValueProviderResult(
      ModelBindingContext bindingContext,
      string propertyName)
    {
      return bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + propertyName) ?? bindingContext.ValueProvider.GetValue(this.Prefix + propertyName);
    }

    protected bool TryParseValueProvider<TEnum>(
      ValueProviderResult value,
      string paramName,
      out TEnum result)
      where TEnum : struct
    {
      if (value != null)
      {
        string str = !(value.RawValue is IList<string>) ? (string) value.RawValue : throw new ArgumentException(Resources.Get("AmbiguousVersion")).Expected(this.ExpectedServiceArea);
        if (Enum.TryParse<TEnum>(str, true, out result))
          return true;
        throw new ArgumentException(Resources.Format("InvalidQueryParam", (object) str, (object) paramName, (object) string.Join(",", Enum.GetNames(typeof (TEnum))))).Expected(this.ExpectedServiceArea);
      }
      result = default (TEnum);
      return false;
    }

    public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
    {
      if (bindingContext.ModelType != typeof (TVersionDescriptor))
        return false;
      this.Prefix = this.GetPrefix(bindingContext);
      bindingContext.Model = (object) this.CreateVersionDescriptor(bindingContext);
      return true;
    }

    protected abstract TVersionDescriptor CreateVersionDescriptor(ModelBindingContext bindingContext);
  }
}
