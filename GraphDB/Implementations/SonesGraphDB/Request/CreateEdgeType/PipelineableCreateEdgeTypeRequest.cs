/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System.Collections.Generic;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Linq;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a create edge type on the database
    /// </summary>
    public sealed class PipelineableCreateEdgeTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestCreateEdgeType _request;

        /// <summary>
        /// The parentVertex type that has been created during execution
        /// </summary>
        private IEdgeType _createdEdgeType = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable create edge type request
        /// </summary>
        /// <param name="myRequestCreateEdgeType">The create edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableCreateEdgeTypeRequest(RequestCreateEdgeType myRequestCreateEdgeType,
                                                    SecurityToken mySecurity, 
                                                    Int64 myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myRequestCreateEdgeType;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager
                .EdgeTypeManager
                .CheckManager
                .AddTypes(new List<ATypePredefinition> { _request.EdgeTypePredefinition }, 
                            Int64, 
                            SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _createdEdgeType = 
                myMetaManager
                    .EdgeTypeManager
                    .ExecuteManager
                    .AddTypes(new List<ATypePredefinition> { _request.EdgeTypePredefinition },
                                Int64,
                                SecurityToken).FirstOrDefault();
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the myResult of a create parentVertex type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _createdEdgeType);
        }

        #endregion
    }
}