/*
 * EasingCore (https://github.com/setchi/EasingCore)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/EasingCore/blob/master/LICENSE)
 */

namespace UnityEngine.UI.Extensions.EasingCore
{
    public enum Ease
    {
        Linear,
        InBack,
        InBounce,
        InCirc,
        InCubic,
        InElastic,
        InExpo,
        InQuad,
        InQuart,
        InQuint,
        InSine,
        OutBack,
        OutBounce,
        OutCirc,
        OutCubic,
        OutElastic,
        OutExpo,
        OutQuad,
        OutQuart,
        OutQuint,
        OutSine,
        InOutBack,
        InOutBounce,
        InOutCirc,
        InOutCubic,
        InOutElastic,
        InOutExpo,
        InOutQuad,
        InOutQuart,
        InOutQuint,
        InOutSine,
    }

    public delegate float EasingFunction(float t);

    public static class Easing
    {
        /// <summary>
        /// Gets the easing function
        /// </summary>
        /// <param name="type">Ease type</param>
        /// <returns>Easing function</returns>
        public static EasingFunction Get(Ease type)
        {
            switch (type)
            {
                case Ease.Linear: return linear;
                case Ease.InBack: return inBack;
                case Ease.InBounce: return inBounce;
                case Ease.InCirc: return inCirc;
                case Ease.InCubic: return inCubic;
                case Ease.InElastic: return inElastic;
                case Ease.InExpo: return inExpo;
                case Ease.InQuad: return inQuad;
                case Ease.InQuart: return inQuart;
                case Ease.InQuint: return inQuint;
                case Ease.InSine: return inSine;
                case Ease.OutBack: return outBack;
                case Ease.OutBounce: return outBounce;
                case Ease.OutCirc: return outCirc;
                case Ease.OutCubic: return outCubic;
                case Ease.OutElastic: return outElastic;
                case Ease.OutExpo: return outExpo;
                case Ease.OutQuad: return outQuad;
                case Ease.OutQuart: return outQuart;
                case Ease.OutQuint: return outQuint;
                case Ease.OutSine: return outSine;
                case Ease.InOutBack: return inOutBack;
                case Ease.InOutBounce: return inOutBounce;
                case Ease.InOutCirc: return inOutCirc;
                case Ease.InOutCubic: return inOutCubic;
                case Ease.InOutElastic: return inOutElastic;
                case Ease.InOutExpo: return inOutExpo;
                case Ease.InOutQuad: return inOutQuad;
                case Ease.InOutQuart: return inOutQuart;
                case Ease.InOutQuint: return inOutQuint;
                case Ease.InOutSine: return inOutSine;
                default: return linear;
            }
        }

        static float linear(float t)
        {
            return t;
        }

        static float inBack(float t)
        {
            return t * t * t - t * Mathf.Sin(t * Mathf.PI);
        }

        static float outBack(float t)
        {
            return 1f - inBack(1f - t);
        }

        static float inOutBack(float t)
        {
            return t < 0.5f
? 0.5f * inBack(2f * t)
: 0.5f * outBack(2f * t - 1f) + 0.5f;
        }

        static float inBounce(float t)
        {
            return 1f - outBounce(1f - t);
        }

        static float outBounce(float t)
        {
            return t < 4f / 11.0f ?
(121f * t * t) / 16.0f :
t < 8f / 11.0f ?
(363f / 40.0f * t * t) - (99f / 10.0f * t) + 17f / 5.0f :
t < 9f / 10.0f ?
(4356f / 361.0f * t * t) - (35442f / 1805.0f * t) + 16061f / 1805.0f :
(54f / 5.0f * t * t) - (513f / 25.0f * t) + 268f / 25.0f;
        }

        static float inOutBounce(float t)
        {
            return t < 0.5f
? 0.5f * inBounce(2f * t)
: 0.5f * outBounce(2f * t - 1f) + 0.5f;
        }

        static float inCirc(float t)
        {
            return 1f - Mathf.Sqrt(1f - (t * t));
        }

        static float outCirc(float t)
        {
            return Mathf.Sqrt((2f - t) * t);
        }

        static float inOutCirc(float t)
        {
            return t < 0.5f
? 0.5f * (1 - Mathf.Sqrt(1f - 4f * (t * t)))
: 0.5f * (Mathf.Sqrt(-((2f * t) - 3f) * ((2f * t) - 1f)) + 1f);
        }

        static float inCubic(float t)
        {
            return t * t * t;
        }

        static float outCubic(float t)
        {
            return inCubic(t - 1f) + 1f;
        }

        static float inOutCubic(float t)
        {
            return t < 0.5f
? 4f * t * t * t
: 0.5f * inCubic(2f * t - 2f) + 1f;
        }

        static float inElastic(float t)
        {
            return Mathf.Sin(13f * (Mathf.PI * 0.5f) * t) * Mathf.Pow(2f, 10f * (t - 1f));
        }

        static float outElastic(float t)
        {
            return Mathf.Sin(-13f * (Mathf.PI * 0.5f) * (t + 1)) * Mathf.Pow(2f, -10f * t) + 1f;
        }

        static float inOutElastic(float t)
        {
            return t < 0.5f
? 0.5f * Mathf.Sin(13f * (Mathf.PI * 0.5f) * (2f * t)) * Mathf.Pow(2f, 10f * ((2f * t) - 1f))
: 0.5f * (Mathf.Sin(-13f * (Mathf.PI * 0.5f) * ((2f * t - 1f) + 1f)) * Mathf.Pow(2f, -10f * (2f * t - 1f)) + 2f);
        }

        static float inExpo(float t)
        {
            return Mathf.Approximately(0.0f, t) ? t : Mathf.Pow(2f, 10f * (t - 1f));
        }

        static float outExpo(float t)
        {
            return Mathf.Approximately(1.0f, t) ? t : 1f - Mathf.Pow(2f, -10f * t);
        }

        static float inOutExpo(float v)
        {
            return Mathf.Approximately(0.0f, v) || Mathf.Approximately(1.0f, v)
? v
: v < 0.5f
? 0.5f * Mathf.Pow(2f, (20f * v) - 10f)
: -0.5f * Mathf.Pow(2f, (-20f * v) + 10f) + 1f;
        }

        static float inQuad(float t)
        {
            return t * t;
        }

        static float outQuad(float t)
        {
            return -t * (t - 2f);
        }

        static float inOutQuad(float t)
        {
            return t < 0.5f
? 2f * t * t
: -2f * t * t + 4f * t - 1f;
        }

        static float inQuart(float t)
        {
            return t * t * t * t;
        }

        static float outQuart(float t)
        {
            var u = t - 1f;
            return u * u * u * (1f - t) + 1f;
        }

        static float inOutQuart(float t)
        {
            return t < 0.5f
? 8f * inQuart(t)
: -8f * inQuart(t - 1f) + 1f;
        }

        static float inQuint(float t)
        {
            return t * t * t * t * t;
        }

        static float outQuint(float t)
        {
            return inQuint(t - 1f) + 1f;
        }

        static float inOutQuint(float t)
        {
            return t < 0.5f
? 16f * inQuint(t)
: 0.5f * inQuint(2f * t - 2f) + 1f;
        }

        static float inSine(float t)
        {
            return Mathf.Sin((t - 1f) * (Mathf.PI * 0.5f)) + 1f;
        }

        static float outSine(float t)
        {
            return Mathf.Sin(t * (Mathf.PI * 0.5f));
        }

        static float inOutSine(float t)
        {
            return 0.5f * (1f - Mathf.Cos(t * Mathf.PI));
        }
    }
}