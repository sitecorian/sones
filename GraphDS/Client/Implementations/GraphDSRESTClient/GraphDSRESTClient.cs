﻿using System;
using sones.GraphDB.Request;
using sones.GraphDS;
using sones.GraphQL.Result;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDSClient
{
    /// <summary>
    /// A GraphDS client that communicates via REST
    /// </summary>
    public sealed class GraphDSRESTClient : IGraphDSClient
    {
        #region IGraphDS Members

        public GraphDB.IGraphDB GraphDB
        {
            get { throw new NotImplementedException(); }
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserAuthentication Members

        public SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}