// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.EntityMemberInfoToTableMapper
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class EntityMemberInfoToTableMapper
  {
    private static Lazy<EntityMemberInfoToTableMapper> _instance = new Lazy<EntityMemberInfoToTableMapper>(LazyThreadSafetyMode.ExecutionAndPublication);
    private Dictionary<MemberInfo, HashSet<string>> _mapper;

    public static EntityMemberInfoToTableMapper Instance => EntityMemberInfoToTableMapper._instance.Value;

    public EntityMemberInfoToTableMapper()
    {
      this._mapper = new Dictionary<MemberInfo, HashSet<string>>();
      IEnumerable<Type> types = ((IEnumerable<Type>) Assembly.GetExecutingAssembly().GetTypes()).Where<Type>((Func<Type, bool>) (c => c.IsClass && c.Namespace == "Microsoft.VisualStudio.Services.Analytics.Model"));
      HashSet<MemberInfo> memberInfoSet = new HashSet<MemberInfo>();
      foreach (Type type in types)
      {
        memberInfoSet.Add((MemberInfo) type);
        memberInfoSet.UnionWith((IEnumerable<MemberInfo>) type.GetMembers());
      }
      foreach (MemberInfo memberInfo in memberInfoSet)
      {
        IEnumerable<string> strings = memberInfo.GetCustomAttributes<ModelTableMapping>().Select<ModelTableMapping, string>((Func<ModelTableMapping, string>) (a => a.ModelTable));
        if (strings.Count<string>() > 0)
        {
          if (!this._mapper.ContainsKey(memberInfo))
            this._mapper.Add(memberInfo, strings.ToHashSet<string>());
          else
            this._mapper[memberInfo].UnionWith(strings);
        }
      }
    }

    public ISet<string> GetModelTables(IEnumerable<MemberInfo> members)
    {
      HashSet<string> modelTables = new HashSet<string>();
      foreach (MemberInfo member in members)
      {
        if (this._mapper.ContainsKey(member))
          modelTables.UnionWith((IEnumerable<string>) this._mapper[member]);
      }
      return (ISet<string>) modelTables;
    }
  }
}
