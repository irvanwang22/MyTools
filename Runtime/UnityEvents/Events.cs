using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace MyTools.UnityEvents
{
    // Basics
    [Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    [Serializable]
    public class IntEvent : UnityEvent<int> { }

    [Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Serializable]
    public class StringEvent : UnityEvent<string> { }

    [Serializable]
    public class VoidEvent : UnityEvent{ }

    [Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }

    // Additionals
    [Serializable]
    public class Vector2Event : UnityEvent<Vector2> { }

    [Serializable]
    public class Vector3Event : UnityEvent<Vector3> { }

    [Serializable]
    public class TransformEvent : UnityEvent<Transform> { }

    [Serializable]
    public class QuaternionEvent : UnityEvent<Quaternion> { }

    [Serializable]
    public class ColorEvent : UnityEvent<Color> { }

    [Serializable]
    public class Color32Event : UnityEvent<Color32> { }

    [Serializable]
    public class SpriteEvent : UnityEvent<Sprite> { }

    [Serializable]
    public class RectTransformEvent : UnityEvent<RectTransform> { }

    [Serializable]
    public class AudioClipEvent : UnityEvent<AudioClip> { }

    [Serializable]
    public class LocaleEvent : UnityEvent<Locale> { }
}