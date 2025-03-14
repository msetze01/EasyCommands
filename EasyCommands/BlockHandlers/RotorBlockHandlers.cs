﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        static Func<IMyMotorStator, bool> IsHinge = b => b.BlockDefinition.SubtypeId.Contains("Hinge");

        public class RotorBlockHandler : FunctionalBlockHandler<IMyMotorStator> {
            Func<IMyMotorStator, bool> blockFilter;
            public RotorBlockHandler(Func<IMyMotorStator, bool> filter) {
                AddPropertyHandler(Property.ANGLE, new RotorAngleHandler());
                AddDirectionHandlers(Property.RANGE, Direction.UP,
                    TypeHandler(NumericHandler(b => b.UpperLimitDeg, (b,v) => b.UpperLimitDeg = v, 10), Direction.UP, Direction.FORWARD, Direction.CLOCKWISE),
                    TypeHandler(NumericHandler(b => b.LowerLimitDeg, (b, v) => b.LowerLimitDeg= v, 10), Direction.DOWN, Direction.BACKWARD, Direction.COUNTERCLOCKWISE));
                AddNumericHandler(Property.VELOCITY, (b) => b.TargetVelocityRPM, (b, v) => b.TargetVelocityRPM = v, 1);
                AddNumericHandler(Property.LEVEL, (b) => b.Displacement, (b, v) => b.Displacement = v, 0.1f);
                AddBooleanHandler(Property.CONNECTED, b => b.IsAttached, (b, v) => { if (v) b.Attach(); else b.Detach(); });
                AddBooleanHandler(Property.LOCKED, b => b.RotorLock, (b, v) => b.RotorLock = v);
                AddNumericHandler(Property.STRENGTH, b => b.Torque, (b,v) => b.Torque = v, 1000);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.ANGLE;
                defaultPropertiesByDirection.Add(Direction.UP, Property.LEVEL);
                defaultPropertiesByDirection.Add(Direction.DOWN, Property.LEVEL);
                defaultPropertiesByDirection.Add(Direction.CLOCKWISE, Property.ANGLE);
                defaultPropertiesByDirection.Add(Direction.COUNTERCLOCKWISE, Property.ANGLE);
                defaultDirection = Direction.CLOCKWISE;
                blockFilter = filter;
            }

            public override List<IMyMotorStator> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector) =>
                base.GetBlocksOfType(selector).Where(blockFilter).ToList();

            public override List<IMyMotorStator> GetBlocksOfTypeInGroup(string groupName) =>
                base.GetBlocksOfTypeInGroup(groupName).Where(blockFilter).ToList();
        }

        public class RotorAngleHandler : PropertyHandler<IMyMotorStator> {
            public RotorAngleHandler() {
                Get = (b, p) => ResolvePrimitive(b.Angle * (float)(180 / Math.PI));
                GetDirection = (b, p, d) => Get(b, p);
                Set = (b, p, v) => RotateToValue(b, v);
                SetDirection = (b, p, d, v) => RotateToValue(b, v, d);
                IncrementValueDirection = (b, p, d, v) => {
                    if (d == Direction.CLOCKWISE || d == Direction.UP) RotateToValue(b, Get(b, p).Plus(v), d);
                    if (d == Direction.COUNTERCLOCKWISE || d == Direction.DOWN) RotateToValue(b, Get(b, p).Minus(v), d);
                };
                IncrementValue = (b, p, v) => IncrementValueDirection(b, p, Direction.CLOCKWISE, v);
                Increment = (b, p) => IncrementValue(b, p, ResolvePrimitive(10));
                Move = (b, p, d) => {
                    if (d == Direction.CLOCKWISE) b.TargetVelocityRPM = Math.Abs(b.TargetVelocityRPM);
                    if (d == Direction.COUNTERCLOCKWISE) b.TargetVelocityRPM = -Math.Abs(b.TargetVelocityRPM);
                };
                Reverse = (b, p) => b.TargetVelocityRPM *= -1;
            }
        }

        static void RotateToValue(IMyMotorStator rotor, Primitive primitive) {
            if(primitive.returnType!=Return.NUMERIC) {
                throw new Exception("Cannot rotate rotor to non-numeric value: " + primitive);
            }

            float value = CastNumber(primitive);
            float newValue = GetCorrectedAngle(value);

            //TODO: We might find that in some cases, it's faster to go the other way.
            if (rotor.Angle * (180 / Math.PI) < value) {
                rotor.UpperLimitDeg = newValue;
                rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
            } else {
                rotor.LowerLimitDeg = newValue;
                rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
            }
        }

        static void RotateToValue(IMyMotorStator rotor, Primitive primitive, Direction direction) {
            if (primitive.returnType != Return.NUMERIC) {
                throw new Exception("Cannot rotate rotor to non-numeric value: " + primitive);
            }

            float value = GetCorrectedAngle(CastNumber(primitive));
            float currentAngle = rotor.Angle * (180 / (float)Math.PI);

            switch (direction) {
                case Direction.CLOCKWISE:
                    if (value < currentAngle) value = GetCorrectedAngle(value + 360);
                    rotor.UpperLimitDeg = value;
                    rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
                    break;
                case Direction.COUNTERCLOCKWISE:
                    if (value > currentAngle) value = GetCorrectedAngle(value - 360);
                    rotor.LowerLimitDeg = value;
                    rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
                    break;
                default:
                    RotateToValue(rotor, primitive);
                    break;
            }
        }

        static float GetCorrectedAngle(float angle) {
            float newAngle = angle;
            if (newAngle > 360) newAngle %= 360;
            else if (newAngle < -360) {
                newAngle = -((-newAngle) % 360);
            }
            return newAngle;
        }
    }
}
