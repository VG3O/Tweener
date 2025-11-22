using UnityEngine;
using UnityEngine.Splines;

namespace VG
{
    namespace Tweener
    {
        // Struct to hold a multitude of different values for tweening
        public readonly struct ValueContainer 
        {
            public readonly float _number;
            public readonly Vector2 _vec2;
            public readonly Vector3 _vec3;
            public readonly Vector4 _vec4;
            public readonly Quaternion _quat;
            public readonly Color _color;
            public readonly Rect _rect;
            public readonly BezierKnot _knot;
            
            public ValueContainer(float value) : this() { _number = value; }
            public ValueContainer(Vector2 value) : this() { _vec2 = value; }
            public ValueContainer(Vector3 value) : this() { _vec3 = value; }
            public ValueContainer(Vector4 value) : this() { _vec4 = value; }
            public ValueContainer(Quaternion value) : this() { _quat = value; }
            public ValueContainer(Color value) : this() { _color = value; }
            public ValueContainer(Rect value) : this() { _rect = value; }
            public ValueContainer(BezierKnot value) : this() { _knot = value; }
        }
    }
}