﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceOutgoingEdgePredefinition : ServiceAttributePredefinition
    {
        #region Constant

        /// <summary>
        /// The name of the predefined edge type that represents a normal edge.
        /// </summary>
        [DataMember]
        public const string Edge = "Edge";

        /// <summary>
        /// The name of the predefined edge type that represents a edges with an attribute Weight of type double.
        /// </summary>
        [DataMember]
        public const string WeightedEdge = "Weighted";

        /// <summary>
        /// The name of the predefined edge type that represents a edges with an attribute Order.
        /// </summary>
        [DataMember]
        public const string OrderedEdge = "Ordered";

        #endregion
        
        
        
        /// <summary>
        /// The edge type of this edge definition
        /// </summary>
        [DataMember]
        public String EdgeType;


        /// <summary>
        /// The multiplicity of the edge.
        /// </summary>
        [DataMember]
        public ServiceEdgeMultiplicity Multiplicity;

        /// <summary>
        /// The inner edge type of a multi edge.
        /// </summary>
        [DataMember]
        public string InnerEdgeType;

        public OutgoingEdgePredefinition ToOutgoingEdgePredefinition()
        {
            OutgoingEdgePredefinition OutgoingEdgePreDef = new OutgoingEdgePredefinition(this.AttributeName);

            OutgoingEdgePreDef.SetAttributeType(this.AttributeType);
            OutgoingEdgePreDef.SetComment(this.Comment);
            OutgoingEdgePreDef.InnerEdgeType = this.InnerEdgeType;

            if (this.Multiplicity == ServiceEdgeMultiplicity.HyperEdge)
                OutgoingEdgePreDef.SetMultiplicityAsHyperEdge();
            if (this.Multiplicity == ServiceEdgeMultiplicity.MultiEdge)
                OutgoingEdgePreDef.SetMultiplicityAsMultiEdge();

            return OutgoingEdgePreDef;
        }
    }
}