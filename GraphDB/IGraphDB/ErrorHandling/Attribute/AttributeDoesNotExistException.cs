﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if an attribute is used, that does not exists on an specific vertex type or edge type.
    /// </summary>
    public class AttributeDoesNotExistException: AGraphDBAttributeException  
    {
        /// <summary>
        /// The name of the vertex type or edge type that should define the attribute.
        /// </summary>
        /// <remarks><c>NULL</c>, if unknown.</remarks>
        public string TypeName { get; private set; }

        /// <summary>
        /// The name of the attribute that does not exist.
        /// </summary>
        public string AttributeName { get; private set; }

        /// <summary>
        /// Create a new instance of AttributeDoesNotExistException.
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute that does not exist.</param>
        /// <param name="myTypeName">The name of the vertex type or edge type that should define the attribute.</param>
        public AttributeDoesNotExistException(String myAttributeName, String myTypeName = null)
        {
            TypeName = myTypeName;
            AttributeName = myAttributeName;
            _msg = (myTypeName == null)
                ? String.Format("The attribute {0} does not exist.", myAttributeName)
                : String.Format("The attribute {1}.{0} does not exist.", myAttributeName, myTypeName);
        }

    }
}