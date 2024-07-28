// Decompiled with JetBrains decompiler
// Type: Nest.FieldSecurityDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FieldSecurityDescriptor<T> : 
    DescriptorBase<FieldSecurityDescriptor<T>, IFieldSecurity>,
    IFieldSecurity
    where T : class
  {
    Fields IFieldSecurity.Except { get; set; }

    Fields IFieldSecurity.Grant { get; set; }

    public FieldSecurityDescriptor<T> Grant(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IFieldSecurity, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Grant = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));

    public FieldSecurityDescriptor<T> Grant(Fields fields) => this.Assign<Fields>(fields, (Action<IFieldSecurity, Fields>) ((a, v) => a.Grant = v));

    public FieldSecurityDescriptor<T> Except(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IFieldSecurity, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Except = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));

    public FieldSecurityDescriptor<T> Except(Fields fields) => this.Assign<Fields>(fields, (Action<IFieldSecurity, Fields>) ((a, v) => a.Except = v));
  }
}
