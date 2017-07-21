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
        private static readonly Dictionary<SubPacketOpcode, (Type Type, SubPacketAttribute Attribute)> opcodeClientSubPackets = new Dictionary<SubPacketOpcode, (Type Type, SubPacketAttribute Attribute)>();
        private static readonly Dictionary<SubPacketOpcode, (Type Type, SubPacketAttribute Attribute)> opcodeServerSubPackets = new Dictionary<SubPacketOpcode, (Type Type, SubPacketAttribute Attribute)>();

        public delegate void SubPacketHandler(Session session, SubPacket subPacket);
        private static readonly Dictionary<SubPacketType, SubPacketHandler> subPacketTypeHandlers = new Dictionary<SubPacketType, SubPacketHandler>();
        private static readonly Dictionary<SubPacketOpcode, SubPacketHandler> subPacketOpcodeHandlers = new Dictionary<SubPacketOpcode, SubPacketHandler>();

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
                    if (attribute.Opcode != SubPacketOpcode.None)
                    {
                        if ((attribute.Direction & SubPacketDirection.Client) != 0)
                            opcodeClientSubPackets[attribute.Opcode] = (type, attribute);
                        if ((attribute.Direction & SubPacketDirection.Server) != 0)
                            opcodeServerSubPackets[attribute.Opcode] = (type, attribute);
                    }
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
                        if (attribute.Opcode != SubPacketOpcode.None)
                            subPacketOpcodeHandlers[attribute.Opcode] = handler;
                        else if (attribute.Type != SubPacketType.None)
                            subPacketTypeHandlers[attribute.Type] = handler;
                    }
                }
            }

            Console.WriteLine($"Initialised {subPacketTypeHandlers.Count + subPacketOpcodeHandlers.Count} packet handler(s) in {sw.ElapsedMilliseconds}ms.");
        }

        private static (Type Type, SubPacketAttribute Attribute) GetSubPacketInfo(SubPacketType type, SubPacketOpcode opcode, SubPacketDirection direction)
        {
            (Type Type, SubPacketAttribute Attribute) subPacketInfo;
            if (opcode != SubPacketOpcode.None)
            {
                if (direction == SubPacketDirection.Client)
                    opcodeClientSubPackets.TryGetValue(opcode, out subPacketInfo);
                else
                    opcodeServerSubPackets.TryGetValue(opcode, out subPacketInfo);
            }
                
            else
                typeSubPackets.TryGetValue(type, out subPacketInfo);

            return subPacketInfo;
        }

        public static SubPacket GetSubPacket(SubPacketType type, SubPacketOpcode opcode, SubPacketDirection direction)
        {
            (Type Type, SubPacketAttribute Attribute) subPacketInfo = GetSubPacketInfo(type, opcode, direction);
            return subPacketInfo.Type != null ? (SubPacket)Activator.CreateInstance(subPacketInfo.Type) : null;
        }

        public static void InvokeHandler(Session session, SubPacket subPacket)
        {
            CLogPacket(SubPacketDirection.Client, subPacket);

            SubPacketHandler handler;
            if (subPacket.SubMessageHeader.Opcode != SubPacketOpcode.None)
                subPacketOpcodeHandlers.TryGetValue(subPacket.SubMessageHeader.Opcode, out handler);
            else
                subPacketTypeHandlers.TryGetValue(subPacket.SubHeader.Type, out handler);

            handler?.Invoke(session, subPacket);
        }
        
        [Conditional("DEBUG")]
        public static void CLogPacket(SubPacketDirection direction, SubPacket subPacket)
        {
            (Type Type, SubPacketAttribute Attribute) subPacketInfo = GetSubPacketInfo(subPacket.SubHeader.Type, subPacket.SubMessageHeader.Opcode, direction);
            if (subPacketInfo.Type == null || subPacketInfo.Attribute.Log)
                Console.WriteLine($"Packet({(direction == SubPacketDirection.Client ? "C->S" : "S->C")}) - Size: {subPacket.SubHeader.Size}, Type: {subPacket.SubHeader.Type}, Opcode: {subPacket.SubMessageHeader.Opcode}");
        }
    }
}
