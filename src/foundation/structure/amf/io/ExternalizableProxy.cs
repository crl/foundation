/*
	FluorineFx open source library 
	Copyright (C) 2007 Zoltan Csibi, zoltan@TheSilentGroup.com, FluorineFx.com 
	
	This library is free software; you can redistribute it and/or
	modify it under the terms of the GNU Lesser General Public
	License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.
	
	This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	Lesser General Public License for more details.
	
	You should have received a copy of the GNU Lesser General Public
	License along with this library; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
using System;

namespace foundation
{
    class ExternalizableProxy : IObjectProxy
    {
        #region IObjectProxy Members

        public bool GetIsExternalizable(object instance)
        {
            return true;
        }

        public bool GetIsDynamic(object instance)
        {
            return false;
        }

        public ClassDefinition GetClassDefinition(object instance)
        {
            Type type = instance.GetType();
            string customClassName =  ObjectFactory.GetCustomClass(type);
            ClassDefinition classDefinition = new ClassDefinition(customClassName, ClassDefinition.EmptyClassMembers, true, false);
            return classDefinition;
        }

        public object GetValue(object instance, ClassMember member)
        {
            throw new NotSupportedException();
        }

        public void SetValue(object instance, ClassMember member, object value)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
