﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Request;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.VersionedPluginManager;
using sones.Plugins.GraphDS.DrainPipeLog.Storage;
using System.Threading;

namespace sones.Plugins.GraphDS.DrainPipeLog
{
    /// <summary>
    /// this is a GraphDS plugin which can be used to create a GraphDS bypass if you like. This
    /// plugin will be notified of each and every GQL and API query and can react uppon this
    /// </summary>
    public class DrainPipeLog : IDrainPipe
    {
        private AppendLog _AppendLog = null;
        // this holds the information if this plugin handles requests asyncronously or syncronously
        // this will have a large impact on performance and/or reliability
        private Boolean AsynchronousMode = false;
        // when writing in asynchronous mode everything will be written in a separate thread
        private Thread Async_WriteThread = null;
        // the max number of bytes to hold in the buffer, defaults to 10 MByte
        private Int32 MaximumAsyncBufferSize = 1024*1024*10;    // 10 MB
        private WriteThread WriteThreadInstance = null;        

        #region IPluginable
        public string PluginName
        {
            get { return "DrainPipeLog"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type> 
                { 
                    { "AsynchronousMode", typeof(Boolean) },
                    { "MaximumAsyncBufferSize", typeof(Int32) },
                    { "AppendLogPathAndName", typeof(String) },
                    { "CreateNew", typeof(Boolean) },
                    { "FlushOnWrite", typeof(Boolean) },                    
                };
            }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            #region handle parameters
            String AppendLogPathAndName = "";
            Boolean CreateNew = false;
            Boolean FlushOnWrite = true;

            #region AsynchronousMode
            if (myParameters.ContainsKey("AsynchronousMode"))
            {
                AsynchronousMode = (Boolean)myParameters["AsynchronousMode"];
            }
            else
            {
                AsynchronousMode = false;
            }
            #endregion

            #region MaximumAsyncBufferSize
            if (myParameters.ContainsKey("MaximumAsyncBufferSize"))
            {
                MaximumAsyncBufferSize = (Int32)myParameters["MaximumAsyncBufferSize"];
            }
            else
            {
                MaximumAsyncBufferSize = 1024 * 1024 * 10;
            }
            #endregion

            #region AppendLogPathAndName
            if (myParameters.ContainsKey("AppendLogPathAndName"))
            {
                AppendLogPathAndName = (String)myParameters["AppendLogPathAndName"];
            }
            else
            {
                AppendLogPathAndName = "DrainPipeLog";
            }
            #endregion

            #region CreateNew
            if (myParameters.ContainsKey("CreateNew"))
            {
                CreateNew = (Boolean)myParameters["CreateNew"];
            }
            #endregion

            #region FlushOnWrite
            if (myParameters.ContainsKey("FlushOnWrite"))
            {
                FlushOnWrite = (Boolean)myParameters["FlushOnWrite"];
            }
            #endregion

            #endregion

            _AppendLog = new AppendLog(AppendLogPathAndName,CreateNew,FlushOnWrite);
            WriteThreadInstance = new WriteThread(_AppendLog);

            #region Handle Asynchronous Mode            
            if (AsynchronousMode)
            {                
                Async_WriteThread = new Thread(new ThreadStart(WriteThreadInstance.Run));
            }
            #endregion

            return new DrainPipeLog();
        }
        #endregion
        
        #region IGraphDS
        /// <summary>
        /// Shutdown of this plugin / GraphDS interface handling
        /// </summary>
        /// <param name="mySecurityToken"></param>
        public void Shutdown(sones.Library.Commons.Security.SecurityToken mySecurityToken)
        {
            WriteThreadInstance.Shutdown();

            while (!WriteThreadInstance.ShutdownComplete)
                Thread.Sleep(1);

            // flush and close up
            if (_AppendLog != null)
                _AppendLog.Shutdown();

        }

        /// <summary>
        /// This will receive a query and store it to the log
        /// </summary>
        public sones.GraphQL.Result.QueryResult Query(sones.Library.Commons.Security.SecurityToken mySecurityToken, sones.Library.Commons.Transaction.TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            byte[] Data = null;
            byte[] Part1,Part2,Part3,Part4 = null;
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            #region Generate byte represenation of query
            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, mySecurityToken);
            Part1 = stream.ToArray();

            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, myTransactionToken);
            Part2 = stream.ToArray();

            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, myQueryString);
            Part3 = stream.ToArray();

            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, myQueryLanguageName);
            Part4 = stream.ToArray();

            #endregion
            
            Write(Data);
            return null;
        }
        #endregion

        #region Write
        private void Write(byte[] Data)
        {
            // allow this only once at a time...
            lock (this)
            {
                if (AsynchronousMode)
                {
                    if (MaximumAsyncBufferSize < Data.Length)
                    {
                        // if the Maximum Async Buffer Size if larger than the size of the data we have to write it synchronously
                        // And to write it syncronously we have to make sure we do not write in the middle of the existing queue
                        while (WriteThreadInstance.BytesInAsyncBuffer > 0)
                        {
                            Thread.Sleep(1);
                        }
                        // the buffer is empty now, write the new element
                        _AppendLog.Write(Data);
                        Data = null;
                    }
                    else
                    {
                        if (WriteThreadInstance.BytesInAsyncBuffer + Data.Length <= MaximumAsyncBufferSize)
                        {
                            // yeah, it will fit into the buffer
                            WriteThreadInstance.Write(Data);
                        }
                        else
                        {
                            // obviously the buffer is filled - wait till there's room
                            while (WriteThreadInstance.BytesInAsyncBuffer + Data.Length <= MaximumAsyncBufferSize)
                            {
                                Thread.Sleep(1);
                            }
                            WriteThreadInstance.Write(Data);
                        }
                    }
                }
                else
                {
                    // Syncronous-Mode writes are easy
                    _AppendLog.Write(Data);
                }
            }
        }
        #endregion

        #region IGraphDB Members

        public TResult CreateVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult TraverseVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurity, TransactionToken myTransactionToken, RequestTraverseVertex myRequestTraverseVertex, Converter.TraverseVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertex<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Truncate<TResult>(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestTruncate myRequestTruncate, Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ITransactionable Members

        public TransactionToken BeginTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction(sones.Library.Commons.Security.SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserAuthentication Members

        public sones.Library.Commons.Security.SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(sones.Library.Commons.Security.SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}