/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/*
 * AlterType_ChangeComment
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.NewAPI;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;

using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public class AlterType_ChangeComment : AAlterTypeCommand
    {

        public String NewComment { get; set; }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.ChangeComment; }
        }

        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {
            return myDBContext.DBTypeManager.ChangeCommentOnType(myGraphDBType, NewComment);
        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            var payload = new Dictionary<String, Object>();

            payload.Add("TYPE", myGraphDBType);
            payload.Add("ACTION", "CHANGE COMMENT");
            payload.Add("NEW COMMENT", NewComment);

            return new List<Vertex> { new Vertex(payload) };

        }

    }

}