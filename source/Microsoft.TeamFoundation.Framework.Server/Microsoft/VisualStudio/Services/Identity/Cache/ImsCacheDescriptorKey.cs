// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheDescriptorKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [TypeConverter(typeof (ImsCacheDescriptorKeyTypeConverter))]
  internal class ImsCacheDescriptorKey : ImsCacheKey<IdentityDescriptor>
  {
    internal ImsCacheDescriptorKey()
    {
    }

    internal ImsCacheDescriptorKey(IdentityDescriptor id)
      : base(id)
    {
    }

    internal override string Serialize() => this.Id.ToString();

    internal static bool TryParse(string input, out ImsCacheDescriptorKey result)
    {
      result = (ImsCacheDescriptorKey) null;
      try
      {
        string[] strArray = input.Split(new char[1]{ ';' }, 2);
        if (strArray.Length == 2)
        {
          if (!string.IsNullOrEmpty(strArray[0]))
          {
            if (!string.IsNullOrEmpty(strArray[1]))
            {
              result = new ImsCacheDescriptorKey(new IdentityDescriptor(strArray[0], strArray[1]));
              return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        return false;
      }
      return false;
    }

    public override object Clone() => (object) new ImsCacheDescriptorKey(this.Id == (IdentityDescriptor) null ? (IdentityDescriptor) null : new IdentityDescriptor(this.Id.IdentityType, this.Id.Identifier));

    protected override bool IdEquals(object other) => IdentityDescriptorComparer.Instance.Equals(this.Id, other as IdentityDescriptor);

    public override int GetHashCode() => !(this.Id == (IdentityDescriptor) null) ? IdentityDescriptorComparer.Instance.GetHashCode(this.Id) : -1;
  }
}
