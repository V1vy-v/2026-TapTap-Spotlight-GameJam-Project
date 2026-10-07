Shader "Hidden/Arui/AruiPostEffect1_0_URP"
{
    Properties
    {
        _MainTex ("Screen", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;
            float _AruiUnscaledTime;

            float _ShakeWeight;
            float _ShakeStrength;
            float4 _ShakeAmplitude;
            float _ShakeFrequency;
            float _ShakeRotation;
            float4 _ShakeCenter;

            float _ColorSplitWeight;
            float _ColorSplitMode;
            float _ColorSplitStrength;
            float4 _ColorSplitCenter;
            float _ColorSplitDirection;
            float _ColorSplitEdgeBoost;
            float _ColorSplitRBScale;

            float _BlurWeight;
            float _BlurMode;
            float4 _BlurCenter;
            float _BlurRadius;
            float _BlurMix;
            float _BlurSamples;

            float _LetterboxWeight;
            float4 _LetterboxColor;
            float _LetterboxTopBottom;
            float _LetterboxLeftRight;
            float _LetterboxSoftness;
            float _LetterboxOpacity;

            float _RadialRayWeight;
            float _RadialRayMode;
            float4 _RadialRayColor;
            float4 _RadialRayCenter;
            float _RadialRayCount;
            float _RadialRayWidth;
            float _RadialRayGlow;
            float _RadialRayRotation;
            float _RadialRayFlowSpeed;
            float _RadialRayFlicker;
            float _RadialRayDarken;

            float _WideWeight;
            float _WideDistortion;
            float4 _WideCenter;

            float _VignetteWeight;
            float4 _VignetteCenter;
            float4 _VignetteColor;
            float _VignetteIntensity;
            float _VignetteRange;
            float _VignetteSoftness;
            float _VignetteShapeMode;
            float4 _VignetteScale;
            float _VignetteCornerRoundness;

            float _SkillBWWeight;
            float _SkillBWMode;
            float4 _SkillBWBlackColor;
            float4 _SkillBWWhiteColor;
            float _SkillBWThreshold;
            float _SkillBWSoftness;
            float _SkillBWContrast;
            float _SkillBWSwitchCount;
            float _SkillBWProgress;
            float _SkillBWEdgeStrength;
            float _SkillBWEdgeWidth;
            float _SkillBWEdgeThreshold;

            float _MangaWeight;
            float _MangaMode;
            float4 _MangaInkColor;
            float4 _MangaPaperColor;
            float _MangaThreshold;
            float _MangaSoftness;
            float _MangaContrast;
            float4 _MangaCenter;
            float _MangaBrushCount;
            float _MangaBrushWidth;
            float _MangaBrushLength;
            float _MangaInnerBlank;
            float _MangaIrregularity;
            float _MangaBreakup;
            float _MangaBrushStrength;
            float _MangaEdgeStrength;
            float _MangaEdgeWidth;
            float _MangaEdgeThreshold;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float2 ClampUv(float2 uv)
            {
                return clamp(uv, 0.001, 0.999);
            }

            float2 ApplyWideAngle(float2 uv)
            {
                if (_WideWeight <= 0.0001 || abs(_WideDistortion) <= 0.00001)
                {
                    return ClampUv(uv);
                }

                float aspectValue = max(_ScreenParams.x / max(_ScreenParams.y, 1.0), 0.0001);
                float2 centeredUv = uv - _WideCenter.xy;
                centeredUv.x *= aspectValue;
                float radiusSquared = dot(centeredUv, centeredUv);
                float scaleValue = 1.0 + _WideDistortion * radiusSquared * 2.5 * _WideWeight;
                centeredUv *= scaleValue;
                centeredUv.x /= aspectValue;
                return ClampUv(centeredUv + _WideCenter.xy);
            }

            float2 ApplyScreenShake(float2 uv)
            {
                if (_ShakeWeight <= 0.0001 || _ShakeStrength <= 0.000001)
                {
                    return ClampUv(uv);
                }

                float timeValue = _AruiUnscaledTime * max(_ShakeFrequency, 0.0);
                float2 waveOffset = float2(
                    sin(timeValue * 1.173 + 0.31),
                    cos(timeValue * 1.739 + 1.07));
                waveOffset *= _ShakeAmplitude.xy * _ShakeStrength * _ShakeWeight;

                float2 centeredUv = uv - _ShakeCenter.xy;
                float rotationValue = sin(timeValue * 0.87) * _ShakeRotation * _ShakeWeight;
                float cosineValue = cos(rotationValue);
                float sineValue = sin(rotationValue);
                float2 rotatedUv;
                rotatedUv.x = centeredUv.x * cosineValue - centeredUv.y * sineValue;
                rotatedUv.y = centeredUv.x * sineValue + centeredUv.y * cosineValue;
                return ClampUv(rotatedUv + _ShakeCenter.xy + waveOffset);
            }

            float4 SampleBlurred(float2 uv)
            {
                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv));
                if (_BlurWeight <= 0.0001 || _BlurRadius <= 0.0001 || _BlurMix <= 0.0001)
                {
                    return baseColor;
                }

                float sampleCount = clamp(_BlurSamples, 4.0, 24.0);
                float2 texelRadius = _MainTex_TexelSize.xy * _BlurRadius * _BlurWeight;
                float2 directionValue = uv - _BlurCenter.xy;
                float directionLength = length(directionValue);
                directionValue = directionLength > 0.0001 ? directionValue / directionLength : float2(1.0, 0.0);

                float4 accumulatedColor = baseColor;
                float totalWeight = 1.0;
                const float goldenAngle = 2.39996323;

                for (int sampleIndex = 0; sampleIndex < 24; sampleIndex++)
                {
                    if ((float)sampleIndex >= sampleCount)
                    {
                        break;
                    }

                    float normalizedIndex = ((float)sampleIndex + 0.5) / sampleCount;
                    float radialFactor = sqrt(normalizedIndex);
                    float sampleAngle = goldenAngle * (float)sampleIndex;
                    float2 diskOffset = float2(cos(sampleAngle), sin(sampleAngle)) * radialFactor;
                    float2 sampleOffset;

                    if (_BlurMode < 0.5)
                    {
                        sampleOffset = diskOffset * texelRadius;
                    }
                    else if (_BlurMode < 1.5)
                    {
                        sampleOffset = -directionValue * radialFactor * texelRadius;
                    }
                    else
                    {
                        sampleOffset = directionValue * radialFactor * texelRadius;
                    }

                    accumulatedColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv + sampleOffset));
                    totalWeight += 1.0;
                }

                float4 blurredColor = accumulatedColor / max(totalWeight, 0.0001);
                return lerp(baseColor, blurredColor, saturate(_BlurMix));
            }

            float3 ApplyColorSplit(float2 uv)
            {
                float3 baseColor = SampleBlurred(uv).rgb;
                if (_ColorSplitWeight <= 0.0001 || _ColorSplitStrength <= 0.000001)
                {
                    return baseColor;
                }

                float2 splitDirection;
                if (_ColorSplitMode < 0.5)
                {
                    splitDirection = float2(cos(_ColorSplitDirection), sin(_ColorSplitDirection));
                }
                else
                {
                    splitDirection = uv - _ColorSplitCenter.xy;
                    float splitLength = length(splitDirection);
                    splitDirection = splitLength > 0.0001 ? splitDirection / splitLength : float2(1.0, 0.0);
                }

                float edgeDistance = length(uv - _ColorSplitCenter.xy) * 1.41421356;
                float edgeMultiplier = 1.0 + saturate(edgeDistance) * _ColorSplitEdgeBoost;
                float2 splitOffset = splitDirection * _ColorSplitStrength * edgeMultiplier * _ColorSplitWeight;
                float redValue = SampleBlurred(uv + splitOffset * _ColorSplitRBScale).r;
                float greenValue = baseColor.g;
                float blueValue = SampleBlurred(uv - splitOffset * _ColorSplitRBScale).b;
                return float3(redValue, greenValue, blueValue);
            }

            float RepeatingStripe(float scalarValue, float stripeCount, float stripeWidth)
            {
                float stripePosition = abs(frac(scalarValue * max(stripeCount, 1.0)) - 0.5);
                return 1.0 - smoothstep(stripeWidth * 0.5, max(stripeWidth, 0.0001), stripePosition);
            }

            float3 ApplyCinematicLetterbox(float2 uv, float3 sourceColor)
            {
                if (_LetterboxWeight <= 0.0001 || _LetterboxOpacity <= 0.0001)
                {
                    return sourceColor;
                }

                float softAmount = max(_LetterboxSoftness, 0.0001);
                float lowerBar = 1.0 - smoothstep(_LetterboxTopBottom - softAmount, _LetterboxTopBottom + softAmount, uv.y);
                float upperBar = smoothstep(1.0 - _LetterboxTopBottom - softAmount, 1.0 - _LetterboxTopBottom + softAmount, uv.y);
                float leftBar = 1.0 - smoothstep(_LetterboxLeftRight - softAmount, _LetterboxLeftRight + softAmount, uv.x);
                float rightBar = smoothstep(1.0 - _LetterboxLeftRight - softAmount, 1.0 - _LetterboxLeftRight + softAmount, uv.x);
                float barMask = saturate(max(max(lowerBar, upperBar), max(leftBar, rightBar)));
                float blendValue = saturate(barMask * _LetterboxOpacity * _LetterboxWeight);
                return lerp(sourceColor, _LetterboxColor.rgb, blendValue);
            }

            float3 ApplyRadialRays(float2 uv, float3 sourceColor)
            {
                if (_RadialRayWeight <= 0.0001)
                {
                    return sourceColor;
                }

                float aspectValue = max(_ScreenParams.x / max(_ScreenParams.y, 1.0), 0.0001);
                float2 localOffset = uv - _RadialRayCenter.xy;
                localOffset.x *= aspectValue;
                float distanceToCenter = length(localOffset);
                float polarAngle = atan2(localOffset.y, localOffset.x) + _RadialRayRotation + _AruiUnscaledTime * _RadialRayFlowSpeed;
                float normalizedAngle = polarAngle / 6.2831853;
                float rayValue = RepeatingStripe(normalizedAngle, _RadialRayCount, _RadialRayWidth);
                float flickerValue = lerp(1.0, 0.65 + 0.35 * sin(_AruiUnscaledTime * 26.0 + polarAngle * 3.0), _RadialRayFlicker);
                float radialFade = _RadialRayMode < 0.5 ? 1.0 : smoothstep(0.03, 0.28, distanceToCenter);
                rayValue *= radialFade * flickerValue;

                float3 darkenedColor = sourceColor * (1.0 - _RadialRayDarken);
                float3 overlayColor = darkenedColor + _RadialRayColor.rgb * rayValue * _RadialRayGlow;
                return lerp(sourceColor, overlayColor, saturate(_RadialRayWeight));
            }

            float ComputeVignetteDistance(float2 uv)
            {
                float aspectValue = max(_ScreenParams.x / max(_ScreenParams.y, 1.0), 0.0001);
                float2 localOffset = uv - _VignetteCenter.xy;
                localOffset.x *= aspectValue;
                localOffset.x /= max(_VignetteScale.x, 0.001);
                localOffset.y /= max(_VignetteScale.y, 0.001);
                float2 absoluteOffset = abs(localOffset);

                if (_VignetteShapeMode < 0.5)
                {
                    return length(localOffset);
                }
                if (_VignetteShapeMode < 1.5)
                {
                    return max(absoluteOffset.x, absoluteOffset.y);
                }
                if (_VignetteShapeMode < 2.5)
                {
                    return absoluteOffset.x + absoluteOffset.y;
                }

                float rectangleDistance = max(absoluteOffset.x, absoluteOffset.y);
                float circleDistance = length(localOffset);
                return lerp(rectangleDistance, circleDistance, saturate(_VignetteCornerRoundness));
            }

            float3 ApplyVignette(float2 uv, float3 sourceColor)
            {
                if (_VignetteWeight <= 0.0001 || _VignetteIntensity <= 0.0001)
                {
                    return sourceColor;
                }

                float distanceValue = ComputeVignetteDistance(uv);
                float edgeValue = smoothstep(_VignetteRange - _VignetteSoftness, _VignetteRange + _VignetteSoftness, distanceValue);
                float blendValue = saturate(edgeValue * _VignetteIntensity * _VignetteWeight);
                return lerp(sourceColor, _VignetteColor.rgb, blendValue);
            }

            float SkillBlackWhiteLuminance(float3 colorValue)
            {
                return dot(colorValue, float3(0.299, 0.587, 0.114));
            }

            float ComputeSkillBlackWhiteEdge(float2 uv)
            {
                if (_SkillBWEdgeStrength <= 0.0001)
                {
                    return 0.0;
                }

                float2 edgeOffset = _MainTex_TexelSize.xy * max(_SkillBWEdgeWidth, 0.25);
                float lumaLeft = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv - float2(edgeOffset.x, 0.0))).rgb);
                float lumaRight = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv + float2(edgeOffset.x, 0.0))).rgb);
                float lumaDown = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv - float2(0.0, edgeOffset.y))).rgb);
                float lumaUp = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv + float2(0.0, edgeOffset.y))).rgb);
                float edgeValue = abs(lumaRight - lumaLeft) + abs(lumaUp - lumaDown);
                return smoothstep(_SkillBWEdgeThreshold, _SkillBWEdgeThreshold + 0.20, edgeValue);
            }

            // 黑 → 白 → 黑 = 1 次往返。
            // 每次往返保留三个完整状态段：初始黑白、反相黑白、回到初始黑白。
            // 这样 Timeline 的 Clip 末段仍然能看见最后一次“黑”状态，不会只在强度归零时才回到黑色。
            float GetSkillBlackWhiteReturnInvert(float normalizedProgress)
            {
                float returnTripCount = max(_SkillBWSwitchCount, 1.0);
                float stateSegmentCount = returnTripCount * 2.0 + 1.0;
                float stateIndex = floor(saturate(normalizedProgress) * stateSegmentCount + 0.0001);
                stateIndex = min(stateIndex, stateSegmentCount - 1.0);
                return fmod(stateIndex, 2.0);
            }

            float3 ApplySkillBlackWhite(float2 uv, float3 sourceColor)
            {
                if (_SkillBWWeight <= 0.0001)
                {
                    return sourceColor;
                }

                float3 targetColor;
                if (_SkillBWMode >= 2.5)
                {
                    targetColor = _SkillBWBlackColor.rgb;
                }
                else if (_SkillBWMode >= 1.5)
                {
                    targetColor = _SkillBWWhiteColor.rgb;
                }
                else
                {
                    float lumaValue = SkillBlackWhiteLuminance(sourceColor);
                    lumaValue = saturate((lumaValue - 0.5) * _SkillBWContrast + 0.5);
                    float splitValue = smoothstep(_SkillBWThreshold - _SkillBWSoftness, _SkillBWThreshold + _SkillBWSoftness, lumaValue);

                    if (_SkillBWMode >= 0.5)
                    {
                        float invertValue = GetSkillBlackWhiteReturnInvert(_SkillBWProgress);
                        splitValue = lerp(splitValue, 1.0 - splitValue, invertValue);
                    }

                    targetColor = lerp(_SkillBWBlackColor.rgb, _SkillBWWhiteColor.rgb, splitValue);

                    float edgeValue = saturate(ComputeSkillBlackWhiteEdge(uv) * _SkillBWEdgeStrength);
                    float3 inverseEdgeColor = lerp(_SkillBWBlackColor.rgb, _SkillBWWhiteColor.rgb, 1.0 - splitValue);
                    targetColor = lerp(targetColor, inverseEdgeColor, edgeValue);
                }

                return lerp(sourceColor, targetColor, saturate(_SkillBWWeight));
            }


            float MangaHash(float scalarValue)
            {
                return frac(sin(scalarValue * 127.1 + 311.7) * 43758.5453123);
            }

            float MangaHash2(float2 value)
            {
                return frac(sin(dot(value, float2(12.9898, 78.233))) * 43758.5453123);
            }

            float ComputeMangaEdge(float2 uv)
            {
                if (_MangaEdgeStrength <= 0.0001)
                {
                    return 0.0;
                }

                float2 sampleOffset = _MainTex_TexelSize.xy * max(_MangaEdgeWidth, 0.25);
                float lumaLeft = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv - float2(sampleOffset.x, 0.0))).rgb);
                float lumaRight = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv + float2(sampleOffset.x, 0.0))).rgb);
                float lumaDown = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv - float2(0.0, sampleOffset.y))).rgb);
                float lumaUp = SkillBlackWhiteLuminance(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, ClampUv(uv + float2(0.0, sampleOffset.y))).rgb);
                float edgeValue = abs(lumaRight - lumaLeft) + abs(lumaUp - lumaDown);
                return smoothstep(_MangaEdgeThreshold, _MangaEdgeThreshold + 0.20, edgeValue);
            }

            float ComputeMangaBrushMask(float2 uv)
            {
                float aspectValue = max(_ScreenParams.x / max(_ScreenParams.y, 1.0), 0.0001);
                float2 localOffset = uv - _MangaCenter.xy;
                localOffset.x *= aspectValue;
                float radiusValue = length(localOffset);
                float polarAngle = atan2(localOffset.y, localOffset.x);
                float normalizedAngle = polarAngle / 6.2831853 + 0.5;
                float brushCount = max(_MangaBrushCount, 4.0);
                float brushCell = normalizedAngle * brushCount;
                float brushIndex = floor(brushCell);
                float brushPhase = frac(brushCell) - 0.5;
                float randomness = MangaHash(brushIndex + 1.0);
                float widthScale = lerp(1.0, lerp(0.45, 1.80, randomness), saturate(_MangaIrregularity));
                float effectiveWidth = max(_MangaBrushWidth * widthScale, 0.0005);
                float angularMask = 1.0 - smoothstep(effectiveWidth * 0.5, effectiveWidth * 0.5 + 0.018, abs(brushPhase));

                float startDistance = _MangaInnerBlank + (randomness - 0.5) * _MangaIrregularity * 0.18;
                float lengthScale = lerp(1.0, lerp(0.55, 1.25, MangaHash(brushIndex + 37.0)), saturate(_MangaIrregularity));
                float endDistance = startDistance + _MangaBrushLength * lengthScale;
                float radialStart = smoothstep(startDistance, startDistance + 0.026, radiusValue);
                float radialEnd = 1.0 - smoothstep(endDistance - 0.075, endDistance, radiusValue);

                float segmentIndex = floor(radiusValue * 64.0 + brushIndex * 1.731);
                float segmentNoise = MangaHash2(float2(brushIndex, segmentIndex));
                float breakMask = step(_MangaBreakup, segmentNoise);
                return saturate(angularMask * radialStart * radialEnd * breakMask);
            }

            float3 ApplyMangaFilter(float2 uv, float3 sourceColor)
            {
                if (_MangaWeight <= 0.0001)
                {
                    return sourceColor;
                }

                float lumaValue = SkillBlackWhiteLuminance(sourceColor);
                lumaValue = saturate((lumaValue - 0.5) * _MangaContrast + 0.5);
                float splitValue = smoothstep(_MangaThreshold - _MangaSoftness, _MangaThreshold + _MangaSoftness, lumaValue);
                float3 highContrastColor = lerp(_MangaInkColor.rgb, _MangaPaperColor.rgb, splitValue);
                float brushMask = ComputeMangaBrushMask(uv) * _MangaBrushStrength;
                float edgeMask = ComputeMangaEdge(uv) * _MangaEdgeStrength;

                float3 mangaColor;
                if (_MangaMode < 0.5)
                {
                    mangaColor = lerp(highContrastColor, _MangaInkColor.rgb, saturate(brushMask + edgeMask));
                }
                else if (_MangaMode < 1.5)
                {
                    mangaColor = lerp(highContrastColor, _MangaInkColor.rgb, saturate(edgeMask));
                }
                else
                {
                    mangaColor = lerp(sourceColor, _MangaInkColor.rgb, saturate(brushMask + edgeMask));
                }

                return lerp(sourceColor, mangaColor, saturate(_MangaWeight));
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = ApplyWideAngle(input.uv);
                uv = ApplyScreenShake(uv);

                float3 colorValue = ApplyColorSplit(uv);
                colorValue = ApplyCinematicLetterbox(uv, colorValue);
                colorValue = ApplyRadialRays(uv, colorValue);
                colorValue = ApplyVignette(uv, colorValue);
                colorValue = ApplySkillBlackWhite(uv, colorValue);
                colorValue = ApplyMangaFilter(uv, colorValue);
                return half4(colorValue, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
