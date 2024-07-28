// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesData
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  public class KubernetesData
  {
    public class V1_13
    {
      [DataContract]
      public class ObjectMeta
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Namespace { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string SelfLink { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Uid { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ResourceVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime CreationTimestamp { get; set; }
      }

      [DataContract]
      public class ObjectReference
      {
        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }
      }

      [DataContract]
      public class ServiceAccount
      {
        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ApiVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.ObjectMeta Metadata { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<KubernetesData.V1_13.ObjectReference> Secrets { get; set; }
      }

      [DataContract]
      public class Secret
      {
        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ApiVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.ObjectMeta Metadata { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.SecretData Data { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Type { get; set; }
      }

      [DataContract]
      public class SecretData
      {
        [DataMember(EmitDefaultValue = false, Name = "ca.crt")]
        public string Ca_Crt { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Namespace { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Token { get; set; }
      }

      [DataContract]
      public class Subject
      {
        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Namespace { get; set; }
      }

      [DataContract]
      public class RoleRef
      {
        [DataMember(EmitDefaultValue = false)]
        public string ApiGroup { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Name { get; set; }
      }

      [DataContract]
      public class RoleBinding
      {
        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ApiVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.ObjectMeta Metadata { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<KubernetesData.V1_13.Subject> Subjects { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.RoleRef RoleRef { get; set; }
      }

      [DataContract]
      public class ClusterRoleBinding
      {
        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ApiVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.ObjectMeta Metadata { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public List<KubernetesData.V1_13.Subject> Subjects { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.RoleRef RoleRef { get; set; }
      }

      [DataContract]
      public class Namespace
      {
        [DataMember(EmitDefaultValue = false)]
        public string Kind { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ApiVersion { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public KubernetesData.V1_13.ObjectMeta Metadata { get; set; }
      }
    }
  }
}
