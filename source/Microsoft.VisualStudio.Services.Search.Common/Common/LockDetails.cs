// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.LockDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public sealed class LockDetails
  {
    public LockDetails(string resourceName, LockMode lockMode, string lockOwner)
    {
      if (string.IsNullOrWhiteSpace(resourceName))
        throw new ArgumentNullException(nameof (resourceName));
      if (string.IsNullOrWhiteSpace(lockOwner))
        throw new ArgumentNullException(nameof (lockOwner));
      this.ResourceName = resourceName;
      this.LockMode = lockMode;
      this.LockOwner = lockOwner;
    }

    public string ResourceName { get; }

    public LockMode LockMode { get; }

    public string LockOwner { get; }

    public override int GetHashCode() => this.ResourceName.GetHashCode() ^ this.LockMode.ToString().GetHashCode() ^ this.LockOwner.GetHashCode();

    public override bool Equals(object obj) => obj is LockDetails lockDetails && this.ResourceName == lockDetails.ResourceName && this.LockMode == lockDetails.LockMode && this.LockOwner == lockDetails.LockOwner;

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("ResourceName :{0}, LockMode: {1}, LockOwner {2}", (object) this.ResourceName, (object) this.LockMode, (object) this.LockOwner));

    public static void ValidateLockingRequirements(IList<LockDetails> lockingRequirements)
    {
      if (lockingRequirements == null || !lockingRequirements.Any<LockDetails>())
        throw new ArgumentException("lockingRequirements is null or empty");
      HashSet<string> stringSet = new HashSet<string>();
      foreach (LockDetails lockingRequirement in (IEnumerable<LockDetails>) lockingRequirements)
      {
        string str = lockingRequirement.ResourceName + lockingRequirement.LockOwner;
        if (stringSet.Contains(str))
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("locking requirements are not correct, {0} is trying to acquire multiple locks on resource {1}", (object) lockingRequirement.LockOwner, (object) lockingRequirement.ResourceName)));
        stringSet.Add(str);
      }
    }

    public static string ToString(IList<LockDetails> lockDetailsList)
    {
      if (lockDetailsList == null)
        throw new ArgumentNullException(nameof (lockDetailsList));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      foreach (LockDetails lockDetails in (IEnumerable<LockDetails>) lockDetailsList)
      {
        stringBuilder.Append("(");
        stringBuilder.Append((object) lockDetails);
        stringBuilder.Append(")");
      }
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
