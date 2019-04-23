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
using System.Collections;
using System.Collections.Generic;


namespace foundation
{
    public sealed class ObjectProxyRegistry
    {
        private static ObjectProxyRegistry _instance;
        private static IObjectProxy _defaultObjectProxy;

        private Dictionary<Type, IObjectProxy> _registeredProxies;

        private ObjectProxyRegistry()
        {
            _registeredProxies = new Dictionary<Type, IObjectProxy>();

            _registeredProxies.Add(typeof (ASObject), new ASObjectProxy());
            _registeredProxies.Add(typeof (IExternalizable), new ExternalizableProxy());
            _registeredProxies.Add(typeof (Exception), new ExceptionProxy());
        }

        private static ObjectProxyRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    _defaultObjectProxy = new ObjectProxy();
                    _instance = new ObjectProxyRegistry();
                }
                return _instance;
            }
        }

        public static void RegisterProxies<T>(IObjectProxy value)
        {
            Instance.registerProxies(typeof (T), value);
        }

        private void registerProxies(Type type, IObjectProxy value)
        {
            _registeredProxies.Add(type, value);
        }

        public static IObjectProxy GetObjectProxy(Type type)
        {
            return Instance.getObjectProxy(type);
        }

        private IObjectProxy getObjectProxy(Type type)
        {
            if (type.GetInterface(typeof (IExternalizable).FullName, true) != null)
                return _registeredProxies[typeof (IExternalizable)] as IObjectProxy;
            if (type.GetInterface("INHibernateProxy", false) != null)
            {
                //TODO
                //Quick fix for INHibernateProxy
                type = type.BaseType;
            }
            if (_registeredProxies.ContainsKey(type))
                return _registeredProxies[type] as IObjectProxy;
            foreach (DictionaryEntry entry in (IDictionary) _registeredProxies)
            {
                if (type.IsSubclassOf(entry.Key as Type))
                    return entry.Value as IObjectProxy;
            }
            return _defaultObjectProxy;
        }
    }
}
