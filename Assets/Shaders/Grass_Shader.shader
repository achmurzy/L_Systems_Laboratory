Shader "Custom/Geometry/Grass_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_TopColor("Color", Color) = (1,1,1,1)
		_BottomColor("Color", Color) = (1,1,1,1)
		_BendRotationRandom("Bend Rotation Random", Range(0,1)) = 0.2

		_BladeWidth("Blade Width", Float) = 0.1
		_BladeWidthVariance("Blade Width Variance", Float) = 0.5
		_BladeHeight("Blade Height", Float) = 0.1
		_BladeHeightVariance("Blade Height Variance", Float) = 0.5

		//"The hardware has a limit of 64 subdivisions per patch"
		_TessellationUniform("Tessellation Uniform", Range(1, 64)) = 1

		_CharacterPosition("Character Position", Vector) = (1,1,1)
		_CharacterGenerationRadius("Character Generation Radius", Range(2, 8)) = 4
		_CharacterDeformationRadius("Character Deformation Radius", Range(1, 2)) = 1
		_CharacterDeformationStrength("Character Deformation Strength", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
			#pragma hull hull
			#pragma domain domain
			#pragma geometry geo
			#pragma fragment frag

            #include "UnityCG.cginc"
			#include "CustomTessellation.cginc"

			/*Defined in CustomTessellation
			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};
			
			v2g vert(vertexInput v)
			{
				v2g o;
				o.pos = v.vertex;
				o.normal = v.normal;
				o.tangent = v.tangent;
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			struct v2g - Same as vertexOutput defined in CustomTessellation
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};
			
			*/

			struct g2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD1;
			};

            sampler2D _MainTex;
            float4 _MainTex_ST;



			g2f VertexOutput(float3 pos, float2 uv)
			{
				g2f g;
				g.pos = UnityObjectToClipPos(pos);
				g.uv = uv;
				return g;
			}

			//Dug up from: https://answers.unity.com/questions/399751/randomity-in-cg-shaders-beginner.html
			float rand(float3 myVector) {
				return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			// Construct a rotation matrix that rotates around the provided axis, sourced from:
			// https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
			float3x3 AngleAxis3x3(float angle, float3 axis)
			{
				float c, s;
				sincos(angle, s, c);

				float t = 1 - c;
				float x = axis.x;
				float y = axis.y;
				float z = axis.z;

				return float3x3(
					t * x * x + c, t * x * y - s * z, t * x * z + s * y,
					t * x * y + s * z, t * y * y + c, t * y * z - s * x,
					t * x * z - s * y, t * y * z + s * x, t * z * z + c
					);
			}//HELP

			float3 _CharacterPosition;
			float _CharacterGenerationRadius, _CharacterDeformationRadius, _CharacterDeformationStrength;
			float _BendRotationRandom;
			float _BladeWidth, _BladeWidthVariance;
			float _BladeHeight, _BladeHeightVariance;

			[maxvertexcount(3)]
			void geo(triangle vertexOutput IN[3] : SV_POSITION, inout TriangleStream<g2f> triStream)
			{
				float3 pos = IN[0].vertex.xyz;
				float3 world = mul(unity_ObjectToWorld, IN[0].vertex).xyz;
				float dist = distance(world, _CharacterPosition);
				if (dist > _CharacterGenerationRadius)
					return;

				float3 vNormal = IN[0].normal;
				float4 vTangent = IN[0].tangent;
				float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;

				float3x3 tangentToLocal = float3x3(
					vTangent.x, vBinormal.x, vNormal.x,
					vTangent.y, vBinormal.y, vNormal.y,
					vTangent.z, vBinormal.z, vNormal.z);
				float3x3 facingRotationMatrix = AngleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0, 1));
				float random_bend = rand(pos.zyx) * _BendRotationRandom * UNITY_PI * 0.5;
				float character_bend = dist > _CharacterDeformationRadius ? 0 : ((UNITY_PI * 0.5) - random_bend) * _CharacterDeformationStrength * ((rand(pos.yxz)+1) /2);
				float3x3 bendRotationMatrix = AngleAxis3x3(random_bend + character_bend, float3(-1, 0, 0));
				float3x3 transformationMatrix = mul(mul(tangentToLocal, facingRotationMatrix), bendRotationMatrix);

				float width = _BladeWidth + (rand(pos.xzy) * 2 - 1) * _BladeWidthVariance;// *(1 / dist);
				float height = _BladeHeight + (rand(pos.zxy) * 2 - 1) * _BladeHeightVariance;// *(1 / dist);

				triStream.Append(VertexOutput(pos + mul(transformationMatrix, float3(width, 0, 0)), float2(0,0)));
				triStream.Append(VertexOutput(pos + mul(transformationMatrix, float3(-width, 0, 0)), float2(1,0)));
				triStream.Append(VertexOutput(pos + mul(transformationMatrix, float3(0, 0, height)), float2(0,1)));

				//New
			}

			float4 _TopColor, _BottomColor;
            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                return lerp(_BottomColor, _TopColor, i.uv.y);
            }
            ENDCG
        }
    }
}
