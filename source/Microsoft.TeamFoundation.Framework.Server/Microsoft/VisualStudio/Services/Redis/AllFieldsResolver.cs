// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.AllFieldsResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Redis
{
  internal class AllFieldsResolver : DefaultContractResolver
  {
    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
      HashSet<FieldInfo> source = new HashSet<FieldInfo>((IEqualityComparer<FieldInfo>) AllFieldsResolver.FieldInfoComparer.Instance);
      for (; objectType != typeof (object) && objectType != (Type) null; objectType = objectType.BaseType)
      {
        FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        source.UnionWith((IEnumerable<FieldInfo>) fields);
      }
      return ((IEnumerable<MemberInfo>) source).ToList<MemberInfo>();
    }

    protected override JsonProperty CreateProperty(
      MemberInfo member,
      MemberSerialization memberSerialization)
    {
      JsonProperty property = base.CreateProperty(member, memberSerialization);
      property.Readable = true;
      property.Writable = true;
      property.Ignored = false;
      return property;
    }

    private class FieldInfoComparer : IEqualityComparer<FieldInfo>
    {
      public static readonly AllFieldsResolver.FieldInfoComparer Instance = new AllFieldsResolver.FieldInfoComparer();

      public bool Equals(FieldInfo x, FieldInfo y) => x.DeclaringType == y.DeclaringType && x.Name == y.Name;

      public int GetHashCode(FieldInfo obj) => obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
    }
  }
}
