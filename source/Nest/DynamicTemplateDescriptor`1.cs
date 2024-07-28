// Decompiled with JetBrains decompiler
// Type: Nest.DynamicTemplateDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DynamicTemplateDescriptor<T> : 
    DescriptorBase<DynamicTemplateDescriptor<T>, IDynamicTemplate>,
    IDynamicTemplate
    where T : class
  {
    IProperty IDynamicTemplate.Mapping { get; set; }

    string IDynamicTemplate.Match { get; set; }

    string IDynamicTemplate.MatchMappingType { get; set; }

    MatchType? IDynamicTemplate.MatchPattern { get; set; }

    string IDynamicTemplate.PathMatch { get; set; }

    string IDynamicTemplate.PathUnmatch { get; set; }

    string IDynamicTemplate.Unmatch { get; set; }

    public DynamicTemplateDescriptor<T> Match(string match) => this.Assign<string>(match, (Action<IDynamicTemplate, string>) ((a, v) => a.Match = v));

    public DynamicTemplateDescriptor<T> MatchPattern(MatchType? matchPattern) => this.Assign<MatchType?>(matchPattern, (Action<IDynamicTemplate, MatchType?>) ((a, v) => a.MatchPattern = v));

    public DynamicTemplateDescriptor<T> Unmatch(string unMatch) => this.Assign<string>(unMatch, (Action<IDynamicTemplate, string>) ((a, v) => a.Unmatch = v));

    public DynamicTemplateDescriptor<T> MatchMappingType(string matchMappingType) => this.Assign<string>(matchMappingType, (Action<IDynamicTemplate, string>) ((a, v) => a.MatchMappingType = v));

    public DynamicTemplateDescriptor<T> PathMatch(string pathMatch) => this.Assign<string>(pathMatch, (Action<IDynamicTemplate, string>) ((a, v) => a.PathMatch = v));

    public DynamicTemplateDescriptor<T> PathUnmatch(string pathUnmatch) => this.Assign<string>(pathUnmatch, (Action<IDynamicTemplate, string>) ((a, v) => a.PathUnmatch = v));

    public DynamicTemplateDescriptor<T> Mapping(
      Func<SingleMappingSelector<T>, IProperty> mappingSelector)
    {
      return this.Assign<Func<SingleMappingSelector<T>, IProperty>>(mappingSelector, (Action<IDynamicTemplate, Func<SingleMappingSelector<T>, IProperty>>) ((a, v) => a.Mapping = v != null ? v(new SingleMappingSelector<T>()) : (IProperty) null));
    }
  }
}
