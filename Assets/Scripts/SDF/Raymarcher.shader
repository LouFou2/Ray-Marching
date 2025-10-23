Shader "Hidden/Raymarcher"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            uniform float4x4 _CamFrustum, _CamToWorld;
            uniform float _maxDistance;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ray : TEXCOORD1;
            };

            struct sphereData
            {
                float4 sphereInfo;
            };

            StructuredBuffer<sphereData> _spheresBuffer; //this can be used like a "list" to iterate
            int _sphereCount;

            v2f vert (appdata v)
            {
                v2f o;
                half index = v.vertex.z;
                v.vertex.z = 0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.ray = _CamFrustum[(int)index].xyz;

                o.ray /= abs(o.ray.z);

                o.ray = mul(_CamToWorld, o.ray);

                return o;
            }

            float sdSphere(float3 p, float s)
            {
                return length(p) - s;
            }

            //this is the most basic "union" (adding both)
            float unionSDF(float distA, float distB) {
                return min(distA, distB);
            }

            // polynomial smooth min
            // from https://www.iquilezles.org/www/articles/smin/smin.htm
            float smoothUnion(float distA, float distB, float strength)
            {
                float h = clamp(0.5 + 0.5 * (distB - distA) / strength, 0.0, 1.0);
                return lerp(distB, distA, h) - strength * h * (1.0 - h);
            }

            float distanceField(float3 p)
            {
                float addedSpheres = sdSphere(p - _spheresBuffer[0].sphereInfo.xyz, _spheresBuffer[0].sphereInfo.w);
                for(int i = 1; i < _sphereCount; i++) 
                {
                    float nextSphere = sdSphere(p - _spheresBuffer[i].sphereInfo.xyz, _spheresBuffer[i].sphereInfo.w);
                    addedSpheres = smoothUnion(addedSpheres, nextSphere, 0.3);
                }
                
                return addedSpheres;
            }

            float3 getNormal(float3 p)
            {
                const float2 offset = float2(0.001,0.0);
                float3 n = float3(
                    distanceField(p + offset.xyy) - distanceField(p - offset.xyy),
                    distanceField(p + offset.yxy) - distanceField(p - offset.yxy),
                    distanceField(p + offset.yyx) - distanceField(p - offset.yyx));

                return normalize(n);    
            }

            float fresnel(float3 n, float3 rd)
            {
                float dotProduct = 1 - saturate(abs(dot(n, rd)));
                return dotProduct;
            }

            fixed4 raymarching(float3 ro, float3 rd) // ray origin and ray direction
            {
                fixed4 result = fixed4(1,1,1,1);

                const int max_iteration = 256; //*** might need higher number for more complex scenes

                float t = 0; //distance travelled along ray direction

                for(int i = 0; i < max_iteration; i++)
                {
                    if(t > _maxDistance)
                    {
                        //no hit, draw environment
                        result = fixed4(rd, 1); // ***temporary
                        break;
                    }

                    float3 p = ro + rd * t;

                    // check for hit in distance field
                    float d = distanceField(p);

                    if(d < 0.001)
                    {
                        //SHADING
                        float3 n = getNormal(p);
                        float f = fresnel(n, rd);
                        result = fixed4(f,f,f,1);
                        break;
                    }
                    t += d;
                }

                return result;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 rayDirection = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;

                fixed4 result = raymarching(rayOrigin, rayDirection);

                return result;
                //return fixed4(rayDirection, 1);
            }
            ENDCG
        }
    }
}
