﻿using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The type is already referenced
    /// </summary>
    public sealed class DuplicateReferenceOccurrenceException : AGraphQLException
    {
        public String TypeName { get; private set; }
                
        /// <summary>
        /// Creates a new DuplicateReferenceOccurrenceException exception
        /// </summary>
        /// <param name="myType">The name of the type</param>
        public DuplicateReferenceOccurrenceException(String myTypeName)
        {
            TypeName = myTypeName;
            _msg = String.Format("There is already a reference for type \"{0}\"!", TypeName);
        }
        
    }
}