using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Network
{
    public static class PacketManager
    {
        private static readonly Dictionary<SubPacketType, (Type Type, SubPacketAttribute Attribute)> typeSubPackets = new Dictionary<SubPacketType, (Type Type, SubPacketAttribute Attribute)>();
        private static readonly Dictionary<SubPacketClientOpcode, (Type Type, SubPacketAttribute Attribute)> opcodeClientSubPackets = new Dictionary<SubPacketClientOpcode, (Type Type, SubPacketAttribute Attribute)>();
        private static readonly Dictionary<SubPacketServerOpcode, (Type Type, SubPacketAttribute Attribute)> opcodeServerSubPackets = new Dictionary<SubPacketServerOpcode, (Type Type, SubPacketAttribute Attribute)>();

        public delegate void SubPacketHandler(Session session, SubPacket subPacket);
        private static readonly Dictionary<SubPacketType, (SubPacketHandler Handler, SubPacketHandlerAttribute Attribute)> subPacketTypeHandlers = new Dictionary<SubPacketType, (SubPacketHandler Handler, SubPacketHandlerAttribute Attribute)>();
        private static readonly Dictionary<SubPacketClientOpcode, (SubPacketHandler Handler, SubPacketHandlerAttribute Attribute)> subPacketOpcodeHandlers = new Dictionary<SubPacketClientOpcode, (SubPacketHandler Handler, SubPacketHandlerAttribute Attribute)>();

        public static void Initialise()
        {
            InitialisePackets();
            InitialisePacketHandlers();
        }

        private static void InitialisePackets()
        {
            var sw = new Stopwatch();
            sw.Start();

            foreach (Type type in Assembly.GetEntryAssembly().GetTypes().Concat(Assembly.GetExecutingAssembly().GetTypes()))
            {
                foreach (SubPacketAttribute attribute in type.GetCustomAttributes<SubPacketAttribute>())
                {
                    if (attribute.ClientOpcode != SubPacketClientOpcode.None)
                        opcodeClientSubPackets[attribute.ClientOpcode] = (type, attribute);
                    else if (attribute.ServerOpcode != SubPacketServerOpcode.None)
                        opcodeServerSubPackets[attribute.ServerOpcode] = (type, attribute);
                    else if (attribute.Type != SubPacketType.None)
                        typeSubPackets[attribute.Type] = (type, attribute);
                }
            }

            Console.WriteLine($"Initialised {typeSubPackets.Count + opcodeClientSubPackets.Count + opcodeServerSubPackets.Count} packet(s) in {sw.ElapsedMilliseconds}ms.");
        }

        private static void InitialisePacketHandlers()
        {
            var sw = new Stopwatch();
            sw.Start();

            foreach (Type type in Assembly.GetEntryAssembly().GetTypes().Concat(Assembly.GetExecutingAssembly().GetTypes()))
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    foreach (SubPacketHandlerAttribute attribute in method.GetCustomAttributes<SubPacketHandlerAttribute>())
                    {
                        ParameterInfo[] handlerParameters = method.GetParameters();
                        Debug.Assert(handlerParameters.Length == 2);
                        Debug.Assert(handlerParameters[0].ParameterType == typeof(Session) || handlerParameters[0].ParameterType.IsSubclassOf(typeof(Session)));
                        Debug.Assert(handlerParameters[1].ParameterType == typeof(SubPacket) || handlerParameters[1].ParameterType.IsSubclassOf(typeof(SubPacket)));

                        ParameterExpression sessionParameter   = Expression.Parameter(typeof(Session));
                        ParameterExpression subPacketParameter = Expression.Parameter(typeof(SubPacket));
                        MethodCallExpression callExpression = Expression.Call(method,
                            Expression.Convert(sessionParameter, handlerParameters[0].ParameterType),
                            Expression.Convert(subPacketParameter, handlerParameters[1].ParameterType));

                        Expression<SubPacketHandler> lambda = Expression.Lambda<SubPacketHandler>(callExpression, sessionParameter, subPacketParameter);

                        SubPacketHandler handler = lambda.Compile();
                        if (attribute.ClientOpcode != SubPacketClientOpcode.None)
                            subPacketOpcodeHandlers[attribute.ClientOpcode] = (handler, attribute);
                        else if (attribute.Type != SubPacketType.None)
                            subPacketTypeHandlers[attribute.Type] = (handler, attribute);
                    }
                }
            }

            Console.WriteLine($"Initialised {subPacketTypeHandlers.Count + subPacketOpcodeHandlers.Count} packet handler(s) in {sw.ElapsedMilliseconds}ms.");
        }

        private static (Type Type, SubPacketAttribute Attribute) GetSubPacketInfo(SubPacketType type, SubPacketClientOpcode clientOpcode, SubPacketServerOpcode serverOpcode)
        {
            (Type Type, SubPacketAttribute Attribute) subPacketInfo;
            if (clientOpcode != SubPacketClientOpcode.None)
                opcodeClientSubPackets.TryGetValue(clientOpcode, out subPacketInfo);
            else if (serverOpcode != SubPacketServerOpcode.None)
                opcodeServerSubPackets.TryGetValue(serverOpcode, out subPacketInfo);
            else
                typeSubPackets.TryGetValue(type, out subPacketInfo);

            return subPacketInfo;
        }

        public static SubPacketHandlerAttribute GetSubPacketHandlerInfo(SubPacket subPacket)
        {
            return _GetSubPacketHandlerInfo(subPacket).Attribute;
        }

        private static (SubPacketHandler Handler, SubPacketHandlerAttribute Attribute) _GetSubPacketHandlerInfo(SubPacket subPacket)
        {
            Debug.Assert(subPacket != null);

            (SubPacketHandler Handler, SubPacketHandlerAttribute Attribute) info;
            if ((SubPacketClientOpcode)subPacket.SubMessageHeader.Opcode != SubPacketClientOpcode.None)
                subPacketOpcodeHandlers.TryGetValue((SubPacketClientOpcode)subPacket.SubMessageHeader.Opcode, out info);
            else
                subPacketTypeHandlers.TryGetValue(subPacket.SubHeader.Type, out info);

            return info;
        }

        public static SubPacket GetSubPacket(SubPacketType type, SubPacketClientOpcode clientOpcode, SubPacketServerOpcode serverOpcode)
        {
            (Type Type, SubPacketAttribute Attribute) subPacketInfo = GetSubPacketInfo(type, clientOpcode, serverOpcode);
            return subPacketInfo.Type != null ? (SubPacket)Activator.CreateInstance(subPacketInfo.Type) : null;
        }

        public static void InvokeHandler(Session session, SubPacket subPacket)
        {
            CLogPacket(SubPacketDirection.Client, subPacket);

            (SubPacketHandler Handler, SubPacketHandlerAttribute Attribute) info = _GetSubPacketHandlerInfo(subPacket);
            info.Handler?.Invoke(session, subPacket);
        }

        [Conditional("DEBUG")]
        public static void CLogPacket(SubPacketDirection direction, SubPacket subPacket)
        {
            (Type Type, SubPacketAttribute Attribute) subPacketInfo;
            if (direction == SubPacketDirection.Client)
            {
                subPacketInfo = GetSubPacketInfo(subPacket.SubHeader.Type, (SubPacketClientOpcode)subPacket.SubMessageHeader.Opcode, SubPacketServerOpcode.None);
                if(subPacketInfo.Type == null || subPacketInfo.Attribute.Log)
                    Console.WriteLine($"Packet(C->S) - Size: {subPacket.SubHeader.Size}, Type: {subPacket.SubHeader.Type}, Opcode: {(SubPacketClientOpcode)subPacket.SubMessageHeader.Opcode}");
            }
            else
            {
                subPacketInfo = GetSubPacketInfo(subPacket.SubHeader.Type, SubPacketClientOpcode.None, (SubPacketServerOpcode)subPacket.SubMessageHeader.Opcode);
                if (subPacketInfo.Type == null || subPacketInfo.Attribute.Log)
                    Console.WriteLine($"Packet(S->C) - Size: {subPacket.SubHeader.Size}, Type: {subPacket.SubHeader.Type}, Opcode: {(SubPacketServerOpcode)subPacket.SubMessageHeader.Opcode}");
            }
        }
    }
}
