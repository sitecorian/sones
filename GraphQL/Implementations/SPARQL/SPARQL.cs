﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;
using sones.Library.Internal.Token;
using sones.GraphDB.Transaction;

namespace sones.GraphQL
{
    /// <summary>
    /// A SPARQL implementation
    /// </summary>
    public sealed class SPARQL : IGraphQL
    {
        #region IGraphQL

        public QueryResult Query(SessionToken mySessionToken, TransactionToken myTransactionToken, string myQueryString)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return "SPARQL"; }
        }

        public IEnumerable<string> ExportGraphDDL(SessionToken mySessionToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML(SessionToken mySessionToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}