﻿using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The different GraphDB access modes
    /// </summary>
    public enum GraphDBAccessMode
    {
        /// <summary>
        /// vertex or edge type changes like alter, create
        /// </summary>
        TypeChange,
        
        /// <summary>
        /// graph traversal, get vertex
        /// </summary>
        ReadOnly,
        
        /// <summary>
        /// insert, update ...
        /// </summary>
        ReadWrite
    }
}