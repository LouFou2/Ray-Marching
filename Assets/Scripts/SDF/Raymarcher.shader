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
            uniform float4 _sphere1;

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

            float distanceField(float3 p)
            {
                float sphere1 = sdSphere(p - _sphere1.xyz, _sphere1.w);
                return sphere1;
            }

            fixed4 raymarching(float3 ro, float3 rd) // ray origin and ray direction
            {
                fixed4 result = fixed4(1,1,1,1);

                const int max_iteration = 64; //*** might need higher number for more complex scenes

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
                        //we have hit something : do shading
                        result = fixed4(1,1,1,1);
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
